using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using HtmlAgilityPack;
using ErinWave.Math;
using ErinWave.Extensions;

namespace ErinWave.CourseraExtractor
{
    public partial class MainWindow : Window
    {
        private string _extractedText = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
		}

		private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "HTML 파일 선택",
                Filter = "HTML 파일 (*.html;*.htm)|*.html;*.htm|텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*",
                FilterIndex = 3,
                FileName = "1_2.txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text.Trim();
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("파일을 선택해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SetLoadingState(true, "HTML 텍스트 추출 중...");

                string htmlContent = File.ReadAllText(filePath, Encoding.UTF8);
                string cleanText = ExtractAndCleanText(htmlContent);

                ExtractedTextTextBox.Text = cleanText;

                SetLoadingState(false, $"추출 완료 - {cleanText.Length}글자");
                SaveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                SetLoadingState(false, "오류 발생");
                MessageBox.Show($"텍스트 추출 중 오류가 발생했습니다:\n{ex.Message}",
                    "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ExtractAndCleanText(string htmlContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var cleanTexts = new List<string>();

            // rc-Phrase 클래스에서 텍스트 추출 (한국어 대본)
            var phraseNodes = htmlDoc.DocumentNode.SelectNodes("//*[contains(@class, 'rc-Phrase')]");
            if (phraseNodes != null)
            {
                foreach (var node in phraseNodes)
                {
                    var text = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(text) && text.Length > 10)
                    {
                        // HTML 엔티티 디코딩 및 정리
                        text = System.Net.WebUtility.HtmlDecode(text);
                        text = Regex.Replace(text, @"&nbsp;", " ");
                        text = Regex.Replace(text, @"<[^>]+>", "");
                        text = Regex.Replace(text, @"\s+", " ");
                        text = text.Trim();

                        // 한국어 텍스트만 필터링
                        if (ContainsKorean(text))
                        {
                            cleanTexts.Add(text);
                        }
                    }
                }
            }

            // css-4s48ix 클래스에서 텍스트 추출
            var spanNodes = htmlDoc.DocumentNode.SelectNodes("//span[contains(@class, 'css-4s48ix')]");
            if (spanNodes != null)
            {
                foreach (var node in spanNodes)
                {
                    var text = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(text) && text.Length > 10)
                    {
                        text = System.Net.WebUtility.HtmlDecode(text);
                        text = Regex.Replace(text, @"&nbsp;", " ");
                        text = Regex.Replace(text, @"\s+", " ");
                        text = text.Trim();

                        if (ContainsKorean(text))
                        {
                            cleanTexts.Add(text);
                        }
                    }
                }
            }

            // 타임스탬프 정보 추가
            var timestampNodes = htmlDoc.DocumentNode.SelectNodes("//button[contains(@class, 'timestamp')]");
            if (timestampNodes != null)
            {
                var timestamps = new List<string>();
                foreach (var node in timestampNodes)
                {
                    var text = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, @"\d+:\d+"))
                    {
                        timestamps.Add(text);
                    }
                }

                if (timestamps.Any())
                {
                    cleanTexts.Insert(0, $"타임스탬프: {string.Join(", ", timestamps)}");
                    cleanTexts.Insert(1, new string('=', 50));
                }
            }

            // 중복 제거 및 정리
            var uniqueTexts = cleanTexts.Distinct().ToList();

            // 최종 텍스트 정리
            var result = new StringBuilder();
            for (int i = 0; i < uniqueTexts.Count; i++)
            {
                var text = uniqueTexts[i];

                // 여러 개의 공백을 하나로 정리
                text = Regex.Replace(text, @"\s+", " ");

                // 문장 끝 정리
                if (!text.EndsWith("."))
                {
                    text += ".";
                }

                result.AppendLine($"{i + 1}. {text}");
                result.AppendLine(); // 빈 줄 추가
            }

            return result.ToString().Trim();
        }

        private bool ContainsKorean(string text)
        {
            return Regex.IsMatch(text, @"[가-힣]");
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ExtractedTextTextBox.Text))
            {
                MessageBox.Show("저장할 텍스트가 없습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "추출된 텍스트 저장",
                    Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*",
                    DefaultExt = "txt",
                    FileName = $"extracted_text_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var content = $"추출 시간: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                $"파일: {FilePathTextBox.Text}\n" +
                                new string('=', 50) + "\n\n" +
                                ExtractedTextTextBox.Text;

                    File.WriteAllText(saveFileDialog.FileName, content, Encoding.UTF8);

                    MessageBox.Show($"추출된 텍스트가 {saveFileDialog.FileName}에 저장되었습니다.",
                        "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"저장 중 오류가 발생했습니다:\n{ex.Message}",
                    "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ExtractedTextTextBox.Text = string.Empty;
            SaveButton.IsEnabled = false;
            FilePathTextBox.Text = "D:\\Project\\CS\\ErinWave\\Coursera\\1_2.txt";
            FooterStatusText.Text = "준비됨";
        }

        private void CopyAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(ExtractedTextTextBox.Text);
                MessageBox.Show("전체 텍스트가 클립보드에 복사되었습니다.",
                    "복사 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"클립보드 복사 중 오류가 발생했습니다:\n{ex.Message}",
                    "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetLoadingState(bool isLoading, string message)
        {
            ExtractButton.IsEnabled = !isLoading;
            ProgressBar.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            FooterStatusText.Text = message;
        }
    }
}