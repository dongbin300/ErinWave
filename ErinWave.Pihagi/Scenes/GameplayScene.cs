using ErinWave.Frame.Raylibs.Effects;
using ErinWave.Frame.Raylibs.Enums;
using ErinWave.Frame.Raylibs.Overlays;
using ErinWave.Frame.Raylibs.Scenes;
using ErinWave.Frame.Raylibs.Systems;
using ErinWave.Pihagi.Core;
using ErinWave.Pihagi.Entities;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.Pihagi.Scenes
{
	public class GameplayScene : IScene
	{
		private readonly SceneManager _sceneManager;
		private readonly GameContext _context;

		// Entities
		private Player player = new();
		private List<Bullet> bullets = [];

		// Systems
		private MovementSystem movementSystem = new();
		private CollisionSystem collisionSystem = new();
		private SpawnSystem<Bullet> spawnSystem;

		// Effects
		private CameraShake _shake = new();

		// Overlays
		private List<FloatingTextOverlay> _floatingTexts = [];

		public GameplayScene(SceneManager manager, GameContext context)
		{
			_sceneManager = manager;
			_context = context;

			spawnSystem = CreateSpawnSystem();
		}

		public void Enter()
		{
			Reset();
		}

		public void Exit() { }

		public void Update(float dt)
		{
			// Movement
			movementSystem.Update(player, dt);
			collisionSystem.ClampToScreen(player);

			// Collision
			foreach (var bullet in bullets)
			{
				if (collisionSystem.CheckCollision(player, bullet))
				{
					_shake.Start(0.3f, 8f);
					_sceneManager.ChangeScene(new GameOverScene(_sceneManager, _context));
					return;
				}
			}

			// Spawn
			var newBullet = spawnSystem.Update(dt);
			if (newBullet != null)
				bullets.Add(newBullet);

			// Bullet Update
			foreach (var bullet in bullets)
			{
				if (!bullet.IsActive) continue;

				bullet.Update(dt);

				if (bullet.Position.Y > Raylib.GetScreenHeight() + 10)
				{
					bullet.IsActive = false;
					_context.Score += 10;

					_floatingTexts.Add(new FloatingTextOverlay("+10", bullet.Position, 1f)
					{
						RiseSpeed = 40f,
						BaseColor = Color.Yellow,
						Anchor = Anchor.Center
					});
				}
			}

			bullets.RemoveAll(b => !b.IsActive);

			// Effects
			_shake.Update(dt);

			// Overlays
			foreach (var ft in _floatingTexts)
				ft.Update(dt);

			_floatingTexts.RemoveAll(f => !f.IsAlive);
		}

		public void Render()
		{
			Raylib.BeginMode2D(new Camera2D(_shake.Offset, Vector2.Zero, 0, 1));

			player.Render();
			foreach (var bullet in bullets)
				bullet.Render();

			Raylib.DrawText($"{_context.Score}", 10, 10, 20, Color.White);

			foreach (var ft in _floatingTexts)
				ft.Render(18);

			Raylib.EndMode2D();
		}

		private void Reset()
		{
			bullets.Clear();
			player = new Player();
			_context.Score = 0;
			spawnSystem = CreateSpawnSystem();
		}

		private SpawnSystem<Bullet> CreateSpawnSystem()
		{
			return new SpawnSystem<Bullet>(
				time => MathF.Max(0.05f, 0.2f - time * 0.01f),
				() =>
				{
					return new Bullet
					{
						Position = new Vector2(
							Raylib.GetRandomValue(5, Raylib.GetScreenWidth() - 5),
							-10
						)
					};
				});
		}
	}
}
