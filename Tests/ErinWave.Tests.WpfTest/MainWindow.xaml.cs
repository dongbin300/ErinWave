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

			timer = new System.Timers.Timer(990);
			timer.Elapsed += Timer_Elapsed;
			timer.Start();

			RM.Human = new RicherHuman
			{
				Wallet = new RicherWallet()
			};
			RM.Human.Wallet.Assets.Add(new RicherWalletAsset("A", 123));
			RM.Human.Wallet.Assets.Add(new RicherWalletAsset("B", 1423));
			RM.Human.Wallet.Assets.Add(new RicherWalletAsset("C", 12423));
			//NameDataGrid.ItemsSource = RM.Human.Wallet.Assets;
		}

		private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			DispatcherService.Invoke(() =>
			{
				NameDataGrid.ItemsSource = RM.Human.Wallet.Assets;
			});
		}

		private void OrderCancelButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}