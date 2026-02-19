using ErinWave.OsuSkinManager.Models;
using System.IO;
using System.Text.Json;

namespace ErinWave.OsuSkinManager.Services
{
	public static class SkinManager
	{
		public static List<OsuSkin> LoadSkins(string? skinsPath = null)
		{
			var skins = new List<OsuSkin>();
			skinsPath ??= OsuPathDetector.GetSkinsPath();

			if (string.IsNullOrEmpty(skinsPath) || !Directory.Exists(skinsPath))
				return skins;

			var directories = Directory.GetDirectories(skinsPath);
			foreach (var dir in directories)
			{
				var skin = LoadSkin(dir);
				if (skin != null)
				{
					skins.Add(skin);
				}
			}

			return skins.OrderByDescending(s => s.LastModified).ToList();
		}

		public static OsuSkin? LoadSkin(string skinPath)
		{
			if (!Directory.Exists(skinPath))
				return null;

			var skinName = Path.GetFileName(skinPath);
			var skin = new OsuSkin
			{
				Name = skinName,
				Path = skinPath,
				LastModified = Directory.GetLastWriteTime(skinPath)
			};

			// skin.ini 파일에서 메타데이터 읽기
			var skinIniPath = Path.Combine(skinPath, "skin.ini");
			if (File.Exists(skinIniPath))
			{
				LoadSkinMetadata(skin, skinIniPath);
			}

			// 파일 목록 로드
			LoadSkinFiles(skin);

			// 미리보기 이미지 찾기
			skin.PreviewImagePath = FindPreviewImage(skinPath);

			return skin;
		}

		private static void LoadSkinMetadata(OsuSkin skin, string skinIniPath)
		{
			try
			{
				var lines = File.ReadAllLines(skinIniPath);
				foreach (var line in lines)
				{
					if (line.StartsWith("Name:") || line.StartsWith("Author:") || line.StartsWith("Version:"))
					{
						var parts = line.Split(':', 2);
						if (parts.Length == 2)
						{
							var key = parts[0].Trim();
							var value = parts[1].Trim();

							switch (key)
							{
								case "Name":
									skin.Name = value;
									break;
								case "Author":
									skin.Author = value;
									break;
								case "Version":
									skin.Version = value;
									break;
							}
						}
					}
				}
			}
			catch
			{
				// skin.ini 읽기 오류 무시
			}
		}

		private static void LoadSkinFiles(OsuSkin skin)
		{
			var files = Directory.GetFiles(skin.Path, "*", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				var extension = Path.GetExtension(file).ToLowerInvariant();
				var relativePath = Path.GetRelativePath(skin.Path, file);

				switch (extension)
				{
					case ".png":
					case ".jpg":
					case ".jpeg":
					case ".bmp":
					case ".gif":
						skin.ImageFiles.Add(relativePath);
						break;
					case ".wav":
					case ".mp3":
					case ".ogg":
						skin.AudioFiles.Add(relativePath);
						break;
					case ".ini":
					case ".cfg":
					case ".json":
					case ".txt":
						skin.ConfigFiles.Add(relativePath);
						break;
				}
			}
		}

		private static string? FindPreviewImage(string skinPath)
		{
			var previewNames = new[] { "preview.png", "skinpreview.png", "preview.jpg", "skinpreview.jpg" };
			foreach (var previewName in previewNames)
			{
				var previewPath = Path.Combine(skinPath, previewName);
				if (File.Exists(previewPath))
					return previewPath;
			}

			// 첫 번째 이미지 파일을 미리보기로 사용
			var imageFiles = Directory.GetFiles(skinPath, "*.png")
				.Concat(Directory.GetFiles(skinPath, "*.jpg"))
				.Concat(Directory.GetFiles(skinPath, "*.jpeg"))
				.FirstOrDefault();

			return imageFiles;
		}

		public static bool DeleteSkin(OsuSkin skin)
		{
			try
			{
				if (Directory.Exists(skin.Path))
				{
					Directory.Delete(skin.Path, true);
					return true;
				}
			}
			catch
			{
				// 삭제 오류
			}
			return false;
		}

		public static bool CopySkin(OsuSkin sourceSkin, string newSkinName)
		{
			try
			{
				var skinsPath = Path.GetDirectoryName(sourceSkin.Path);
				if (string.IsNullOrEmpty(skinsPath))
					return false;

				var destinationPath = Path.Combine(skinsPath, newSkinName);
				if (Directory.Exists(destinationPath))
					return false;

				// 디렉토리 복사
				CopyDirectory(sourceSkin.Path, destinationPath);

				// 새 스킨의 ini 파일 이름 업데이트
				UpdateSkinIni(destinationPath, newSkinName);

				return true;
			}
			catch
			{
				// 복사 오류
			}
			return false;
		}

		private static void CopyDirectory(string source, string destination)
		{
			Directory.CreateDirectory(destination);

			foreach (var file in Directory.GetFiles(source))
			{
				var destFile = Path.Combine(destination, Path.GetFileName(file));
				File.Copy(file, destFile);
			}

			foreach (var directory in Directory.GetDirectories(source))
			{
				var destDir = Path.Combine(destination, Path.GetFileName(directory));
				CopyDirectory(directory, destDir);
			}
		}

		private static void UpdateSkinIni(string skinPath, string newSkinName)
		{
			var skinIniPath = Path.Combine(skinPath, "skin.ini");
			if (File.Exists(skinIniPath))
			{
				var lines = File.ReadAllLines(skinIniPath);
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines[i].StartsWith("Name:"))
					{
						lines[i] = $"Name: {newSkinName}";
						break;
					}
				}
				File.WriteAllLines(skinIniPath, lines);
			}
		}
	}
}