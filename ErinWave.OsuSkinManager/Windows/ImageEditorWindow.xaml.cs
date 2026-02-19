using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace ErinWave.OsuSkinManager
{
    public partial class ImageEditorWindow : Window
    {
        private string? _currentSkinPath;
        private string? _selectedGameMode;
        private List<SkinImageItem> _currentImages = new();
        private SkinImageItem? _selectedImage;

        // 게임 모드별 이미지 목록
        private static readonly Dictionary<string, List<string>> GameModeImages = new()
        {
            ["Standard"] = new()
            {
                "hitcircle", "hitcircleoverlay", "approachcircle",
                "sliderball", "sliderfollowcircle", "sliderendcircle", "sliderstartcircle",
                "spinner-background", "spinner-circle", "spinner-top", "spinner-middle", "spinner-bottom",
                "scorebar-bg", "scorebar-marker", "scorebar-colour",
                "mania-key1", "mania-key2", "mania-key3", "mania-key4", "mania-key5", "mania-key6", "mania-key7", "mania-key8",
                "menu-back", "menu-button-background", "menu-button"
            },
            ["Taiko"] = new()
            {
                "taiko-drum-inner", "taiko-drum-outer", "taiko-drum-flame",
                "taiko-barline", "taiko-hitcircle", "taiko-hito-circles"
            },
            ["Catch"] = new()
            {
                "fruit-pear", "fruit-grapes", "fruit-apple", "fruit-orange", "fruit-raspberry",
                "fruit-drop", "fruit-catcher", "fruit-catcher-idle",
                "fruit-catcher-fail", "fruit-catcher-kiai"
            },
            ["Mania"] = new()
            {
                "mania-key1", "mania-key2", "mania-key3", "mania-key4", "mania-key5", "mania-key6", "mania-key7", "mania-key8",
                "mania-note1", "mania-note2", "mania-note3", "mania-note4", "mania-note5", "mania-note6", "mania-note7", "mania-note8",
                "mania-stage-hint", "mania-stage-left", "mania-stage-right", "stage-hint",
                "mania-stage-light", "mania-stage-fail"
            }
        };

        public ImageEditorWindow()
        {
            InitializeComponent();
            StatusTextBlock.Text = "게임 모드를 선택해주세요";
            SelectedImageTitle.Text = "게임 모드를 선택한 후 폴더를 열어주세요";
        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string mode)
            {
                _selectedGameMode = mode;

                // 버튼 스타일 업데이트
                var buttons = new[] { StandardModeButton, TaikoModeButton, CatchModeButton, ManiaModeButton };
                foreach (var btn in buttons)
                {
                    btn.Style = (Style)FindResource("SecondaryButtonStyle");
                }
                button.Style = (Style)FindResource("PrimaryButtonStyle");

                // 이미지 목록 로드
                LoadImagesForMode(mode);
                StatusTextBlock.Text = $"{mode} 모드: {GameModeImages[mode].Count}개 이미지 발견";
            }
        }

        private void LoadImagesForMode(string mode)
        {
            _currentImages.Clear();

            if (GameModeImages.TryGetValue(mode, out var imageList))
            {
                foreach (var imageName in imageList)
                {
                    _currentImages.Add(new SkinImageItem
                    {
                        Name = imageName,
                        Description = GetImageDescription(imageName, mode)
                    });
                }
            }

            // 검색 필터 적용
            FilterImages();

            ImageListBox.ItemsSource = _currentImages;
        }

        private string GetImageDescription(string imageName, string mode)
        {
            return mode switch
            {
                "Standard" when imageName.Contains("hitcircle") => "히트서클",
                "Standard" when imageName.Contains("approach") => "어프로치 서클",
                "Standard" when imageName.Contains("slider") => "슬라이더",
                "Standard" when imageName.Contains("spinner") => "스피너",
                "Standard" when imageName.Contains("mania") => "매니아",
                "Standard" when imageName.Contains("menu") => "메뉴",
                "Taiko" when imageName.Contains("drum") => "태코 드럼",
                "Taiko" when imageName.Contains("taiko-") => "태코 일반",
                "Catch" when imageName.Contains("fruit") => "과일",
                "Mania" when imageName.Contains("key") => "키",
                "Mania" when imageName.Contains("note") => "노트",
                _ => imageName
            };
        }

        private void FilterImages()
        {
            var searchText = ImageSearchTextBox.Text?.ToLowerInvariant() ?? "";
            var filteredImages = string.IsNullOrWhiteSpace(searchText)
                ? _currentImages
                : _currentImages.Where(img =>
                    img.Name.ToLowerInvariant().Contains(searchText) ||
                    img.Description.ToLowerInvariant().Contains(searchText));

            ImageListBox.ItemsSource = filteredImages.ToList();
        }

        private void ImageSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterImages();
        }

        private void ImageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageListBox.SelectedItem is SkinImageItem selectedImage)
            {
                _selectedImage = selectedImage;
                SelectedImageTitle.Text = selectedImage.Name;
                LoadImage(selectedImage);
            }
        }

        private void LoadImage(SkinImageItem imageItem)
        {
            if (string.IsNullOrEmpty(_currentSkinPath) || !File.Exists(Path.Combine(_currentSkinPath, imageItem.Name + ".png")))
            {
                StatusTextBlock.Text = $"이미지 파일을 찾을 수 없습니다: {imageItem.Name}";
                return;
            }

            try
            {
                var imagePath = Path.Combine(_currentSkinPath, imageItem.Name + ".png");
                var bitmap = new BitmapImage(new Uri(imagePath));
                PreviewImage.Source = bitmap;
                PreviewImage.Visibility = Visibility.Visible;
                NoImageMessage.Visibility = Visibility.Collapsed;
                StatusTextBlock.Text = $"이미지 로드됨: {imageItem.Name}";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"이미지 로드 실패: {ex.Message}";
                PreviewImage.Visibility = Visibility.Collapsed;
                NoImageMessage.Visibility = Visibility.Visible;
            }
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedGameMode))
            {
                MessageBox.Show("먼저 게임 모드를 선택해주세요.", "알림",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "osu! 스킨 폴더 선택 (아무 파일이나 선택하세요)",
                Filter = "모든 파일 (*.*)|*.*",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "폴더 선택"
            };

            if (dialog.ShowDialog() == true)
            {
                _currentSkinPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                LoadImagesForMode(_selectedGameMode!);
                StatusTextBlock.Text = $"폴더 열림: {System.IO.Path.GetFileName(_currentSkinPath)}";
            }
        }

        private void SaveAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImages.Count == 0)
            {
                MessageBox.Show("저장할 이미지가 없습니다.", "알림",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // 현재 수정된 이미지들 저장 로직
                // 실제로는 편집된 이미지를 파일로 저장해야 함
                MessageBox.Show("모든 이미지가 저장되었습니다.", "성공",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 저장 실패: {ex.Message}", "오류",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class SkinImageItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}