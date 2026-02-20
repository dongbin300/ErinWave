using ErinWave.Frame.Raylibs.Enums;

using Raylib_cs;

using System.Numerics;

namespace ErinWave.Frame.Raylibs.Overlays
{
	public class FloatingTextOverlay(string text, Vector2 position, float lifetime = 1f)
	{
		public Vector2 Position = position;
		public string Text = text;

		public float Lifetime { get; } = lifetime;
		public float Elapsed { get; private set; }

		public float RiseSpeed { get; set; } = 30f;
		public Anchor Anchor { get; set; } = Anchor.Center;

		public Color BaseColor { get; set; } = Color.White;

		public bool IsAlive => Elapsed < Lifetime;

		public void Update(float dt)
		{
			Elapsed += dt;
			Position.Y -= RiseSpeed * dt;
		}

		public void Render(int fontSize = 18)
		{
			float t = Elapsed / Lifetime;
			float alpha = 1f - t;

			var color = new Color(BaseColor.R, BaseColor.G, BaseColor.B, (byte)(BaseColor.A * alpha));

			RaylibHelper.DrawTextAligned(Text, Position.X, Position.Y, fontSize, color, Anchor);
		}
	}
}
