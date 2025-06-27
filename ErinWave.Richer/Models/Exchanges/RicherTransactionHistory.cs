using ErinWave.Richer.Enums;

using System.ComponentModel;
using System.Windows.Media;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherTransactionHistory
	{
		public DateTime Time { get; set; }
		/// <summary>
		/// Transaction ID
		/// </summary>
		public int Id { get; set; }
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
		/// Order Side
		/// </summary>
		public OrderSide Side { get; set; }
		/// <summary>
		/// Whether maker or taker
		/// </summary>
		public bool IsMaker { get; set; }
		public string MakerString => IsMaker ? "Maker" : "Taker";
		public SolidColorBrush SideColor => Side == OrderSide.Buy ? Common.LongColor : Common.ShortColor;


		public RicherTransactionHistory(int id, DateTime time, decimal price, decimal quantity, OrderSide side, bool isMaker)
		{
			Id = id;
			Time = time;
			Price = price;
			Quantity = quantity;
			Side = side;
			IsMaker = isMaker;
		}
	}
}
