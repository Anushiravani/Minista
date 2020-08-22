using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using System.Net;
using Newtonsoft.Json;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json.Linq;
using InstagramApiSharp.Classes.ResponseWrappers;
using System.Net.Http;
using System.Linq;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using static Helper;
namespace Minista.Helpers
{
    public class InstaUploader
    {
        internal string GenerateUploadId()
        {
            //173258081688145
            //1595581225629
            string r = "15";
            for (int i = 0; i < 13; i++)
                r += Views.Uploads.Uploader.Rnd.Next(0, 9).ToString();
            return r;
            //var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            //var uploadId = (long)timeSpan.TotalMilliseconds;
            //return uploadId.ToString();
        }
        static Uri GetMediaConfigureUri()
        {
            return new Uri("https://i.instagram.com/api/v1/direct_v2/threads/broadcast/configure_photo/", UriKind.RelativeOrAbsolute);
        }
        static Uri GetUploadPhotoUri(string uploadId, int fileHashCode)
        {
            return new Uri(string.Format("https://i.instagram.com/rupload_igphoto/{0}_0_{1}", uploadId, fileHashCode), UriKind.RelativeOrAbsolute);
        }

        private string GetRetryContext()
        {
            return new JObject
                {
                    {"num_step_auto_retry", 0},
                    {"num_reupload", 0},
                    {"num_step_manual_retry", 0}
                }.ToString(Formatting.None);
        }
        CancellationTokenSource cts;
        string UploadId { get; set; }
        string ThreadId { get; set; }
        string Recipient { get; set; }
        InstaInboxMedia MediaShare { get; set; }
        InstaDirectInboxThread Thread;
        InstaDirectInboxItem Item;
        public async void UploadSinglePhoto(StorageFile File, string threadId, string recipient, 
            InstaInboxMedia mediaShare, InstaDirectInboxThread thread, InstaDirectInboxItem inboxItem)
        {
            Thread = thread;
            MediaShare = mediaShare;
            UploadId = GenerateUploadId();
            ThreadId = threadId;
            Recipient = recipient;
            Item = inboxItem;
            var photoHashCode = Path.GetFileName(File.Path ?? $"C:\\{13.GenerateRandomStringStatic()}.jpg").GetHashCode();
            var photoEntityName = $"{UploadId}_0_{photoHashCode}";
            var instaUri = GetUploadPhotoUri(UploadId, photoHashCode);

            var device = InstaApi.GetCurrentDevice();
            var httpRequestProcessor = InstaApi.HttpRequestProcessor;

            BackgroundUploader BGU = new BackgroundUploader();

            var cookies = httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(httpRequestProcessor.Client.BaseAddress);
            var strCookies = string.Empty;
            foreach (Cookie cook in cookies)
                strCookies += $"{cook.Name}={cook.Value}; ";
            // header haye asli in ha hastan faghat
            BGU.SetRequestHeader("Cookie", strCookies);
            var r = InstaApi.HttpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, device);
            foreach (var item in r.Headers)
            {
                BGU.SetRequestHeader(item.Key, string.Join(" ", item.Value));
            }

            //{
            //  "upload_id": "1595581225629",
            //  "media_type": "1",
            //  "retry_context": "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}",
            //  "image_compression": "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"0\"}",
            //  "xsharing_user_ids": "[\"1647718432\",\"1581245356\"]"
            //} 
            var photoUploadParamsObj = new JObject
            {
                {"upload_id", UploadId},
                {"media_type", "1"},
                {"retry_context", GetRetryContext()},
                {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"0\"}"},
                {"xsharing_user_ids", $"[{recipient ?? string.Empty}]"},
            };
            var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
            long fileLength = 0;
            using (var openedFile = await File.OpenAsync(FileAccessMode.Read))
                fileLength = openedFile.AsStream().Length;

