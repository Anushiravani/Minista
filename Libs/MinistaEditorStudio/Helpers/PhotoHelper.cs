//using Lumia.Imaging;
using Lumia.Imaging;
using Microsoft.Graphics.Canvas;
using MinistaEditorStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Minista.Helpers
{
    public class PhotoHelper : IDisposable
    {
        private static async Task<StorageFile> CropImageAsync(StorageFile sourceFile, StorageFile resizedImageFile)
        {
            var writeableBitmap = new WriteableBitmap(1, 1);
            using (var stream = await sourceFile.OpenReadAsync())
                await writeableBitmap.SetSourceAsync(stream);
            using (var resizedStream = await resizedImageFile.OpenAsync(FileAccessMode.ReadWrite))
            using (var sourceStream = writeableBitmap.PixelBuffer.AsStream())
            {
                var buffer = new byte[sourceStream.Length];
                await sourceStream.ReadAsync(buffer, 0, buffer.Length);
                var bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, resizedStream);
                bitmapEncoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight, 96.0, 96.0, buffer);
                await bitmapEncoder.FlushAsync();
            }
            return resizedImageFile;
        }
        private async Task<StorageFile> RescaleImage(StorageFile sourceFile, StorageFile resizedImageFile, uint width, uint height)
        {
            var writeableBitmap = new WriteableBitmap(1, 1);
            using (var stream = await sourceFile.OpenReadAsync())
                await writeableBitmap.SetSourceAsync(stream);
            using (var imageStream = await sourceFile.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(imageStream);
                using (var sourceStream = writeableBitmap.PixelBuffer.AsStream())
                using (var resizedStream = await resizedImageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var buffer = new byte[sourceStream.Length];
                    await sourceStream.ReadAsync(buffer, 0, buffer.Length);
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, resizedStream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, width, height, 96.0, 96.0, buffer);

                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;
                    encoder.BitmapTransform.ScaledWidth = width;
                    encoder.BitmapTransform.ScaledHeight = height;
                    await encoder.FlushAsync();
                }
            }
            return resizedImageFile;
        }
        /*
        public async Task<StorageFile> SaveToImageForPost(StorageFile file)
        {
            try
            {
                Helper.CreateCachedFolder();
                //{"message": "Uploaded image isn't in an allowed aspect ratio", "status": "fail"}
                double width = 0, height = 0;
                using (var source = new StorageFileImageSource(file))
                using (var renderer = new JpegRenderer(source, JpegOutputColorMode.Yuv420, OutputOption.PreserveAspectRatio))
                {
                    var info = await source.GetInfoAsync();
                    width = info.ImageSize.Width;
                    height = info.ImageSize.Height;
                    bool tried = false;
                    bool needsAttention = false;
                LBLTRY:
                    var wRatio = AspectRatioHelper.GetAspectRatioForMedia(width, height);
                    var hRatio = AspectRatioHelper.GetAspectRatioForMedia(height, width);




                    if (width > 320)
                    {
                        var widthScale = height / hRatio;
                        var heightScale = width / wRatio;


                        height = widthScale;
                        var folder = await GetOutputFolder();
                        var saveAsTarget = await folder.CreateFileAsync(Helper.GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
                        return await RescaleImage(file, saveAsTarget, (uint)width, (uint)height);

                    }
                    else
                    {
                        var widthScale = height * 2;
                        var heightScale = width * 2;



                        width = widthScale;
                        height = heightScale;
                        var folder = await GetOutputFolder();
                        var saveAsTarget = await folder.CreateFileAsync(Helper.GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
                        return await RescaleImage(file, saveAsTarget, (uint)width, (uint)height);
                    }
                    ($"{info.ImageSize.Width}x{info.ImageSize.Height}\t\t=>>>\t\t{width}x{height}").PrintDebug();
                    renderer.Size = new Size(width, height);


                }
            }
            catch { }
            return file;
        }
        public async Task<StorageFile> SaveToImage(StorageFile file, bool calculateMax = true, bool isCached = true)
        {
            try
            {
                Helper.CreateCachedFolder();
                //{"message": "Uploaded image isn't in an allowed aspect ratio", "status": "fail"}
                using (var source = new StorageFileImageSource(file))
                using (var renderer = new JpegRenderer(source, JpegOutputColorMode.Yuv420, OutputOption.Stretch))
                {
                    var info = await source.GetInfoAsync();
                    var size = AspectRatioHelper.GetAspectRatioX2(info.ImageSize.Width, info.ImageSize.Height);
                    var max = Math.Max(size.Height, size.Width);
                    if (calculateMax)
                        renderer.Size = new Size(max, max);
                    else
                        renderer.Size = new Size(info.ImageSize.Width, info.ImageSize.Height);
                    var folder = await GetOutputFolder(isCached);
                    var saveAsTarget = await folder.CreateFileAsync(Helper.GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
                    var render = await renderer.RenderAsync();
                    using (var fs = await saveAsTarget.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await fs.WriteAsync(render);
                        await fs.FlushAsync();
                        return saveAsTarget;
                    }
                }
            }
            catch { }
            return file;
        }*/
        public async Task<StorageFile> SaveToImage(StorageFile file, bool calculateMax = true, bool isCached = true)
        {
            try
            {
                Helper.CreateCachedFolder();
                //{"message": "Uploaded image isn't in an allowed aspect ratio", "status": "fail"}
                using (var source = new StorageFileImageSource(file))
                using (var renderer = new JpegRenderer(source, JpegOutputColorMode.Yuv420, OutputOption.Stretch))
                {
                    var info = await source.GetInfoAsync();
                    var size = AspectRatioHelper.GetAspectRatioX2(info.ImageSize.Width, info.ImageSize.Height);
                    var max = Math.Max(size.Height, size.Width);
                    if (calculateMax)
                        renderer.Size = new Size(size.Width, size.Height);
                    else
                        renderer.Size = new Size(info.ImageSize.Width, info.ImageSize.Height);
                    var folder = await GetOutputFolder(isCached);
                    var saveAsTarget = await folder.CreateFileAsync(Helper.GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
                    var render = await renderer.RenderAsync();
                    using (var fs = await saveAsTarget.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await fs.WriteAsync(render);
                        await fs.FlushAsync();
                        return saveAsTarget;
                    }
                }
            }
            catch { }
            return file;
        }

        public async Task<StorageFile> SaveToImageX(StorageFile file, bool calculateMax = true, bool isCached = true)
        {
            try
            {
                Helper.CreateCachedFolder();
                //{"message": "Uploaded image isn't in an allowed aspect ratio", "status": "fail"}
                using (var source = new StorageFileImageSource(file))
                using (var renderer = new JpegRenderer(source, JpegOutputColorMode.Yuv422, OutputOption.Stretch))
                {
                    var info = await source.GetInfoAsync();
                    var size = AspectRatioHelper.GetDesireSize(info.ImageSize.Width, info.ImageSize.Height, info.ImageSize.Height < info.ImageSize.Width);
                    //var max = Math.Max(size.Height, size.Width);
                    var ratio = info.ImageSize.Height > info.ImageSize.Width ? info.ImageSize.Height / info.ImageSize.Width : info.ImageSize.Width / info.ImageSize.Height;
                    var h = (size.Height / (float)ratio);
                    var w = (size.Width / (float)ratio);

                    if (calculateMax)
                        renderer.Size = new Size(Math.Round(w), Math.Round(h));
                    else
                        renderer.Size = new Size(info.ImageSize.Width, info.ImageSize.Height);
                    var folder = await GetOutputFolder(isCached);
                    var saveAsTarget = await folder.CreateFileAsync(Helper.GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
                    var render = await renderer.RenderAsync();
                    using (var fs = await saveAsTarget.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await fs.WriteAsync(render);
                        await fs.FlushAsync();
                        return await SaveToImageX2(saveAsTarget);
                    }
                }
            }
            catch { }
            return file;
        }
        public async Task<StorageFile> SaveToImageX2(StorageFile file)
        {
            try
            {
                Helper.CreateCachedFolder();
                var folder = await GetOutputFolder(true);
                var saveAsTarget = await folder.CreateFileAsync(Helper.GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
                return await CropImageAsync(file, saveAsTarget);
            }
            catch { }
            return file;
        }

        public async Task<StorageFile> SaveToImageXx(StorageFile file, bool calculateMax = true, bool isCached = true)
        {
            try
            {
                Helper.CreateCachedFolder();

                /*using (*/
                CanvasDevice device = CanvasDevice.GetSharedDevice();
                /*using (*/
                var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                /*using (*/
                var image = await CanvasBitmap.LoadAsync(device, stream);
                {
                    //var size = AspectRatioHelper.GetAspectRatioX2(image.ImageSize.Width, image.ImageSize.Height);
                    //var max = Math.Max(size.Height, size.Width);
                    //if (calculateMax)
                    //    image.Size = new Size(max, max);
                    //else
                    //    image.Size = new Size(info.ImageSize.Width, info.ImageSize.Height);
                    var folder = await GetOutputFolder(isCached);
                    var saveAsTarget = await folder.CreateFileAsync(Helper.GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
                    var targetStream = await saveAsTarget.OpenAsync(FileAccessMode.ReadWrite);

                    float dpiLimit = 96.0f;
                    if (image.Dpi > dpiLimit)
                    {
                        dpiLimit = dpiLimit / image.Dpi;
                    }
                    await image.SaveAsync(targetStream, CanvasBitmapFileFormat.Jpeg, dpiLimit);
                    try
                    {
                        await targetStream.FlushAsync();
                    }
                    catch { }
                    try
                    {
                        targetStream.Dispose();
                        targetStream = null;
                    }
                    catch { }
                    return saveAsTarget;
                }
            }
            catch { }
            return file;
        }
        async Task<StorageFolder> GetOutputFolder()
        {
            try
            {
                return await KnownFolders.PicturesLibrary.GetFolderAsync(Helper.AppName);
            }
            catch { }
            return await ApplicationData.Current.LocalFolder.GetFolderAsync("Cache");
        }
        public async Task<StorageFile> CreateRandomFile()
        {
            var folder = await GetOutputFolder(true);
            var saveAsTarget = await folder.CreateFileAsync(Helper.GenerateString("IMG") + ".jpg", CreationCollisionOption.GenerateUniqueName);
            return saveAsTarget;
        }
        async Task<StorageFolder> GetOutputFolder(bool isCahed)
        {
            try
            {
                if (isCahed)
                    return await ApplicationData.Current.LocalFolder.GetFolderAsync("Cache");
                return await KnownFolders.PicturesLibrary.GetFolderAsync(Helper.AppName);
            }
            catch { }
            return await ApplicationData.Current.LocalFolder.GetFolderAsync("Cache");
        }
        public void Dispose()
        {
            GC.Collect();
        }
    }
}
