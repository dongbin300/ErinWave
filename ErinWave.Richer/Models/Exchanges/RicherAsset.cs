namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherWalletAsset(string name, decimal quantity)
	{
		public string Name { get; set; } = name;
		public decimal Quantity { get; set; } = quantity;
	}
}
