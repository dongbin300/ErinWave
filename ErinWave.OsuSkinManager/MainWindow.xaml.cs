using ErinWave.OsuSkinManager.Models;
using ErinWave.OsuSkinManager.Services;

using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ErinWave.OsuSkinManager
{
	public partial class MainWindow : Window
	{
		private List<OsuSkin> _allSkins = new();
		private OsuSkin? _selectedSkin;
		private List<SkinFileInfo> _currentFiles = new();

		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
		}

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			await LoadSkinsAsync();
		}

		private async Task LoadSkinsAsync()
		{
			StatusTextBlock.Text = "스킨 목록 로딩 중...";
			RefreshButton.IsEnabled = false;

			await Task.Run(() =>
			{
				var osuPath = OsuPathDetector.GetOsuInstallationPath();
				if (string.IsNullOrEmpty(osuPath))
				{
					Dispatcher.Invoke(() =>
					{
						MessageBox.Show("osu! 설치 경로를 찾을 수 없습니다.\nosu!가 정상적으로 설치되어 있는지 확인해주세요.",
							"오류", MessageBoxButton.OK, MessageBoxImage.Error);
					});
					return;
				}

				var skinsPath = OsuPathDetector.GetSkinsPath(osuPath);
				if (string.IsNullOrEmpty(skinsPath))
				{
					Dispatcher.Invoke(() =>
					{
						MessageBox.Show("osu! 스킨 폴더를 찾을 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
					});
					return;
				}

				_allSkins = SkinManager.LoadSkins(skinsPath);
			});

			SkinsListView.ItemsSource = _allSkins;
			StatusTextBlock.Text = $"총 {_allSkins.Count}개의 스킨을 찾았습니다.";
			RefreshButton.IsEnabled = true;
		}

		private async void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			await LoadSkinsAsync();
		}

		private void SkinsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SkinsListView.SelectedItem is OsuSkin selectedSkin)
			{
				_selectedSkin = selectedSkin;
				DisplaySkinInfo(selectedSkin);
				LoadSkinFiles(selectedSkin);
			}
			else
			{
				_selectedSkin = null;
				ClearSkinInfo();
				FilesListView.ItemsSource = null;
			}
		}

		private void DisplaySkinInfo(OsuSkin skin)
		{
			SkinNameTextBlock.Text = skin.Name;
			AuthorTextBlock.Text = skin.Author ?? "-";
			VersionTextBlock.Text = skin.Version ?? "-";
			PathTextBlock.Text = skin.Path;

			// 미리보기 이미지 로드
			if (!string.IsNullOrEmpty(skin.PreviewImagePath) && File.Exists(skin.PreviewImagePath))
			{
				try
				{
					var bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.UriSource = new Uri(skin.PreviewImagePath);
					bitmap.CacheOption = BitmapCacheOption.OnLoad;
					bitmap.EndInit();
					PreviewImage.Source = bitmap;
				}
				catch
				{
					PreviewImage.Source = null;
				}
			}
			else
			{
				PreviewImage.Source = null;
			}
		}

		private void ClearSkinInfo()
		{
			SkinNameTextBlock.Text = "선택된 스킨 없음";
			AuthorTextBlock.Text = "-";
			VersionTextBlock.Text = "-";
			PathTextBlock.Text = "-";
			PreviewImage.Source = null;
		}

		private void LoadSkinFiles(OsuSkin skin)
		{
			_currentFiles.Clear();

			// 이미지 파일
			foreach (var imageFile in skin.ImageFiles)
			{
				var fullPath = Path.Combine(skin.Path, imageFile);
				if (File.Exists(fullPath))
				{
					var fileInfo = new FileInfo(fullPath);
					_currentFiles.Add(new SkinFileInfo
					{
						Name = imageFile,
						FullPath = fullPath,
						Type = FileType.Image,
						Size = fileInfo.Length,
						LastModified = fileInfo.LastWriteTime
					});
				}
			}

			// 오디오 파일
			foreach (var audioFile in skin.AudioFiles)
			{
				var fullPath = Path.Combine(skin.Path, audioFile);
				if (File.Exists(fullPath))
				{
					var fileInfo = new FileInfo(fullPath);
					_currentFiles.Add(new SkinFileInfo
					{
						Name = audioFile,
						FullPath = fullPath,
						Type = FileType.Audio,
						Size = fileInfo.Length,
						LastModified = fileInfo.LastWriteTime
					});
				}
			}

			// 설정 파일
			foreach (var configFile in skin.ConfigFiles)
			{
				var fullPath = Path.Combine(skin.Path, configFile);
				if (File.Exists(fullPath))
				{
					var fileInfo = new FileInfo(fullPath);
					_currentFiles.Add(new SkinFileInfo
					{
						Name = configFile,
						FullPath = fullPath,
						Type = FileType.Config,
						Size = fileInfo.Length,
						LastModified = fileInfo.LastWriteTime
					});
				}
			}

			// 파일 필터 적용
			FilterFiles();
		}

		private void FilterFiles()
		{
			IEnumerable<SkinFileInfo> filteredFiles = _currentFiles;

			if (ImagesRadio?.IsChecked == true)
				filteredFiles = _currentFiles.Where(f => f.Type == FileType.Image);
			else if (AudioRadio?.IsChecked == true)
				filteredFiles = _currentFiles.Where(f => f.Type == FileType.Audio);
			else if (ConfigRadio?.IsChecked == true)
				filteredFiles = _currentFiles.Where(f => f.Type == FileType.Config);

			FilesListView?.ItemsSource = filteredFiles.OrderBy(f => f.Name).ToList();
		}

		private void FileFilter_Checked(object sender, RoutedEventArgs e)
		{
			FilterFiles();
		}

		private void FilesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (FilesListView.SelectedItem is SkinFileInfo file)
			{
				try
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = file.FullPath,
						UseShellExecute = true
					});
				}
				catch (Exception ex)
				{
					MessageBox.Show($"파일을 열 수 없습니다: {ex.Message}", "오류",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var searchText = SearchTextBox.Text.ToLowerInvariant();
			if (string.IsNullOrWhiteSpace(searchText) || searchText == "스킨 검색...")
			{
				SkinsListView?.ItemsSource = _allSkins;
			}
			else
			{
				var filteredSkins = _allSkins.Where(s =>
					s.Name.ToLowerInvariant().Contains(searchText) ||
					(s.Author?.ToLowerInvariant().Contains(searchText) ?? false));
				SkinsListView?.ItemsSource = filteredSkins.ToList();
			}
		}

		private async void CopySkinButton_Click(object sender, RoutedEventArgs e)
		{
			if (_selectedSkin == null)
			{
				MessageBox.Show("복사할 스킨을 선택해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var dialog = new InputDialog("새 스킨 이름을 입력하세요:", _selectedSkin.Name + "_copy");
			if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
			{
				if (SkinManager.CopySkin(_selectedSkin, dialog.InputText))
				{
					MessageBox.Show("스킨 복사가 완료되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
					await LoadSkinsAsync();
				}
				else
				{
					MessageBox.Show("스킨 복사에 실패했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private async void DeleteSkinButton_Click(object sender, RoutedEventArgs e)
		{
			if (_selectedSkin == null)
			{
				MessageBox.Show("삭제할 스킨을 선택해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"'{_selectedSkin.Name}' 스킨을 삭제하시겠습니까?\n\n⚠️ 이 작업은 되돌릴 수 없습니다!",
				"스킨 삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				if (SkinManager.DeleteSkin(_selectedSkin))
				{
					MessageBox.Show("스킨이 삭제되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
					await LoadSkinsAsync();
				}
				else
				{
					MessageBox.Show("스킨 삭제에 실패했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void ImageEditorButton_Click(object sender, RoutedEventArgs e)
		{
			var imageEditorWindow = new ImageEditorWindow();
			imageEditorWindow.Owner = this;
			imageEditorWindow.Show();
		}
	}
}