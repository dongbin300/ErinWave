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
	/// MainWindow2.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow2 : Window
	{
		private D2DFactory _d2dFactory = default!;
		private WindowRenderTarget _renderTarget = default!;
		private DispatcherTimer _timer = default!;
		private DispatcherTimer movementTimer = default!;
		private bool isMovingUp, isMovingDown, isMovingLeft, isMovingRight;

		public MainWindow2()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
			SizeChanged += MainWindow_SizeChanged;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			InitializeDirect2D();
			GameManager.Init(_renderTarget);
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
		}

		private void StartRenderLoop()
		{
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(10)
			};
			_timer.Tick += (s, e) => Render();
			_timer.Start();

			movementTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(20)
			};
			movementTimer.Tick += MovementTimer_Tick;
		}

		private void Render()
		{
			_renderTarget.BeginDraw();
			_renderTarget.Clear(new RawColor4(0, 0, 0, 1));

			GameManager.Render();

			_renderTarget.EndDraw();
		}

		

		protected override void OnClosed(EventArgs e)
		{
			_renderTarget.Dispose();
			_d2dFactory.Dispose();
			base.OnClosed(e);
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					Environment.Exit(0);
					break;
				case Key.C:
					isMovingUp = true;
					break;
				case Key.Down:
					isMovingDown = true;
					break;
				case Key.Left:
					isMovingLeft = true;
					break;
				case Key.Right:
					isMovingRight = true;
					break;
			}

			if (!movementTimer.IsEnabled)
			{
				movementTimer.Start();
			}
		}

		private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.C:
					isMovingUp = false;
					break;
				case Key.Down:
					isMovingDown = false;
					break;
				case Key.Left:
					isMovingLeft = false;
					break;
				case Key.Right:
					isMovingRight = false;
					break;
			}

			if (!isMovingUp && !isMovingDown && !isMovingLeft && !isMovingRight)
			{
				movementTimer.Stop();
			}
		}

		private void MovementTimer_Tick(object? sender, EventArgs e)
		{
			if (isMovingUp)
			{
				GameManager.Me.Jump();
			}
			if (isMovingDown)
			{
				GameManager.Me.MoveDown();
			}
			if (isMovingLeft)
			{
				GameManager.Me.MoveLeft();
			}
			if (isMovingRight)
			{
				GameManager.Me.MoveRight();
			}
		}
	}
}
