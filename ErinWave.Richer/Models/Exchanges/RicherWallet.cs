namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherWallet
	{
		public List<RicherWalletAsset> Assets { get; set; } = [];
		public decimal KrwQuantity => GetAssetQuantity("KRW");

		public decimal GetAssetQuantity(string assetName)
		{
			var asset = Assets.Find(x => x.Name.Equals(assetName));
			if (asset == null)
			{
				return 0m;
			}

			return asset.Quantity;
		}

		public string IncomeAsset(string assetName, decimal quantity)
		{
			if (quantity < 0)
			{
				var asset = Assets.Find(x => x.Name.Equals(assetName));
				if (asset == null)
				{
					return $"No Asset: {assetName}";
				}
				if (asset.Quantity < quantity)
				{
					return $"Require Asset: {assetName}";
				}

				asset.Quantity += quantity;
				return string.Empty;
			}
			else
			{
				var asset = Assets.Find(x => x.Name.Equals(assetName));
				if (asset == null)
				{
					Assets.Add(new RicherWalletAsset(assetName, quantity));
				}
				else
				{
					asset.Quantity += quantity;
				}
				return string.Empty;
			}
		}
	}
}
