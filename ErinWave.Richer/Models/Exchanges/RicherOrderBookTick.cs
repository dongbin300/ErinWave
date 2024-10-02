using ErinWave.Richer.Enums;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherOrderBookTick(OrderSide orderSide, decimal price, decimal quantity)
	{
		public OrderSide OrderSide { get; set; } = orderSide;
		public decimal Price { get; set; } = price;
		public decimal Quantity { get; set; } = quantity;
	}
}
