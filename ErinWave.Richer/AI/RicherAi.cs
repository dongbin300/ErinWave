using ErinWave.Richer.Enums;
using ErinWave.Richer.Models;
using ErinWave.Richer.Models.Exchanges;

using Newtonsoft.Json;

namespace ErinWave.Richer.AI
{
	public class RicherAi : RicherPlayer
	{
		public RicherAiType Type { get; set; } = RicherAiType.None;

		public List<string> MonitorSymbols { get; set; } = [];
		[JsonIgnore]
		public List<RicherPair> MonitorPairs { get; set; } = [];

		public RicherWhaleMode WhaleMode { get; set; } = RicherWhaleMode.None;

		public RicherAi(string id, string name, RicherAiType type)
		{
			Id = id;
			Name = name;
			Type = type;
			Wallet = new RicherWallet();
		}
	}
}
