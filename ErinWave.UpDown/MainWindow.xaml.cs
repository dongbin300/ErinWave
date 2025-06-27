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

namespace ErinWave.UpDown
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		int round = 1;
		int money = 1000;
		int num = 0;
		decimal winPer = 10m;
		decimal losePer = 10m;
		Random random = new();

		public MainWindow()
		{
			InitializeComponent();

			num = random.Next(100) + 1;
			NumText.Text = num.ToString();
			RoundText.Text = round.ToString();
			MoneyText.Text = money.ToString("#,###");
			WinText.Text = ((int)winPer).ToString() + "%";
			LoseText.Text = ((int)losePer).ToString() + "%";
		}

		void Win()
		{
			money = (int)(money * (1 + winPer / 100));
		}

		void Lose()
		{
			money = (int)(money * (1 - losePer / 100));
		}

		void AdjustPer(bool wait)
		{
			if (wait)
			{
				winPer = Math.Clamp(winPer - 1, 5, 20);
				losePer = Math.Clamp(losePer + 1, 5, 20);
			}
			else
			{
				winPer = Math.Clamp(winPer + random.Next(-5, 5), 5, 20);
				losePer = Math.Clamp(losePer + random.Next(-5, 5), 5, 20);
			}
		}

		void NextRound(int choice)
		{
			var nextNum = random.Next(100) + 1;

			if (num < nextNum)
			{
				if (choice == 1)
				{
					Lose();
					AdjustPer(false);
				}
				else if (choice == 2)
				{
					Win();
					AdjustPer(false);
				}
				else
				{
					AdjustPer(true);
				}
			}
			else if (num > nextNum)
			{
				if (choice == 1)
				{
					Win();
					AdjustPer(false);
				}
				else if (choice == 2)
				{
					Lose();
					AdjustPer(false);
				}
				else
				{
					AdjustPer(true);
				}
			}
			else
			{
				if (choice == 1)
				{
					Lose();
					AdjustPer(false);
				}
				else if (choice == 2)
				{
					Lose();
					AdjustPer(false);
				}
				else
				{
					for (int i = 0; i < 30; i++)
					{
						Win();
					}
					AdjustPer(false);
				}
			}

			MoneyText.Text = money.ToString("#,###");
			num = nextNum;
			NumText.Text = num.ToString();
			round++;
			RoundText.Text = round.ToString();
			WinText.Text = ((int)winPer).ToString() + "%";
			LoseText.Text = ((int)losePer).ToString() + "%";
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Up:
					UpButton_Click(sender, e);
					break;

				case Key.Down:
					DownButton_Click(sender, e);
					break;

				case Key.Space:
					WaitButton_Click(sender, e);
					break;
			}
		}

		private void DownButton_Click(object sender, RoutedEventArgs e)
		{
			NextRound(1);
		}

		private void UpButton_Click(object sender, RoutedEventArgs e)
		{
			NextRound(2);
		}

		private void WaitButton_Click(object sender, RoutedEventArgs e)
		{
			NextRound(3);
		}
	}
}