using ErinWave.Richer.Enums;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherPair
	{
		/// <summary>
		/// BTCUSDT
		/// </summary>
		public string Symbol { get; set; }
		/// <summary>
		/// BTC
		/// </summary>
		public string BaseAsset { get; set; }
		/// <summary>
		/// USDT
		/// </summary>
		public string QuoteAsset { get; set; }
		/// <summary>
		/// Current Price
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// Managing only open orders
		/// </summary>
		public List<RicherOpenOrder> Orders { get; set; } = [];
		public List<RicherOpenOrder> SellOrders => [.. Orders.Where(x => x.OrderSide.Equals(OrderSide.Sell)).OrderBy(x => x.Price).ThenBy(x => x.Time)];
		public List<RicherOpenOrder> BuyOrders => [.. Orders.Where(x => x.OrderSide.Equals(OrderSide.Buy)).OrderByDescending(x => x.Price).ThenBy(x => x.Time)];
		public List<RicherTransaction> Transactions { get; set; } = [];
		public RicherOrderBook OrderBook => GetOrderBook();
		public RicherChart Chart { get; set; }

		public decimal MinPrice { get; set; }
		public decimal MaxPrice { get; set; }
		public decimal TickPrice { get; set; }
		public decimal TotalCount { get; set; }
		public int MaxLeverage { get; set; }
		public decimal MinOrderQuantity { get; set; }
		public decimal MaxOrderQuantity { get; set; }

		/// <summary>
		/// 시장가 주문시 Max 비율
		/// </summary>
		public decimal MarketMaxRate { get; set; }

		/// <summary>
		/// 시가총액
		/// 가격 * 총 수량
		/// </summary>
		public decimal MarketCap => Price * TotalCount;

		public RicherPair(string symbol)
		{
			Symbol = symbol;
			Chart = new RicherChart();
		}

		public RicherOrderBook GetOrderBook()
		{
			var orderBook = new RicherOrderBook();

			var sellOrders = Orders
				.Where(o => o.OrderSide == OrderSide.Sell && o.Remained > 0)
				.GroupBy(o => o.Price)
				.Select(g => new RicherOrderBookTick(OrderSide.Sell, g.Key, g.Sum(o => o.Remained)))
				.OrderBy(tick => tick.Price)
				.Take(100)
				.ToList();

			var buyOrders = Orders
				.Where(o => o.OrderSide == OrderSide.Buy && o.Remained > 0)
				.GroupBy(o => o.Price)
				.Select(g => new RicherOrderBookTick(OrderSide.Buy, g.Key, g.Sum(o => o.Remained)))
				.OrderByDescending(tick => tick.Price)
				.Take(100)
				.ToList();

			orderBook.Ticks.AddRange(sellOrders);
			orderBook.Ticks.AddRange(buyOrders);

			return orderBook;
		}

		public void AddOrder(DateTime time, string makerId, OrderSide orderSide, decimal price, decimal quantity)
		{
			Orders.Add(new RicherOpenOrder(time, makerId, orderSide, price, quantity));
		}

		public void RemoveOrder(RicherOpenOrder order)
		{
			Orders.Remove(order);
		}

		public void RemoveOrder(DateTime time, string makerId)
		{
			var order = Orders.Find(x => x.Time.Equals(time) && x.MakerId.Equals(makerId));
			if (order == null)
			{
				return;
			}

			RemoveOrder(order);
		}

		public void AddTransaction(DateTime time, decimal price, decimal quantity, string makerId, string takerId)
		{
			var id = Transactions.Count + 1;
			Transactions.Add(new RicherTransaction(id, time, price, quantity, makerId, takerId));
		}
	}
}
