using ErinWave.Frame.Raylibs.Entities;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.Frame.Raylibs.Systems
{
	public class MovementSystem
	{
		public void Update(EntityBase entity, float delta)
		{
			Vector2 velocity = Vector2.Zero;

			if (Raylib.IsKeyDown(KeyboardKey.Left))
				velocity.X -= entity.Velocity.X;

			if (Raylib.IsKeyDown(KeyboardKey.Right))
				velocity.X += entity.Velocity.X;

			entity.Position += velocity * delta;
		}
	}
}
