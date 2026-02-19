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

using ErinWave.Windows;

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ErinWave.Imaging
{
    [SupportedOSPlatform("windows")]
    public class ScreenShot
    {
        public static void SaveAsFile(int left, int top, int width, int height, string dirPath, string fileName, ImageFormat format)
        {
            var bitmap = Take(left, top, width, height);

            var dInfo = Directory.CreateDirectory(dirPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);

            bitmap.Save($"{dirPath}{(dirPath.LastIndexOf("\\") < dirPath.Length - 1 ? "\\" : "")}{fileName}", format);

            bitmap.Dispose();
        }

        public static Bitmap Take(int left, int top, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var destGrp = Graphics.FromImage(bitmap))
            {
                using var sourceGrp = Graphics.FromHwnd(IntPtr.Zero);
                WinApi.BitBlt(destGrp.GetHdc(), 0, 0, bitmap.Width, bitmap.Height, sourceGrp.GetHdc(), left, top, WinApi.SRCCOPY);
            }

            return bitmap;
        }
    }
}