            BGU.SetRequestHeader("X_FB_PHOTO_WATERFALL_ID", Guid.NewGuid().ToString());
            BGU.SetRequestHeader("X-Entity-Length", fileLength.ToString());
            BGU.SetRequestHeader("X-Entity-Name", photoEntityName);
            BGU.SetRequestHeader("X-Instagram-Rupload-Params", photoUploadParams);
            BGU.SetRequestHeader("X-Entity-Type", "image/jpeg");
            BGU.SetRequestHeader("Offset", "0");
            BGU.SetRequestHeader("Content-Transfer-Encoding", "binary");
            BGU.SetRequestHeader("Content-Type", "application/octet-stream");

            // hatman bayad noe file ro moshakhas koni, mese in zirie> oun PHOTO mishe name 
            // requestContent.Add(imageContent, "photo", $"pending_media_{ApiRequestMessage.GenerateUploadId()}.jpg");
            // vase video nemikhad esmi bezari!

            //BackgroundTransferContentPart filePart = new BackgroundTransferContentPart(/*"photo"*/);
            //filePart.SetFile(File);
            //// chon binary hast:
            //filePart.SetHeader("Content-Transfer-Encoding", "binary");
            //filePart.SetHeader("Content-Type", "application/octet-stream");

            //var parts = new List<BackgroundTransferContentPart>
            //{
            //    filePart
            //};
            Debug.WriteLine("----------------------------------------Start upload----------------------------------");
            
            //var uploadX = await BGU.CreateUploadAsync(instaUri, parts, "", UploadId);
            var upload = BGU.CreateUpload(instaUri, File);
            upload.Priority = BackgroundTransferPriority.High;
            await HandleUploadAsync(upload, true);
        }

        BackgroundTransferContentPart GetTextPart(string key, string value)
        {
            // namesh mishe KEY va textesh mishe VALUE
            BackgroundTransferContentPart part = new BackgroundTransferContentPart(key);
            // chon typeshon text hast va utf-8 bayad content typeshon ro set konim be in
            part.SetHeader("Content-Type", "text/plain; charset=utf-8");
            part.SetText(value);
            // sare ham ye chize in shekli mogheye upload ijad mishe:

            //Content-Type: text/plain; charset=utf-8
            //Content-Disposition: form-data; name="_uuid"
            //
            //86665e74-5514-8236-basu-15aef8ceb0d6
            return part;
        }
        async Task HandleUploadAsync(UploadOperation upload, bool start)
        {
            cts = new CancellationTokenSource();
            try
            {
                LogStatus("Running: " + upload.Guid);

                Progress<UploadOperation> progressCallback = new Progress<UploadOperation>(UploadProgress);
                if (start)
                {

                    // Start the upload and attach a progress handler.
                    await upload.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The upload was already running when the application started, re-attach the progress handler.
                    await upload.AttachAsync().AsTask(cts.Token, progressCallback);
                }

                ResponseInformation response = upload.GetResponseInformation();

                LogStatus(string.Format("Completed: {0}, Status Code: {1}", upload.Guid, response.StatusCode));
            }
            catch (TaskCanceledException)
            {
                LogStatus("Canceled: " + upload.Guid);
            }
            catch (Exception ex)
            {

                //throw;

            }
        }

