using System.Timers;

using Timer = System.Timers.Timer;

namespace ErinWave.NeuroExposePcSound
{
	public class VolumeController
	{
		private Timer _onTimer; // 소리 켜짐 (10초) 타이머
		private Timer _offTimer; // 음소거 (15초) 타이머
		private bool _isMuted = false;

		public void StartControl()
		{
			// 1. 소리 켜기 (초기 상태)
			SetSystemMasterMute(false);
			_isMuted = false;

			// 2. 켜짐 타이머 설정 (10초 후 음소거 실행)
			_onTimer = new Timer(10000); // 10초
			_onTimer.Elapsed += OnTimerElapsed;
			_onTimer.AutoReset = false;
			_onTimer.Start();
		}

		private void OnTimerElapsed(object sender, ElapsedEventArgs e)
		{
			// 10초 경과 -> 음소거 실행
			SetSystemMasterMute(true);
			_isMuted = true;

			// 켜짐 타이머 멈추고, 음소거 타이머 시작 (15초)
			_onTimer.Stop();

			_offTimer = new Timer(15000); // 15초
			_offTimer.Elapsed += OffTimerElapsed;
			_offTimer.AutoReset = false;
			_offTimer.Start();
		}

		private void OffTimerElapsed(object sender, ElapsedEventArgs e)
		{
			// 15초 경과 -> 소리 켜기 실행
			SetSystemMasterMute(false);
			_isMuted = false;

			// 음소거 타이머 멈추고, 다시 켜짐 타이머 시작 (10초)
			_offTimer.Stop();

			_onTimer.Start();
		}

		private void SetSystemMasterMute(bool mute)
		{
			try
			{
				// 완성된 도우미 클래스의 정적 메서드 호출
				VolumeControlHelper.SetSystemMasterMute(mute);
			}
			catch (Exception ex)
			{
				// 오류 처리: COM 객체 생성 실패, 장치 없음 등
				// 실제 애플리케이션에서는 로그를 남기거나 사용자에게 알림
				System.Diagnostics.Debug.WriteLine($"볼륨 제어 오류: {ex.Message}");
				// 필요하다면 예외를 다시 던져 상위 호출자가 처리하도록 함
				throw;
			}
		}

		public void StopControl()
		{
			// 타이머가 실행 중인 경우 안전하게 중지
			_onTimer?.Stop();
			_onTimer?.Dispose();
			_offTimer?.Stop();
			_offTimer?.Dispose();

			// 혹시 음소거 상태로 중지되었더라도 소리 복구
			SetSystemMasterMute(false);
			_isMuted = false;
		}
	}
}
