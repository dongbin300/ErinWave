using ErinWave.Frame.Raylibs.Entities;
using ErinWave.Frame.Raylibs.Physics;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.Pihagi.Entities
{
	public class MedkitItem : RectangleEntity
	{
		public MedkitItem(Vector2 position)
		{
			Position = position;
			Size = new(16, 16);
			Color = Color.Green;
			Velocity = new(0, 250f);
			Collider = new RectangleCollider(this);
		}
	}
}
