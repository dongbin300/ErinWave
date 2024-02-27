using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;

namespace ErinWave.M5
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DispatcherTimer logTimer = new DispatcherTimer();
		M5Worker worker = default!;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				var ids = File.ReadAllLines("id.txt");
				var processes = Process.GetProcessesByName("ErinWave.M5");
				var id = processes.Length > 1 ? ids[1] : ids[0];

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
	}
}