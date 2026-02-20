using ErinWave.Frame.Raylibs.Entities;
using ErinWave.Frame.Raylibs.Physics;

using Raylib_cs;

namespace ErinWave.Pihagi.Entities
{
	public class Player : RectangleEntity
	{
		public Player()
		{
			Color = Color.Magenta;
			Size = new(20, 20);
			Position = new(120, 580);
			Velocity = new(200, 0);

			Collider = new RectangleCollider(this);
		}
	}
}
