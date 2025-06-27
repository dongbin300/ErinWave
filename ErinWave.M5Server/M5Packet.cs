namespace ErinWave.M5Server
{
	public class M5Packet(string type, string source, string data)
	{
		public string Type { get; set; } = type;
		public string Source { get; set; } = source;
		public string Data { get; set; } = data;

		public override string ToString()
		{
			return $"{Type},{Source},{Data}";
		}
	}
}
