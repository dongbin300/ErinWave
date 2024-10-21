using System.Windows.Media;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherTransaction
	{
		public DateTime Time { get; set; }
		/// <summary>
		/// Transaction ID
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Maker User ID
		/// </summary>
		public string MakerId { get; set; }
		/// <summary>
		/// Taker User ID
		/// </summary>
		public string TakerId { get; set; }
		/// <summary>
		/// Transaction Price
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// Transaction Quantity
		/// </summary>
		public decimal Quantity { get; set; }
		/// <summary>
		/// Transaction Amount
		/// </summary>
		public decimal Amount => Price * Quantity;
		/// <summary>
		/// Whether Taker Buy
		/// </summary>
		public bool IsTakerBuy { get; set; }
		public SolidColorBrush PriceColor => IsTakerBuy ? Common.LongColor : Common.ShortColor;


		public RicherTransaction(int id, DateTime time, decimal price, decimal quantity, string makerId, string takerId, bool isTakerBuy)
		{
			Id = id;
			Time = time;
			Price = price;
			Quantity = quantity;
			MakerId = makerId;
			TakerId = takerId;
			IsTakerBuy = isTakerBuy;
		}
	}
}
