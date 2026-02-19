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
    public enum ExtensionTypeCode
    {
        Empty,
        Object,
        SByte,
        Byte,
        ByteArray,
        Int16,
        Int32,
        Int64,
        UInt16,
        UInt32,
        UInt64,
    }

    public static class ObjectExtension
    {
        /// <summary>
        /// Get TypeCode
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ExtensionTypeCode GetTypeCode(this object input)
        {
            if(input == null)
            {
                return ExtensionTypeCode.Empty;
            }

            return input.GetType().Name switch
            {
                "sbyte" or "SByte" => ExtensionTypeCode.SByte,
                "byte" or "Byte" => ExtensionTypeCode.Byte,
                "byte[]" or "Byte[]" => ExtensionTypeCode.ByteArray,
                "short" or "Int16" => ExtensionTypeCode.Int16,
                "ushort" or "UInt16" => ExtensionTypeCode.UInt16,
                "int" or "Int32" => ExtensionTypeCode.Int32,
                "uint" or "UInt32" => ExtensionTypeCode.UInt32,
                "long" or "Int64" => ExtensionTypeCode.Int64,
                "ulong" or "UInt64" => ExtensionTypeCode.UInt64,
                _ => ExtensionTypeCode.Object
            };
        }

        /// <summary>
        /// Get Size
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetSize(this object input)
        {
            if (input == null)
            {
                return 0;
            }

            return input.GetTypeCode() switch
            {
                ExtensionTypeCode.SByte => 1,
                ExtensionTypeCode.Byte => 1,
                ExtensionTypeCode.ByteArray => ((byte[])input).Length,
                ExtensionTypeCode.Int16 => 2,
                ExtensionTypeCode.UInt16 => 2,
                ExtensionTypeCode.Int32 => 4,
                ExtensionTypeCode.UInt32 => 4,
                ExtensionTypeCode.Int64 => 8,
                ExtensionTypeCode.UInt64 => 8,
                _ => 0
            };
        }

        public static byte[] ToBytes(this object input, bool reverse = false)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var result = input.GetTypeCode() switch
            {
                ExtensionTypeCode.SByte => new byte[] { (byte)((sbyte)input & 0xFF) },
                ExtensionTypeCode.Byte => new byte[] { (byte)input },
                ExtensionTypeCode.ByteArray => (byte[])input,
                ExtensionTypeCode.Int16 => BitConverter.GetBytes((short)input),
                ExtensionTypeCode.UInt16 => BitConverter.GetBytes((ushort)input),
                ExtensionTypeCode.Int32 => BitConverter.GetBytes((int)input),
                ExtensionTypeCode.UInt32 => BitConverter.GetBytes((uint)input),
                ExtensionTypeCode.Int64 => BitConverter.GetBytes((long)input),
                ExtensionTypeCode.UInt64 => BitConverter.GetBytes((ulong)input),
                _ => throw new NotSupportedException("Unsupported data type(" + input.GetType().Name + ")"),
            };

            return reverse ? result.ReverseBytes() : result;
        }

        /// <summary>
        /// Big Endian / Little Endian<br/>
        /// (byte)15 -> "0F" / "0F"<br/>
        /// (int)15 -> "0000000F" / "0F000000"<br/>
        /// </summary>
        /// <param name="input">input type: byte, byte[], short, ushort, int, uint, long, ulong</param>
        /// <param name="autoSize"></param>
        /// <returns></returns>
        public static string ToHexString(this object input, bool reverse = false)
        { 
            return BitConverter.ToString(input.ToBytes(reverse)).Replace("-", "");
        }
    }
}
