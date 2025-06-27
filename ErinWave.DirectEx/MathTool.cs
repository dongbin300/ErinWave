﻿namespace ErinWave.DirectEx
{
	public class MathTool
    {
		private static Random _random = new Random();

		public static double NormalDistributionRandom(double mean, double deviation)
        {
			double u1 = 1.0 - _random.NextDouble();
			double u2 = 1.0 - _random.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); 

			return mean + deviation * randStdNormal;
		}
    }
}
