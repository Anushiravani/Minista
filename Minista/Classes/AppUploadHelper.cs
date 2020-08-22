using Minista.Helpers;
using Minista.Views.Uploads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minista.Classes
{
    static class AppUploadHelper
    {
        public static List<StorageUpload> Uploaders = new List<StorageUpload>();
        public static List<PhotoUploaderHelper> SinglePhotoUploads = new List<PhotoUploaderHelper>();
    }
}
