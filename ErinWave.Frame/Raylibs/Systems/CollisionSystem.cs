using ErinWave.Frame.Raylibs.Entities;

using Raylib_cs;

namespace ErinWave.Frame.Raylibs.Systems
{
	public class CollisionSystem
	{
		public void ClampToScreen(RectangleEntity entity)
		{
			float screenWidth = Raylib.GetScreenWidth();
			float screenHeight = Raylib.GetScreenHeight();

			if (entity.Position.X < 0)
				entity.Position.X = 0;

			if (entity.Position.X + entity.Size.X > screenWidth)
				entity.Position.X = screenWidth - entity.Size.X;

			if (entity.Position.Y < 0)
				entity.Position.Y = 0;

			if (entity.Position.Y + entity.Size.Y > screenHeight)
				entity.Position.Y = screenHeight - entity.Size.Y;
		}

		public bool CheckCollision(EntityBase a, EntityBase b)
		{
			if (!a.IsActive || !b.IsActive) return false;
			if (a.Collider == null || b.Collider == null) return false;

			return a.Collider.CheckCollision(b.Collider);
		}
	}
}
