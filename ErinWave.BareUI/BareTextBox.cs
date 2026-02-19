
using System.Windows;
using System.Windows.Controls;

namespace ErinWave.BareUI
{
	public class BareTextBox : TextBox
	{
		static BareTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BareTextBox), new FrameworkPropertyMetadata(typeof(BareTextBox)));
		}
	}

}
