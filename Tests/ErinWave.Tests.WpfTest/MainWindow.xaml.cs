using ErinWave.Richer.Maths;

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ErinWave.Tests.WpfTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		System.Timers.Timer timer;

		public MainWindow()
		{
			InitializeComponent();

			SmartRandom s = new SmartRandom();

			for (int i = 0; i < 1_000_0000; i++)
			{
				s.Next(1, 100);
			}

			for (int i = 0; i < 1_000_0000; i++)
			{
				s.NextDouble();
			}

			for (int i = 0; i < 1_000_0000; i++)
			{
				s.NextNd(1.0, 100.0);
			}

			for (int i = 0; i < 1_000_0000; i++)
			{
				s.NextNd(1m, 100m);
			}
		}

		private void OrderCancelButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}