using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Minista.Helpers;
using Minista.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MinistaEditorStudio
{
    public sealed partial class EditorStudio : UserControl
    {
        private double AspectRatio { get; set; } = 1.62d;
        private double MinRange { get; set; } = 1.91d;
        private double MaxRange { get; set; } = 0.80d;
        public event EventHandler<StorageFile> OnDone;
        private StorageFile OriginalFile;
        private StorageFile EditedFile; 
        public StorageFile OutputFile { get; set; }

        Rect CurrentCroppedRectForVideo;
        #region Dependecies
        public EditorMode EditorMode
        {
            get { return (EditorMode)GetValue(EditorModeProperty); }
            set
            {
                SetValue(EditorModeProperty, value);
                switch (value)
                {
                    case EditorMode.Media:
                        MinRange = 0.80d;
                        MaxRange = 1.91d;
                        AspectRatio = 1.62d;
                        break;
                    case EditorMode.Story:
                        MinRange = 0.50d;
                        MaxRange = 0.9d;
                        AspectRatio = 1.62d; 
                        break;
                }
                InvalidateSlider();
            }
        }
        public static readonly DependencyProperty EditorModeProperty =
            DependencyProperty.Register(
                "EditorMode",
                typeof(EditorMode),
                typeof(EditorStudio),
                new PropertyMetadata(EditorMode.Media));
        #endregion

        private int filterIndex = 0;
        public EditorStudio()
        {
            this.InitializeComponent();
            Loaded += EditorStudioLoaded;
            Window.Current.SizeChanged += CurrentSizeChanged;
        }

        private void EditorStudioLoaded(object sender, RoutedEventArgs e)
        {
            InvalidateSlider();
        }

        public async Task LoadFile(StorageFile file, bool innerCall = false)
        {
            if (!innerCall)
                OriginalFile = file;

            try
            {
                EditedFile = await new PhotoHelper().SaveToImage(file, false);
                AspectRatioSlider.Value = AspectRatio;
                ImageCropper.AspectRatio = AspectRatio;
                ShowCropper(innerCall);
                ImageCropper.CropShape = CropShape.Rectangular;
                await ImageCropper.LoadImageFromFile(EditedFile);

            }
            catch { }
        }
        void ShowImagePreview(StorageFile file)
        {
            try
            {
                MainCanvas.Visibility = Visibility.Visible;
                CropGrid.Visibility = Visibility.Collapsed;
                MenuGrid.Visibility = Visibility.Visible;
                Show(file);
            }
            catch { }
        }
        void ShowCropper(bool innerCall = false)
        {
            try
            {
                MainCanvas.Visibility = Visibility.Collapsed;
                CropGrid.Visibility = Visibility.Visible;
                MenuGrid.Visibility = Visibility.Collapsed;
                if (innerCall)
                    CropCancelButton.Visibility = Visibility.Visible;
                else
                    CropCancelButton.Visibility = Visibility.Collapsed;
            }
            catch { }
        }
        private void MainCanvasDraw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {

        }

        private void AspectRatioSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            try
            {
                if (AspectRatioSlider.Value != -1)
                    ImageCropper.AspectRatio = AspectRatioSlider.Value;
            }
            catch { }
        }

        private void CropButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {

        }


        #region Canvas

        private void CurrentSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            SetCanvas();
            MainCanvas.Invalidate();
        }
        private CanvasBitmap _image;
        public async void Show(StorageFile image)
        {
            try
            {
                Show();
                //WaitLoading.IsActive = true;
                CanvasDevice cd = CanvasDevice.GetSharedDevice();
                var stream = await image.OpenAsync(FileAccessMode.Read);
                _image = await CanvasBitmap.LoadAsync(cd, stream);
                //WaitLoading.IsActive = false;
                MainCanvas.Invalidate();
            }
            catch
            {
            }
        }
        private void Show()
        {
            SetCanvas();
        }
        /// <summary>
        /// Canvas drawing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MainCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            var target = GetDrawings(true);
            if (target != null)
            {
                args.DrawingSession.DrawImage(target);
            }
        }
        private void SetCanvas()
        {
            try
            {

                var w = MainWorkSapce.ActualWidth - 40;
                var h = MainWorkSapce.ActualHeight - 40;

                MainCanvas.Width = w;
                MainCanvas.Height = h;
                MainCanvas.Invalidate();
            }
            catch { }
        }
        private async void GenerateResultImage()
        {
            var img = GetDrawings(false);
            if (img != null)
            {
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                await img.SaveAsync(stream, CanvasBitmapFileFormat.Jpeg);
                BitmapImage result = new BitmapImage();
                stream.Seek(0);
                await result.SetSourceAsync(stream);
            }
        }
        private Rect GetImageDrawingRect()
        {
            Rect des;

            var image_w = _image.Size.Width;
            var image_h = _image.Size.Height;


            var w = MainCanvas.Width - 20;
            var h = MainCanvas.Height - 20;
            if (image_w / image_h > w / h)
            {
                var left = 10;

                var width = w;
                var height = (image_h / image_w) * width;

                var top = (h - height) / 2 + 10;

                des = new Rect(left, top, width, height);
            }
            else
            {
                var top = 10;
                var height = h;
                var width = (image_w / image_h) * height;
                var left = (w - width) / 2 + 10;
                des = new Rect(left, top, width, height);
            }
            return des;
        }
        private CanvasRenderTarget GetDrawings(bool edit)
        {
            double w, h;
            if (edit)
            {
                w = MainCanvas.ActualWidth;
                h = MainCanvas.ActualHeight;
            }
            else
            {
                Rect des = GetImageDrawingRect();

                w = (_image.Size.Width / des.Width) * MainCanvas.Width;
                h = (_image.Size.Height / des.Height) * MainCanvas.Height;
            }
            var scale = edit ? 1 : w / MainCanvas.Width;

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget target = new CanvasRenderTarget(device, (float)w, (float)h, 96);
            using (CanvasDrawingSession graphics = target.CreateDrawingSession())
                DrawBackImage(graphics, scale);

            return target;
        }
        private void DrawBackImage(CanvasDrawingSession graphics, double scale)
        {
            if (_image != null)
            {
                Rect des = GetImageDrawingRect();
                des.X *= scale;
                des.Y *= scale;
                des.Width *= scale;
                des.Height *= scale;

                //ICanvasImage image = GetBrightnessEffect(_image);
                var image = ApplyFilterTemplate(_image);

                graphics.DrawImage(image, des, _image.Bounds);
            }
        }

        private ICanvasImage ApplyFilterTemplate(ICanvasImage source)
        {
            return source;
        }

        #endregion



        #region Canvas Filters

        private ICanvasImage ApplyFilter(ICanvasImage source)
        {
            if (filterIndex == 0) // NONE
                return source;
            else if (filterIndex == 1)  
            {
                return new GrayscaleEffect
                {
                    Source = source
                };
            }
            else if (filterIndex == 2)
            {
                return new InvertEffect
                {
                    Source = source
                };
            }
            else if (filterIndex == 3) 
            {
                var hueRotationEffect = new HueRotationEffect
                {
                    Source = source,
                    Angle = 0.5f
                };
                return hueRotationEffect;
            }
            else if (filterIndex == 4) 
            {
                var temperatureAndTintEffect = new TemperatureAndTintEffect
                {
                    Source = source
                };
                temperatureAndTintEffect.Temperature = 0.6f;
                temperatureAndTintEffect.Tint = 0.6f;

                return temperatureAndTintEffect;
            }
            else if (filterIndex == 5)
            {
                var temperatureAndTintEffect = new TemperatureAndTintEffect
                {
                    Source = source
                };
                temperatureAndTintEffect.Temperature = -0.6f;
                temperatureAndTintEffect.Tint = -0.6f;

                return temperatureAndTintEffect;
            }
            else if (filterIndex == 6)
            {
                var vignetteEffect = new VignetteEffect
                {
                    Source = source
                };
                vignetteEffect.Color = Color.FromArgb(255, 0xFF, 0xFF, 0xFF);
                vignetteEffect.Amount = 0.6f;

                return vignetteEffect;
            }
            else if (filterIndex == 7)
            {
                var embossEffect = new EmbossEffect
                {
                    Source = source
                };
                embossEffect.Amount = 5;
                embossEffect.Angle = 0;
                return embossEffect;
            }
            else if (filterIndex == 8)
            {
                var sepiaEffect = new SepiaEffect
                {
                    Source = source
                };
                sepiaEffect.Intensity = 1;
                return sepiaEffect;
            }
            else // NONE
                return source;
        }

        #endregion Canvas Filters

        void Done(StorageFile file)
        {
            OutputFile = file;
            Done();
        }
        void Done()
        {
            OnDone?.Invoke(this, OutputFile);
        }


        private void InvalidateSlider()
        {
            try
            {
                if (AspectRatioSlider != null)
                {
                    AspectRatioSlider.Maximum = MaxRange;
                    AspectRatioSlider.Minimum = MinRange;
                    AspectRatioSlider.Value = AspectRatio;
                }
            }
            catch { }
        }

        private void CropVisibilityButtonClick(object sender, RoutedEventArgs e)
        {
            ShowCropper(true);
        }
    }
}
