using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ErinWave.OsuSkinManager
{
	public partial class ColorPickerDialog : Window
	{
		public Color SelectedColor { get; set; } = Colors.White;

		public ColorPickerDialog()
		{
			InitializeComponent();
			InitializeColorPalette();
			UpdateColorFromSliders();
		}

		private void InitializeColorPalette()
		{
			var predefinedColors = new[]
			{
				Colors.White, Colors.Black, Colors.Red, Colors.Green, Colors.Blue,
				Colors.Yellow, Colors.Cyan, Colors.Magenta, Colors.Orange, Colors.Purple,
				Colors.Brown, Colors.Pink, Colors.Lime, Colors.Navy, Colors.Teal,
				Colors.Silver, Colors.Gray, Colors.Maroon, Colors.Olive, Colors.DarkGreen
			};

			foreach (var color in predefinedColors)
			{
				var rect = new Rectangle
				{
					Width = 20,
					Height = 20,
					Fill = new SolidColorBrush(color),
					Stroke = Brushes.White,
					StrokeThickness = 1,
					Margin = new Thickness(2)
				};

				rect.MouseLeftButtonDown += (s, e) =>
				{
					SelectedColor = color;
					UpdateControlsFromColor(color);
				};

				ColorPalette.Children.Add(rect);
			}
		}

		private void UpdateColorFromSliders()
		{
			var color = Color.FromArgb(
				(byte)AlphaSlider.Value,
				(byte)RedSlider.Value,
				(byte)GreenSlider.Value,
				(byte)BlueSlider.Value);

			SelectedColor = color;
			ColorPreview.Fill = new SolidColorBrush(color);
			ArgbTextBlock.Text = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
			AlphaText.Text = ((byte)AlphaSlider.Value).ToString();
			RedText.Text = ((byte)RedSlider.Value).ToString();
			GreenText.Text = ((byte)GreenSlider.Value).ToString();
			BlueText.Text = ((byte)BlueSlider.Value).ToString();
		}

		private void UpdateControlsFromColor(Color color)
		{
			AlphaSlider.Value = color.A;
			RedSlider.Value = color.R;
			GreenSlider.Value = color.G;
			BlueSlider.Value = color.B;
			UpdateColorFromSliders();
		}

		private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded)
			{
				UpdateColorFromSliders();
			}
		}

		private void AlphaText_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (byte.TryParse(AlphaText.Text, out byte value))
			{
				AlphaSlider.Value = value;
			}
		}

		private void ColorText_TextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox == null) return;

			if (byte.TryParse(textBox.Text, out byte value))
			{
				if (textBox.Name == "RedText")
					RedSlider.Value = value;
				else if (textBox.Name == "GreenText")
					GreenSlider.Value = value;
				else if (textBox.Name == "BlueText")
					BlueSlider.Value = value;
			}
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}