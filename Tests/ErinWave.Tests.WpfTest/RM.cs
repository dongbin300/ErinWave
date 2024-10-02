namespace ErinWave.Tests.WpfTest
{
	public static class RM
	{
		public static RicherHuman Human { get; set; } = default!;
	}

	public class RicherHuman : RicherPlayer
	{

	}

	public class RicherPlayer
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public RicherWallet Wallet { get; set; }

		public RicherPlayer() : this("", "")
		{

		}

		public RicherPlayer(string id, string name)
		{
			Id = id;
			Name = name;
			Wallet = new RicherWallet();
		}
	}

	public class RicherWallet
	{
		public List<RicherWalletAsset> Assets { get; set; } = [];
	}

	public class RicherWalletAsset(string name, decimal quantity)
	{
		public string Name { get; set; } = name;
		public decimal Quantity { get; set; } = quantity;
	}
}
