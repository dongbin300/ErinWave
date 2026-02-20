using ErinWave.Frame.Raylibs.Systems;
using ErinWave.RayLibTest.Entities;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.RayLibTest.Core
{
	public class Game
	{
		public Player player = new();
		public List<Bullet> bullets = [];

		public MovementSystem movementSystem = new();
		public CollisionSystem collisionSystem = new();
		public SpawnSystem<Bullet> spawnSystem;

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
					bullet.IsActive = false;
			}
			bullets.RemoveAll(b => !b.IsActive);
		}

		public void Render()
		{
			player.Render();

			foreach (var bullet in bullets)
				bullet.Render();
		}

		private void GameOver()
		{
			Reset();
		}

		private void Reset()
		{
			bullets.Clear();
			player = new Player();
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
