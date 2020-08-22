using FFmpegInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Minista
{
    static class FrameHelper
    {
        static FFmpegInteropMSS FFmpegMSS;
        
        public static async Task<TimeSpan> GetDurationAsync(this StorageFile file)
        {
            try
            {
                FFmpegMSS = await FFmpegInteropMSS
                                  .CreateFromStreamAsync(await file.OpenReadAsync(), Helper.FFmpegConfig);

                var MssTest = FFmpegMSS.GetMediaStreamSource();
                return FFmpegMSS.Duration;
            }
            catch { }
            finally
            {
                try
                {
                    FFmpegMSS.Dispose();
                    FFmpegMSS = null;
                }
                catch { }
            }
            return TimeSpan.Zero;
        }
        public static async Task<VideoStreamInfo> GetVideoInfoAsync(this StorageFile file)
        {
            try
            {
                FFmpegMSS = await FFmpegInteropMSS
                                  .CreateFromStreamAsync(await file.OpenReadAsync(), Helper.FFmpegConfig);

                var MssTest = FFmpegMSS.GetMediaStreamSource();
                return FFmpegMSS.VideoStream;
            }
            catch { }
            finally
            {
                try
                {
                    FFmpegMSS.Dispose();
                    FFmpegMSS = null;
                }
                catch { }
            }
            return null;
        }
    }
    public class Resolution
    {
        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;
        internal static Resolution Empty => new Resolution();
    }
}
