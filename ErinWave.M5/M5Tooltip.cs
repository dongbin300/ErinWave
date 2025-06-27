using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ErinWave.M5
{
	public class M5Tooltip : ToolTip
	{
        Popup popup;
        TextBlock textBlock;

        public M5Tooltip(UIElement target, string text)
        {
            popup = new Popup
            {
                Placement = PlacementMode.Mouse,
                StaysOpen = false,
                PlacementTarget = target
            };

            textBlock = new TextBlock
            {
                Text = text,
                Background = Brushes.Black,
                Padding = new Thickness(5),
                FontSize = 16
            };

            popup.Child = textBlock;
        }
    }
}
