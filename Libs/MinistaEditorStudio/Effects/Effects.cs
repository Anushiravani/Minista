using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace MinistaEditorStudio.Effects
{
    public sealed class SharpenEffectX : IEffect
    {
        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 100;
        public SharpenEffect Effect { get; private set; }
        public ICanvasImage ApplyEffect(ICanvasImage source, float value)
        {
            Effect = new SharpenEffect
            {
                Source = source,
                Amount = (float)(value * 0.1)
            };
            return Effect;
        }
    }

    public sealed class BlurEffectX : IEffect
    {
        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 100;
        public GaussianBlurEffect Effect { get; private set; }
        public ICanvasImage ApplyEffect(ICanvasImage source, float value)
        {
            Effect = new GaussianBlurEffect
            {
                Source = source,
                BlurAmount = (float)(value / 100 * 12)
            };
            return Effect;
        }
    }

    public sealed class StraightenEffectX : IEffect
    {
        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 100;
        public StraightenEffect Effect { get; private set; }
        public ICanvasImage ApplyEffect(ICanvasImage source, float value)
        {
            Effect = new StraightenEffect
            {
                Source = source,
                Angle = (float)(value / 500 * 2),
                MaintainSize = true
            };
            return Effect;
        }
    }

    public sealed class ExposureEffectX : IEffect
    {  
        public double Minimum { get; set; } = -50;
        public double Maximum { get; set; } = 50;
        public ExposureEffect Effect { get; private set; }
        public ICanvasImage ApplyEffect(ICanvasImage source, float value)
        {
            Effect = new ExposureEffect
            {
                Source = source,
                Exposure = (float)(value / 500 * 2)
            };
            return Effect;
        }
    }

    public sealed class HighlightsAndShadowsEffectX : IEffect 
    {
        public double Minimum { get; set; } = -50;
        public double Maximum { get; set; } = 50;
        public HighlightsAndShadowsEffect Effect { get; private set; }
        public ICanvasImage ApplyEffect(ICanvasImage source, float value)
        {
            Effect = new HighlightsAndShadowsEffect
            {
                Source = source,
                //s = (float)(value / 500 * 2)
            };
            return Effect;
        }
    }
}
