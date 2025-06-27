using ErinWave.Richer.AI;
using ErinWave.Richer.Enums;
using ErinWave.Richer.Json;
using ErinWave.Richer.Maths;
using ErinWave.Richer.Models;
using ErinWave.Richer.Models.Exchanges;

using Newtonsoft.Json;

using System.IO;

namespace ErinWave.Richer
{
	/// <summary>
	/// Richer Master
	/// TODO
	/// 
	/// </summary>
	public static class RM
	{
		private static SmartRandom r = default!;
		public static readonly string ExchangeFilePath = "data/ex.json";
		public static readonly string HumanFilePath = "data/human.json";
		public static readonly string AisFilePath = "data/ai.json";
		public static readonly string BaseFilePath = "data/base.json";
		public static readonly decimal MakerFeeRate = 0.0001m; // 0.01%
		public static readonly decimal TakerFeeRate = 0.0005m; // 0.05%

		public static RicherExchange Exchange { get; set; } = default!;
		public static RicherHuman Human { get; set; } = default!;
		public static List<RicherAi> Ais { get; set; } = [];
		public static DateTime RicherTime { get; set; } = default!;

		#region Base
		public static void Init()
		{
			if (!Directory.Exists("data"))
			{
				Directory.CreateDirectory("data");
			}

			r = new SmartRandom();
		}

		public static void InitAfterLoad()
		{
			foreach (var pair in Exchange.Pairs)
			{
				pair.Chart = new RicherChart(pair.Transactions);
			}
		}

		public static RicherPlayer GetPlayer(string id)
		{
			if (Human.Id == id)
			{
				return Human;
			}

			var ai = Ais.Find(x => x.Id.Equals(id)) ?? throw new Exception("No player");
			return ai;
		}

		public static void Reset()
		{
			foreach (var pair in Exchange.Pairs)
			{
				pair.Chart.H1 = [];
				pair.Chart.M1 = [];
				pair.Chart.S1 = [];
				pair.Orders = [];
				pair.Price = pair.ListPrice;
				pair.Transactions = [];
			}

			Human.Wallet.Assets = [];
			Human.Income("KRW", 1_000_000);

			Ais = [];

			RicherTime = new DateTime(2000, 1, 1, 0, 0, 0);
		}

		public static void MakeUpAis(string monitorSymbol, int numOfWhale, int numOfCommoner)
		{
			CreateAi(RicherAiType.Master, monitorSymbol);

			for (var i = 0; i < numOfWhale; i++)
			{
				CreateAi(RicherAiType.Whale, monitorSymbol);
			}

			for (var i = 0; i < numOfCommoner; i++)
			{
				CreateAi(RicherAiType.Commoner, monitorSymbol);
			}
		}
		#endregion

		#region File
		public static void Save()
		{
			var jsonSetting = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				ContractResolver = new IgnoreReadOnlyResolver()
			};

			File.WriteAllText(ExchangeFilePath, JsonConvert.SerializeObject(Exchange, jsonSetting));
			File.WriteAllText(HumanFilePath, JsonConvert.SerializeObject(Human, jsonSetting));
			File.WriteAllText(AisFilePath, JsonConvert.SerializeObject(Ais, jsonSetting));
			File.WriteAllText(BaseFilePath, JsonConvert.SerializeObject(RicherTime, jsonSetting));
		}

		public static void Load()
		{
			Exchange = JsonConvert.DeserializeObject<RicherExchange>(File.ReadAllText(ExchangeFilePath)) ?? default!;
			Human = JsonConvert.DeserializeObject<RicherHuman>(File.ReadAllText(HumanFilePath)) ?? default!;
			Ais = JsonConvert.DeserializeObject<List<RicherAi>>(File.ReadAllText(AisFilePath)) ?? default!;
			RicherTime = JsonConvert.DeserializeObject<DateTime>(File.ReadAllText(BaseFilePath));

			Exchange ??= new RicherExchange();
			Human ??= new RicherHuman();
			Ais ??= [];
		}
		#endregion

