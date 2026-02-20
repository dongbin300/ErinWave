namespace ErinWave.Frame.Raylibs.Stats
{
	public class Stat(int max)
	{
		public int Max { get; private set; } = max;
		public int Current { get; private set; } = max;

		public bool IsEmpty => Current <= 0;
		public float Ratio => Max == 0 ? 0f : (float)Current / Max;

		public void Decrease(int amount)
		{
			Current -= amount;
			if (Current < 0)
				Current = 0;
		}

		public void Increase(int amount)
		{
			Current += amount;
			if (Current > Max)
				Current = Max;
		}

		public void Reset()
		{
			Current = Max;
		}

		public void SetMax(int newMax)
		{
			Max = newMax;
			if (Current > Max)
				Current = Max;
		}
	}
}
