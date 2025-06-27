using ErinWave.Richer.Enums;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherOrderBook(decimal price, decimal tickPrice)
	{
		public decimal Price { get; set; } = price;
		public decimal TickPrice { get; set; } = tickPrice;
		public List<RicherOrderBookTick> Ticks { get; set; } = [];
		public List<RicherOrderBookTick> SellTicks => Ticks.FindAll(x => x.OrderSide.Equals(OrderSide.Sell));
		public List<RicherOrderBookTick> BuyTicks => Ticks.FindAll(x => x.OrderSide.Equals(OrderSide.Buy));
		public decimal TopSellPrice => SellTicks.Count == 0 ? Price + TickPrice : SellTicks[0].Price;
		public decimal TopBuyPrice => BuyTicks.Count == 0 ? Price - TickPrice : BuyTicks[0].Price;
	}
}
