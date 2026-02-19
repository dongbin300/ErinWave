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
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace ErinWave.Imaging
{
    [SupportedOSPlatform("windows")]
    public static class ImageExtension
    {
        public static void Resize(this Image source, string destFileName, float scale)
        {
			using var resized = ResizeBitmap(source, scale, InterpolationMode.HighQualityBicubic);
			resized.Save(destFileName);
		}

        public static Bitmap Resize(this Image source, int imageWidth)
        {
            float scale = (float)imageWidth / source.Width;
            return source.ResizeBitmap(scale, InterpolationMode.HighQualityBicubic);
        }

        public static Bitmap ResizeBitmap(this Image source, float scale, InterpolationMode quality)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // Figure out the new size.
            var width = (int)(source.Width * scale);
            var height = (int)(source.Height * scale);

            // Create the new bitmap.
            // Note that Bitmap has a resize constructor, but you can't control the quality.
            var bmp = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = quality;
                g.DrawImage(source, new Rectangle(0, 0, width, height));
                g.Save();
            }

            return bmp;
        }

        public static Bitmap CropImage(this Image source, Rectangle section)
        {
            var bmp = new Bitmap(section.Width, section.Height);
            Graphics.FromImage(bmp).DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            return bmp;
        }
    }
}
