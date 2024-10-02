using ErinWave.Richer.Models.Exchanges;

namespace ErinWave.Richer.Models
{
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

		/// <summary>
		/// 자산의 흐름
		/// </summary>
		/// <param name="assetName"></param>
		/// <param name="quantity"></param>
		public void Income(string assetName, decimal quantity)
		{
			Wallet.IncomeAsset(assetName, quantity);
		}
	}
}
