using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using D2DFactory = SharpDX.Direct2D1.Factory;

namespace ErinWave.DirectEx
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		int STAR_COUNT = 5000;
		double FRAME_INTERVAL = 4.1667;

		private D2DFactory _d2dFactory;
		private WindowRenderTarget _renderTarget;
		private DispatcherTimer _timer;
		private List<Particle> _particles;
		private Random _random;
		private PathGeometry _starGeometry;

		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
			SizeChanged += MainWindow_SizeChanged;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			InitializeDirect2D();
			InitializeParticles();
			StartRenderLoop();
		}

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_renderTarget?.Dispose();
			InitializeDirect2D();
		}

		private void InitializeDirect2D()
		{
			_d2dFactory = new D2DFactory();
			var renderProps = new HwndRenderTargetProperties
			{
				Hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle,
				PixelSize = new Size2((int)ActualWidth, (int)ActualHeight),
				PresentOptions = PresentOptions.None
			};
			_renderTarget = new WindowRenderTarget(_d2dFactory, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)), renderProps);

			_starGeometry = CreateStarGeometry();
		}

		private void InitializeParticles()
		{
			_particles = [];
			_random = new Random();
			for (int i = 0; i < STAR_COUNT; i++)
			{
				_particles.Add(new Particle
				{
					Position = new RawVector2((float)_random.NextDouble() * (float)ActualWidth, (float)_random.NextDouble() * (float)ActualHeight),
					Size = (float)MathTool.NormalDistributionRandom(0.5, 0.25) * 1.2f + 0.1f,
					Speed = new RawVector2((float)(_random.NextDouble() - 0.5) * 0.01f, (float)(_random.NextDouble() - 0.5) * 0.01f),
					Color = new RawColor4(1, 1, 1, (float)_random.NextDouble())
				});
			}
		}

		private void StartRenderLoop()
		{
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(FRAME_INTERVAL)
			};
			_timer.Tick += (s, e) => Render();
			_timer.Start();
		}

		private void Render()
		{
			_renderTarget.BeginDraw();
			_renderTarget.Clear(new RawColor4(0, 0, 0, 1));

			foreach (var particle in _particles)
			{
				var newOpacity = Math.Clamp(particle.Color.A + (float)_random.NextDouble() - 0.5f * 0.1f, 0, 1);
				particle.Color = new RawColor4(1, 1, 1, newOpacity);
				var starBrush = new SolidColorBrush(_renderTarget, particle.Color);
				_renderTarget.FillEllipse(new Ellipse(particle.Position, particle.Size, particle.Size), starBrush);

				var newPosition = particle.Position;
				newPosition.X += particle.Speed.X;
				newPosition.Y += particle.Speed.Y;

				if (newPosition.X > (float)ActualWidth) newPosition.X = 0;
				if (newPosition.X < 0) newPosition.X = (float)ActualWidth;
				if (newPosition.Y > (float)ActualHeight) newPosition.Y = 0;
				if (newPosition.Y < 0) newPosition.Y = (float)ActualHeight;

				particle.Position = newPosition;
			}

			_renderTarget.EndDraw();
		}

		private PathGeometry CreateStarGeometry()
		{
			var geometry = new PathGeometry(_d2dFactory);

			using (var sink = geometry.Open())
			{
				const int numPoints = 5;
				const float outerRadius = 1.0f;
				const float innerRadius = 0.5f;
				var angleStep = Math.PI / numPoints;
				var angle = -Math.PI / 2;

				var points = new RawVector2[numPoints * 2];

				for (int i = 0; i < numPoints * 2; i += 2)
				{
					points[i] = new RawVector2((float)Math.Cos(angle) * outerRadius, (float)Math.Sin(angle) * outerRadius);
					angle += angleStep;
					points[i + 1] = new RawVector2((float)Math.Cos(angle) * innerRadius, (float)Math.Sin(angle) * innerRadius);
					angle += angleStep;
				}

				sink.BeginFigure(points[0], FigureBegin.Filled);
				for (int i = 1; i < points.Length; i++)
				{
					sink.AddLine(points[i]);
				}
				sink.AddLine(points[0]);
				sink.EndFigure(FigureEnd.Closed);

				sink.Close();
			}

			return geometry;
		}

		protected override void OnClosed(EventArgs e)
		{
			_renderTarget.Dispose();
			_d2dFactory.Dispose();
			_starGeometry.Dispose();
			base.OnClosed(e);
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				Environment.Exit(0);
			}
			else if (e.Key >= Key.D0 && e.Key <= Key.D9)
			{
				int monitorIndex = (int)e.Key - (int)Key.D0 - 1;

				Screen[] screens = Screen.AllScreens;
				if (monitorIndex >= 0 && monitorIndex < screens.Length)
				{
					var targetScreen = screens[monitorIndex];
					var window = System.Windows.Application.Current.MainWindow;

					if (window.WindowState == System.Windows.WindowState.Maximized)
					{
						window.WindowState = System.Windows.WindowState.Normal;
					}

					window.Left = targetScreen.Bounds.Left;
					window.Top = targetScreen.Bounds.Top;
					window.Width = targetScreen.Bounds.Width;
					window.Height = targetScreen.Bounds.Height;

					window.WindowState = System.Windows.WindowState.Maximized;
				}
			}
		}
	}

	public class Particle
	{
		public RawVector2 Position { get; set; }
		public float Size { get; set; }
		public RawVector2 Speed { get; set; }
		public RawColor4 Color { get; set; }
	}
}