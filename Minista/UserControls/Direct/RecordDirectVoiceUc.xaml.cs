using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading;
using System.Threading.Tasks;
// using haye morede niaz
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.Render;
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.Media.MediaProperties;
using Windows.Media.Capture;
using Minista.Helpers.Uploaders;
using Windows.Media.Transcoding;

namespace Minista.UserControls.Direct
{
    public sealed partial class RecordDirectVoiceUc : UserControl
    {
        readonly DispatcherTimer RootTimer = new DispatcherTimer();

        //AudioGraph graph;
        //AudioFileOutputNode outputNode;
        private StorageFile file;
        private StorageFolder localFolder;
        private MediaCapture mediaCapture;
        //LowLagMediaRecording _mediaRecording;


        public bool IsWorking = false;
        public string ThreadId { get; private set; }
        private int Duration = 0;
        public void SetThreadId(string threadId) => ThreadId = threadId;

        public RecordDirectVoiceUc()
        {
            InitializeComponent();
            RootTimer.Interval = TimeSpan.FromSeconds(1);
            RootTimer.Tick += RootTimer_Tick;
            Loaded += RecordDirectVoiceUc_Loaded;
        }

        private void RootTimer_Tick(object sender, object e)
        {
            try
            {
                txtTime.Text = Duration.ToString("00");
                Duration++;
                if (Duration > 58)
                    Stop();
            }
            catch { }
        }

        private async void RecordDirectVoiceUc_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                localFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Cache");
            }
            catch { }
        }

        async void Start()
        {
            if (!IsWorking)
            {
                try
                {
                    if (mediaCapture != null)
                    {
                        try
                        {
                            mediaCapture.Failed -= MediaCapture_Failed;
                            mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
                        }
                        catch { }
                        try
                        {
                            mediaCapture.Dispose();
                            mediaCapture = null;
                        }
                        catch { }
                    }
                    Duration = 0;
                    mediaCapture = new MediaCapture();
                    await mediaCapture.InitializeAsync();
                    mediaCapture.Failed += MediaCapture_Failed;
                    mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
                    if (file != null)
                    {
                        try
                        {
                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                        catch { }
                    }
                    //var localFolder = ApplicationData.Current.LocalFolder;
                    var name = Helper.GenerateRandomString();
                    file = await localFolder.CreateFileAsync($"{name}.m4a", CreationCollisionOption.GenerateUniqueName);
                    //_mediaRecording = await mediaCapture.PrepareLowLagRecordToStorageFileAsync(
                    //        MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low), file);

                    await mediaCapture.StartRecordToStorageFileAsync(
                            MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Low), file);
                    IsWorking = true;
                    StopGrid.Visibility = Visibility.Visible;
                    RecordGrid.Visibility = Visibility.Collapsed;
                    RootTimer.Start();
                }
                catch (Exception ex) { ex.PrintException("Start"); }
            }
            //else
            //{
            //    IsWorking = false;
            //    RootTimer.Stop();

            //    //SearchButton.Content = SearchIcon;
            //    Stop();

            //}
        }
        private void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
           // ??
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs e)
        {
            ("MediaCapture_Failed: " + e.Message).PrintDebug();
            //throw new NotImplementedException();
        }
        VoiceUploader Uploader = new VoiceUploader();
        async void Stop(bool hide = false)
        {
            try
            {
                //ShowLoading();
                await mediaCapture.StopRecordAsync();
                //await _mediaRecording.StopAsync();
                IsWorking = false;
                RootTimer.Stop();
                //SearchButton.Content = SearchIcon;
                var fName = file.Name;
                Debug.WriteLine(fName);
                file = null;
                file = await localFolder.GetFileAsync(fName);
                //Uploader.UploadSingleVoice(file, ThreadId);
                Transcode(file, hide);
                //var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                //var datas = await stream.AsStream().ToByteArray();
                //try
                //{
                //    await file.DeleteAsync();
                //}
                //catch (Exception ex) { Debug.WriteLine("ex while deleting file: " + ex.Message); }


            }
            catch (Exception ex) { ex.PrintException("Upload").ShowMsg("ERR"); }
        }
        public async void Hide()
        {
            try
            {
                Visibility = Visibility.Collapsed;
                file = null;
                StopGrid.Visibility =  Visibility.Collapsed;
                RecordGrid.Visibility = Visibility.Visible;

                if (mediaCapture != null)
                    await mediaCapture.StopRecordAsync();

                RootTimer.Stop();
            }
            catch { }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => Hide();

        private void RecordButtonClick(object sender, RoutedEventArgs e) => Start();


        private void StopAndSendButtonClick(object sender, RoutedEventArgs e)
        {
            Stop();
            //Hide(true);
        }

        MediaTranscoder _Transcoder = new MediaTranscoder();
        MediaEncodingProfile _Profile;
        CancellationTokenSource _cts;
        StorageFile _OutputFile;
        async void Transcode(StorageFile input, bool hide = false)
        {
            try
            {
                _cts = new CancellationTokenSource();
                _Profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
                //_Profile.Video.Width = 720;
                //_Profile.Video.Height = 1080;
                //_Profile.Video.Bitrate = 70000;

                //_Profile.Audio.Bitrate = 96000;
                //_Profile.Audio.SampleRate = 44100;
                //_Profile.Audio.
                _OutputFile = await localFolder.CreateFileAsync(13.GenerateRandomStringStatic() +".mp4", CreationCollisionOption.GenerateUniqueName);

                var preparedTranscodeResult = await _Transcoder.PrepareFileTranscodeAsync(input, _OutputFile, _Profile);
                Hide();
                _Transcoder.VideoProcessingAlgorithm = MediaVideoProcessingAlgorithm.Default;
                if (preparedTranscodeResult.CanTranscode)
                {
                    var progress = new Progress<double>(TranscodeProgress);
                    //stopwatch.Reset();
                    //stopwatch.Start();
                    //MASettings.GPVisibility = Visibility.Visible;
                    //StopRadioButton.Visibility = Visibility.Visible;
                    await preparedTranscodeResult.TranscodeAsync().AsTask(_cts.Token, progress);
                }
                else
                {
                    ("Failed to start.\r\nError: " + preparedTranscodeResult.FailureReason).PrintDebug();

                }
            }
            catch { }
        }
        void TranscodeProgress(double percent)
        {
            try
            {
                Debug.WriteLine($"{(int)percent}%");
                if ((int)percent == 100)
                    Uploader.UploadSingleVoice(_OutputFile, ThreadId);
            }
            catch { }
        }
    }
}
