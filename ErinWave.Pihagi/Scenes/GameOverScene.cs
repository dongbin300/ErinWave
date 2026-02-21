using ErinWave.Frame.Raylibs;
using ErinWave.Frame.Raylibs.Enums;
using ErinWave.Frame.Raylibs.Scenes;
using ErinWave.Pihagi.Core;

using Raylib_cs;

namespace ErinWave.Pihagi.Scenes
{
	public class GameOverScene(SceneManager manager, GameContext context) : SceneBase
	{
		protected override void Initialize()
		{
		}

		protected override void OnEnter()
		{
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float dt)
		{
			if (Raylib.IsKeyPressed(KeyboardKey.Enter))
			{
				manager.ChangeScene(new GameplayScene(manager, context));
			}
		}

		protected override void OnRender()
		{
			int w = Raylib.GetScreenWidth();
			int h = Raylib.GetScreenHeight();

			RaylibHelper.DrawTextAnchored("GAME OVER", w / 2, h / 2, 40, Color.Red, Anchor.Center);
			RaylibHelper.DrawTextAnchored($"Score: {context.Score}", w / 2, h / 2 + 40, 20, Color.White, Anchor.Center);
		}
	}
}
