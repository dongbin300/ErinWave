using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace ErinWave.M5
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DispatcherTimer logTimer = new ();
		M5Worker worker = default!;

		public MainWindow()
		{
			InitializeComponent();
		}

		private BitmapImage? GetImage(string? url)
		{
			return url == null ? null : new BitmapImage(new Uri(Common.ImageResourceUrl + url + ".png"));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				var ids = File.ReadAllLines("id.txt");
				var processes = Process.GetProcessesByName("ErinWave.M5");
				var id = processes.Length > 1 ? ids[1] : ids[0];

				RemainTimeProgressBar.SetMinimum(0);
				RemainTimeProgressBar.SetMaximum(300);

				logTimer.Interval = TimeSpan.FromMilliseconds(50);
				logTimer.Tick += LogTimer_Tick;
				logTimer.Start();

				worker = new M5Worker(id);
				worker.Start();
			}
			catch (Exception ex)
			{
				LogQueue.Enqueue($"오류 발생: {ex}");
			}
		}

		private void LogTimer_Tick(object? sender, EventArgs e)
		{
			while (LogQueue.Count > 0)
			{
				var item = LogQueue.Dequeue();
				LogListBox.Items.Add(item);
				LogListBox.ScrollIntoView(item);
			}

			AbilityButton.Visibility = Common.MeJobImageSource == null ? Visibility.Hidden : Visibility.Visible;
			MeDeckCountText.Visibility = Common.MeJobImageSource == null ? Visibility.Hidden : Visibility.Visible;
			MeUsedCountText.Visibility = Common.MeJobImageSource == null ? Visibility.Hidden : Visibility.Visible;
			YouDeckCountText.Visibility = Common.YouJobImageSource == null ? Visibility.Hidden : Visibility.Visible;
			YouUsedCountText.Visibility = Common.YouJobImageSource == null ? Visibility.Hidden : Visibility.Visible;
			FieldDungeonCountText.Visibility = Common.FieldDungeonImageSource == null ? Visibility.Hidden : Visibility.Visible;

			MeDeckCountText.Text = Common.MeDeckCount.ToString();
			MeUsedCountText.Text = Common.MeUsedCount.ToString();
			YouDeckCountText.Text = Common.YouDeckCount.ToString();
			YouUsedCountText.Text = Common.YouUsedCount.ToString();
			FieldDungeonCountText.Text = Common.FieldDungeonCount.ToString();

			RemainTimeProgressBar.SetText(Common.RemainTimeString);
			RemainTimeProgressBar.SetValue((double)Common.RemainSeconds);

			// 이미지 처리
			MeJobImage.Source = GetImage(Common.MeJobImageSource);
			MeDeckImage.Source = GetImage(Common.MeDeckImageSource);
			MeUsedImage.Source = GetImage(Common.MeUsedImageSource);
			YouJobImage.Source = GetImage(Common.YouJobImageSource);
			YouDeckImage.Source = GetImage(Common.YouDeckImageSource);
			YouUsedImage.Source = GetImage(Common.YouUsedImageSource);
			FieldBossImage.Source = GetImage(Common.FieldBossImageSource);
			FieldDungeonImage.Source = GetImage(Common.FieldDungeonImageSource);
			FieldCurrentDungeonImage.Source = GetImage(Common.FieldCurrentDungeonImageSource);

			MeHandPanel.Children.Clear();
			var meHandWidth = Common.MeHandImageSource.Count > 12 ? 59 : 98;
			for (int i = 0; i < Common.MeHandImageSource.Count; i++)
			{
				string? source = Common.MeHandImageSource[i];
				var image = new Image
				{
					Source = GetImage(source),
					Width = meHandWidth,
					Margin = new Thickness(1),
					Tag = i
				};

				image.MouseLeftButtonDown += (sender, e) =>
				{
					var image = sender as Image;
					worker.SendUseCardEventPacket(int.Parse(image?.Tag.ToString() ?? "0"));
				};
				MeHandPanel.Children.Add(image);
			}

			YouHandPanel.Children.Clear();
			var youHandWidth = Common.YouHandImageSource.Count > 12 ? 59 : 98;
			for (int i = 0; i < Common.YouHandImageSource.Count; i++)
			{
				string? source = Common.YouHandImageSource[i];
				var image = new Image
				{
					Source = GetImage(source),
					Width = youHandWidth,
					Margin = new Thickness(1)
				};
				YouHandPanel.Children.Add(image);
			}

			FieldPanel.Children.Clear();
			var fieldWidth = Common.FieldCurrentCardImageSource.Count > 30 ? 39 :
				Common.FieldCurrentCardImageSource.Count > 12 ? 59 :
				98;
			for (int i = 0; i < Common.FieldCurrentCardImageSource.Count; i++)
			{
				string? source = Common.FieldCurrentCardImageSource[i];
				var image = new Image
				{
					Source = GetImage(source),
					Width = fieldWidth,
					Margin = new Thickness(1)
				};
				FieldPanel.Children.Add(image);
			}

			foreach (var item in LogListBox.Items)
			{
				ListBoxItem listBoxItem = LogListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem ?? default!;
				if (listBoxItem != null && item is string str && str.Contains("[SYSTEM]"))
				{
					listBoxItem.Foreground = new SolidColorBrush(Color.FromRgb(3, 158, 225));
					listBoxItem.FontWeight = FontWeights.Bold;
				}
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Common.IsExit = true;
			Environment.Exit(0);
		}

		private void MessageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				var message = MessageTextBox.Text;
				if (string.IsNullOrEmpty(message))
				{
					return;
				}

				worker.SendChatMessagePacket(message);
				MessageTextBox.Text = "";
			}
		}

		private void MeDeckImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Common.MeDeckImageSource == null)
			{
				return;
			}

			worker.SendDeckDrawEventPacket();
		}

		private void FieldCurrentDungeonImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Common.FieldCurrentDungeonImageSource == null)
			{
				return;
			}

			worker.SendConfirmCurrentDungeonPacket();
		}

		private void FieldDungeonImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Common.FieldDungeonImageSource == null)
			{
				return;
			}

			worker.SendNextDungeonOpenPacket();
		}

		private void FieldBossImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Common.FieldBossImageSource == null)
			{
				return;
			}

			worker.SendConfirmBossPacket();
		}

		private void AbilityButton_Click(object sender, RoutedEventArgs e)
		{
			worker.SendUseAbilityPacket();
		}
	}
}