using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ErinWave.TransMaster
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// 
	/// script process
	/// === 번역 사전 작업
	/// 1. 작품 구입
	/// 2. 작품 번역 작업 신청
	/// 3. 번역 작업 승인
	/// 
	/// === 번역 작업(오디오)
	/// 4. 오디오 파일 다운로드
	/// 5. whisper로 일어 텍스트 추출(.vtt)
	/// 6. 추출한 텍스트 후처리 (자동화)
	/// 7. chatgpt로 일어->한국어 번역
	/// 8. 다시 .vtt로 타임스탬프 맞춰서 정리 (자동화)
	/// 9. 사이트에 .vtt 업로드
	/// 
	/// === 마무리 작업
	/// 10. 검수
	/// 11. 번역 완료 후 제출
	/// 
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string WhisperExePath = @"C:\Users\Gaten\AppData\Local\Programs\Python\Python310\Scripts\whisper.exe";

		string taskDirectory = string.Empty;
		string selectedVttFileName = string.Empty;
		string selectedAudioFileName = string.Empty;
		List<VttSubtitle> subtitles = [];

		public MainWindow()
		{
			InitializeComponent();

			TaskDirectoryTextBox.Text = Settings1.Default.TaskDirectory;
		}

		private void TaskDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			taskDirectory = TaskDirectoryTextBox.Text;

			var vttFiles = Directory.GetFiles(taskDirectory, "*.vtt", SearchOption.AllDirectories);
			VttFileListBox.ItemsSource = vttFiles.Select(x => Path.GetRelativePath(taskDirectory, x));

			var mp3Files = Directory.GetFiles(taskDirectory, "*.mp3", SearchOption.AllDirectories);
			var wavFiles = Directory.GetFiles(taskDirectory, "*.wav", SearchOption.AllDirectories);
			var audioFiles = mp3Files.Concat(wavFiles).ToArray();
			AudioFileListBox.ItemsSource = audioFiles.Select(x => Path.GetRelativePath(taskDirectory, x));

			Settings1.Default.TaskDirectory = taskDirectory;
			Settings1.Default.Save();
		}

		private void VttFileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				var selectedItem = VttFileListBox.SelectedItem.ToString();

				if (string.IsNullOrEmpty(selectedItem))
				{
					return;
				}

				selectedVttFileName = Path.Combine(taskDirectory, selectedItem);

				subtitles = VttHelper.ParseVtt(selectedVttFileName);
				VttParsedTextBox.Text = string.Join(Environment.NewLine, subtitles.Select(x => $"{x.StartTime},{x.EndTime}\n{x.Text}"));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void VttParsedCopyButton_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(VttParsedTextBox.Text);
		}

		private void VttTranslatedPasteButton_Click(object sender, RoutedEventArgs e)
		{
			VttTranslatedTextBox.Text = Clipboard.GetText();
		}

		private void VttTranslatedWriteButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var directory = Path.GetDirectoryName(selectedVttFileName);
				var fileNameWithoutExt = Path.GetFileNameWithoutExtension(selectedVttFileName);

				if (directory == null)
				{
					return;
				}

				var newFileName = Path.Combine(directory, fileNameWithoutExt + "_t.vtt");

				VttHelper.ApplyTranslatedText(subtitles, VttTranslatedTextBox.Text);
				VttHelper.WriteVtt(newFileName, subtitles);

				MessageBox.Show($"VTT 생성 완료\n{newFileName}");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void VttTranslatedTrimButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var lines = VttTranslatedTextBox.Text.Split([Environment.NewLine], StringSplitOptions.None);
				var trimmedLines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => line.Trim());
				VttTranslatedTextBox.Text = string.Join(Environment.NewLine, trimmedLines);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void WhisperLog(string message)
		{
			var item = new ComboBoxItem() { Content = message };
			AudioWhisperLogListBox.Items.Add(item);
			AudioWhisperLogListBox.ScrollIntoView(item);
		}

		private void AudioFileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				var selectedItem = AudioFileListBox.SelectedItem.ToString();

				if (string.IsNullOrEmpty(selectedItem))
				{
					return;
				}

				selectedAudioFileName = Path.Combine(taskDirectory, selectedItem);

				AudioArgumentsTextBox.Text = $"\"{selectedAudioFileName}\" --device cuda --language ja --fp16 True --threads 16 --output_format vtt --word_timestamps True";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void AudioWhisperButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string arguments = AudioArgumentsTextBox.Text;

				var psi = new ProcessStartInfo
				{
					FileName = WhisperExePath,
					Arguments = arguments,
					RedirectStandardOutput = true,  // 표준 출력 리디렉션 (info, warning 포함 가능)
					RedirectStandardError = true,   // 표준 에러 리디렉션 (error, warning 포함 가능)
					UseShellExecute = false,
					CreateNoWindow = true
				};

				var process = new Process { StartInfo = psi };

				process.OutputDataReceived += (sender, e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						Dispatcher.Invoke(() => WhisperLog("[INFO] " + e.Data));
					}
				};

				process.ErrorDataReceived += (sender, e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						if (e.Data.Contains("warning", StringComparison.CurrentCultureIgnoreCase))
						{
							Dispatcher.Invoke(() => WhisperLog("[WARNING] " + e.Data));
						}
						else
						{
							Dispatcher.Invoke(() => WhisperLog("[ERROR] " + e.Data));
						}
					}
				};

				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}