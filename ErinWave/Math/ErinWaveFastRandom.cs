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
	/// <summary>
	/// A high-performance pseudo-random number generator based on the xoshiro128** algorithm.
	/// 
	/// This implementation offers significantly better statistical quality and uniformity than
	/// System.Random or classic Xorshift generators, while maintaining extremely fast throughput.
	/// 
	/// The generator uses a 128-bit internal state and produces 32-bit outputs with excellent 
	/// equidistribution properties. It is suitable for simulations, games, procedural content 
	/// generation, Monte-Carlo methods, and other non-cryptographic workloads requiring both 
	/// speed and high-quality randomness.
	/// 
	/// Initialization uses a SplitMix32-based mixer to ensure strong seed diffusion and avoid 
	/// weak initial states. All public methods (Next, NextUInt, NextDouble, NextBytes, NextBool) 
	/// follow a familiar API similar to System.Random, making it a drop-in replacement for most 
	/// use cases.
	/// 
	/// NOTE: This generator is NOT cryptographically secure. Do not use it for security-critical 
	/// purposes such as token generation, password creation, or key material.
	/// </summary>
	public class ErinWaveFastRandom
	{
		private uint s0, s1, s2, s3;

		public ErinWaveFastRandom() : this(Environment.TickCount)
		{
		}

		public ErinWaveFastRandom(int seed)
		{
			Initialize(seed);
		}

		public void Initialize(int seed)
		{
			// SplitMix32로 seed 확산 (xoshiro 계열에 권장)
			uint x = (uint)seed;
			s0 = SplitMix32(ref x);
			s1 = SplitMix32(ref x);
			s2 = SplitMix32(ref x);
			s3 = SplitMix32(ref x);
		}

		private static uint SplitMix32(ref uint x)
		{
			x += 0x9E3779B9;
			uint z = x;
			z = (z ^ (z >> 16)) * 0x85EBCA6B;
			z = (z ^ (z >> 13)) * 0xC2B2AE35;
			return z ^ (z >> 16);
		}

		// xoshiro128** core
		private uint NextUIntCore()
		{
			uint result = Rotl(s1 * 5, 7) * 9;

			uint t = s1 << 9;

			s2 ^= s0;
			s3 ^= s1;
			s1 ^= s2;
			s0 ^= s3;

			s2 ^= t;

			s3 = Rotl(s3, 11);

			return result;
		}

		private static uint Rotl(uint x, int k) => (x << k) | (x >> (32 - k));

		public uint NextUInt() => NextUIntCore();

		public int Next() => (int)(NextUInt() & 0x7FFFFFFF);

		public int Next(int upperBound) => upperBound > 0 ? (int)(NextDouble() * upperBound) : throw new ArgumentOutOfRangeException(nameof(upperBound));

		public int Next(int lowerBound, int upperBound) => lowerBound <= upperBound ? lowerBound + Next(upperBound - lowerBound) : throw new ArgumentOutOfRangeException();

		public double NextDouble()
		{
			// 53-bit precision double generation
			ulong a = NextUInt();
			ulong b = NextUInt();
			return ((a >> 5) * 67108864.0 + (b >> 6)) / (1UL << 53);
		}

		public void NextBytes(byte[] buffer)
		{
			int i = 0;
			while (i + 4 <= buffer.Length)
			{
				uint r = NextUIntCore();
				buffer[i++] = (byte)r;
				buffer[i++] = (byte)(r >> 8);
				buffer[i++] = (byte)(r >> 16);
				buffer[i++] = (byte)(r >> 24);
			}

			if (i < buffer.Length)
			{
				uint r = NextUIntCore();
				for (int j = 0; i < buffer.Length; j++, i++)
					buffer[i] = (byte)(r >> (8 * j));
			}
		}

		public bool NextBool() => (NextUIntCore() & 1) == 1;
	}
}
