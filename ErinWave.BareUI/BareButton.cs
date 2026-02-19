using System.Windows;
using System.Windows.Controls;

namespace ErinWave.BareUI
{
	public class BareButton : Button
	{
		static BareButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BareButton), new FrameworkPropertyMetadata(typeof(BareButton)));
		}
	}
}