        async void UploadProgress(UploadOperation upload)
        {
            LogStatus(string.Format("Progress: {0}, Status: {1}", upload.Guid, upload.Progress.Status));

            BackgroundUploadProgress progress = upload.Progress;

            double percentSent = 100;
            if (progress.TotalBytesToSend > 0)
            {
                var bs = progress.BytesSent;
                percentSent = bs * 100 / progress.TotalBytesToSend;
            }

            SendNotify(percentSent);
            //MarshalLog(String.Format(" - Sent bytes: {0} of {1} ({2}%)",
            //  progress.BytesSent, progress.TotalBytesToSend, percentSent));

            LogStatus(string.Format(" - Sent bytes: {0} of {1} ({2}%), Received bytes: {3} of {4}",
                progress.BytesSent, progress.TotalBytesToSend, percentSent,
                progress.BytesReceived, progress.TotalBytesToReceive));

            if (progress.HasRestarted)
            {
                LogStatus(" - Upload restarted");
            }

            if (progress.HasResponseChanged)
            {
                var resp = upload.GetResponseInformation();
                // We've received new response headers from the server.
                LogStatus(" - Response updated; Header count: " + resp.Headers.Count);
                var response = upload.GetResultStreamAt(0);
                StreamReader stream = new StreamReader(response.AsStreamForRead());
                Debug.WriteLine("----------------------------------------Response from upload----------------------------------");
                var st = stream.ReadToEnd();
                Debug.WriteLine(st);
                //var res = JsonConvert.DeserializeObject<APIResponseOverride>(st);
                //await ConfigureMediaPhotoAsync(res.upload_id, Caption, null, null);
                await ConfigurePhotoAsync();
                //UploadCompleted?.Invoke(null, Convert.ToInt64(res.upload_id));
                // If you want to stream the response data this is a good time to start.
                // upload.GetResultStreamAt(0);
            }
        }
        private async Task<IResult<bool>> ConfigurePhotoAsync()
        {
            try
            {
                Debug.WriteLine("----------------------------------------ConfigurePhotoAsync----------------------------------");
                var instaUri = GetMediaConfigureUri();
                var retryContext = GetRetryContext();
                var _user = InstaApi.GetLoggedUser();
                var _deviceInfo = InstaApi.GetCurrentDevice();
                var data = new Dictionary<string, string>
                {
                    {"action", "send_item"},
                    {"client_context", Guid.NewGuid().ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"device_id", _deviceInfo.DeviceId},
                    {"mutation_token", Guid.NewGuid().ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"allow_full_aspect_ratio", "true"},
                    {"upload_id", UploadId},
                };
                if (!string.IsNullOrEmpty(ThreadId))
                    data.Add("thread_ids", $"[{ThreadId}]");
                else if (!string.IsNullOrEmpty(Recipient))
                    data.Add("recipient_users", $"[[{Recipient}]]");

                var _httpRequestProcessor = InstaApi.HttpRequestProcessor;
                var request = InstaApi.HttpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Add("retry_context", GetRetryContext());
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<InstaDirectRespondResponse>(json);
                    var payload = ConvertersFabric.Instance.GetDirectRespondConverter(result).Convert().Payload;
                    Thread.ThreadId = payload.ThreadId;
                    Item.ItemId = payload.ItemId;
                    Item.SendingType = InstagramApiSharp.Enums.InstaDirectInboxItemSendingType.Sent;
                    SendNotify(102);
                }
                return Result.Success(true);
            }
            catch (HttpRequestException httpException)
            {
                SendNotify(102);
                return Result.Fail(httpException, false, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                SendNotify(102);
                return Result.Fail<bool>(exception);
            }
        }

