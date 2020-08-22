using Windows.Graphics.Effects;
using Windows.UI.Composition;

namespace UICompositionAnimations.Helpers.Cache
{
    /// <summary>
    /// A simple class that holds information on a <see cref="CompositionBackdropBrush"/> instance and its effects pipeline
    /// </summary>
    internal sealed class HostBackdropInstanceWrapper
    {
        /// <summary>
        /// Gets the partial pipeline with the host backdrop effect
        /// </summary>
        
        public IGraphicsEffectSource Pipeline { get; }

        /// <summary>
        /// Gets the host backdrop effect brush instance
        /// </summary>
        
        public CompositionBackdropBrush Brush { get; }

        /// <summary>
        /// Creates a new wrapper instance with the given parameters
        /// </summary>
        /// <param name="pipeline">The current effects pipeline</param>
        /// <param name="brush">The host backdrop brush instance</param>
        public HostBackdropInstanceWrapper( IGraphicsEffectSource pipeline,  CompositionBackdropBrush brush)
        {
            Pipeline = pipeline;
            Brush = brush;
        }
    }
}
