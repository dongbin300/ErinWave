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

namespace ErinWave.Extensions
{
    public static class LinqExtension
    {
        public static IEnumerable<T> Substring<T>(this IEnumerable<T> source, int startIndex)
        {
            return source.Skip(startIndex);
        }

        public static IEnumerable<T> Substring<T>(this IEnumerable<T> source, int startIndex, int length)
        {
            return source.Skip(startIndex).Take(length);
        }

        public static T Median<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => x).ToArray()[source.Count() / 2];
        }

        public static IEnumerable<string> Remove(this IEnumerable<string> source, string value)
        {
            return source.Select(x => x.Replace(value, string.Empty));
        }

        public static IEnumerable<int> Rank<T>(this IEnumerable<T> source)
        {
            return source
                .GroupBy(x => x)
                .OrderByDescending(x => x.Key)
                .Select((x, index) => (num: x.Key, rank: index + 1))
                .Select(x => x.rank)
                .ToList();
        }
    }
}
