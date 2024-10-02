namespace ErinWave.Richer.Maths
{
	public class SmartRandom
	{
		long seed;

		public SmartRandom()
		{
			seed = GetCurrentMicroseconds() % 1337;
		}

		public SmartRandom(long seed)
		{
			this.seed = seed;
		}

		public int Next()
		{
			long abc = Math.Abs(GetCurrentMicroseconds() * seed % 1_000_000_000L / 10);
			seed += 13;

			return Convert.ToInt32(abc);
		}

		public int Next(int min, int max)
		{
			return Next() % (max - min) + min;
		}

		public int Next(int max)
		{
			return Next() % max;
		}

		public string Next(string str)
		{
			return str[Next(str.Length)].ToString();
		}

		public string Next(string str, int count)
		{
			string result = string.Empty;
			for (int i = 0; i < count; i++)
			{
				result += Next(str);
			}
			return result;
		}

		public T Next<T>(IEnumerable<T> values)
		{
			return values.ElementAt(Next(values.Count()));
		}

		public double NextDouble()
		{
			return (double)Next() / 1_000_000_000L;
		}

		public double NextDouble(double min, double max)
		{
			return NextDouble() % (max - min) + min;
		}

		public double NextDouble(double max)
		{
			return NextDouble() % max;
		}

		/// <summary>
		/// Normal Distribution Random
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public int NextNd(int min, int max)
		{
			double u1 = 1.0 - NextDouble();
			double u2 = 1.0 - NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

			double mean = (min + max) / 2.0;
			double stddev = (max - min) / 6.0;

			double randNormal = mean + stddev * randStdNormal;

			return (int)Math.Round(Math.Clamp(randNormal, min, max));
		}

		/// <summary>
		/// Normal Distribution Random Double
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public double NextNd(double min, double max)
		{
			double u1 = 1.0 - NextDouble();
			double u2 = 1.0 - NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

			double mean = (min + max) / 2.0;
			double stddev = (max - min) / 6.0;

			double randNormal = mean + stddev * randStdNormal;

			return Math.Clamp(randNormal, min, max);
		}

		/// <summary>
		/// Normal Distribution Random Decimal (Slower twice than Double)
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public decimal NextNd(decimal min, decimal max)
		{
			double u1 = 1.0 - NextDouble();
			double u2 = 1.0 - NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

			decimal mean = (min + max) / 2;
			decimal stddev = (max - min) / 6;

			decimal randNormal = mean + stddev * (decimal)randStdNormal;

			return Math.Clamp(randNormal, min, max);
		}

		long GetCurrentMicroseconds()
		{
			return DateTime.Now.Ticks / 10L;
		}
	}
}
