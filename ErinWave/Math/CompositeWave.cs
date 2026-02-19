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

using System.Numerics;

namespace ErinWave.Math
{
    public class CompositeWave : IWave
    {
        public List<IWave> Waves { get; set; }
        public List<double> Magnitudes => CalculateMagnitudes();
        public Complex[] Complices => ToComplex();

        public CompositeWave()
        {
			Waves = [];
        }

        public void AddWave(IWave wave)
        {
            Waves.Add(wave);
        }

        public void RemoveWave(IWave wave)
        {
            Waves.Remove(wave);
        }

        public Complex[] ToComplex()
        {
            Complex[] result = new Complex[Magnitudes.Count];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Complex(Magnitudes[i], 0);
            }

            return result;
        }

        private List<double> CalculateMagnitudes()
        {
            var magnitudes = new List<double>();
            int maxCount = Waves.Max(x => x.Magnitudes.Count);
            for (int i = 0; i < maxCount; i++)
            {
                magnitudes.Add(Waves.Sum(x => x.Magnitudes.ElementAt(i)));
            }
            return magnitudes;
        }
    }
}
