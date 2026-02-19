using Raylib_cs;

using System.Numerics;

namespace ErinWave.Frame.Raylibs.Entities
{
	public class RectangleEntity : EntityBase
	{
		public Vector2 Size;
		public Color Color;

		public Rectangle GetBounds()
		{
			return new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
		}

		public override void Render()
		{
			Raylib.DrawRectangleV(Position, Size, Color);
		}
	}
}
