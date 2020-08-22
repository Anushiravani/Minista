using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MinistaEditorStudio
{
    static class Helper
    {
        public const string AppName = "Minista";
        public const string FolderToken = "MinistaFTokenRmt";
        public static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        public static readonly string[] SupportedImageTypes = new string[] { ".jpg", ".jpeg", ".png" };
        public static readonly string[] SupportedVideoTypes = new string[] { ".mp4" };
        public static readonly string[] SupportedExtraVideoTypes = new string[]
        {
            ".mpeg",
            ".mpeg4",
            ".mov",
            ".mkv",
            ".m4v",
            ".avi",
            ".3gp",
            ".wmv"
        };
        public static readonly Random Rnd = new Random();

        public static async Task<StorageFolder> GetOutputFolder()
        {
            try
            {
                return await KnownFolders.PicturesLibrary.GetFolderAsync(AppName);
            }
            catch { }
            return await LocalFolder.GetFolderAsync("Cache");
        }
        public static async Task<StorageFile> CreateRandomFile()
        {
            var folder = await GetOutputFolder(true);
            var saveAsTarget = await folder.CreateFileAsync(GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
            return saveAsTarget;
        }
        public static async Task<StorageFolder> GetOutputFolder(bool isCahed)
        {
            try
            {
                if (isCahed)
                    return await LocalFolder.GetFolderAsync("Cache");
                return await KnownFolders.PicturesLibrary.GetFolderAsync(AppName);
            }
            catch { }
            return await LocalFolder.GetFolderAsync("Cache");
        }


        #region Get Color from Hex
        public static SolidColorBrush GetColorBrush(this string hexColorString)
        {
            return new SolidColorBrush(GetColorFromHex(hexColorString));
        }
        private static readonly Regex _hexColorMatchRegex = new Regex("^#?(?<a>[a-z0-9][a-z0-9])?(?<r>[a-z0-9][a-z0-9])(?<g>[a-z0-9][a-z0-9])(?<b>[a-z0-9][a-z0-9])$", RegexOptions.IgnoreCase);
        public static Color GetColorFromHex(this string hexColorString)
        {
            if (hexColorString == null)
                throw new NullReferenceException("Hex string can't be null.");


            var match = _hexColorMatchRegex.Match(hexColorString);


            if (!match.Success)
                throw new InvalidCastException(string.Format("Can't convert string \"{0}\" to argb or rgb color. Needs to be 6 (rgb) or 8 (argb) hex characters long. It can optionally start with a #.", hexColorString));
            byte a = 255;
            if (match.Groups["a"].Success)
                a = Convert.ToByte(match.Groups["a"].Value, 16);
            byte r = Convert.ToByte(match.Groups["r"].Value, 16);
            byte b = Convert.ToByte(match.Groups["b"].Value, 16);
            byte g = Convert.ToByte(match.Groups["g"].Value, 16);
            return Color.FromArgb(a, r, g, b);
        }
        #endregion

        #region Random string generator
        public static string GenerateRandomStringStatic(this int length)
        {
            return GenerateRandomString(length);
        }
        public static string GenerateRandomString(int length = 10)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[Rnd.Next(0, pool.Length)]);
            return AppName + new string(chars.ToArray());
        }
        public static string GenerateString()
        {
            return $"{AppName}_{DateTime.Now:yyyyMMdd_hhmmss}";
        }
        public static string GenerateString(string name)
        {
            return $"[{AppName}] {name}_{DateTime.Now:yyyyMMdd_hhmmss}";
        }
        #endregion  Random string generator

        #region DEBUG
        public static string PrintDebug(this object obj)
        {
            var content = Convert.ToString(obj);
            Debug.WriteLine(content);
            return content;
        }
        public static string PrintException(this Exception ex, string name = null)
        {
            var sb = new StringBuilder();
            if (name == null)
                sb.AppendLine("Exeption thrown:");
            else sb.AppendLine($"Exeption thrown in '{name}': ");
            sb.AppendLine(ex.Message);
            sb.AppendLine("Source: " + ex.Source);
            sb.AppendLine("StackTrace: " + ex.StackTrace);
            sb.AppendLine();
            var content = sb.PrintDebug();
            return content;
        }
        #endregion DEBUG


        #region Check supported extensions
        public static bool NeedsConvert(this StorageFile file)
        {
            return file.Path.NeedsConvert();
        }
        public static bool NeedsConvert(this string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return SupportedExtraVideoTypes.Contains(ext);
        }
        public static bool IsVideo(this StorageFile file)
        {
            return file.Path.NeedsConvert() || file.Path.IsVideo();
        }
        public static bool IsVideo(this string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return SupportedVideoTypes.Contains(ext) || path.NeedsConvert();
        }
        public static bool IsImage(this StorageFile file)
        {
            var ext = Path.GetExtension(file.Path).ToLower();
            return SupportedImageTypes.Contains(ext);
        }
        public static bool IsSupported(this StorageFile file)
        {
            return file.IsVideo() || file.IsImage();
        }
        #endregion Check supported extensions

        public static uint GetHeight(this ImageProperties props)
        {
            return props.Height;
            //return props.Orientation == PhotoOrientation.Rotate180 ? props.Height : props.Width;
        }

        public static uint GetWidth(this ImageProperties props)
        {
            return props.Width;
            //return props.Orientation == PhotoOrientation.Rotate180 ? props.Width : props.Height;
        }



        public static uint GetHeight(this VideoProperties props)
        {
            return props.Orientation == VideoOrientation.Rotate180 || props.Orientation == VideoOrientation.Normal ? props.Height : props.Width;
        }

        public static uint GetWidth(this VideoProperties props)
        {
            return props.Orientation == VideoOrientation.Rotate180 || props.Orientation == VideoOrientation.Normal ? props.Width : props.Height;
        }
        public async static void CreateCachedFolder()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cache", CreationCollisionOption.FailIfExists);
            }
            catch { }
        }
    }
}
