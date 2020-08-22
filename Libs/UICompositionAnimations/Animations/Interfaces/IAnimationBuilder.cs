using System;
using System.Numerics;
using System.Threading.Tasks;
using UICompositionAnimations.Enums;

namespace UICompositionAnimations.Animations.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> that represents an animation builder that uses a specific set of APIs
    /// </summary>
    public interface IAnimationBuilder
    {
        /// <summary>
        /// Schedules an opacity animation
        /// </summary>
        /// <param name="to">The target opacity value to animate to</param>
        /// <param name="ease">The easing function to use for the opacity animation</param>
        IAnimationBuilder Opacity(double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules an opacity animation
        /// </summary>
        /// <param name="from">The opacity value to animate from</param>
        /// <param name="to">The target opacity value to animate to</param>
        /// <param name="ease">The easing function to use for the opacity animation</param>
        IAnimationBuilder Opacity(double from, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a translation animation
        /// </summary>
        /// <param name="axis">The translation axis to animate</param>
        /// <param name="to">The target translation value to animate to</param>
        /// <param name="ease">The easing function to use for the translation animation</param>
        
        IAnimationBuilder Translation(Axis axis, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a translation animation
        /// </summary>
        /// <param name="axis">The translation axis to animate</param>
        /// <param name="from">The initial translation value to animate from</param>
        /// <param name="to">The target translation value to animate to</param>
        /// <param name="ease">The easing function to use for the translation animation</param>
        
        IAnimationBuilder Translation(Axis axis, double from, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a translation animation
        /// </summary>
        /// <param name="to">The target translation position to animate to</param>
        /// <param name="ease">The easing function to use for the translation animation</param>
        
        IAnimationBuilder Translation(Vector2 to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a translation animation
        /// </summary>
        /// <param name="from">The initial translation position to animate from</param>
        /// <param name="to">The target translation position to animate to</param>
        /// <param name="ease">The easing function to use for the translation animation</param>
        
        IAnimationBuilder Translation(Vector2 from, Vector2 to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules an offset animation
        /// </summary>
        /// <param name="axis">The offset axis to animate</param>
        /// <param name="to">The target offset value to animate to</param>
        /// <param name="ease">The easing function to use for the offset animation</param>
        
        IAnimationBuilder Offset(Axis axis, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules an offset animation
        /// </summary>
        /// <param name="axis">The offset axis to animate</param>
        /// <param name="from">The initial offset value to animate from</param>
        /// <param name="to">The target offset value to animate to</param>
        /// <param name="ease">The easing function to use for the offset animation</param>
        
        IAnimationBuilder Offset(Axis axis, double from, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules an offset animation
        /// </summary>
        /// <param name="to">The target offset position to animate to</param>
        /// <param name="ease">The easing function to use for the offset animation</param>
        
        IAnimationBuilder Offset(Vector2 to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules an offset animation
        /// </summary>
        /// <param name="from">The initial offset position to animate from</param>
        /// <param name="to">The target offset position to animate to</param>
        /// <param name="ease">The easing function to use for the offset animation</param>
        
        IAnimationBuilder Offset(Vector2 from, Vector2 to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a scale animation
        /// </summary>
        /// <param name="to">The target scale value to animate to</param>
        /// <param name="ease">The easing function to use for the scale animation</param>
        
        IAnimationBuilder Scale(double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a scale animation
        /// </summary>
        /// <param name="from">The scale value to animate from</param>
        /// <param name="to">The target scale value to animate to</param>
        /// <param name="ease">The easing function to use for the scale animation</param>
        
        IAnimationBuilder Scale(double from, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a rotation animation
        /// </summary>
        /// <param name="to">The target rotation value to animate to</param>
        /// <param name="ease">The easing function to use for the rotation animation</param>
        
        IAnimationBuilder Rotate(double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a rotation animation
        /// </summary>
        /// <param name="from">The rotation value to animate from</param>
        /// <param name="to">The target rotation value to animate to</param>
        /// <param name="ease">The easing function to use for the rotation animation</param>
        
        IAnimationBuilder Rotate(double from, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a clip animation
        /// </summary>
        /// <param name="side">The clip side to animate</param>
        /// <param name="to">The target clip value to animate to</param>
        /// <param name="ease">The easing function to use for the clip animation</param>
        
        IAnimationBuilder Clip(Side side, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a clip animation
        /// </summary>
        /// <param name="side">The clip side to animate</param>
        /// <param name="from">The clip value to animate from</param>
        /// <param name="to">The target clip value to animate to</param>
        /// <param name="ease">The easing function to use for the clip animation</param>
        
        IAnimationBuilder Clip(Side side, double from, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a size animation
        /// </summary>
        /// <param name="axis">The size axis to animate</param>
        /// <param name="to">The target size value to animate to</param>
        /// <param name="ease">The easing function to use for the size animation</param>
        
        IAnimationBuilder Size(Axis axis, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a size animation
        /// </summary>
        /// <param name="axis">The size axis to animate</param>
        /// <param name="from">The initial size value to animate from</param>
        /// <param name="to">The target size value to animate to</param>
        /// <param name="ease">The easing function to use for the size animation</param>
        
        IAnimationBuilder Size(Axis axis, double from, double to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a size animation
        /// </summary>
        /// <param name="to">The target size position to animate to</param>
        /// <param name="ease">The easing function to use for the size animation</param>
        
        IAnimationBuilder Size(Vector2 to, Easing ease = Easing.Linear);

        /// <summary>
        /// Schedules a size animation
        /// </summary>
        /// <param name="from">The initial size position to animate from</param>
        /// <param name="to">The target size position to animate to</param>
        /// <param name="ease">The easing function to use for the size animation</param>
        
        IAnimationBuilder Size(Vector2 from, Vector2 to, Easing ease = Easing.Linear);

        /// <summary>
        /// Sets the duration of the animation
        /// </summary>
        /// <param name="ms">The animation duration, in milliseconds</param>
        
        IAnimationBuilder Duration(int ms);

        /// <summary>
        /// Sets the duration of the animation
        /// </summary>
        /// <param name="duration">The <see cref="TimeSpan"/> value indicating the animation duration</param>
        
        IAnimationBuilder Duration(TimeSpan duration);

        /// <summary>
        /// Sets the optional initial delay for the animation
        /// </summary>
        /// <param name="ms">The delay duration, in milliseconds</param>
        
        IAnimationBuilder Delay(int ms);

        /// <summary>
        /// Sets the optional initial delay for the animation
        /// </summary>
        /// <param name="duration">The <see cref="TimeSpan"/> value indicating the animation delay duration</param>
        
        IAnimationBuilder Delay(TimeSpan duration);

        /// <summary>
        /// Starts the animation
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the animation and executes a callback when it completes
        /// </summary>
        /// <param name="callback">The callback to execute when the animation completes</param>
        void Start( Action callback);

        /// <summary>
        /// Starts the animation and returns a <see cref="Task"/> to track its completion
        /// </summary>
        Task StartAsync();
    }
}
