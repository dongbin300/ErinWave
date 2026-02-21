using ErinWave.Frame.Raylibs;

using Raylib_cs;

namespace ErinWave.RayLibTest
{
	internal class Program
	{
		static void Main()
		{
			RaylibHelper.Initialize(250, 600, 240, "Pihagi", ConfigFlags.TransparentWindow | ConfigFlags.UndecoratedWindow);

			//string fontPath = @"C:\Users\Gaten\AppData\Local\Microsoft\Windows\Fonts\NanumBarunGothic.ttf";
			//var font = KoreanFontHelper.LoadFont(fontPath, 16);

			while (!Raylib.WindowShouldClose())
			{
				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.Blank);

				//Raylib.DrawTextEx(font, $"FPS: {Raylib.GetFPS()}", new Vector2(0,0), 16, 1f, Color.LightGray);

				Raylib.EndDrawing();
			}

			//Raylib.UnloadFont(font);
			Raylib.CloseWindow();
		}
	}
}
