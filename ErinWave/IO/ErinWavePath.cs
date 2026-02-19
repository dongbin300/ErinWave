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

namespace ErinWave.IO
{
    public static class ErinWavePath
    {
        public static string Windows => Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static string Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string AppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string Fonts => Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        public static string ProgramFiles => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static string ProgramFilesX86 => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string System => Environment.GetFolderPath(Environment.SpecialFolder.System);
        public static string SystemX86 => Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
        public static string Users => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string Documents => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string Downloads => Users.Down("Downloads");
        public static string Assets => "D:\\Assets";

		public static string Down(this string path, params string[] downPaths)
        {
            return Path.Combine(path, Path.Combine(downPaths));
        }

        public static void TryCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
