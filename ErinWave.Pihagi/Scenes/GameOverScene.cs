using ErinWave.Frame.Raylibs;
using ErinWave.Frame.Raylibs.Enums;
using ErinWave.Frame.Raylibs.Scenes;
using ErinWave.Pihagi.Core;

using Raylib_cs;

namespace ErinWave.Pihagi.Scenes
{
	public class GameOverScene(SceneManager manager, GameContext context) : IScene
	{
		private float _fade = 0f;

		public void Enter()
		{
			_fade = 0f;
		}

		public void Exit() { }

		public void Update(float dt)
		{
			_fade += dt * 2f;
			if (_fade > 1f) _fade = 1f;

			if (_fade >= 1f && Raylib.IsKeyPressed(KeyboardKey.Enter))
			{
				manager.ChangeScene(new GameplayScene(manager, context));
			}
		}

		public void Render()
		{
			int w = Raylib.GetScreenWidth();
			int h = Raylib.GetScreenHeight();

			var overlay = new Color(0, 0, 0, 200 * _fade);
			Raylib.DrawRectangle(0, 0, w, h, overlay);

			RaylibHelper.DrawTextAligned("GAME OVER", w / 2, h / 2, 40, new Color(255, 0, 0, 255 * _fade), Anchor.Center);
			RaylibHelper.DrawTextAligned($"Score: {context.Score}", w / 2, h / 2 + 40, 20, new Color(255, 255, 255, 255 * _fade), Anchor.Center);
		}
	}
}