		#region Order
		public static string PlaceOrder(RicherPlayer player, string symbol, OrderSide orderSide, OrderType orderType, decimal quantity, decimal? price = null)
		{
			try
			{
				var pair = Exchange.GetPair(symbol);
				if (pair == null)
				{
					return "No Symbol";
				}

				if (quantity < pair.MinOrderQuantity || quantity > pair.MaxOrderQuantity)
				{
					return "Invalid Order Quantity";
				}

				var flooredQuantity = Math.Floor(quantity / pair.TickQuantity) * pair.TickQuantity;

				var time = RicherTime;
				switch (orderType)
				{
					case OrderType.Limit:
						{
							if (price == null)
							{
								return "Limit order needs price";
							}

							var roundedPrice = Math.Round(price.Value / pair.TickPrice) * pair.TickPrice;

							switch (orderSide)
							{
								case OrderSide.Buy:
									{
										if (player.Wallet.KrwQuantity < roundedPrice * flooredQuantity)
										{
											return "Require KRW";
										}

										if (roundedPrice < pair.OrderBook.TopSellPrice) // Maker
										{
											var amount = roundedPrice * flooredQuantity;
											var maker = player;
											maker.Income(pair.QuoteAsset, -amount);
											pair.AddOrder(time, player.Id, orderSide, roundedPrice, flooredQuantity);
										}
										else // Taker
										{
											var removeOpenOrders = new List<RicherOpenOrder>();
											var remainQuantity = flooredQuantity;
											for (var i = 0; i < pair.SellOrders.Count; i++)
											{
												var sellOrder = pair.SellOrders[i];
												if (sellOrder.Price > roundedPrice)
												{
													break;
												}

												if (sellOrder.Remained > remainQuantity) // 체결 마지막 호가
												{
													var transactionQuantity = remainQuantity;
													var amount = sellOrder.Price * transactionQuantity;

													var maker = GetPlayer(sellOrder.MakerId);
													maker.Income(pair.QuoteAsset, amount);
													maker.Income(pair.QuoteAsset, -amount * MakerFeeRate);
													var taker = player;
													taker.Income(pair.BaseAsset, transactionQuantity);
													taker.Income(pair.QuoteAsset, -amount);
													taker.Income(pair.QuoteAsset, -amount * TakerFeeRate);

													pair.AddTransaction(time, sellOrder.Price, transactionQuantity, sellOrder.MakerId, taker.Id, true);
													sellOrder.Filled += transactionQuantity;
													pair.Price = sellOrder.Price;
													remainQuantity = 0;
													break;
												}
												else
												{
													var transactionQuantity = sellOrder.Remained;
													var amount = sellOrder.Price * transactionQuantity;

													var maker = GetPlayer(sellOrder.MakerId);
													maker.Income(pair.QuoteAsset, amount);
													maker.Income(pair.QuoteAsset, -amount * MakerFeeRate);
													var taker = player;
													taker.Income(pair.BaseAsset, transactionQuantity);
													taker.Income(pair.QuoteAsset, -amount);
													taker.Income(pair.QuoteAsset, -amount * TakerFeeRate);

													pair.AddTransaction(time, sellOrder.Price, transactionQuantity, sellOrder.MakerId, taker.Id, true);
													removeOpenOrders.Add(sellOrder);
													pair.Price = sellOrder.Price;
													remainQuantity -= transactionQuantity;
												}
											}

											foreach (RicherOpenOrder order in removeOpenOrders)
											{
												pair.Orders.Remove(order);
											}

											if (remainQuantity > 0) // 수량이 남아있으면 나머지는 Maker
											{
												var amount = roundedPrice * remainQuantity;
												var maker = player;
												maker.Income(pair.QuoteAsset, -amount);
												pair.AddOrder(time, player.Id, orderSide, roundedPrice, remainQuantity);
											}
										}
									}
									break;

								case OrderSide.Sell:
									{
										if (player.Wallet.GetAssetQuantity(pair.BaseAsset) < flooredQuantity)
										{
											return $"Require {pair.BaseAsset}";
										}

										if (roundedPrice > pair.OrderBook.TopBuyPrice) // Maker
										{
											var maker = player;
											maker.Income(pair.BaseAsset, -flooredQuantity);
											pair.AddOrder(time, player.Id, orderSide, roundedPrice, flooredQuantity);
										}
										else // Taker
										{
											var removeOpenOrders = new List<RicherOpenOrder>();
											var remainQuantity = flooredQuantity;
											for (var i = 0; i < pair.BuyOrders.Count; i++)
											{
												var buyOrder = pair.BuyOrders[i];
												if (buyOrder.Price < roundedPrice)
												{
													break;
												}

												if (buyOrder.Remained > remainQuantity) // 체결 마지막 호가
												{
													var transactionQuantity = remainQuantity;
													var amount = buyOrder.Price * transactionQuantity;

													var maker = GetPlayer(buyOrder.MakerId);
													maker.Income(pair.BaseAsset, transactionQuantity);
													maker.Income(pair.QuoteAsset, -amount * MakerFeeRate);
													var taker = player;
													taker.Income(pair.BaseAsset, -transactionQuantity);
													taker.Income(pair.QuoteAsset, amount);
													taker.Income(pair.QuoteAsset, -amount * TakerFeeRate);

													pair.AddTransaction(time, buyOrder.Price, transactionQuantity, buyOrder.MakerId, taker.Id, false);
													buyOrder.Filled += transactionQuantity;
													pair.Price = buyOrder.Price;
													remainQuantity = 0;
													break;
												}
												else
												{
													var transactionQuantity = buyOrder.Remained;
													var amount = buyOrder.Price * transactionQuantity;

													var maker = GetPlayer(buyOrder.MakerId);
													maker.Income(pair.BaseAsset, transactionQuantity);
													maker.Income(pair.QuoteAsset, -amount * MakerFeeRate);
													var taker = player;
													taker.Income(pair.BaseAsset, -transactionQuantity);
													taker.Income(pair.QuoteAsset, amount);
													taker.Income(pair.QuoteAsset, -amount * TakerFeeRate);

													pair.AddTransaction(time, buyOrder.Price, transactionQuantity, buyOrder.MakerId, taker.Id, false);
													removeOpenOrders.Add(buyOrder);
													pair.Price = buyOrder.Price;
													remainQuantity -= transactionQuantity;
												}
											}

											foreach (RicherOpenOrder order in removeOpenOrders)
											{
												pair.Orders.Remove(order);
											}

											if (remainQuantity > 0) // 수량이 남아있으면 나머지는 Maker
											{
												var maker = player;
												maker.Income(pair.BaseAsset, -remainQuantity);
												pair.AddOrder(time, player.Id, orderSide, roundedPrice, remainQuantity);
											}
										}
									}
									break;

								default:
									break;
							}
						}
						break;

					case OrderType.Market:
						{
							switch (orderSide)
							{
								case OrderSide.Buy:
									{
										if (player.Wallet.KrwQuantity * pair.MarketMaxRate < pair.OrderBook.TopSellPrice * flooredQuantity)
										{
											return "Require KRW";
										}

										var removeOpenOrders = new List<RicherOpenOrder>();
										var remainQuantity = flooredQuantity;
										for (var i = 0; i < pair.SellOrders.Count; i++)
										{
											var sellOrder = pair.SellOrders[i];
											if (sellOrder.Remained > remainQuantity) // 체결 마지막 호가
											{
												var transactionQuantity = remainQuantity;
												var amount = sellOrder.Price * transactionQuantity;

												var maker = GetPlayer(sellOrder.MakerId);
												maker.Income(pair.QuoteAsset, amount);
												maker.Income(pair.QuoteAsset, -amount * MakerFeeRate);
												var taker = player;
												taker.Income(pair.BaseAsset, transactionQuantity);
												taker.Income(pair.QuoteAsset, -amount);
												taker.Income(pair.QuoteAsset, -amount * TakerFeeRate);

												pair.AddTransaction(time, sellOrder.Price, transactionQuantity, sellOrder.MakerId, taker.Id, true);
												sellOrder.Filled += transactionQuantity;
												pair.Price = sellOrder.Price;
												break;
											}
											else
											{
												var transactionQuantity = sellOrder.Remained;
												var amount = sellOrder.Price * transactionQuantity;

												var maker = GetPlayer(sellOrder.MakerId);
												maker.Income(pair.QuoteAsset, amount);
												maker.Income(pair.QuoteAsset, -amount * MakerFeeRate);
												var taker = player;
												taker.Income(pair.BaseAsset, transactionQuantity);
												taker.Income(pair.QuoteAsset, -amount);
												taker.Income(pair.QuoteAsset, -amount * TakerFeeRate);

												pair.AddTransaction(time, sellOrder.Price, transactionQuantity, sellOrder.MakerId, taker.Id, true);
												removeOpenOrders.Add(sellOrder);
												pair.Price = sellOrder.Price;
												remainQuantity -= transactionQuantity;
											}
										}

										foreach (RicherOpenOrder order in removeOpenOrders)
										{
											pair.Orders.Remove(order);
										}
									}
									break;

								case OrderSide.Sell:
									{
										if (player.Wallet.GetAssetQuantity(pair.BaseAsset) < flooredQuantity)
										{
											return $"Require {pair.BaseAsset}";
										}

										var removeOpenOrders = new List<RicherOpenOrder>();
										var remainQuantity = flooredQuantity;
										for (var i = 0; i < pair.BuyOrders.Count; i++)
										{
											var buyOrder = pair.BuyOrders[i];
											if (buyOrder.Remained > remainQuantity) // 체결 마지막 호가
											{
												var transactionQuantity = remainQuantity;
												var amount = buyOrder.Price * transactionQuantity;

												var maker = GetPlayer(buyOrder.MakerId);
												maker.Income(pair.BaseAsset, transactionQuantity);
												maker.Income(pair.QuoteAsset, -amount * MakerFeeRate);
												var taker = player;
												taker.Income(pair.BaseAsset, -transactionQuantity);
												taker.Income(pair.QuoteAsset, amount);
												taker.Income(pair.QuoteAsset, -amount * TakerFeeRate);

												pair.AddTransaction(time, buyOrder.Price, transactionQuantity, buyOrder.MakerId, taker.Id, false);
												buyOrder.Filled += transactionQuantity;
												pair.Price = buyOrder.Price;
												break;
											}
											else
											{
												var transactionQuantity = buyOrder.Remained;
												var amount = buyOrder.Price * transactionQuantity;

												var maker = GetPlayer(buyOrder.MakerId);
												maker.Income(pair.BaseAsset, transactionQuantity);
												maker.Income(pair.QuoteAsset, -amount * MakerFeeRate);
												var taker = player;
												taker.Income(pair.BaseAsset, -transactionQuantity);
												taker.Income(pair.QuoteAsset, amount);
												taker.Income(pair.QuoteAsset, -amount * TakerFeeRate);

												pair.AddTransaction(time, buyOrder.Price, transactionQuantity, buyOrder.MakerId, taker.Id, false);
												removeOpenOrders.Add(buyOrder);
												pair.Price = buyOrder.Price;
												remainQuantity -= transactionQuantity;
											}
										}

										foreach (RicherOpenOrder order in removeOpenOrders)
										{
											pair.Orders.Remove(order);
										}
									}
									break;

								default:
									break;
							}
						}
						break;

					default:
						break;
				}

				return string.Empty;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public static string CancelOrder(RicherPlayer player, string symbol, RicherOpenOrder order)
		{
			try
			{
				var pair = Exchange.GetPair(symbol);
				if (pair == null)
				{
					return "No Symbol";
				}

				var openOrder = pair.Orders.Find(x => x != null && x.Time.Equals(order.Time) && x.MakerId.Equals(order.MakerId));
				if (openOrder == null)
				{
					return "No Open Order";
				}

				var maker = player;
				switch (openOrder.OrderSide)
				{
					case OrderSide.Buy:
						{
							maker.Income(pair.QuoteAsset, openOrder.RemainedAmount);
							pair.RemoveOrder(openOrder);
						}
						break;

					case OrderSide.Sell:
						{
							maker.Income(pair.BaseAsset, openOrder.Remained);
							pair.RemoveOrder(openOrder);
						}
						break;

					default:
						break;
				}

				return string.Empty;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public static string PlaceBestMakerOrder(RicherPlayer player, string symbol, OrderSide orderSide, decimal quantity)
		{
			var pair = Exchange.GetPair(symbol);
			if (pair == null)
			{
				return "No Symbol";
			}

			var price = orderSide == OrderSide.Buy ? pair.OrderBook.TopBuyPrice : pair.OrderBook.TopSellPrice;

			return PlaceOrder(player, symbol, orderSide, OrderType.Limit, quantity, price);
		}

		public static string PlaceChaseOrder(RicherPlayer player, string symbol, OrderSide orderSide, decimal quantity)
		{
			var pair = Exchange.GetPair(symbol);
			if (pair == null)
			{
				return "No Symbol";
			}

			var price = orderSide == OrderSide.Buy ? pair.OrderBook.TopBuyPrice : pair.OrderBook.TopSellPrice;

			return PlaceOrder(player, symbol, orderSide, OrderType.Limit, quantity, price);
		}
		#endregion

		#region AI
		public static string CreateAi(RicherAiType type, string symbol)
		{
			try
			{
				var pair = Exchange.GetPair(symbol);
				if (pair == null)
				{
					return "No Symbol";
				}

				var latestAi = Ais.OrderByDescending(x => int.Parse(x.Id[3..])).FirstOrDefault();

				var latestNumber = latestAi != null ? int.Parse(latestAi.Id[3..]) : 0;
				var newNumber = latestNumber + 1;
				var newId = $"AI_{newNumber:D6}";

				int newNameNumber = latestAi != null ? int.Parse(latestAi.Name[3..]) : 0;
				string newName = $"AI#{newNameNumber + 1}";

				var ai = new RicherAi(newId, newName, type);
				ai.MonitorSymbols.Add(symbol);
				ai.MonitorPairs.Add(pair);

				switch (type)
				{
					// 마스터: pair 물량 분배, 마스터는 1명 고정, 마스터는 매수 금지
					case RicherAiType.Master:
						{
							ai.Income(pair.BaseAsset, pair.TotalCount);
						}
						break;

					// 고래: pair 시총의 15%의 자금을 가지고 시작, 고래는 5명 고정
					case RicherAiType.Whale:
						{
							ai.WhaleMode = RicherWhaleMode.Buying;
							var initKrwQuantity = pair.MarketCap * 0.15m;
							ai.Income(pair.QuoteAsset, initKrwQuantity);
						}
						break;

					// 일반AI: pair 시총의 0.001% ~ 1%의 자금을 가지고 시작, 일반AI는 500명
					case RicherAiType.Commoner:
						{
							var initKrwQuantity = pair.MarketCap * r.NextNd(0.00001m, 0.01m);
							ai.Income(pair.QuoteAsset, initKrwQuantity);
						}
						break;

					default:
						break;
				}

				Ais.Add(ai);

				return string.Empty;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public static void ClearAi()
		{
			Ais.Clear();
		}

		/// <summary>
		/// 1초에 1번 이 메서드 실행
		/// freqTimes: 1초 실행 기준으로 1.0, 500ms 실행 기준으로 0.5, 더 빈번하게 작동시키려면 임의로 높혀도 됨
		/// </summary>
		public static void ProcessAi(double freqTimes = 1.0)
		{
			foreach (var ai in Ais)
			{
				switch (ai.Type)
				{
					case RicherAiType.Master:
						{
							foreach (var pair in ai.MonitorPairs)
							{
								//if (r.NextDouble() < 0.5 * freqTimes)
								{
									var orderCount = r.Next(1, 21);
									for (int i = 0; i < orderCount; i++)
									{
										// 총 수량의 0.0001% ~ 0.001% Sell
										var sellQuantity = Math.Min(ai.GetAssetQuantity(pair.BaseAsset), pair.TotalCount * r.NextDecimal(0.00001m, 0.0001m));
										var price = pair.Price * r.NextNd(0.98m, 1.02m);
										PlaceOrder(ai, pair.Symbol, OrderSide.Sell, OrderType.Limit, sellQuantity, price);
									}
								}

								// BestMaker 
								//if (r.NextDouble() < 0.15 * freqTimes)
								//{
								//	PlaceBestMakerOrder(ai, pair.Symbol, OrderSide.Sell, sellQuantity);
								//}
								// Market
								//if (r.NextDouble() < 0.12 * freqTimes)
								//{
								//	PlaceOrder(ai, pair.Symbol, OrderSide.Sell, OrderType.Market, sellQuantity);
								//}
							}
						}
						break;

					case RicherAiType.Whale:
						{
							foreach (var pair in ai.MonitorPairs)
							{
								// 매집상태에서 20초에 1번 이전 1분간 1 ~ 5% 이상 하락 시 매수가능의 2% ~ 3% limit buy
								if (ai.WhaleMode == RicherWhaleMode.Buying &&
									r.NextDouble() < 0.05 * freqTimes &&
									pair.Chart.M1.Count > 0 &&
									pair.Chart.M1[^1].Change <= (decimal)r.NextDouble(-1, -5))
								{
									var price = pair.Price * r.NextNd(0.99m, 1.01m); // 현재가의 -1% ~ +1% 호가
									var quantity = ai.GetAvailableBuyQuantity(pair) * r.NextDecimal(0.02m, 0.03m);
									PlaceOrder(ai, pair.Symbol, OrderSide.Buy, OrderType.Limit, quantity, price);
								}
								// Forcing상태에서 8초에 1번 자산의 1% ~ 3% market buy
								else if (ai.WhaleMode == RicherWhaleMode.Forcing &&
									r.NextDouble() < 0.125 * freqTimes)
								{
									var buyQuantity = ai.GetAvailableBuyQuantity(pair) * r.NextDecimal(0.01m, 0.03m);
									PlaceOrder(ai, pair.Symbol, OrderSide.Buy, OrderType.Market, buyQuantity);
								}
								// 매도상태에서 5초에 1번 매도가능의 5% ~ 10% market sell
								else if (ai.WhaleMode == RicherWhaleMode.Selling &&
									r.NextDouble() < 0.2 * freqTimes)
								{
									var sellQuantity = ai.GetAssetQuantity(pair.BaseAsset) * r.NextDecimal(0.05m, 0.1m);
									PlaceOrder(ai, pair.Symbol, OrderSide.Sell, OrderType.Market, sellQuantity);
								}
								// 매집상태에서 보유량이 자산대비 50% 이상이면 Forcing
								else if (ai.WhaleMode == RicherWhaleMode.Buying &&
									ai.GetHoldingRatio(pair) >= 0.5m)
								{
									ai.WhaleMode = RicherWhaleMode.Forcing;
								}
								// Forcing 상태에서 보유량이 자산대비 80% 이상이면 매도모드
								else if (ai.WhaleMode == RicherWhaleMode.Forcing &&
									ai.GetHoldingRatio(pair) >= 0.8m)
								{
									ai.WhaleMode = RicherWhaleMode.Selling;
								}
								// 매도상태에서 보유량이 자산대비 10% 이하면 매집모드
								else if (ai.WhaleMode == RicherWhaleMode.Selling &&
									ai.GetHoldingRatio(pair) <= 0.1m)
								{
									ai.WhaleMode = RicherWhaleMode.Buying;
								}
							}
						}
						break;

					case RicherAiType.Commoner:
						{
							foreach (var pair in ai.MonitorPairs)
							{
								var openOrders = pair.GetOpenOrders(ai.Id);

								if(openOrders.Count > 0)
								{
									// 33초에 1번 취소
									if (r.NextDouble() < 0.03 * freqTimes)
									{
										CancelOrder(ai, pair.Symbol, openOrders[0]);
									}
								}

								// 20초에 1번 매매
								if (r.NextDouble() < 0.05 * freqTimes)
								{
									switch (r.Next(2))
									{
										case 0:
											{
												var price = pair.Price * r.NextNd(0.98m, 1.02m); // 현재가의 -2% ~ +2% 호가
												var quantity = ai.GetAvailableBuyQuantity(pair) * r.NextDecimal(1.0m);
												PlaceOrder(ai, pair.Symbol, OrderSide.Buy, OrderType.Limit, quantity, price);

												//switch (r.Next(5))
												//{
												//	//case 0:
												//	//case 2:
												//	//case 3:
												//	//case 4:
												//	//	{
												//	//		var quantity = ai.GetAvailableBuyQuantity(pair) * pair.MarketMaxRate * 0.5m;
												//	//		PlaceOrder(ai, pair.Symbol, OrderSide.Buy, OrderType.Market, quantity);
												//	//	}
												//	//	break;

												//	case 0:
												//	case 2:
												//	case 3:
												//	case 4:
												//	case 1:
												//		{
															
												//		}
												//		break;
												//}
											}
											break;

										case 1:
											{
												var price = pair.Price * r.NextNd(0.98m, 1.02m); // 현재가의 -2% ~ +2% 호가
												var quantity = ai.GetAssetQuantity(pair.BaseAsset) * r.NextDecimal(1.0m);
												PlaceOrder(ai, pair.Symbol, OrderSide.Sell, OrderType.Limit, quantity, price);

												//switch (r.Next(5))
												//{
												//	case 0:
												//	case 2:
												//	case 3:
												//	case 4:
												//		{
												//			var quantity = ai.GetAssetQuantity(pair.BaseAsset) * 0.5m;
												//			PlaceOrder(ai, pair.Symbol, OrderSide.Sell, OrderType.Market, quantity);
												//		}
												//		break;

												//	case 1:
												//		{
															
												//		}
												//		break;
												//}
											}
											break;
									}
								}
							}
						}
						break;

					default:
						break;
				}
			}
		}

		public static void ProcessAiv2(double freqTimes = 1.0)
		{
			foreach (var ai in Ais)
			{
				switch (ai.Type)
				{
					case RicherAiType.Master:
						{
							foreach (var pair in ai.MonitorPairs)
							{
								var openOrders = pair.GetOpenOrders(ai.Id);

								if (openOrders.Count > 0)
								{
									CancelOrder(ai, pair.Symbol, openOrders[0]);
								}

								var orderCount = r.Next(1, 30);
								for (int i = 0; i < orderCount; i++)
								{
									// 총 수량의 0.0001% ~ 0.001% Sell
									var price = pair.Price * r.NextNd(0.9995m, 1.001m);
									var quantity = pair.TotalCount * r.NextDecimal(0.00001m, 0.0001m);
									var sellQuantity = Math.Min(ai.GetAssetQuantity(pair.BaseAsset), quantity);
									PlaceOrder(ai, pair.Symbol, OrderSide.Sell, OrderType.Limit, sellQuantity, price);
								}

								// Test
								//r.Seed = (r.Seed + 13) % 1337;

								ai.Income(pair.BaseAsset, r.Next(10, 2500));
							}
						}
						break;

					case RicherAiType.Whale:
					case RicherAiType.Commoner:
						{
							foreach (var pair in ai.MonitorPairs)
							{
								var openOrders = pair.GetOpenOrders(ai.Id);

								if (openOrders.Count > 0)
								{
									// 매매 취소
									if (r.NextDouble() < 0.08 * freqTimes)
									{
										CancelOrder(ai, pair.Symbol, openOrders[0]);
									}
								}

								// 매매
								if (r.NextDouble() < 0.12 * freqTimes)
								{
									switch (r.Next(2))
									{
										case 0:
											{
												var price = pair.Price * r.NextNd(0.995m, 1.00275m);
												var quantity = ai.GetAvailableBuyQuantity(pair) * r.NextNd(0m, ai.Type == RicherAiType.Whale ? 0.02m : 1.0m);
												PlaceOrder(ai, pair.Symbol, OrderSide.Buy, OrderType.Limit, quantity, price);
											}
											break;

										case 1:
											{
												var price = pair.Price * r.NextNd(0.99725m, 1.005m);
												var quantity = ai.GetAssetQuantity(pair.BaseAsset) * r.NextNd(0m, ai.Type == RicherAiType.Whale ? 0.02m : 1.0m);
												PlaceOrder(ai, pair.Symbol, OrderSide.Sell, OrderType.Limit, quantity, price);
											}
											break;
									}
								}

								ai.Income(pair.QuoteAsset, 400_000m);
							}
						}
						break;

					default:
						break;
				}
			}

		}
		#endregion

	}
}
