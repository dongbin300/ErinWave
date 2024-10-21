using ErinWave.Richer.Maths;

using System.Windows;

namespace ErinWave.Tests.WpfTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		System.Timers.Timer timer;
		private decimal maxQuantity;

		public MainWindow()
		{
			InitializeComponent();

			SmartRandom s = new SmartRandom();

			List<decimal> list = new List<decimal>();
			for(int i=0; i < 1000; i++)
			{
				list.Add(s.NextNd(0.98m, 1.02m));
			}


		}
	}
}