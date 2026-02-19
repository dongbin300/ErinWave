using ErinWave.Frame.Raylibs.Entities;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.Frame.Raylibs.Physics
{
	public class CircleCollider(CircleEntity owner) : ICollider
	{
		public CircleEntity Owner = owner;

		public bool CheckCollision(ICollider other)
		{
			if (other is CircleCollider circle)
			{
				return Vector2.Distance(
					Owner.Position,
					circle.Owner.Position
				) <= Owner.Radius + circle.Owner.Radius;
			}

			if (other is RectangleCollider rect)
			{
				return Raylib.CheckCollisionCircleRec(
					Owner.Position,
					Owner.Radius,
					rect.Owner.GetBounds()
				);
			}

			return false;
		}
	}
}
