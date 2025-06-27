using System.Windows;
using System.Windows.Input;

namespace ErinWave.HelloAkiba
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// 
	/// 0. 목표
	///		- 용량을 많이 모으기
	///		- 사용자의 요구를 충족시켜서 용량을 모을수 있음
	/// 1. 오브젝트
	///		- 라벨, 버튼, 텍스트박스, 체크박스, 라디오버튼, 스택패널, 캔버스, 그리드, 콤보박스, 리스트박스, 리스트뷰, 트리뷰, 데이터그리드, 유저컨트롤, 커스텀컨트롤
	///		- 변수, 연산자, 조건문, 반복문, 리스트, 메서드, 구조체, 속성, 생성자, 상속, 오버로딩, 이벤트, 람다식, LINQ, 제네릭, 튜플, 비동기
	///	2. 세부
	///		- tukuri 1:
	///		- kikai v1: t1 85%, t2 15%
	///		- v2: t1 70%, t2 30%
	///		- v3: t1 50%, t2 50%
	///		- v4: t1 30%, t2 65%, t3 5%
	///		- v5: t1 15%, t2 70%, t3 15%
	///		- v6: t2 65%, t3 30%, t4 5%
	///		- v7: t2 35%, t3 50%, t4 15%
	/// 
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly List<string> _commandHistory = [];
		private int _historyIndex = -1;

		public MainWindow()
		{
			InitializeComponent();
			PrintLine("youkoso");
			ConsoleInput.Focus();

			HaSettings.Load();

			Activated += (s, e) => ConsoleInput.Focus();
		}

		private void PrintLine(string text)
		{
			ConsoleOutput.Text += text + "\n";
			ConsoleScroll.ScrollToEnd();
			ConsoleInput.Focus();
		}

		private void ConsoleInput_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					{
						string input = ConsoleInput.Text.Trim();
						if (!string.IsNullOrEmpty(input))
						{
							PrintLine("> " + input);
							HandleCommand(input);

							if (_commandHistory.Count == 0 || _commandHistory[^1] != input)
								_commandHistory.Add(input);
							_historyIndex = _commandHistory.Count;
						}
						ConsoleInput.Clear();
						e.Handled = true;
						ConsoleInput.Focus();
					}
					break;
				case Key.Up:
					{
						if (_commandHistory.Count > 0 && _historyIndex > 0)
						{
							_historyIndex--;
							ConsoleInput.Text = _commandHistory[_historyIndex];
							ConsoleInput.CaretIndex = ConsoleInput.Text.Length;
						}
						e.Handled = true;
					}
					break;
				case Key.Down:
					{
						if (_commandHistory.Count > 0 && _historyIndex < _commandHistory.Count - 1)
						{
							_historyIndex++;
							ConsoleInput.Text = _commandHistory[_historyIndex];
							ConsoleInput.CaretIndex = ConsoleInput.Text.Length;
						}
						else if (_historyIndex == _commandHistory.Count - 1)
						{
							_historyIndex++;
							ConsoleInput.Clear();
						}
						e.Handled = true;
					}
					break;
				case Key.Escape:
					{
						ConsoleInput.Clear();
						e.Handled = true;
					}
					break;
			}
		}

		private void HandleCommand(string command)
		{
			var commandl = command.ToLower();
			if (commandl.StartsWith("gousei"))
			{
				var parts = commandl.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 3 &&
					int.TryParse(parts[1], out int x1) &&
					int.TryParse(parts[2], out int x2))
				{
					var result = HaSettings.MergeItem(x1, x2);
					PrintLine(result);
					HaSettings.Save();
				}
				else
				{
					PrintLine("gousei [kaban_index1] [kaban_index2]");
				}
				return;
			}
			else if (commandl.StartsWith("ootomaaji"))
			{
				var parts = commandl.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 3 &&
					int.TryParse(parts[1], out int x1) &&
					int.TryParse(parts[2], out int x2))
				{
					var result = HaSettings.AutoMerge(x1, x2);
					PrintLine(result);
					HaSettings.Save();
				}
				else
				{
					PrintLine("ootomaaji [type] [tier]");
				}
				return;
			}
			else if (commandl.StartsWith("age"))
			{
				var parts = commandl.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 2 &&
					int.TryParse(parts[1], out int x1))
				{
					var result = HaSettings.GiveToUser(x1);
					PrintLine(result);
					HaSettings.Save();
				}
				else
				{
					PrintLine("age [yuuzaa_index]");
				}
				return;
			}
			else if (commandl.StartsWith("kotowari"))
			{
				var parts = commandl.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 2 &&
					int.TryParse(parts[1], out int x1))
				{
					var result = HaSettings.RemoveUser(x1);
					PrintLine(result);
					HaSettings.Save();
				}
				else
				{
					PrintLine("kotowari [yuuzaa_index]");
				}
				return;
			}
			else if (commandl.StartsWith("senden"))
			{
				var parts = commandl.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 2 &&
					int.TryParse(parts[1], out int x1))
				{
					while (HaSettings.Users.Count < 5)
					{
						var result = HaSettings.AddUser(x1);
						PrintLine(result);
					}
					HaSettings.Save();
				}
				else
				{
					while (HaSettings.Users.Count < 5)
					{
						var result = HaSettings.AddUser();
						PrintLine(result);
					}
					HaSettings.Save();
				}
				return;
			}
			else if (commandl.StartsWith("tukuri1"))
			{
				var parts = commandl.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 2 &&
					int.TryParse(parts[1], out int x1))
				{
					var result = HaSettings.Generator1.Generate(x1);
					PrintLine(result);
					HaSettings.Save();
				}
				else
				{
					var result = HaSettings.Generator1.Generate();
					PrintLine(result);
					HaSettings.Save();
				}
				return;
			}

			switch (commandl)
			{
				case "kikai":
					PrintLine("genzai kikai_1: " + HaSettings.Generator1.Version);
					break;
				case "tukuri1":
					{
						var result = HaSettings.Generator1.Generate();
						PrintLine(result);
						HaSettings.Save();
					}
					break;
				case "appugureedo1":
					{
						var result = HaSettings.Generator1.Upgrade();
						PrintLine(result);
						HaSettings.Save();
					}
					break;
				case "kaban":
					{
						var result = $"Kane {HaSettings.Kane}\r\n" + string.Join(Environment.NewLine, HaSettings.Items.Select((x, i) => $"[{i}] {x}"));
						PrintLine(result);
					}
					break;
				case "yuuzaa":
					{
						var result = string.Join(Environment.NewLine, HaSettings.Users.Select((x, i) => $"[{i}] {x}"));
						PrintLine(result);
					}
					break;
				case "herupu":
					PrintLine("herupu, kuria, ekizi, kikai, tukuri1 [count], appugureedo1, kaban, senden [tier], yuuzaa, gousei [kaban_index1] [kaban_index2], ootomaaji [type] [tier], age [yuuzaa_index], kotowari [yuuzaa_index]");
					break;
				case "kuria":
					ConsoleOutput.Text = "";
					break;
				case "ekizi":
					Application.Current.Shutdown();
					break;
				case "d":
					WindowState = WindowState.Minimized;
					break;
				default:
					PrintLine("wakaranainodesu. herupu nyuuryoku kudasai.");
					break;
			}
		}
	}
}