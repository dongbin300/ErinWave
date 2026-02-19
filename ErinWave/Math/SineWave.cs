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

namespace ErinWave.Math
{
    public class SineWave : IWave
    {
        public List<double> Magnitudes { get; }
        public int SampleRate { get; set; }
        public double Frequency { get; set; }
        public double Amplitude { get; set; }
        public double Duration { get; set; }
        public double Phase { get; set; }
        public double Crest { get; set; }
        public double Trough { get; set; }

        /// <summary>
        /// 사인파를 만듭니다.
        /// </summary>
        /// <param name="sampleRate">(샘플레이트)1초에 포함되는 magnitude 개수</param>
        /// <param name="frequency">(주파수)1초에 진동하는 회수</param>
        /// <param name="amplitude">(진폭)0과의 거리의 최대값</param>
        /// <param name="duration">(길이)파동의 길이(초 단위)</param>
        public SineWave(int sampleRate, double frequency, double amplitude, double duration, double phase = 0)
        {
            SampleRate = sampleRate;
            Frequency = frequency;
            Amplitude = amplitude;
            Duration = duration;
            Phase = phase;

            Crest = amplitude;
            Trough = -amplitude;

            Magnitudes = [];
            for (int i = 0; i < sampleRate * duration; i++)
            {
                Magnitudes.Add(amplitude * System.Math.Sin(2 * System.Math.PI * (i - phase) * frequency / sampleRate));
            }
        }
    }
}
