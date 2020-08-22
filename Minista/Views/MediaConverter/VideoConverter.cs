using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FFmpegInterop;
using Windows.Foundation;
using Windows.Media.Effects;

namespace Minista.Views.MediaConverter
{
    public class VideoConverter
    {
        const string OutputExtension = ".mp4";
        Windows.UI.Core.CoreDispatcher _dispatcher = Window.Current.Dispatcher;
        CancellationTokenSource Cts;
        MediaEncodingProfile MediaProfile;
        MediaTranscoder Transcoder = new MediaTranscoder();
        TimeSpan StartTime = new TimeSpan(0);
        TimeSpan StopTime = new TimeSpan(0, 0, 59); // MAX TIME, albate age niaz shod!
        MediaStreamSource Mss;
        FFmpegInteropMSS FFmpegMSS;
        List<StorageFile> QueueList = new List<StorageFile>();
        List<StorageFile> ConvertedList = new List<StorageFile>();

        public bool IsConverting { get; private set; } = false;
        //MediaStreamSource MssTest;
        //FFmpegInteropMSS FFmpegMSSTest;

        public VideoConverter()
        {
            Transcoder = new MediaTranscoder();
            Cts = new CancellationTokenSource();
            //Mss = null;
            //FFmpegMSS = null;
            MediaProfile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
        }
        public async Task<List<StorageFile>> ConvertFiles(List<StorageFile> files, bool story, Size? size, Rect? rectSize)
        {
            try
            {
                IsStoryVideo = story;
                if (story)
                    StopTime = TimeSpan.FromSeconds(14.8);
                else
                    StopTime = new TimeSpan(0, 0, 59);
                QueueList.Clear();
                ConvertedList.Clear();
                foreach (var item in files)
                {
                    if (item.IsVideo())
                        try
                        {
                            var test =  await item.GetDurationAsync();
                            //if (item.NeedsConvert() || test.CompareTo(StopTime) > 0)
                                QueueList.Add(item);
                            //else
                            //    ConvertedList.Add(item);

                            //QueueList.Add(item);
                        }
                        catch { }
                }
                if (QueueList.Any())
                {
                    //Visibility = Visibility.Visible;
                    int ix = 1;
                    const string text = "Some of your file(s) needs to be converted first. Please wait...\r\n";
                    foreach (var item in QueueList)
                    {
                        try
                        {
                            if (item.IsVideo())
                            {
                                Text(text + $"{ix} of {QueueList.Count}");
                                IsConverting = true;
                                var vid = await ConvertVideo(item, size, rectSize);
                                ("vid null: " + vid == null).PrintDebug();
                                if (vid != null)
                                    ConvertedList.Add(vid);
                            }
                        }
                        catch { }
                        ix++;
                    }
                }
                try
                {
                    Mss = null;
                    FFmpegMSS = null;
                }
                catch { }
            }
            catch (Exception /*ex*/){
            }
            IsConverting = false;
            //Visibility = Visibility.Collapsed;
            return ConvertedList;
        }

        async Task<StorageFile> ConvertVideo(StorageFile inputFile, Size? imageSize, Rect? rectSize)
        {
            try
            {
                var outputFile = await GenerateRandomOutputFile();

                if (inputFile != null && outputFile != null)
                {
                    MediaProfile = await MediaEncodingProfile.CreateFromFileAsync(inputFile);
                    FFmpegMSS = await FFmpegInteropMSS
                        .CreateFromStreamAsync(await inputFile.OpenReadAsync(), Helper.FFmpegConfig);

                    Mss = FFmpegMSS.GetMediaStreamSource();
                    if (!IsStoryVideo)
                    {
                        if (Mss.Duration.TotalSeconds > 59)
                        {
                            Transcoder.TrimStartTime = StartTime;
                            Transcoder.TrimStopTime = StopTime;
                        }


                        var max = Math.Max(FFmpegMSS.VideoStream.PixelHeight, FFmpegMSS.VideoStream.PixelWidth);
                        if (max > 1920)
                            max = 1920;
                        if (imageSize == null)
                        {
                            MediaProfile.Video.Height = (uint)max;
                            MediaProfile.Video.Width = (uint)max;
                        }
                        else
                        {
                            MediaProfile.Video.Height = (uint)imageSize.Value.Height;
                            MediaProfile.Video.Width = (uint)imageSize.Value.Width;
                        }
                    }
                    else
                    {
                        if (Mss.Duration.TotalSeconds > 14.9)
                        {
                            Transcoder.TrimStartTime = StartTime;
                            Transcoder.TrimStopTime = StopTime;
                        }
                        //var max = Math.Max(FFmpegMSS.VideoStream.PixelHeight, FFmpegMSS.VideoStream.PixelWidth);
                        var size = Helpers.AspectRatioHelper.GetAspectRatioX(FFmpegMSS.VideoStream.PixelWidth, FFmpegMSS.VideoStream.PixelHeight);
                        //if (max > 1920)
                        //    max = 1920;
                        MediaProfile.Video.Height = (uint)size.Height;
                        MediaProfile.Video.Width = (uint)size.Width;
                    }

                    var transform = new VideoTransformEffectDefinition
                    {
                        Rotation = MediaRotation.None,
                        OutputSize = imageSize.Value,
                        Mirror = MediaMirroringOptions.None,
                        CropRectangle = rectSize == null ? Rect.Empty : rectSize.Value
                    };
                    
                    Transcoder.AddVideoEffect(transform.ActivatableClassId, true, transform.Properties);

                    var preparedTranscodeResult = await Transcoder
                        .PrepareMediaStreamSourceTranscodeAsync(Mss,
                        await outputFile.OpenAsync(FileAccessMode.ReadWrite), MediaProfile);

                    Transcoder.VideoProcessingAlgorithm = MediaVideoProcessingAlgorithm.Default;
                    if (preparedTranscodeResult.CanTranscode)
                    {
                        var progress = new Progress<double>(ConvertProgress);
                        await preparedTranscodeResult.TranscodeAsync().AsTask(Cts.Token, progress);
                        ConvertComplete(outputFile);
                        return outputFile;
                    }
                    else
                        preparedTranscodeResult.FailureReason.ToString().ShowMsg();

                }
            }
            catch (Exception ex) { ex.PrintException().ShowMsg("ConvertVideo"); }
            return null;

        }
        bool IsStoryVideo = false;
        async Task<StorageFile> GenerateRandomOutputFile()
        {
            var folder = await KnownFolders.PicturesLibrary.GetFolderAsync(Helper.AppName);
            var outfile = await folder.CreateFileAsync(Helper.GenerateString("MINISTA_")
                + OutputExtension, CreationCollisionOption.GenerateUniqueName);
            return outfile;
        }

        void ConvertProgress(double percent)
        {
            Output("Converting... " + (int)percent + "%");
        }
        void ConvertComplete(StorageFile file)
        {
            Output("Convert completed.");
        }
        async void Output(string content)
        {
            await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                content.PrintDebug();
                //ResultText.Text = content;
            });
        }
        async void Text(string content)
        {
            await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                content.PrintDebug();
                //ContentText.Text = content;
            });
        }
    }
}
