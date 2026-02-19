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

namespace ErinWave.Imaging.Recognition
{
    public enum ColorRangeType
    {
        D3,
        Average
    }

    public class ColorRange
    {
        private ColorRangeType type;
        private Color startColor;
        private Color endColor;
        private int min;
        private int max;

        public ColorRange() : this(0,0)
        {

        }

        public ColorRange(Color startColor, Color endColor)
        {
            type = ColorRangeType.D3;
            this.startColor = startColor;
            this.endColor = endColor;
        }

        public ColorRange(int min, int max)
        {
            type = ColorRangeType.Average;
            this.min = min;
            this.max = max;
        }

        public bool IsValid(Color color)
        {
            double average = (double)(color.R + color.G + color.B) / 3;
            return type switch
            {
                ColorRangeType.D3 => startColor.R <= color.R && color.R <= endColor.R &&
                                        startColor.G <= color.G && color.G <= endColor.G &&
                                        startColor.B <= color.B && color.B <= endColor.B,
                ColorRangeType.Average => min <= average && average <= max,
                _ => false,
            };
        }
    }
}
