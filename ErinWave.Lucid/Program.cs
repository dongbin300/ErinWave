using osuTK;
using osuTK.Graphics.OpenGL;
using osuTK.Graphics;

using System.Drawing;

namespace ErinWave.Lucid
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using var window = new GameWindow(600, 400, GraphicsMode.Default, "Lucid", GameWindowFlags.Default, DisplayDevice.Default, 3, 5, GraphicsContextFlags.ForwardCompatible);

			window.Load += (sender, e) =>
			{
				GL.ClearColor(Color.CornflowerBlue);
			};

			window.RenderFrame += (sender, e) =>
			{
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				GL.Begin(PrimitiveType.Triangles);
				GL.Color3(Color.Red);
				GL.Vertex3(-0.5f, -0.5f, 0.0f);
				GL.Color3(Color.Green);
				GL.Vertex3(0.5f, -0.5f, 0.0f);
				GL.Color3(Color.Blue);
				GL.Vertex3(0.0f, 0.5f, 0.0f);
				GL.End();

				window.SwapBuffers();
			};

			window.Run();
		}
	}
}
