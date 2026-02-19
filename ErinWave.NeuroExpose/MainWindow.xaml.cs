using ErinWave.Math;

using SkiaSharp;
using SkiaSharp.Views.Desktop;

using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ErinWave.NeuroExpose
{
	public partial class MainWindow : Window
	{
		private readonly string[] VersionFolders = { "v1", "v2", "v3", "v4", "v5", "v6" };
		private List<MediaPlayer> players = new List<MediaPlayer>();
		private ErinWaveRandom random = new ErinWaveRandom();

		// Drop 설정
		private const double SPAWN_INTERVAL = 0.2; // 초

		// 타이머
		private DispatcherTimer updateTimer;
		private DispatcherTimer spawnTimer;

		// Drop 리스트
		private List<Drop> drops = new List<Drop>();

		// Paint 캐시
		private SKPaint whitePaint;
		private SKPaint orangePaint;
		private SKPaint limePaint;
		private SKPaint hotPinkPaint;

		public MainWindow()
		{
			InitializeComponent();
			InitializePaint();
			LoadAndPlayMusic();
			SetupTimers();
		}

		private void InitializePaint()
		{
			whitePaint = new SKPaint
			{
				Color = SKColors.White,
				IsAntialias = false,
				Style = SKPaintStyle.Fill
			};

			orangePaint = new SKPaint
			{
				Color = SKColors.Orange,
				IsAntialias = false,
				Style = SKPaintStyle.Fill
			};

			limePaint = new SKPaint
			{
				Color = SKColors.Lime,
				IsAntialias = false,
				Style = SKPaintStyle.Fill
			};

			hotPinkPaint = new SKPaint
			{
				Color = SKColors.HotPink,
				IsAntialias = false,
				Style = SKPaintStyle.Fill
			};
		}

		private void LoadAndPlayMusic()
		{
			var allMp3s = new List<string>();
			foreach (var version in VersionFolders)
			{
				string folder = Path.Combine(@"D:\Project\CS\ErinWave\ErinWave.NeuroExpose\Resources\Songs", version);
				if (Directory.Exists(folder))
				{
					allMp3s.AddRange(Directory.GetFiles(folder, "*.mp3"));
				}
			}

			if (allMp3s.Count < 5)
			{
				System.Windows.MessageBox.Show("Need at least 5 mp3 files across all version folders.");
				System.Windows.Application.Current.Shutdown();
			}

			int channelCount = 7;                // 채널 수 변수
			double maxVolume = 0.8;              // 첫 번째 채널 최대 볼륨
			double minVolume = 0.01;             // 마지막 채널 최소 볼륨

			var selected = allMp3s
				.OrderBy(x => random.Next())
				.Take(channelCount)
				.ToList();

			for (int i = 0; i < selected.Count; i++)
			{
				var player = new MediaPlayer();
				player.Open(new Uri(selected[i], UriKind.RelativeOrAbsolute));

				// ---------------------
				// 🔊 볼륨 자동 감소 공식
				// 선형 감소 (Linear Fade)
				// ---------------------
				double t = (double)i / System.Math.Pow((channelCount - 1), 2); // 0~1
				double volume = maxVolume - (maxVolume - minVolume) * t;

				player.Volume = volume;

				player.MediaEnded += (s, e) =>
				{
					((MediaPlayer)s).Position = TimeSpan.Zero;
					((MediaPlayer)s).Play();
				};

				player.Play();
				players.Add(player);
			}

		}

		private void SetupTimers()
		{
			// 120 FPS (8.33ms)
			updateTimer = new DispatcherTimer(DispatcherPriority.Render);
			updateTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / 120.0);
			updateTimer.Tick += UpdateTick;
			updateTimer.Start();

			// Drop 생성
			spawnTimer = new DispatcherTimer();
			spawnTimer.Interval = TimeSpan.FromSeconds(SPAWN_INTERVAL);
			spawnTimer.Tick += (s, e) => SpawnDrop();
			spawnTimer.Start();
		}

		private void SpawnDrop()
		{
			if (ActualWidth <= 0) return;

			var r1 = random.Next(100);
			var iter = r1 < 40 ? 1 : r1 < 70 ? 2 : r1 < 90 ? 3 : 4;

			for (int i = 0; i < iter; i++)
			{
				// 화면 가로를 4등분
				int section = random.Next(4);
				float sectionWidth = (float)ActualWidth / 4f;
				float x = section * sectionWidth;

				// 12% 확률로 롱노트
				bool isLong = random.Next(100) < 12;
				float height = isLong ? random.Next((int)sectionWidth * 2, (int)sectionWidth * 10) : sectionWidth / 5;

				// 색상 확률: 하양 80%, 오렌지 12%, 라임 4%, 핫핑크 4%
				int colorRoll = random.Next(100);
				SKPaint paint;
				if (colorRoll < 80)
					paint = whitePaint;
				else if (colorRoll < 92)
					paint = orangePaint;
				else if (colorRoll < 96)
					paint = limePaint;
				else
					paint = hotPinkPaint;

				drops.Add(new Drop
				{
					X = x,
					Y = -height,
					Height = height,
					Paint = paint
				});
			}
		}

		private void UpdateTick(object sender, EventArgs e)
		{
			float screenHeight = (float)ActualHeight;

			// Drop 이동
			for (int i = drops.Count - 1; i >= 0; i--)
			{
				drops[i].Y += screenHeight / 50;

				// 화면 밖으로 나가면 제거
				if (drops[i].Y > screenHeight)
					drops.RemoveAt(i);
			}

			// 다시 그리기
			SkiaSurface.InvalidateVisual();
		}

		private void SkiaSurface_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;
			canvas.Clear(SKColors.Black);

			// 모든 Drop 그리기
			foreach (var drop in drops)
			{
				canvas.DrawRect(drop.X, drop.Y, (float)ActualWidth / 4f, drop.Height, drop.Paint);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			updateTimer?.Stop();
			spawnTimer?.Stop();

			whitePaint?.Dispose();
			orangePaint?.Dispose();
			limePaint?.Dispose();
			hotPinkPaint?.Dispose();

			foreach (var p in players)
			{
				p.Stop();
				p.Close();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (Screen.AllScreens.Length < 2)
			{
				System.Windows.MessageBox.Show("두 번째 모니터가 감지되지 않습니다.");
				return;
			}

			var second = Screen.AllScreens[1];
			var bounds = second.Bounds;

			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;

			Left = bounds.Left;
			Top = bounds.Top;
			Width = bounds.Width;
			Height = bounds.Height;
		}
	}

	public class Drop
	{
		public float X;
		public float Y;
		public float Height;
		public SKPaint Paint;
	}
}