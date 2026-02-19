using Gma.System.MouseKeyHook;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace ErinWave.SpeedMacro
{
    public partial class MainWindow : Window
    {
        // 4개의 매크로 상태를 관리하는 클래스
        private class MacroState
        {
            public bool IsRunning { get; set; } = false;
            public bool LoopMode { get; set; } = false;
            public bool FastInputMode { get; set; } = false;
            public CancellationTokenSource? RoutineCts { get; set; }
            public string OpenFileName { get; set; } = string.Empty;
        }

        private readonly DispatcherTimer _tick = new() { Interval = TimeSpan.FromMilliseconds(100) };
        private IKeyboardMouseEvents _globalHook = default!;

        // 4개의 매크로에 대한 상태 및 UI 컨트롤 배열
        private readonly MacroState[] _macroStates = new MacroState[4];
        private readonly System.Windows.Controls.ListBox[] _procedureListBoxes = new System.Windows.Controls.ListBox[4];
        private readonly System.Windows.Controls.CheckBox[] _loopCheckBoxes = new System.Windows.Controls.CheckBox[4];
        private readonly System.Windows.Controls.TextBox[] _xPosTextBoxes = new System.Windows.Controls.TextBox[4];
        private readonly System.Windows.Controls.TextBox[] _yPosTextBoxes = new System.Windows.Controls.TextBox[4];
        private readonly System.Windows.Controls.TextBox[] _keyTextBoxes = new System.Windows.Controls.TextBox[4];
        private readonly System.Windows.Controls.TextBox[] _waitTextBoxes = new System.Windows.Controls.TextBox[4];
        private readonly System.Windows.Controls.CheckBox[] _eachCheckBoxes = new System.Windows.Controls.CheckBox[4];
        private readonly System.Windows.Controls.CheckBox[] _fastInputCheckBoxes = new System.Windows.Controls.CheckBox[4];

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 4; i++)
            {
                _macroStates[i] = new MacroState();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // UI 컨트롤들을 배열에 할당
            _procedureListBoxes[0] = ProcedureListBox1;
            _procedureListBoxes[1] = ProcedureListBox2;
            _procedureListBoxes[2] = ProcedureListBox3;
            _procedureListBoxes[3] = ProcedureListBox4;

            _loopCheckBoxes[0] = LoopCheckBox1;
            _loopCheckBoxes[1] = LoopCheckBox2;
            _loopCheckBoxes[2] = LoopCheckBox3;
            _loopCheckBoxes[3] = LoopCheckBox4;
            
            _xPosTextBoxes[0] = XPosTextBox1;
            _xPosTextBoxes[1] = XPosTextBox2;
            _xPosTextBoxes[2] = XPosTextBox3;
            _xPosTextBoxes[3] = XPosTextBox4;

            _yPosTextBoxes[0] = YPosTextBox1;
            _yPosTextBoxes[1] = YPosTextBox2;
            _yPosTextBoxes[2] = YPosTextBox3;
            _yPosTextBoxes[3] = YPosTextBox4;

            _keyTextBoxes[0] = KeyTextBox1;
            _keyTextBoxes[1] = KeyTextBox2;
            _keyTextBoxes[2] = KeyTextBox3;
            _keyTextBoxes[3] = KeyTextBox4;

            _waitTextBoxes[0] = WaitTextBox1;
            _waitTextBoxes[1] = WaitTextBox2;
            _waitTextBoxes[2] = WaitTextBox3;
            _waitTextBoxes[3] = WaitTextBox4;

            _eachCheckBoxes[0] = EachCheckBox1;
            _eachCheckBoxes[1] = EachCheckBox2;
            _eachCheckBoxes[2] = EachCheckBox3;
            _eachCheckBoxes[3] = EachCheckBox4;

            _fastInputCheckBoxes[0] = FastInputCheckBox1;
            _fastInputCheckBoxes[1] = FastInputCheckBox2;
            _fastInputCheckBoxes[2] = FastInputCheckBox3;
            _fastInputCheckBoxes[3] = FastInputCheckBox4;

            // 이벤트 핸들러 연결
            for (int i = 0; i < 4; i++)
            {
                int macroIndex = i; // 클로저를 위해 인덱스 복사
                _loopCheckBoxes[i].Checked += (s, ev) => _macroStates[macroIndex].LoopMode = true;
                _loopCheckBoxes[i].Unchecked += (s, ev) => _macroStates[macroIndex].LoopMode = false;
                _fastInputCheckBoxes[i].Checked += (s, ev) => _macroStates[macroIndex].FastInputMode = true;
                _fastInputCheckBoxes[i].Unchecked += (s, ev) => _macroStates[macroIndex].FastInputMode = false;

                ((System.Windows.Controls.Button)FindName($"DeleteButton{i + 1}")).Click += (s, ev) => DeleteButton_Click(macroIndex);
                ((System.Windows.Controls.Button)FindName($"ClickButton{i + 1}")).Click += (s, ev) => ClickButton_Click(macroIndex);
                ((System.Windows.Controls.Button)FindName($"DoubleClickButton{i + 1}")).Click += (s, ev) => DoubleClickButton_Click(macroIndex);
                ((System.Windows.Controls.Button)FindName($"RightClickButton{i + 1}")).Click += (s, ev) => RightClickButton_Click(macroIndex);
                ((System.Windows.Controls.Button)FindName($"PressButton{i + 1}")).Click += (s, ev) => PressButton_Click(macroIndex);
                ((System.Windows.Controls.Button)FindName($"WaitButton{i + 1}")).Click += (s, ev) => WaitButton_Click(macroIndex);
                _keyTextBoxes[i].PreviewKeyDown += KeyTextBox_PreviewKeyDown;
            }

            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;

            _tick.Tick += tick_Tick;
            _tick.Start();

            // Load default routines
            string file1 = @"C:\hanq.rt";
            if (File.Exists(file1))
            {
                LoadRoutineFromFile(file1, _procedureListBoxes[0], _macroStates[0]);
            }

            string file2 = @"C:\hanq2.rt";
            if (File.Exists(file2))
            {
                LoadRoutineFromFile(file2, _procedureListBoxes[1], _macroStates[1]);
            }

			string file3 = @"C:\hanq3.rt";
			if (File.Exists(file3))
			{
				LoadRoutineFromFile(file3, _procedureListBoxes[2], _macroStates[2]);
			}
		}

        private void Window_Closed(object sender, EventArgs e)
        {
            _tick.Stop();
            _globalHook.KeyDown -= GlobalHook_KeyDown;
            _globalHook.Dispose();
        }

        private void tick_Tick(object? sender, EventArgs e)
        {
            Title = $"Speed Macro 2 ({System.Windows.Forms.Cursor.Position.X}, {System.Windows.Forms.Cursor.Position.Y})";
            for (int i = 0; i < 4; i++)
            {
                var listBox = _procedureListBoxes[i];
                if (_macroStates[i].IsRunning)
                {
                    listBox.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LimeGreen);
                    listBox.BorderThickness = new Thickness(2);
                }
                else
                {
                    listBox.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                    listBox.BorderThickness = new Thickness(1);
                }
            }
        }

        private void GlobalHook_KeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            int macroIndex = -1;
            switch (e.KeyCode)
            {
                case Keys.F9: macroIndex = 0; break;
                case Keys.F10: macroIndex = 1; break;
                case Keys.F11: macroIndex = 2; break;
                case Keys.F12: macroIndex = 3; break;

                case Keys.F2:
                    Dispatcher.Invoke(() =>
                    {
                        int index = MacroTabControl.SelectedIndex;
                        if (index < 0) return;
                        _xPosTextBoxes[index].Text = "" + System.Windows.Forms.Cursor.Position.X;
                        _yPosTextBoxes[index].Text = "" + System.Windows.Forms.Cursor.Position.Y;
                        ClickButton_Click(index);
                    });
                    e.Handled = true;
                    break;
                case Keys.F3:
                    Dispatcher.Invoke(() =>
                    {
                        int index = MacroTabControl.SelectedIndex;
                        if (index < 0) return;
                        _xPosTextBoxes[index].Text = "" + System.Windows.Forms.Cursor.Position.X;
                        _yPosTextBoxes[index].Text = "" + System.Windows.Forms.Cursor.Position.Y;
                        DoubleClickButton_Click(index);
                    });
                    e.Handled = true;
                    break;
                case Keys.F4:
                    Dispatcher.Invoke(() =>
                    {
                        int index = MacroTabControl.SelectedIndex;
                        if (index < 0) return;
                        _xPosTextBoxes[index].Text = "" + System.Windows.Forms.Cursor.Position.X;
                        _yPosTextBoxes[index].Text = "" + System.Windows.Forms.Cursor.Position.Y;
                        RightClickButton_Click(index);
                    });
                    e.Handled = true;
                    break;
                case Keys.F5:
                    Dispatcher.Invoke(() =>
                    {
                        int index = MacroTabControl.SelectedIndex;
                        if (index < 0) return;
                        WaitButton_Click(index);
                    });
                    e.Handled = true;
                    break;
			}

            if (macroIndex != -1)
            {
                e.Handled = true;
                ToggleMacro(macroIndex);
            }
        }

        private void ToggleMacro(int macroIndex)
        {
            if (_macroStates[macroIndex].IsRunning)
            {
                StopMacro(macroIndex);
            }
            else
            {
                StartMacro(macroIndex);
            }
        }

        private async void StartMacro(int macroIndex)
        {
            if (_macroStates[macroIndex].IsRunning) return;

            _macroStates[macroIndex].IsRunning = true;
            _macroStates[macroIndex].RoutineCts = new CancellationTokenSource();
            await Task.Run(() => Routine(macroIndex, _macroStates[macroIndex].RoutineCts.Token));
        }

        private void StopMacro(int macroIndex)
        {
            if (!_macroStates[macroIndex].IsRunning) return;

            _macroStates[macroIndex].RoutineCts?.Cancel();
            _macroStates[macroIndex].IsRunning = false;
        }

        #region Menu and File Operations

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.MenuItem menuItem) return;

            int selectedIndex = MacroTabControl.SelectedIndex;
            if (selectedIndex < 0) return;

            var currentListBox = _procedureListBoxes[selectedIndex];
            var currentMacroState = _macroStates[selectedIndex];

            switch (menuItem.Name)
            {
                case "New":
                    currentListBox.Items.Clear();
                    currentMacroState.OpenFileName = string.Empty;
                    break;

                case "Load":
                    var openDialog = new Microsoft.Win32.OpenFileDialog()
                    {
                        Filter = "Routine Files (*.rt)|*.rt|All files (*.*)|*.*",
                        RestoreDirectory = true
                    };
                    if (openDialog.ShowDialog() == true)
                    {
                        LoadRoutineFromFile(openDialog.FileName, currentListBox, currentMacroState);
                    }
                    break;

                case "Save":
                    if (!string.IsNullOrEmpty(currentMacroState.OpenFileName))
                    {
                        SaveRoutineToFile(currentMacroState.OpenFileName, currentListBox);
                    }
                    else
                    {
                        SaveAs_Click(currentListBox, currentMacroState);
                    }
                    break;

                case "SaveAs":
                    SaveAs_Click(currentListBox, currentMacroState);
                    break;

                case "Exit":
                    System.Windows.Application.Current.Shutdown();
                    break;

                case "Help":
                    System.Windows.MessageBox.Show("F1 ~ F4 키를 눌러 해당 탭의 매크로를 시작/중지할 수 있습니다.", "도움말", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                case "SpeedMacro":
                    System.Windows.MessageBox.Show("ErinWave SpeedMacro 2\n\nCreated by Gemini", "정보", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }

        private void SaveAs_Click(System.Windows.Controls.ListBox listBox, MacroState state)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Routine Files (*.rt)|*.rt|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (saveDialog.ShowDialog() == true)
            {
                state.OpenFileName = saveDialog.FileName;
                SaveRoutineToFile(state.OpenFileName, listBox);
            }
        }

        private void LoadRoutineFromFile(string filePath, System.Windows.Controls.ListBox listBox, MacroState state)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                listBox.Items.Clear();
                foreach (var line in lines)
                {
                    listBox.Items.Add(line);
                }
                state.OpenFileName = filePath;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"파일을 불러오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void SaveRoutineToFile(string filePath, System.Windows.Controls.ListBox listBox)
        {
            try
            {
                File.WriteAllLines(filePath, listBox.Items.Cast<object>().Select(x => x.ToString() ?? string.Empty));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"파일을 저장하는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            int selectedIndex = MacroTabControl.SelectedIndex;
            if (selectedIndex < 0) return;

            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    LoadRoutineFromFile(files[0], _procedureListBoxes[selectedIndex], _macroStates[selectedIndex]);
                }
            }
        }

        #endregion


        #region Button Handlers
        private void DeleteButton_Click(int macroIndex)
        {
            var listBox = _procedureListBoxes[macroIndex];
            if (listBox.SelectedIndex >= 0)
            {
                int idx = listBox.SelectedIndex;
                listBox.Items.RemoveAt(idx);
                if (listBox.Items.Count > idx)
                {
                    listBox.SelectedIndex = idx;
                }
                else if (idx > 0)
                {
                    listBox.SelectedIndex = idx - 1;
                }
            }
        }

        private void ClickButton_Click(int macroIndex)
        {
            var listBox = _procedureListBoxes[macroIndex];
            listBox.Items.Add($"Click({_xPosTextBoxes[macroIndex].Text}, {_yPosTextBoxes[macroIndex].Text})");
            if (_eachCheckBoxes[macroIndex].IsChecked == true)
            {
                listBox.Items.Add($"Wait({_waitTextBoxes[macroIndex].Text}ms)");
            }
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        private void DoubleClickButton_Click(int macroIndex)
        {
            var listBox = _procedureListBoxes[macroIndex];
            listBox.Items.Add($"DClick({_xPosTextBoxes[macroIndex].Text}, {_yPosTextBoxes[macroIndex].Text})");
            if (_eachCheckBoxes[macroIndex].IsChecked == true)
            {
                listBox.Items.Add($"Wait({_waitTextBoxes[macroIndex].Text}ms)");
            }
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        private void RightClickButton_Click(int macroIndex)
        {
            var listBox = _procedureListBoxes[macroIndex];
            listBox.Items.Add($"RClick({_xPosTextBoxes[macroIndex].Text}, {_yPosTextBoxes[macroIndex].Text})");
            if (_eachCheckBoxes[macroIndex].IsChecked == true)
            {
                listBox.Items.Add($"Wait({_waitTextBoxes[macroIndex].Text}ms)");
            }
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        private void PressButton_Click(int macroIndex)
        {
            var listBox = _procedureListBoxes[macroIndex];
            listBox.Items.Add($"Press({_keyTextBoxes[macroIndex].Text})");
            if (_eachCheckBoxes[macroIndex].IsChecked == true)
            {
                listBox.Items.Add($"Wait({_waitTextBoxes[macroIndex].Text}ms)");
            }
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        private void WaitButton_Click(int macroIndex)
        {
            var listBox = _procedureListBoxes[macroIndex];
            listBox.Items.Add($"Wait({_waitTextBoxes[macroIndex].Text}ms)");
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        private void KeyTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender is not System.Windows.Controls.TextBox keyTextBox) return;

            e.Handled = true;
            string keyText = string.Empty;
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            if (key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LeftShift || key == Key.RightShift || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            if (modifiers.HasFlag(ModifierKeys.Control)) keyText += "Ctrl+";
            if (modifiers.HasFlag(ModifierKeys.Alt)) keyText += "Alt+";
            if (modifiers.HasFlag(ModifierKeys.Shift)) keyText += "Shift+";
            
            var realKey = key == Key.System ? e.SystemKey : key;
            keyText += realKey.ToString();

            keyTextBox.Text = keyText;
        }
        #endregion

        private void Routine(int macroIndex, CancellationToken token)
        {
            var state = _macroStates[macroIndex];
            var listBox = _procedureListBoxes[macroIndex];

            while (state.IsRunning && !token.IsCancellationRequested)
            {
                int itemCount = 0;
                Dispatcher.Invoke(() => itemCount = listBox.Items.Count);

                for (int i = 0; i < itemCount; i++)
                {
                    if (!state.IsRunning || token.IsCancellationRequested) break;

                    string? str = null;
                    Dispatcher.Invoke(() =>
                    {
                        listBox.SelectedIndex = i;
                        str = listBox.Items[i]?.ToString();
                    });

                    if (string.IsNullOrWhiteSpace(str)) continue;

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

                if (!state.LoopMode)
                {
                    state.IsRunning = false;
                }
            }
            // Ensure state is updated on exit
            state.IsRunning = false;
        }
    }
}