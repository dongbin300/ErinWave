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

		public decimal GetEstimatedAsset()
		{
			var result = Wallet.KrwQuantity;

			foreach (var asset in Wallet.Assets)
			{
				var pair = RM.Exchange.GetPair(asset.Name + "KRW");
				if (pair == null)
				{
					continue;
				}

				result += pair.Price * asset.Quantity;
			}

			return result;
		}

		public decimal GetAssetQuantity(string assetName)
		{
			return Wallet.GetAssetQuantity(assetName);
		}

		public decimal GetAssetAmount(string assetName)
		{
			var pair = RM.Exchange.GetPair(assetName + "KRW");
			if (pair == null)
			{
				return 0;
			}
			return pair.Price * GetAssetQuantity(assetName);
		}

		public decimal GetHoldingRatio(RicherPair pair)
		{
			var estimatedAsset = GetEstimatedAsset();
			if (estimatedAsset == 0)
			{
				return 0;
			}
			else
			{
				return GetAssetAmount(pair.BaseAsset) / estimatedAsset;
			}
		}

		public decimal GetAvailableBuyQuantity(RicherPair pair)
		{
			return Wallet.KrwQuantity / pair.Price;
		}
	}
}
