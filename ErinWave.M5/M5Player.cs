namespace ErinWave.M5
{
	public class M5Player
	{
		public string Id { get; set; } = string.Empty;
		public string Job { get; set; } = string.Empty;
		public List<string> Deck { get; set; } = [];
		public List<string> Hand { get; set; } = [];
		public List<string> Used { get; set; } = [];
	}
}
