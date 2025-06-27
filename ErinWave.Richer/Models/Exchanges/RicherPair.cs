using ErinWave.Richer.Enums;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherPair(string symbol)
	{
		/// <summary>
		/// BTCUSDT
		/// </summary>
		public string Symbol { get; set; } = symbol;
		/// <summary>
		/// BTC
		/// </summary>
		public string BaseAsset { get; set; } = string.Empty;
		/// <summary>
		/// USDT
		/// </summary>
		public string QuoteAsset { get; set; } = string.Empty;
		/// <summary>
		/// Current Price
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// List Price
		/// </summary>
		public decimal ListPrice { get; set; }

		/// <summary>
		/// Managing only open orders
		/// </summary>
		public List<RicherOpenOrder> Orders { get; set; } = [];
		public List<RicherOpenOrder> SellOrders => [.. Orders.Where(x => x != null && x.OrderSide.Equals(OrderSide.Sell)).OrderBy(x => x.Price).ThenBy(x => x.Time)];
		public List<RicherOpenOrder> BuyOrders => [.. Orders.Where(x => x != null && x.OrderSide.Equals(OrderSide.Buy)).OrderByDescending(x => x.Price).ThenBy(x => x.Time)];
		public List<RicherTransaction> Transactions { get; set; } = [];
		public RicherTransaction? LastTransaction => GetLastTransaction();
		public RicherOrderBook OrderBook => GetOrderBook(TickPrice);
		public RicherChart Chart { get; set; } = new RicherChart();

		public decimal MinPrice { get; set; }
		public decimal MaxPrice { get; set; }
		public decimal TickPrice { get; set; }
		public decimal TotalCount { get; set; }
		public int MaxLeverage { get; set; }
		public decimal MinOrderQuantity { get; set; }
		public decimal MaxOrderQuantity { get; set; }
		public decimal TickQuantity { get; set; }

		/// <summary>
		/// 시장가 주문시 Max 비율
		/// </summary>
		public decimal MarketMaxRate { get; set; }

		/// <summary>
		/// 시가총액
		/// 가격 * 총 수량
		/// </summary>
		public decimal MarketCap => Price * TotalCount;

		public RicherTransaction? GetLastTransaction()
		{
			var transactionSnapshot = Transactions.Where(x => x != null).ToList();

			return transactionSnapshot.OrderByDescending(x => x.Time).FirstOrDefault();
		}

		public RicherOrderBook GetOrderBook(decimal tickSize)
		{
			var orderBook = new RicherOrderBook(Price, tickSize);

			var orderSnapshot = Orders.Where(o => o != null).ToList();

			var sellOrders = orderSnapshot
				.Where(o => o.OrderSide == OrderSide.Sell && o.Remained > 0)
				.GroupBy(o => Math.Ceiling(o.Price / tickSize) * tickSize)
				.Select(g => new RicherOrderBookTick(OrderSide.Sell, g.Key, g.Sum(o => o.Remained)))
				.OrderBy(tick => tick.Price)
				.Take(100)
				.ToList();

			var buyOrders = orderSnapshot
				.Where(o => o.OrderSide == OrderSide.Buy && o.Remained > 0)
				.GroupBy(o => Math.Floor(o.Price / tickSize) * tickSize)
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

		public List<RicherOpenOrder> GetOpenOrders(string playerId)
		{
			return Orders.Where(x => x != null && x.MakerId.Equals(playerId)).ToList();
		}

		public void AddTransaction(DateTime time, decimal price, decimal quantity, string makerId, string takerId, bool isTakerBuy)
		{
			var id = Transactions.Count + 1;
			var transaction = new RicherTransaction(id, time, price, quantity, makerId, takerId, isTakerBuy);
			Transactions.Add(transaction);
			Chart.Add(transaction);
		}

		public List<RicherTransactionHistory> GetTransactionHistories(string playerId)
		{
			return Transactions.Where(x => x.MakerId.Equals(playerId) || x.TakerId.Equals(playerId)).ToList()
				.Select(x =>
				new RicherTransactionHistory(
					x.Id, x.Time, x.Price, x.Quantity,
					side: x.MakerId == playerId && x.IsTakerBuy || x.TakerId == playerId && !x.IsTakerBuy ? OrderSide.Sell : OrderSide.Buy,
					isMaker: x.MakerId == playerId
					)).Reverse().ToList();
		}

		public List<RicherTransactionHistory> GetTransactionHistoriesLight(string playerId)
		{
			return Transactions.Where(x => x.MakerId.Equals(playerId) || x.TakerId.Equals(playerId)).ToList()
				.Select(x =>
				new RicherTransactionHistory(
					x.Id, x.Time, x.Price, x.Quantity,
					side: x.MakerId == playerId && x.IsTakerBuy || x.TakerId == playerId && !x.IsTakerBuy ? OrderSide.Sell : OrderSide.Buy,
					isMaker: x.MakerId == playerId
					)).Reverse().Take(100).ToList();
		}
	}
}
