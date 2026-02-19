namespace ErinWave.OsuSkinManager.Models
{
	public class OsuSkin
	{
		public string Name { get; set; } = string.Empty;
		public string Path { get; set; } = string.Empty;
		public string? Author { get; set; }
		public string? Version { get; set; }
		public DateTime? LastModified { get; set; }
		public List<string> ImageFiles { get; set; } = new();
		public List<string> AudioFiles { get; set; } = new();
		public List<string> ConfigFiles { get; set; } = new();
		public string? PreviewImagePath { get; set; }

		public string GetDisplayName()
		{
			var displayName = Name;
			if (!string.IsNullOrEmpty(Author))
				displayName += $" - {Author}";
			if (!string.IsNullOrEmpty(Version))
				displayName += $" v{Version}";
			return displayName;
		}
	}

	public enum OsuGameMode
	{
		Standard,
		Taiko,
		CatchTheBeat,
		Mania
	}

	public class SkinFileInfo
	{
		public string Name { get; set; } = string.Empty;
		public string FullPath { get; set; } = string.Empty;
		public FileType Type { get; set; }
		public long Size { get; set; }
		public DateTime LastModified { get; set; }
	}

	public enum FileType
	{
		Image,
		Audio,
		Config,
		Other
	}
}