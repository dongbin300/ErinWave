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

using Microsoft.Win32;

using System.Runtime.Versioning;

namespace ErinWave.Windows
{
    [SupportedOSPlatform("windows")]
    public class Boot
    {
        private static readonly string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// 시작 프로그램 등록
        /// </summary>
        /// <param name="programName">"Program Name"</param>
        /// <param name="exePath">Environment.ProcessPath</param>
        public static void RegisterStartProgram(string programName, string exePath)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(RunKey);
                if (key == null)
                {
                    return;
                }
                if (key.GetValue(programName) != null)
                {
                    return;
                }

                key.Close();
                key = Registry.LocalMachine.OpenSubKey(RunKey, true);
                if (key == null)
                {
                    return;
                }

                key.SetValue(programName, exePath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 시작 프로그램 등록취소
        /// </summary>
        /// <param name="programName">"Program Name"</param>
        public static void UnregisterStartProgram(string programName)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(RunKey, true);
                if (key == null)
                {
                    return;
                }

                key.DeleteValue(programName);
            }
            catch
            {
                throw;
            }
        }
    }
}
