
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
		public static void DrawTextAligned(string text, float x, float y, int fontSize, Color color, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
		{
			int textWidth = Raylib.MeasureText(text, fontSize);
			int textHeight = fontSize;

			switch (horizontalAlignment)
			{
				case HorizontalAlignment.Center: x -= textWidth / 2f; break;
				case HorizontalAlignment.Right: x -= textWidth; break;
			}

			switch (verticalAlignment)
			{
				case VerticalAlignment.Middle: y -= textHeight / 2f; break;
				case VerticalAlignment.Bottom: y -= textHeight; break;
			}

			Raylib.DrawText(text, (int)x, (int)y, fontSize, color);
		}

		public static void DrawTextAligned(string text, Rectangle bounds, int fontSize, Color color, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
		{
			int textWidth = Raylib.MeasureText(text, fontSize);
			int textHeight = fontSize;

			float x = bounds.X;
			float y = bounds.Y;

			switch (horizontalAlignment)
			{
				case HorizontalAlignment.Center: x += (bounds.Width - textWidth) / 2f; break;
				case HorizontalAlignment.Right: x += bounds.Width - textWidth; break;
			}

			switch (verticalAlignment)
			{
				case VerticalAlignment.Middle: y += (bounds.Height - textHeight) / 2f; break;
				case VerticalAlignment.Bottom: y += bounds.Height - textHeight; break;
			}

			Raylib.DrawText(text, (int)x, (int)y, fontSize, color);
		}

		public static void DrawTextAligned(Font font, string text, Rectangle bounds, float fontSize, float spacing, Color color, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
		{
			Vector2 size = Raylib.MeasureTextEx(font, text, fontSize, spacing);

			float x = bounds.X;
			float y = bounds.Y;

			switch (horizontalAlignment)
			{
				case HorizontalAlignment.Center: x += (bounds.Width - size.X) / 2f; break;
				case HorizontalAlignment.Right: x += bounds.Width - size.X; break;
			}

			switch (verticalAlignment)
			{
				case VerticalAlignment.Middle: y += (bounds.Height - size.Y) / 2f; break;
				case VerticalAlignment.Bottom: y += bounds.Height - size.Y; break;
			}

			Raylib.DrawTextEx(font, text, new Vector2(x, y), fontSize, spacing, color);
		}
		#endregion
	}
}
