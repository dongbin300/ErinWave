using Raylib_cs;

namespace ErinWave.Frame.Raylibs.Entities
{
	public class CircleEntity : EntityBase
	{
		public float Radius;
		public Color Color;

		public Rectangle GetBounds()
		{
			return new Rectangle(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
		}

		public override void Render()
		{
			Raylib.DrawCircleV(Position, Radius, Color);
		}
	}
}
