using ErinWave.Frame.Raylibs;
using ErinWave.Pihagi.Core;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.Pihagi
{
	internal class Program
	{
		private static RenderTexture2D sceneTexture;

		static void Main(string[] args)
		{
			RaylibHelper.Init(250, 600, 240, "Pihagi", ConfigFlags.TransparentWindow | ConfigFlags.UndecoratedWindow);
			sceneTexture = Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

			//string fontPath = @"C:\Users\Gaten\AppData\Local\Microsoft\Windows\Fonts\NanumBarunGothic.ttf";
			//var font = KoreanFontHelper.LoadFont(fontPath, 16);

			var game = new Game();
			game.Initialize();

			while (!Raylib.WindowShouldClose())
			{
				game.Update();

				// --- Texture Rendering ---
				Raylib.BeginTextureMode(sceneTexture);
				Raylib.ClearBackground(Color.Blank);

				game.Render();

				Raylib.EndTextureMode();
				// --- End of Texture Rendering ---

				// --- Final Screen Rendering ---
				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.Blank);

				var tint = new Color(255, 255, 255, 70);

				Raylib.DrawTexturePro(
					sceneTexture.Texture,
					new Rectangle(0, 0, sceneTexture.Texture.Width, -sceneTexture.Texture.Height),
					new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
					Vector2.Zero,
					0f,
					tint
				);

				Raylib.EndDrawing();
				// --- End of Frame ---
			}

			//Raylib.UnloadFont(font);
			Raylib.CloseWindow();
		}
	}
}
