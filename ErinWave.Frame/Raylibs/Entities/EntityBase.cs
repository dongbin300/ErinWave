using ErinWave.Frame.Raylibs.Physics;

using System.Numerics;

namespace ErinWave.Frame.Raylibs.Entities
{
	public abstract class EntityBase
	{
		public Vector2 Position;
		public Vector2 Velocity;
		public bool IsActive = true;

		public ICollider? Collider;

		public virtual void Update(float delta)
		{
			Position += Velocity * delta;
		}

		public abstract void Render();
	}
}
