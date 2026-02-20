using ErinWave.Frame.Raylibs.Entities;
using ErinWave.Frame.Raylibs.Physics;

using Raylib_cs;

namespace ErinWave.RayLibTest.Entities
{
	public class Bullet : CircleEntity
	{
		public Bullet()
		{
			Color = Color.DarkGray;
			Radius = 4f;
			Velocity = new(0, 250f);

			Collider = new CircleCollider(this);
		}
	}
}
