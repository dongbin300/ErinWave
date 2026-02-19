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
    public class ErinWaveResource
    {
        /// <summary>
        /// C:\Users\Gaten\AppData\Roaming\Gaten
        /// </summary>
        public static string BaseFilePath => ErinWavePath.Assets;
        public static string MintChocoDirectoryPath => BaseFilePath.Down("MintChoco");
        public static string DotNetDirectoryPath => BaseFilePath.Down("dotnet");
        public static string[] MySqlInfoText => GetTextLines("mysql_info.msq");
        public static string[] BinanceApiKeyText => GetTextLines("binance_api.txt");
        public static string BinanceFuturesDataPath => BaseFilePath.Down("BinanceFuturesData");
        public static string SolutionPath => Environment.CurrentDirectory;

        public static string GetPath(string subPath) => BaseFilePath.Down(subPath);
        public static string GetText(string subPath) => File.ReadAllText(BaseFilePath.Down(subPath));
        public static string[] GetTextLines(string subPath) => File.ReadAllLines(BaseFilePath.Down(subPath));
        public static Dictionary<string, string> GetTextDictionary(string subPath) => ErinWaveFile.ReadToDictionary(BaseFilePath.Down(subPath));

        public static void SetText(string subPath, string text) => File.WriteAllText(BaseFilePath.Down(subPath), text);
        public static void SetTextLines(string subPath, string[] text) => File.WriteAllLines(BaseFilePath.Down(subPath), text);
        public static void SetTextDictionary(string subPath, Dictionary<string, string> text) => ErinWaveFile.WriteByDictionary(BaseFilePath.Down(subPath), text);

        public static void AppendText(string subPath, string text) => File.AppendAllText(BaseFilePath.Down(subPath), text);
        public static void AppendTextLines(string subPath, string[] text) => File.AppendAllLines(BaseFilePath.Down(subPath), text);
        public static void AppendTextDictionary(string subPath, Dictionary<string, string> text) => ErinWaveFile.AppendByDictionary(BaseFilePath.Down(subPath), text);

        public static string SmartExePath(string keyword)
        {
            var mainPath = SolutionPath[..(SolutionPath.IndexOf("\\CS\\") + 3)];
            var files = new DirectoryInfo(mainPath).GetFiles($"*{keyword}*.exe", SearchOption.AllDirectories);

            return files.Length > 0 ? files[0].FullName : string.Empty;
        }

        public static string SmartExePath(string keyword, string mainPath)
        {
            var files = new DirectoryInfo(mainPath).GetFiles($"*{keyword}*.exe", SearchOption.AllDirectories);

            return files.Length > 0 ? files[0].FullName : string.Empty;
        }

        public static string SmartInstallerPath(string keyword, string mainPath)
        {
            var info = new DirectoryInfo(mainPath);
            var files = info.GetFiles($"*{keyword}*.exe", SearchOption.AllDirectories).Union(
                info.GetFiles($"*{keyword}*.msi", SearchOption.AllDirectories)).ToArray();

            return files.Length > 0 ? files[0].FullName : string.Empty;
        }
    }
}
