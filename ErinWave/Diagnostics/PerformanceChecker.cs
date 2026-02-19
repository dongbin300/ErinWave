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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErinWave.Dianostics
{
	public class PerformanceChecker
	{
		/// <summary>
		/// Set of methods to be performed
		/// Methods should have no input and no output
		/// </summary>
		private readonly List<Action> actions;

		/// <summary>
		/// Number of times to perform each method
		/// </summary>
		public int CountOfPerform { get; set; }

		/// <summary>
		/// Just constructor
		/// </summary>
		public PerformanceChecker()
		{
			actions = new List<Action>();
			CountOfPerform = 1;
		}

		/// <summary>
		/// Compare method a and method b
		/// </summary>
		/// <param name="a">Method A</param>
		/// <param name="b">Method B</param>
		/// <param name="countOfPerform">Number of times to perform each method, default value is 1</param>
		public PerformanceChecker(Action a, Action b, int countOfPerform = 1)
		{
			actions =
			[
				a,
				b
			];

			CountOfPerform = countOfPerform;
		}

		/// <summary>
		/// Add additional method
		/// </summary>
		/// <param name="action">Method</param>
		public void AddAction(Action action)
		{
			actions.Add(action);
		}

		/// <summary>
		/// Perform the method and return the time taken in seconds
		/// </summary>
		/// <returns></returns>
		public List<double> Perform()
		{
			var elapsedTimes = new List<double>();

			var stopwatch = new Stopwatch();
			foreach (Action action in actions)
			{
				stopwatch.Restart();
				for (int i = 0; i < CountOfPerform; i++)
				{
					action();
				}
				stopwatch.Stop();

				elapsedTimes.Add((double)stopwatch.ElapsedTicks / 10_000_000);
			}

			return elapsedTimes;
		}

		/// <summary>
		/// Perform the method and return result by string
		/// </summary>
		/// <returns></returns>
		public string PerformResult()
		{
			var elapsedTimes = Perform();

			var builder = new StringBuilder();
			builder.AppendLine("================================");
			for (int i = 0; i < actions.Count; i++)
			{
				builder.Append(actions[i].Method.Name);
				builder.Append(" : ");
				builder.Append(elapsedTimes[i]);
				builder.AppendLine("sec");
			}
			builder.AppendLine("================================");

			return builder.ToString();
		}
	}
}
