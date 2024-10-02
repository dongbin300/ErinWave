using ErinWave.Richer.Enums;
using ErinWave.Richer.Models;
using ErinWave.Richer.Models.Exchanges;

namespace ErinWave.Richer.AI
{
	public class RicherAi : RicherPlayer
	{
		public RicherAiType Type { get; set; } = RicherAiType.None;

		public RicherAi(string id, string name, RicherAiType type)
		{
			Id = id;
			Name = name;
			Type = type;
			Wallet = new RicherWallet();
		}
	}
}
