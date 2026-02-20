using ErinWave.Frame.Raylibs;
using ErinWave.Frame.Raylibs.Scenes;
using ErinWave.Pihagi.Core;
using ErinWave.Pihagi.Scenes;

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

			var sceneManager = new SceneManager();
			var context = new GameContext();

			sceneManager.ChangeScene(new GameplayScene(sceneManager, context));

			while (!Raylib.WindowShouldClose())
			{
				float dt = Raylib.GetFrameTime();

				sceneManager.Update(dt);

				// --- Texture Rendering ---
				Raylib.BeginTextureMode(sceneTexture);
				Raylib.ClearBackground(Color.Blank);

				sceneManager.Render();

				Raylib.EndTextureMode();
				// --- End of Texture Rendering ---

				// --- Final Screen Rendering ---
				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.Blank);

				// Global Transparency
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
