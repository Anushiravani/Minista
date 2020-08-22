using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.UI.Xaml.Input;

namespace Minista.Views.Studio.Drawings
{
    interface IDrawingUI
    {      
        event DoubleTappedEventHandler DoubleTapped;

        CanvasDrawingSession StoredDrawingSession { get; }
        float StoredScale { get; }
        void Draw(CanvasDrawingSession graphics, float scale);
    }
}
