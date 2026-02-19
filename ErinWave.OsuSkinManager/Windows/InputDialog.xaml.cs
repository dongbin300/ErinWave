using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ErinWave.OsuSkinManager
{
	public partial class InputDialog : Window
	{
		public string InputText { get; private set; } = string.Empty;

		public InputDialog(string prompt, string defaultText = "")
		{
			InitializeComponent();
			PromptTextBlock.Text = prompt;
			InputTextBox.Text = defaultText;
			InputTextBox.SelectAll();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			InputText = InputTextBox.Text;
			DialogResult = true;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				OKButton_Click(this, new RoutedEventArgs());
			}
			else if (e.Key == Key.Escape)
			{
				CancelButton_Click(this, new RoutedEventArgs());
			}
		}
	}
}