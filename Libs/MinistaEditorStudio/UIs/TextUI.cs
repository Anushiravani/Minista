using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
 
namespace MinistaEditorStudio.UIs
{
    public class TextUI : IUI
    {
        public CanvasTextFormat TextFormat { get; set; } = new CanvasTextFormat { FontSize = 11.0f , WordWrapping = CanvasWordWrapping.NoWrap};
        public Color TextColor { get; set; } = Colors.White;
        public double X { get; set; }
        public double Y { get; set; }
        public string Text { get; set; }
        public Size Bound { get; set; }
        private Rect _region;
        public Rect Region => _region;
        private Rect _close_region;
        public Rect CloseRegion => _close_region;
        public bool ShowCloseBtn { get; set; }
        public void Draw(CanvasDrawingSession graphics, float scale)
        {
            var x = X * scale;
            var y = Y * scale;

            var radius = 4;
            var color = Color.FromArgb(200, 0xFF, 0xFF, 0xFF);
            var color2 = Color.FromArgb(200, 0x00, 0x00, 0x00);

            graphics.FillCircle((float)x, (float)y, radius, color);
            graphics.DrawCircle((float)x, (float)y, radius, color2);
            var ctLayout = new CanvasTextLayout(graphics, Text, TextFormat, 0.0f, 0.0f);
            var width = ctLayout.DrawBounds.Width + 10 * scale;
            var height = ctLayout.DrawBounds.Height + 12 * scale;

            var w = x + width + 10 * scale + height;
            var pathBuilder = new CanvasPathBuilder(graphics);


            if (w > Bound.Width * scale)  //Beyond Canvas Boundaries
            {
                pathBuilder.BeginFigure((float)x - 5 * scale, (float)y);
                pathBuilder.AddLine((float)x - 5 * scale - 6 * scale, (float)y - (float)height / 2);
                pathBuilder.AddLine((float)x - (float)width - 5 * scale - 6 * scale, (float)y - (float)height / 2);
                pathBuilder.AddLine((float)x - (float)width - 5 * scale - 6 * scale, (float)y + (float)height / 2);
                pathBuilder.AddLine((float)x - 5 * scale - 6 * scale, (float)y + (float)height / 2);

                pathBuilder.EndFigure(CanvasFigureLoop.Closed);

                var geometry = CanvasGeometry.CreatePath(pathBuilder);

                graphics.FillGeometry(geometry, color2);
                graphics.DrawText(Text, (float)x - (float)width - 5 * scale - 6 * scale + 5 * scale, (float)y - (float)height / 2 + 4 * scale, TextColor, TextFormat);


                _region = new Rect((float)x - (float)width - 5 * scale - 6 * scale, (float)y - (float)height / 2, width, height);
                _close_region = new Rect((float)x - (float)width - 5 * scale - 6 * scale - height, (float)y - (float)height / 2, height, height);

                if (ShowCloseBtn && scale == 1) 
                {
                    graphics.FillRectangle(_close_region, color2);
                    graphics.DrawLine((float)_close_region.Left, (float)_close_region.Top, (float)_close_region.Left, (float)_close_region.Top + (float)height, Colors.White, 0.5f);
                    graphics.DrawText("×", (float)_close_region.Left + 6, (float)_close_region.Top, Colors.White, new CanvasTextFormat() { FontSize = 15 });
                }
            }
            else
            {
                pathBuilder.BeginFigure((float)x + 5 * scale, (float)y);
                pathBuilder.AddLine((float)x + 5 * scale + 6 * scale, (float)y - (float)height / 2);
                pathBuilder.AddLine((float)x + (float)width + 5 * scale + 6 * scale, (float)y - (float)height / 2);
                pathBuilder.AddLine((float)x + (float)width + 5 * scale + 6 * scale, (float)y + (float)height / 2);
                pathBuilder.AddLine((float)x + 5 * scale + 6 + scale, (float)y + (float)height / 2);

                pathBuilder.EndFigure(CanvasFigureLoop.Closed);

                var geo = CanvasGeometry.CreatePath(pathBuilder);

                //graphics.FillGeometry(geo, color2);
                graphics.DrawText(Text, (float)x + 5 * scale + 4 * scale + 5 * scale, (float)y - (float)height / 2 + 4 * scale, TextColor, TextFormat);

                _region = new Rect((float)x + 5 * scale + 6 * scale, (float)y - (float)height / 2, width, height);
                Debug.WriteLine("REGION: " + $"x:{_region.X}  y:{_region.Y} | {_region.Width}x{_region.X} | {_region.Left}\t{_region.Top}" +
                    $"\t{_region.Right}\t{_region.Bottom}");
                _close_region = new Rect((float)x + (float)width + 5 * scale + 6 * scale, (float)y - (float)height / 2, height, height);

                if (ShowCloseBtn && scale == 1) 
                {
                    graphics.FillRectangle(_close_region, color2);
                    graphics.DrawLine((float)_close_region.Left, (float)_close_region.Top, (float)_close_region.Left, (float)_close_region.Top + (float)height, Colors.White, 0.5f);
                    graphics.DrawText("×", (float)_close_region.Left + 6, (float)_close_region.Top, Colors.White, new CanvasTextFormat() { FontSize = 15 });
                }
            }

        }
    }
}
