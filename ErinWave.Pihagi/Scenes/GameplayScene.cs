using ErinWave.Frame.Raylibs.Effects;
using ErinWave.Frame.Raylibs.Enums;
using ErinWave.Frame.Raylibs.Overlays;
using ErinWave.Frame.Raylibs.Scenes;
using ErinWave.Frame.Raylibs.Stats;
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
		private Player _player = new();
		private List<Bullet> _bullets = [];
		private List<MedkitItem> _medkits = [];

		// Systems
		private MovementSystem _movementSystem = new();
		private CollisionSystem _collisionSystem = new();
		private SpawnSystem<Bullet> _spawnSystem;
		private SpawnSystem<MedkitItem> _medkitSpawnSystem;

		// Effects
		private CameraShake _shake = new();

		// Overlays
		private List<FloatingTextOverlay> _floatingTexts = [];

		public GameplayScene(SceneManager manager, GameContext context)
		{
			_sceneManager = manager;
			_context = context;

			_spawnSystem = CreateSpawnSystem();
			_medkitSpawnSystem = CreateMedkitSpawnSystem();
		}

		public void Enter()
		{
			Reset();
		}

		public void Exit() { }

		public void Update(float dt)
		{
			// Movement
			_movementSystem.Update(_player, dt);
			_collisionSystem.ClampToScreen(_player);

			// Collision
			foreach (var bullet in _bullets)
			{
				if (_collisionSystem.CheckCollision(_player, bullet)) // Player hit
				{
					_shake.Start(0.3f, 8f);

					var playerHp = _player.Stats.Get(StatType.HP);
					playerHp.Decrease(12);
					bullet.IsActive = false;

					if (playerHp.IsEmpty)
					{
						_sceneManager.ChangeScene(new GameOverScene(_sceneManager, _context));
						return;
					}
				}
			}
			foreach (var medkit in _medkits)
			{
				if (!medkit.IsActive) continue;

				if (_collisionSystem.CheckCollision(_player, medkit)) // Get Medkit
				{
					var playerHp = _player.Stats.Get(StatType.HP);
					playerHp.Increase(50);
					medkit.IsActive = false;
				}
			}

			// Spawn
			var newBullet = _spawnSystem.Update(dt);
			if (newBullet != null)
				_bullets.Add(newBullet);

			var newMedkit = _medkitSpawnSystem.Update(dt);
			if (newMedkit != null)
				_medkits.Add(newMedkit);

			// Entity
			foreach (var bullet in _bullets)
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
			_bullets.RemoveAll(b => !b.IsActive);

			foreach (var medkit in _medkits)
				medkit.Update(dt);
			_medkits.RemoveAll(m => !m.IsActive);

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

			// Entity
			foreach (var bullet in _bullets)
				bullet.Render();
			foreach (var medkit in _medkits)
				medkit.Render();

			// UI
			Raylib.DrawText($"{_context.Score}", 10, 10, 20, Color.White);

			// Player
			_player.Render();
			int width = (int)_player.Size.X * 2;
			int height = 6;
			int x = (int)(_player.Position.X - _player.Size.X / 2);
			int y = (int)_player.Position.Y - 10;
			var playerHp = _player.Stats.Get(StatType.HP);
			float ratio = playerHp.Ratio;
			Raylib.DrawRectangle(x, y, width, height, Color.DarkGray);
			Raylib.DrawRectangle(x, y, (int)(width * ratio), height, Color.Red);

			// Overlays
			foreach (var ft in _floatingTexts)
				ft.Render(18);

			Raylib.EndMode2D();
		}

		private void Reset()
		{
			_bullets.Clear();
			_player = new Player();
			_context.Score = 0;
			_spawnSystem = CreateSpawnSystem();
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

		private SpawnSystem<MedkitItem> CreateMedkitSpawnSystem()
		{
			return new SpawnSystem<MedkitItem>(
				time => 1.5f, // n초마다 체크
				() =>
				{
					if (Raylib.GetRandomValue(0, 100) >= 36) // n%확률
						return default!;

					float x = Raylib.GetRandomValue(10, Raylib.GetScreenWidth() - 10);

					return new MedkitItem(new Vector2(x, -20));
				});
		}
	}
}
