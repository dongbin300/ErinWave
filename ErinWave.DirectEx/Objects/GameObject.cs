using ErinWave.DirectEx.Enums;

using System.Numerics;

namespace ErinWave.DirectEx.Objects
{
	public class GameObject(GameObjectType type, float x, float y, float width, float height, float speed = 1.0f)
	{
		public GameObjectType Type { get; set; } = type;
		public float Width { get; set; } = width;
		public float Height { get; set; } = height;
		public float Speed { get; set; } = speed;
		public Vector2 Position { get; set; } = new Vector2(x, y);
		public Vector2 Velocity { get; set; } = new Vector2(speed, speed);
		public float Top => Position.Y;
		public float Bottom => Position.Y + Height;
		public float Left => Position.X;
		public float Right => Position.X + Width;
		int _jumpCool = 0;

		public RectangleF GetBoundingBox()
		{
			return new RectangleF(Position.X, Position.Y, Width, Height);
		}

		public bool IsColliding(GameObject other)
		{
			return GetBoundingBox().IntersectsWith(other.GetBoundingBox());
		}

		public void ProcessCollision(GameObject other)
		{
			if (!IsColliding(other))
			{
				return;
			}

			if (Bottom > other.Top && Top < other.Top) // 바닥
			{
				Position = new Vector2(Position.X, other.Position.Y - Height);
				Velocity = new Vector2(Velocity.X, 0);
			}
			else if (Top < other.Bottom && Bottom > other.Bottom) // 천장
			{
				Position = new Vector2(Position.X, other.Position.Y + Height);
				Velocity = new Vector2(Velocity.X, 0);
			}
			else if (Left < other.Right && Right > other.Right) // 왼쪽 벽
			{
				Position = new Vector2(other.Position.X + Width, Position.Y);
				Velocity = new Vector2(0, Velocity.Y);
			}
			else if (Right > other.Left && Left < other.Left) // 오른쪽 벽
			{
				Position = new Vector2(other.Position.X - Width, Position.Y);
				Velocity = new Vector2(0, Velocity.Y);
			}
		}

		public void MoveUp()
		{
			Velocity = new Vector2(Velocity.X, -Speed);
		}

		public void MoveDown()
		{
			Velocity = new Vector2(Velocity.X, +Speed);
		}

		public void MoveLeft()
		{
			Velocity = new Vector2(-Speed, Velocity.Y);
		}

		public void MoveRight()
		{
			Velocity = new Vector2(+Speed, Velocity.Y);
		}

		public void Jump()
		{
			if (_jumpCool > 0)
			{
				return;
			}

			_jumpCool = 42;
			Velocity = new Vector2(Velocity.X, -7);
		}

		public void NextFrame()
		{
			_jumpCool--;
			var afterX =
				Math.Abs(Velocity.X) < 0.01f ? 0 :
				//Velocity.Y < 0f ? Velocity.X * 0.8f :
				Velocity.X * 0.2f;
			var afterY = Velocity.Y + 0.49f;
			Velocity = new Vector2(afterX, afterY);
			Position = Vector2.Add(Position, Velocity);
		}
	}
}
