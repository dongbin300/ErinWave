using ErinWave.Frame.Raylibs.Systems;
using ErinWave.RayLibTest.Entities;

using Raylib_cs;

using System.Dynamic;
using System.Numerics;

namespace ErinWave.RayLibTest.Core
{
	public class Game
	{
		public Player player = new();
		public List<Bullet> bullets = [];

		public MovementSystem movementSystem = new();
		public CollisionSystem collisionSystem = new();

		public float spawnTimer;
		public float spawnInterval;
		public int spawnPadding = 5;

		public void Initialize()
		{
			Reset();
		}

		public void Update()
		{
			float delta = Raylib.GetFrameTime();

			movementSystem.Update(player, delta);
			collisionSystem.ClampToScreen(player);

			foreach (var bullet in bullets)
			{
				if (collisionSystem.CheckCollision(player, bullet))
				{
					GameOver();
					break;
				}
			}

			spawnInterval = MathF.Max(0.05f, spawnInterval - delta * 0.01f);
			spawnTimer += delta;

			if (spawnTimer >= spawnInterval)
			{
				spawnTimer = 0f;

				bullets.Add(new Bullet
				{
					Position = new Vector2(Raylib.GetRandomValue(spawnPadding, Raylib.GetScreenWidth() - spawnPadding), -10)
				});
			}

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
			{
				bullet.Render();
			}
		}

		private void GameOver()
		{
			Reset();
		}

		private void Reset()
		{
			bullets.Clear();
			player = new Player();
			spawnTimer = 0f;
			spawnInterval = 0.2f;
		}
	}
}