        public void LogStatus(string Text) => Debug.WriteLine(Text);
        async void SendNotify(double value)
        {
            try
            {
                if (MediaShare == null) return;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MediaShare.Percentage = value;
                });
            }
            catch { }
        }
    }
    public class ProfileUploader
    {
        public bool IsUploading { get; set; } = false;
        public event EventHandler OnCompleted;
        public event EventHandler<string> OnError;
        internal string GenerateUploadId()
        {
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            var uploadId = (long)timeSpan.TotalMilliseconds;
            return uploadId.ToString();
        }
        static Uri GetConfigureUri()
        {
            return new Uri("https://i.instagram.com/api/v1/accounts/change_profile_picture/", UriKind.RelativeOrAbsolute);
        }
        static Uri GetUploadPhotoUri(string uploadId, int fileHashCode)
        {
            return new Uri(string.Format("https://i.instagram.com/rupload_igphoto/{0}_0_{1}", uploadId, fileHashCode), UriKind.RelativeOrAbsolute);
        }

        private string GetRetryContext()
        {
            return new JObject
                {
                    {"num_step_auto_retry", 0},
                    {"num_reupload", 0},
                    {"num_step_manual_retry", 0}
                }.ToString(Formatting.None);
        }
        CancellationTokenSource cts;
        string UploadId { get; set; }
        public async void UploadSinglePhoto(StorageFile File)
        {
            IsUploading = true;
            UploadId = GenerateUploadId();
            var photoHashCode = Path.GetFileName(File.Path ?? $"C:\\{13.GenerateRandomStringStatic()}.jpg").GetHashCode();
            var photoEntityName = $"{UploadId}_0_{photoHashCode}";
            var instaUri = GetUploadPhotoUri(UploadId, photoHashCode);

            var device = InstaApi.GetCurrentDevice();
            var httpRequestProcessor = InstaApi.HttpRequestProcessor;

            BackgroundUploader BGU = new BackgroundUploader();

            var cookies = httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(httpRequestProcessor.Client.BaseAddress);
            var strCookies = string.Empty;
            foreach (Cookie cook in cookies)
                strCookies += $"{cook.Name}={cook.Value}; ";
            // header haye asli in ha hastan faghat
            BGU.SetRequestHeader("Cookie", strCookies);
            var r = InstaApi.HttpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, device);
            foreach (var item in r.Headers)
            {
                BGU.SetRequestHeader(item.Key, string.Join(" ", item.Value));
            }


            var photoUploadParamsObj = new JObject
            {
                {"upload_id", UploadId},
                {"media_type", "1"},
                {"retry_context", GetRetryContext()},
                {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"95\"}"},
                {"xsharing_user_ids", "[]"},
            };
            var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
            var openedFile = await File.OpenAsync(FileAccessMode.Read);

            BGU.SetRequestHeader("X-Entity-Type", "image/jpeg");
            BGU.SetRequestHeader("Offset", "0");
            BGU.SetRequestHeader("X-Instagram-Rupload-Params", photoUploadParams);
            BGU.SetRequestHeader("X-Entity-Name", photoEntityName);
            BGU.SetRequestHeader("X-Entity-Length", openedFile.AsStream().Length.ToString());
            BGU.SetRequestHeader("X_FB_PHOTO_WATERFALL_ID", Guid.NewGuid().ToString());
            BGU.SetRequestHeader("Content-Transfer-Encoding", "binary");
            BGU.SetRequestHeader("Content-Type", "application/octet-stream");

            // hatman bayad noe file ro moshakhas koni, mese in zirie> oun PHOTO mishe name 
            // requestContent.Add(imageContent, "photo", $"pending_media_{ApiRequestMessage.GenerateUploadId()}.jpg");
            // vase video nemikhad esmi bezari!

            //BackgroundTransferContentPart filePart = new BackgroundTransferContentPart(/*"photo"*/);
            //filePart.SetFile(File);
            //// chon binary hast:
            //filePart.SetHeader("Content-Transfer-Encoding", "binary");
            //filePart.SetHeader("Content-Type", "application/octet-stream");

            //var parts = new List<BackgroundTransferContentPart>
            //{
            //    filePart
            //};
            Debug.WriteLine("----------------------------------------Start upload----------------------------------");

            //var uploadX = await BGU.CreateUploadAsync(instaUri, parts, "", UploadId);
            var upload = BGU.CreateUpload(instaUri, File);
            upload.Priority = BackgroundTransferPriority.High;
            await HandleUploadAsync(upload, true);
        }

        async Task HandleUploadAsync(UploadOperation upload, bool start)
        {
            cts = new CancellationTokenSource();
            try
            {
                LogStatus("Running: " + upload.Guid);

                Progress<UploadOperation> progressCallback = new Progress<UploadOperation>(UploadProgress);
                if (start)
                {

                    // Start the upload and attach a progress handler.
                    await upload.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The upload was already running when the application started, re-attach the progress handler.
                    await upload.AttachAsync().AsTask(cts.Token, progressCallback);
                }

                ResponseInformation response = upload.GetResponseInformation();

                LogStatus(string.Format("Completed: {0}, Status Code: {1}", upload.Guid, response.StatusCode));
            }
            catch (TaskCanceledException)
            {
                LogStatus("Canceled: " + upload.Guid);
            }
            catch (Exception)
            {

                throw;

            }
        }

        async void UploadProgress(UploadOperation upload)
        {
            LogStatus(string.Format("Progress: {0}, Status: {1}", upload.Guid, upload.Progress.Status));

            BackgroundUploadProgress progress = upload.Progress;

            double percentSent = 100;
            if (progress.TotalBytesToSend > 0)
            {
                var bs = progress.BytesSent;
                percentSent = bs * 100 / progress.TotalBytesToSend;
            }

            //SendNotify(percentSent);
           

            LogStatus(string.Format(" - Sent bytes: {0} of {1} ({2}%), Received bytes: {3} of {4}",
                progress.BytesSent, progress.TotalBytesToSend, percentSent,
                progress.BytesReceived, progress.TotalBytesToReceive));

            if (progress.HasRestarted)
            {
                LogStatus(" - Upload restarted");
            }

            if (progress.HasResponseChanged)
            {
                var resp = upload.GetResponseInformation();
                // We've received new response headers from the server.
                LogStatus(" - Response updated; Header count: " + resp.Headers.Count);
                var response = upload.GetResultStreamAt(0);
                StreamReader stream = new StreamReader(response.AsStreamForRead());
                Debug.WriteLine("----------------------------------------Response from upload----------------------------------");
                var st = stream.ReadToEnd();
                Debug.WriteLine(st);
                //var res = JsonConvert.DeserializeObject<APIResponseOverride>(st);
                //await ConfigureMediaPhotoAsync(res.upload_id, Caption, null, null);
                await ConfigurePhotoAsync();
                //UploadCompleted?.Invoke(null, Convert.ToInt64(res.upload_id));
                // If you want to stream the response data this is a good time to start.
                // upload.GetResultStreamAt(0);
            }
        }
        private async Task<IResult<bool>> ConfigurePhotoAsync()
        {
            try
            {
                Debug.WriteLine("----------------------------------------ConfigurePhotoAsync----------------------------------");
                var instaUri = GetConfigureUri();
                var retryContext = GetRetryContext();
                var _user = InstaApi.GetLoggedUser();
                var _deviceInfo = InstaApi.GetCurrentDevice();
                //{ "upload_id", uploadId},
                //    { "_csrftoken", _user.CsrfToken},
                //    { "use_fbuploader", "true"},
                //    { "_uuid", _deviceInfo.DeviceGuid.ToString()},
                var data = new Dictionary<string, string>
                {
                    {"upload_id", UploadId},
                    {"_csrftoken", _user.CsrfToken},
                    {"use_fbuploader", "true"},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };

                var _httpRequestProcessor = InstaApi.HttpRequestProcessor;
                var request = InstaApi.HttpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Add("retry_context", GetRetryContext());
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();


                SendCompleted();
                IsUploading = false;
                return Result.Success(true);
            }
            catch (HttpRequestException httpException)
            {
                IsUploading = false;
                SendError($"HttpException thrown: {httpException.Message}\r\nSource: {httpException.Source}\r\nTrace: {httpException.StackTrace}");
                return Result.Fail(httpException, false, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                IsUploading = false;
                SendError($"Exception thrown: {exception.Message}\r\nSource: {exception.Source}\r\nTrace: {exception.StackTrace}");
                return Result.Fail<bool>(exception);
            }
        }

        public void LogStatus(string Text) => Debug.WriteLine(Text);
        void SendCompleted()
        {
            try
            {
                OnCompleted?.Invoke(this, new EventArgs());
            }
            catch { }
        }

        void SendError(string text)
        {
            try
            {
                IsUploading = false;
                OnError?.Invoke(this, text);
            }
            catch { }
        }


    }

}
