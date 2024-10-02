using ErinWave.Richer.Enums;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherOrderBook
	{
		public List<RicherOrderBookTick> Ticks { get; set; } = [];
		public List<RicherOrderBookTick> SellTicks => Ticks.FindAll(x => x.OrderSide.Equals(OrderSide.Sell));
		public List<RicherOrderBookTick> BuyTicks => Ticks.FindAll(x => x.OrderSide.Equals(OrderSide.Buy));
		public decimal TopSellPrice => SellTicks[0].Price;
		public decimal TopBuyPrice => BuyTicks[0].Price;
	}
}
