namespace ErinWave.M5
{
	public class M5Field
	{
		public string Boss { get; set; } = string.Empty;
		public List<string> Dungeons { get; set; } = [];
		public string CurrentDungeon { get; set; } = string.Empty;
		public List<string> CurrentCards { get; set; } = [];
		public List<string> Used { get; set; } = [];
		public int RemainSeconds { get; set; } = -1;
		public bool IsPlaying { get; set; } = false;
	}
}
