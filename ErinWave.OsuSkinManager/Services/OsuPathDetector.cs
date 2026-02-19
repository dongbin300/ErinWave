using Microsoft.Win32;
using System.IO;

namespace ErinWave.OsuSkinManager.Services
{
	public static class OsuPathDetector
	{
		public static string? GetOsuInstallationPath()
		{
			// 레지스트리에서 osu! 설치 경로 찾기
			string? osuPath = null;

			// 64비트 레지스트리 확인
			osuPath = GetOsuPathFromRegistry(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64));
			if (!string.IsNullOrEmpty(osuPath) && Directory.Exists(osuPath))
				return osuPath;

			// 32비트 레지스트리 확인
			osuPath = GetOsuPathFromRegistry(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32));
			if (!string.IsNullOrEmpty(osuPath) && Directory.Exists(osuPath))
				return osuPath;

			// CurrentUser 레지스트리 확인
			osuPath = GetOsuPathFromRegistry(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64));
			if (!string.IsNullOrEmpty(osuPath) && Directory.Exists(osuPath))
				return osuPath;

			osuPath = GetOsuPathFromRegistry(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32));
			if (!string.IsNullOrEmpty(osuPath) && Directory.Exists(osuPath))
				return osuPath;

			// 기본 설치 경로 확인
			var defaultPaths = new[]
			{
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "osu!"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "osu!"),
				@"C:\Games\osu!",
				@"C:\osu!"
			};

			foreach (var path in defaultPaths)
			{
				if (Directory.Exists(path) && File.Exists(Path.Combine(path, "osu!.exe")))
				{
					return path;
				}
			}

			return null;
		}

		private static string? GetOsuPathFromRegistry(RegistryKey baseKey)
		{
			try
			{
				using var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\osu!");
				if (key != null)
				{
					var installLocation = key.GetValue("InstallLocation") as string;
					if (!string.IsNullOrEmpty(installLocation) && Directory.Exists(installLocation))
					{
						return installLocation;
					}
				}
			}
			catch
			{
				// 레지스트리 접근 오류 무시
			}

			return null;
		}

		public static string? GetSkinsPath(string? osuPath = null)
		{
			osuPath ??= GetOsuInstallationPath();
			if (string.IsNullOrEmpty(osuPath))
				return null;

			var skinsPath = Path.Combine(osuPath, "Skins");
			return Directory.Exists(skinsPath) ? skinsPath : null;
		}
	}
}