using Raylib_cs;

using System.Numerics;

namespace ErinWave.Frame.Raylibs.Effects
{
	public class CameraShake
	{
		private float _duration;
		private float _power;
		private float _timer;

		public Vector2 Offset { get; private set; }

		public void Start(float duration, float power)
		{
			_duration = duration;
			_power = power;
			_timer = duration;
		}

		public void Update(float dt)
		{
			if (_timer <= 0)
			{
				Offset = Vector2.Zero;
				return;
			}

			_timer -= dt;

			float intensity = _power * (_timer / _duration);

			Offset = new Vector2(
				Raylib.GetRandomValue(-(int)intensity, (int)intensity),
				Raylib.GetRandomValue(-(int)intensity, (int)intensity)
			);
		}
	}
}
