using Gma.System.MouseKeyHook;

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace ErinWave.SpeedMacro
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private DispatcherTimer tick = new() { Interval = TimeSpan.FromMilliseconds(25) };
		private CancellationTokenSource? routineCts;

		private IKeyboardMouseEvents _globalHook = default!;

		private bool isStart = false;
		private bool fastInputMode = false;
		private bool loopMode = false;
		private string openFileName = string.Empty;

		//MouseSettingForm msForm;
		//KeyboardSettingForm kbsForm;
		//TimeSettingForm tsForm;

		#region 메인
		public MainWindow()
		{
			InitializeComponent();

			openFileName = Settings1.Default.LastOpenFileName;
			if (!string.IsNullOrEmpty(openFileName))
			{
				ProcedureListBox.Items.Clear();
				var lines = File.ReadAllLines(openFileName);
				foreach (var line in lines)
				{
					ProcedureListBox.Items.Add(line);
				}
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_globalHook = Hook.GlobalEvents();
			_globalHook.KeyDown += GlobalHook_KeyDown;
			//_globalHook.MouseDown += GlobalHook_MouseDown;

			XPosTextBox.Text = "1000";
			YPosTextBox.Text = "800";
			WaitTextBox.Text = "200";
			tick.Tick += tick_Tick;
			tick.Start();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Settings1.Default.LastOpenFileName = openFileName;
			Settings1.Default.Save();

			_globalHook.KeyDown -= GlobalHook_KeyDown;
			//_globalHook.MouseDown -= GlobalHook_MouseDown;
			_globalHook.Dispose();
		}

		private void tick_Tick(object? sender, EventArgs e)
		{
			Title = $"Speed Macro ({System.Windows.Forms.Cursor.Position.X}, {System.Windows.Forms.Cursor.Position.Y})";
			if (isStart)
			{
				ProcedureListBox.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
				ProcedureListBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
			}
			else
			{
				ProcedureListBox.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
				ProcedureListBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
			}
		}

		private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
				e.Effects = System.Windows.DragDropEffects.Copy;
			else
				e.Effects = System.Windows.DragDropEffects.None;
		}

		private void Window_Drop(object sender, System.Windows.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
				if (files.Length > 0)
				{
					ProcedureListBox.Items.Clear();
					var lines = File.ReadAllLines(files[0]);
					foreach (var line in lines)
					{
						ProcedureListBox.Items.Add(line);
					}
					openFileName = files[0];
				}
			}
		}
		#endregion

		#region 메뉴
		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			switch (menuItem?.Name)
			{
				case "New":
					ProcedureListBox.Items.Clear();
					break;

				case "Load":
					{
						var dialog = new Microsoft.Win32.OpenFileDialog()
						{
							InitialDirectory = "C:\\",
							Filter = "routine files (*.rt)|*.rt",
							RestoreDirectory = true
						};

						if (dialog.ShowDialog() ?? false)
						{
							ProcedureListBox.Items.Clear();
							var lines = File.ReadAllLines(dialog.FileName);
							foreach (var line in lines)
							{
								ProcedureListBox.Items.Add(line);
							}

							openFileName = dialog.FileName;
						}
					}
					break;

				case "Save":
					{
						var dialog = new Microsoft.Win32.SaveFileDialog()
						{
							Filter = "routine files (*.rt)|*.rt",
							RestoreDirectory = true
						};

						// 기존 파일명이 있으면 바로 저장, 아니면 다이얼로그 표시
						if (!string.IsNullOrEmpty(openFileName))
						{
							File.WriteAllLines(openFileName, ProcedureListBox.Items.Cast<object>().Select(x => x.ToString()));
						}
						else if (dialog.ShowDialog() ?? false)
						{
							File.WriteAllLines(dialog.FileName, ProcedureListBox.Items.Cast<object>().Select(x => x.ToString()));

							// 새 루틴 저장 후, 다음부터 바로 저장되게 파일명 저장
							openFileName = dialog.FileName;
						}
					}
					break;

				case "SaveAs":
					{
						var dialog = new Microsoft.Win32.SaveFileDialog()
						{
							Filter = "routine files (*.rt)|*.rt",
							RestoreDirectory = true
						};

						if (dialog.ShowDialog() ?? false)
						{
							File.WriteAllLines(dialog.FileName, ProcedureListBox.Items.Cast<object>().Select(x => x.ToString()));

							openFileName = dialog.FileName;
						}
					}
					break;

				case "Exit":
					System.Windows.Application.Current.Shutdown();
					break;

				case "Help":
					//HelpForm helpForm = new HelpForm();
					//helpForm.ShowDialog();
					break;

				case "SpeedMacro":
					//AboutForm aboutForm = new AboutForm();
					//aboutForm.ShowDialog();
					break;

				default:
					break;
			}
		}
		#endregion

		#region 단축키
		private void GlobalHook_KeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
		{
			// 매크로를 실행 중이지 않을 때만 키보드 입력 받음
			if (!isStart)
			{
				// 빠른 입력 모드
				if (fastInputMode)
				{
					Key wpfKey = KeyInterop.KeyFromVirtualKey(e.KeyValue);

					var routedEvent = Keyboard.PreviewKeyDownEvent;
					var keyEventArgs = new System.Windows.Input.KeyEventArgs(
						Keyboard.PrimaryDevice,
						PresentationSource.FromVisual(this),
						0,
						wpfKey
					)
					{
						RoutedEvent = routedEvent
					};

					KeyTextBox_PreviewKeyDown(sender, keyEventArgs);
					PressButton_Click(sender, new RoutedEventArgs());
				}
				// 일반 입력 모드(단축키)
				else
				{
					switch (e.KeyCode)
					{
						case Keys.F1:
							//HelpForm helpForm = new HelpForm();
							//helpForm.ShowDialog();
							break;
						case Keys.F2:
							XPosTextBox.Text = "" + System.Windows.Forms.Cursor.Position.X;
							YPosTextBox.Text = "" + System.Windows.Forms.Cursor.Position.Y;
							ClickButton_Click(sender, new RoutedEventArgs());
							break;
						case Keys.F3:
							XPosTextBox.Text = "" + System.Windows.Forms.Cursor.Position.X;
							YPosTextBox.Text = "" + System.Windows.Forms.Cursor.Position.Y;
							DoubleClickButton_Click(sender, new RoutedEventArgs());
							break;
						case Keys.F4:
							XPosTextBox.Text = "" + System.Windows.Forms.Cursor.Position.X;
							YPosTextBox.Text = "" + System.Windows.Forms.Cursor.Position.Y;
							RightClickButton_Click(sender, new RoutedEventArgs());
							break;
						case Keys.F5:
							WaitButton_Click(sender, new RoutedEventArgs());
							break;
						case Keys.F7:
							StartButton_Click(sender, new RoutedEventArgs());
							break;
						case Keys.F8:
							StopButton_Click(sender, new RoutedEventArgs());
							break;
						case Keys.Delete:
							DeleteButton_Click(sender, new RoutedEventArgs());
							break;
					}
				}
			}
		}
		#endregion

		#region 프로시저
		private void ProcedureListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			switch (ProcedureListBox.Items[ProcedureListBox.SelectedIndex].ToString().Split('(')[0])
			{
				case "Click":
				case "DClick":
				case "RClick":
					//msForm.Location = new System.Drawing.Point(Cursor.Position.X + 10, Cursor.Position.Y + 10);
					//msForm.ShowDialog();
					break;
				case "Press":
					//kbsForm.Location = new System.Drawing.Point(Cursor.Position.X + 10, Cursor.Position.Y + 10);
					//kbsForm.ShowDialog();
					break;
				case "Wait":
					//tsForm.Location = new System.Drawing.Point(Cursor.Position.X + 10, Cursor.Position.Y + 10);
					//tsForm.ShowDialog();
					break;
			}
		}

		private void DeleteButton_Click(object? sender, RoutedEventArgs e)
		{
			if (ProcedureListBox.SelectedIndex >= 0)
			{
				int idx = ProcedureListBox.SelectedIndex;
				ProcedureListBox.Items.RemoveAt(idx);
				if (ProcedureListBox.Items.Count > idx)
				{
					ProcedureListBox.SelectedIndex = idx;
				}
				else if (idx != 0)
				{
					ProcedureListBox.SelectedIndex = idx - 1;
				}
			}
		}

		private void LoopCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			loopMode = true;
		}

		private void LoopCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			loopMode = false;
		}
		#endregion

		#region 마우스
		private void ClickButton_Click(object? sender, RoutedEventArgs e)
		{
			ProcedureListBox.Items.Add($"Click({XPosTextBox.Text}, {YPosTextBox.Text})");
			if (EachCheckBox.IsChecked ?? false)
			{
				ProcedureListBox.Items.Add($"Wait({WaitTextBox.Text}ms)");
			}
			ProcedureListBox.SelectedIndex = ProcedureListBox.Items.Count - 1;
		}

		private void DoubleClickButton_Click(object? sender, RoutedEventArgs e)
		{
			ProcedureListBox.Items.Add($"DClick({XPosTextBox.Text}, {YPosTextBox.Text})");
			if (EachCheckBox.IsChecked ?? false)
			{
				ProcedureListBox.Items.Add($"Wait({WaitTextBox.Text}ms)");
			}
			ProcedureListBox.SelectedIndex = ProcedureListBox.Items.Count - 1;
		}

		private void RightClickButton_Click(object? sender, RoutedEventArgs e)
		{
			ProcedureListBox.Items.Add($"RClick({XPosTextBox.Text}, {YPosTextBox.Text})");
			if (EachCheckBox.IsChecked ?? false)
			{
				ProcedureListBox.Items.Add($"Wait({WaitTextBox.Text}ms)");
			}
			ProcedureListBox.SelectedIndex = ProcedureListBox.Items.Count - 1;
		}
		#endregion

		#region 키보드
		private void FastInputCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			fastInputMode = true;
		}

		private void FastInputCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			fastInputMode = false;
		}

		private void PressButton_Click(object? sender, RoutedEventArgs e)
		{
			ProcedureListBox.Items.Add($"Press({KeyTextBox.Text})");
			if (EachCheckBox.IsChecked ?? false)
			{
				ProcedureListBox.Items.Add($"Wait({WaitTextBox.Text}ms)");
			}
			ProcedureListBox.SelectedIndex = ProcedureListBox.Items.Count - 1;
		}
		#endregion

		#region 시간
		private void WaitButton_Click(object? sender, RoutedEventArgs e)
		{
			ProcedureListBox.Items.Add($"Wait({WaitTextBox.Text}ms)");
			ProcedureListBox.SelectedIndex = ProcedureListBox.Items.Count - 1;
		}
		#endregion

		#region 시작/정지
		private async void StartButton_Click(object? sender, RoutedEventArgs e)
		{
			isStart = true;
			routineCts = new CancellationTokenSource();
			await Task.Run(() => Routine(routineCts.Token));
		}

		private void StopButton_Click(object? sender, RoutedEventArgs e)
		{
			isStart = false;
			routineCts?.Cancel();
		}
		#endregion

		private void KeyTextBox_PreviewKeyDown(object? sender, System.Windows.Input.KeyEventArgs e)
		{
			string keyText = string.Empty;

			var modifiers = Keyboard.Modifiers;
			var key = e.Key;

			if (modifiers != ModifierKeys.None)
			{
				if (modifiers.HasFlag(ModifierKeys.Control))
					keyText += "Ctrl+";
				if (modifiers.HasFlag(ModifierKeys.Alt))
					keyText += "Alt+";
				if (modifiers.HasFlag(ModifierKeys.Shift))
					keyText += "Shift+";
				if (modifiers.HasFlag(ModifierKeys.Windows))
					keyText += "Win+";
			}

			// 실제 키(알파벳, 숫자 등) 붙이기
			// SystemKey는 Alt와 조합된 특수키를 의미하므로 처리 필요
			var realKey = key == Key.System ? e.SystemKey : key;
			keyText += Mapping.KeysString(realKey.ToString());

			KeyTextBox.Text = keyText;
			e.Handled = true;
		}

		private void Routine(CancellationToken token)
		{
			while (isStart && !token.IsCancellationRequested)
			{
				int itemCount = 0;
				System.Windows.Application.Current.Dispatcher.Invoke(() => itemCount = ProcedureListBox.Items.Count);

				for (int i = 0; i < itemCount; i++)
				{
					if (!isStart || token.IsCancellationRequested)
					{
						break;
					}

					string? str = null;
					System.Windows.Application.Current.Dispatcher.Invoke(() =>
					{
						ProcedureListBox.SelectedIndex = i;
						str = ProcedureListBox.Items[i]?.ToString();
					});

					if (string.IsNullOrWhiteSpace(str))
					{
						continue;
					}

					string command = str.Split('(')[0];
					switch (command)
					{
						case "Click":
							var (x, y) = Mapping.ParseMouse(str);
							InputSimulator.MouseClick(x, y);
							break;
						case "DClick":
							(x, y) = Mapping.ParseMouse(str);
							InputSimulator.MouseDoubleClick(x, y);
							break;
						case "RClick":
							(x, y) = Mapping.ParseMouse(str);
							InputSimulator.MouseRightClick(x, y);
							break;
						case "Press":
							var keyString = Mapping.ParseKey(str);
							var sendKeyString = Mapping.SendKeysString(keyString);
							SendKeys.SendWait(sendKeyString);
							break;
						case "Wait":
							var waitTime = Mapping.ParseWait(str);
							Thread.Sleep(waitTime);
							break;
					}
				}
				if (!loopMode)
					isStart = false;
			}
		}
	}
}