using ErinWave.Frame.Raylibs.Systems;
using ErinWave.Pihagi.Entities;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.Pihagi.Core
{
	public class Game
	{
		public Player player = new();
		public List<Bullet> bullets = [];

		public MovementSystem movementSystem = new();
		public CollisionSystem collisionSystem = new();
		public SpawnSystem<Bullet> spawnSystem;

		private int score = 0;

		public Game()
		{
			spawnSystem = CreateSpawnSystem();
		}

		public void Initialize()
		{
			Reset();
		}

		public void Update()
		{
			float delta = Raylib.GetFrameTime();

			// Movement
			movementSystem.Update(player, delta);

			// Collision
			collisionSystem.ClampToScreen(player);
			foreach (var bullet in bullets)
			{
				if (collisionSystem.CheckCollision(player, bullet))
				{
					GameOver();
					return;
				}
			}

			// Spawn
			var newBullet = spawnSystem.Update(delta);
			if (newBullet != null)
				bullets.Add(newBullet);

			// Bullet Update
			foreach (var bullet in bullets)
			{
				if (!bullet.IsActive) continue;

				bullet.Update(delta);

				if (bullet.Position.Y > Raylib.GetScreenHeight() + 10)
				{
					bullet.IsActive = false;
					score += 10;
				}
			}
			bullets.RemoveAll(b => !b.IsActive);
		}

		public void Render()
		{
			player.Render();

			foreach (var bullet in bullets)
				bullet.Render();

			Raylib.DrawText($"{score}", 10, 10, 20, Color.LightGray);
		}

		private void GameOver()
		{
			Reset();
		}

		private void Reset()
		{
			bullets.Clear();
			player = new Player();
			score = 0;
			spawnSystem = CreateSpawnSystem();
		}

		private SpawnSystem<Bullet> CreateSpawnSystem()
		{
			return new SpawnSystem<Bullet>(
				time => MathF.Max(0.05f, 0.2f - time * 0.01f), // 시간에 따라 감소
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
