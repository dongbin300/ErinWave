using ErinWave.Richer.Enums;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherOpenOrder
	{
		public DateTime Time { get; set; }
		public string MakerId { get; set; }
		public OrderSide OrderSide { get; set; }
		public decimal Price { get; set; }
		public decimal Quantity { get; set; }
		public decimal Filled { get; set; }
		public decimal Remained => Quantity - Filled;
		public bool IsCompleted => Remained == 0;
		public decimal Amount => Price * Quantity;
		public decimal FilledAmount => Price * Filled;
		public decimal RemainedAmount => Amount - FilledAmount;

		public RicherOpenOrder(DateTime time, string makerId, OrderSide orderSide, decimal price, decimal quantity)
		{
			Time = time;
			MakerId = makerId;
			OrderSide = orderSide;
			Price = price;
			Quantity = quantity;
			Filled = 0;
		}
	}
}
