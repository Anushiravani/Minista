using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
//using Windows.Web.Http;

namespace Minista.Helpers
{
    public static class HttpHelper
    {
        private readonly static HttpClient Client = new HttpClient();

        public static async Task<Uri> DownloadFileAsync(Uri uri, string name, StorageFolder savedFolder)
        {
            try
            {
                var file = await savedFolder.CreateFileAsync(name);
                using(var result = await Client.GetAsync(uri))
                {
                    await FileIO.WriteBytesAsync(file, await result.Content.ReadAsByteArrayAsync());

                    //using (var filestream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    //{
                    //    await result.Content.WriteToStreamAsync(filestream);
                    //    await filestream.FlushAsync();
                    //}
                }
                return new Uri(file.Path, UriKind.RelativeOrAbsolute);
            }
            catch { }
            return uri;
        }
    }
}
