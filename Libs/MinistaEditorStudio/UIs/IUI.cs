using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace MinistaEditorStudio.UIs
{
    public interface IUI 
    {
        void Draw(CanvasDrawingSession graphics, float scale = 1);
    }
}
