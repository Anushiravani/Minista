using Microsoft.Xaml.Interactivity;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Minista.Behaviors
{
    public class PointersEvent : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerExited += PointerExited;
            AssociatedObject.PointerEntered += PointerEntered;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PointerExited -= PointerExited;
            AssociatedObject.PointerEntered -= PointerEntered;
        }


        private void PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }

        private void PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
        }
    }

}
