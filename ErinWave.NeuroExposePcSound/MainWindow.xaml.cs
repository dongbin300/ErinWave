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

namespace ErinWave.NeuroExposePcSound
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly VolumeController _controller;

		public MainWindow()
		{
			InitializeComponent();
			// 실제 볼륨 제어 로직을 가진 컨트롤러 초기화
			_controller = new VolumeController();
		}

		// '훈련 시작' 버튼 클릭 이벤트 핸들러
		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// 2. 컨트롤러의 시작 메서드 호출
				_controller.StartControl();

				// 3. UI 상태 업데이트
				StartButton.IsEnabled = false;
				StopButton.IsEnabled = true;
				MessageBox.Show("미세노출 훈련을 시작합니다.", "시작", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				// 볼륨 제어 실패 시 예외 처리
				MessageBox.Show($"훈련 시작 중 오류 발생: {ex.Message}\nCore Audio API 접근 권한 등을 확인하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// '훈련 중지' 버튼 클릭 이벤트 핸들러
		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			// 4. 컨트롤러의 중지 메서드 호출
			_controller.StopControl(); // VolumeController에 StopControl() 메서드를 구현해야 합니다.

			// 5. UI 상태 업데이트
			StartButton.IsEnabled = true;
			StopButton.IsEnabled = false;
			MessageBox.Show("미세노출 훈련을 중지했습니다. 시스템 볼륨이 복구됩니다.", "중지", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}