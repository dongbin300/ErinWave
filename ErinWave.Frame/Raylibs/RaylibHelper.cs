
using ErinWave.Frame.Raylibs.Enums;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.Frame.Raylibs
{
	public class RaylibHelper
	{
		#region Base
		public static void Init(int screenWidth, int screenHeight, int fps, string title = "", ConfigFlags configFlags = ConfigFlags.ResizableWindow)
		{
			Raylib.SetConfigFlags(configFlags);
			Raylib.InitWindow(screenWidth, screenHeight, title);
			Raylib.SetTargetFPS(fps);
		}
		#endregion

		#region Text
		public static void DrawTextAnchored(string text, float x, float y, int fontSize, Color color, Anchor anchor = Anchor.TopLeft)
		{
			int w = Raylib.MeasureText(text, fontSize);
			int h = fontSize;

			switch (anchor)
			{
				case Anchor.TopCenter: x -= w / 2f; break;
				case Anchor.TopRight: x -= w; break;
				case Anchor.MiddleLeft: y -= h / 2f; break;
				case Anchor.Center: x -= w / 2f; y -= h / 2f; break;
				case Anchor.MiddleRight: x -= w; y -= h / 2f; break;
				case Anchor.BottomLeft: y -= h; break;
				case Anchor.BottomCenter: x -= w / 2f; y -= h; break;
				case Anchor.BottomRight: x -= w; y -= h; break;
			}

			Raylib.DrawText(text, (int)x, (int)y, fontSize, color);
		}

		public static void DrawTextAnchored(string text, float x, float y, Font font, int fontSize, float spacing, Color color, Anchor anchor = Anchor.TopLeft)
		{
			Vector2 size = Raylib.MeasureTextEx(font, text, fontSize, spacing);
			float w = size.X;
			float h = size.Y;

			switch (anchor)
			{
				case Anchor.TopCenter: x -= w / 2f; break;
				case Anchor.TopRight: x -= w; break;
				case Anchor.MiddleLeft: y -= h / 2f; break;
				case Anchor.Center: x -= w / 2f; y -= h / 2f; break;
				case Anchor.MiddleRight: x -= w; y -= h / 2f; break;
				case Anchor.BottomLeft: y -= h; break;
				case Anchor.BottomCenter: x -= w / 2f; y -= h; break;
				case Anchor.BottomRight: x -= w; y -= h; break;
			}

			Raylib.DrawTextEx(font, text, new Vector2(x, y), fontSize, spacing, color);
		}

		public static void DrawRectangleAnchored(float x, float y, float width, float height, Color color, Anchor anchor = Anchor.TopLeft)
		{
			switch (anchor)
			{
				case Anchor.TopCenter: x -= width / 2f; break;
				case Anchor.TopRight: x -= width; break;
				case Anchor.MiddleLeft: y -= height / 2f; break;
				case Anchor.Center: x -= width / 2f; y -= height / 2f; break;
				case Anchor.MiddleRight: x -= width; y -= height / 2f; break;
				case Anchor.BottomLeft: y -= height; break;
				case Anchor.BottomCenter: x -= width / 2f; y -= height; break;
				case Anchor.BottomRight: x -= width; y -= height; break;
			}

			Raylib.DrawRectangle((int)x, (int)y, (int)width, (int)height, color);
		}
		#endregion
	}
}
