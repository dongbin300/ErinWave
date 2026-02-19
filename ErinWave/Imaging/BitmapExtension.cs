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

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ErinWave.Imaging
{
    public enum MergeType
    {
        Horizontal,
        Vertical
    }

    [SupportedOSPlatform("windows")]
    public static class BitmapExtension
    {
        public static Bitmap Merge(this IList<Bitmap> bitmaps, MergeType type)
        {
            switch (type)
            {
                case MergeType.Horizontal:
                    var result = new Bitmap(bitmaps.Sum(x=>x.Width), bitmaps.Max(x=>x.Height));
                    using (Graphics g = Graphics.FromImage(result))
                    {
                        g.DrawImage(bitmaps[0], new Point(0, 0));
                        int p = bitmaps[0].Width;
                        for(int i = 1; i < bitmaps.Count; i++)
                        {
                            g.DrawImage(bitmaps[i], new Point(p, 0));
                            p += bitmaps[i].Width;
                        }
                    }
                    return result;

                case MergeType.Vertical:
                    var result2 = new Bitmap(bitmaps.Max(x => x.Width), bitmaps.Sum(x => x.Height));
                    using (Graphics g = Graphics.FromImage(result2))
                    {
                        g.DrawImage(bitmaps[0], new Point(0, 0));
                        int p = bitmaps[0].Height;
                        for (int i = 1; i < bitmaps.Count; i++)
                        {
                            g.DrawImage(bitmaps[i], new Point(0, p));
                            p += bitmaps[i].Height;
                        }
                    }
                    return result2;

                default:
                    return default!;
            }
        }

        /// <summary>
        /// 비트맵의 RGB값 얻기
        /// </summary>
        /// <param name="b">비트맵</param>
        /// <returns>RGB 값</returns>
        public static byte[] GetPixelData(this Bitmap b)
        {
            Rectangle rt = new Rectangle(0, 0, b.Width, b.Height);
            BitmapData bd = b.LockBits(rt, ImageLockMode.ReadWrite, b.PixelFormat);
            IntPtr ptr = bd.Scan0;
            int bytes = System.Math.Abs(bd.Stride) * b.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            b.UnlockBits(bd);

            return rgbValues;
        }

        public static Color[] GetPixelColor(this Bitmap b)
        {
            Color[] results = new Color[b.Width * b.Height];
            var data = GetPixelData(b);

            for (int i = 0; i < data.Length; i += 4)
            {
                results[i / 4] = Color.FromArgb(data[i + 3], data[i + 2], data[i + 1], data[i]);
            }

            return results;
        }

        public static Color[,] GetPixelColor2D(this Bitmap b)
        {
            Color[,] results = new Color[b.Width, b.Height];
            var data = GetPixelData(b);

            for (int i = 0; i < b.Height; i++)
            {
                for (int j = 0; j < b.Width; j++)
                {
                    var offset = 4 * (i * b.Width + j);
                    results[j, i] = Color.FromArgb(data[offset + 3], data[offset + 2], data[offset + 1], data[offset]);
                }
            }

            return results;
        }

        public static List<Color> GetPixelColorList(this Bitmap b)
        {
            List<Color> results = new List<Color>();
            var data = GetPixelData(b);

            for (int i = 0; i < data.Length; i += 4)
            {
                results.Add(Color.FromArgb(data[i + 3], data[i + 2], data[i + 1], data[i]));
            }

            return results;
        }
    }
}
