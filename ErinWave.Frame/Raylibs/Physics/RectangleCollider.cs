using ErinWave.Frame.Raylibs.Entities;

using Raylib_cs;

namespace ErinWave.Frame.Raylibs.Physics
{
	public class RectangleCollider(RectangleEntity owner) : ICollider
	{
		public RectangleEntity Owner = owner;

		public bool CheckCollision(ICollider other)
		{
			if (other is RectangleCollider rect)
			{
				return Raylib.CheckCollisionRecs(
					Owner.GetBounds(),
					rect.Owner.GetBounds()
				);
			}

			if (other is CircleCollider circle)
			{
				return Raylib.CheckCollisionCircleRec(
					circle.Owner.Position,
					circle.Owner.Radius,
					Owner.GetBounds()
				);
			}

			return false;
		}
	}
}
