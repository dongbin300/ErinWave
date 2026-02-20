using ErinWave.Frame.Raylibs.Entities;
using ErinWave.Frame.Raylibs.Physics;
using ErinWave.Frame.Raylibs.Stats;

using Raylib_cs;

namespace ErinWave.Pihagi.Entities
{
	public class Player : RectangleEntity, IHasStats
	{
		public StatContainer Stats { get; } = new();

		public Player()
		{
			Color = Color.Magenta;
			Size = new(20, 20);
			Position = new(120, 580);
			Velocity = new(200, 0);

			Collider = new RectangleCollider(this);

			Stats.Add(StatType.HP, 100);
		}
	}
}
