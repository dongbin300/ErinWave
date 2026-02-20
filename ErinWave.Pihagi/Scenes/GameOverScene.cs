using ErinWave.Frame.Raylibs;
using ErinWave.Frame.Raylibs.Enums;
using ErinWave.Frame.Raylibs.Scenes;
using ErinWave.Pihagi.Core;

using Raylib_cs;

namespace ErinWave.Pihagi.Scenes
{
	public class GameOverScene(SceneManager manager, GameContext context) : IScene
	{
		public void Enter()
		{
		}

		public void Exit() { }

		public void Update(float dt)
		{
			if (Raylib.IsKeyPressed(KeyboardKey.Enter))
			{
				manager.ChangeScene(new GameplayScene(manager, context));
			}
		}

		public void Render()
		{
			int w = Raylib.GetScreenWidth();
			int h = Raylib.GetScreenHeight();

			RaylibHelper.DrawTextAnchored("GAME OVER", w / 2, h / 2, 40, Color.Red, Anchor.Center);
			RaylibHelper.DrawTextAnchored($"Score: {context.Score}", w / 2, h / 2 + 40, 20, Color.White, Anchor.Center);
		}
	}
}
