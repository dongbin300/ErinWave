//-----------------------------------------------------------------------
// 
// MIT License
// 
// Copyright (c) 2025 Erin Wave
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//-----------------------------------------------------------------------
using System.Security.Cryptography;

namespace ErinWave.Maths
{
	public class EWRandom
	{
		public long Seed { get; set; }

		/// <summary>
		/// deprecated
		/// </summary>
		/// <returns></returns>
		private long GetCurrentMicroseconds() => DateTime.Now.Ticks / 10L;

		private int _GetRandomInt() => RandomNumberGenerator.GetInt32(int.MaxValue);

		public EWRandom()
		{
			Seed = _GetRandomInt() % 1337;
		}

		public EWRandom(long seed)
		{
			Seed = seed;
		}

		/// <summary>
		/// Range: 0 ~ 2,147,483,647(int.MaxValue)
		/// </summary>
		/// <returns></returns>
		public int Next()
		{
			Seed = (Seed + _GetRandomInt()) % long.MaxValue;
			return (int)((_GetRandomInt() * Seed % int.MaxValue + int.MaxValue) % int.MaxValue);
		}


		/// <summary>
		/// Range: min ~ max-1
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public int Next(int min, int max)
		{
			return Next() % (max - min) + min;
		}

		/// <summary>
		/// Range: 0 ~ max-1
		/// </summary>
		/// <param name="max"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Range: 0.0 ~ 1.0
		/// </summary>
		/// <returns></returns>
		public double NextDouble()
		{
			return (double)Next() / int.MaxValue;
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
		/// Range: 0.0 ~ 1.0
		/// </summary>
		/// <returns></returns>
		public decimal NextDecimal()
		{
			return (decimal)Next() / int.MaxValue;
		}

		public decimal NextDecimal(decimal min, decimal max)
		{
			return NextDecimal() % (max - min) + min;
		}

		public decimal NextDecimal(decimal max)
		{
			return NextDecimal() % max;
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
	}
}
