using ErinWave.Richer.AI;
using ErinWave.Richer.Enums;
using ErinWave.Richer.Json;
using ErinWave.Richer.Models;
using ErinWave.Richer.Models.Exchanges;

using Newtonsoft.Json;

using System.IO;

namespace ErinWave.Richer
{
	/// <summary>
	/// Richer Master
	/// TODO
	/// Add fee system
	/// </summary>
	public static class RM
	{
		public static readonly string ExchangeFilePath = "data/ex.json";
		public static readonly string HumanFilePath = "data/human.json";
		public static readonly string AisFilePath = "data/ai.json";
		public static readonly string BaseFilePath = "data/base.json";
		public static readonly decimal MakerFeeRate = 0.0001m; // 0.01%
		public static readonly decimal TakerFeeRate = 0.0005m; // 0.05%

		public static RicherExchange Exchange { get; set; } = default!;
		public static RicherHuman Human { get; set; } = default!;
		public static List<RicherAi> Ais { get; set; } = default!;
		public static DateTime RicherTime { get; set; } = default!;

		#region Base
		public static void Init()
		{
			if (!Directory.Exists("data"))
			{
				Directory.CreateDirectory("data");
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

		#region Human
		public static string HumanOrder(RicherHuman human, string symbol, OrderSide orderSide, OrderType orderType, decimal quantity, decimal? price = null)
		{
			try
			{
				var pair = Exchange.Pairs.Find(x => x.Symbol.Equals(symbol));
				if (pair == null)
				{
					return "No Symbol";
				}

				var time = RicherTime;
				switch (orderType)
				{
					case OrderType.Limit:
						{
							if (price == null)
							{
								return "Limit order needs price";
							}

							switch (orderSide)
							{
								case OrderSide.Buy:
									{
										if (human.Wallet.KrwQuantity < price * quantity)
										{
											return "Require KRW";
										}

										if (price < pair.OrderBook.TopSellPrice) // Maker
										{
											var amount = price.Value * quantity;
											var maker = human;
											maker.Income(pair.QuoteAsset, -amount);
											pair.AddOrder(time, human.Id, orderSide, price.Value, quantity);
										}
										else // Taker
										{
											var removeOpenOrders = new List<RicherOpenOrder>();
											var remainQuantity = quantity;
											for (var i = 0; i < pair.SellOrders.Count; i++)
											{
												var sellOrder = pair.SellOrders[i];
												if (sellOrder.Price > price)
												{
													break;
												}

												if (sellOrder.Remained > remainQuantity) // 체결 마지막 호가
												{
													var transactionQuantity = remainQuantity;
													var amount = sellOrder.Price * transactionQuantity;

													var maker = GetPlayer(sellOrder.MakerId);
													maker.Income(pair.QuoteAsset, amount);
													var taker = human;
													taker.Income(pair.BaseAsset, transactionQuantity);
													taker.Income(pair.QuoteAsset, -amount);

													pair.AddTransaction(time, sellOrder.Price, transactionQuantity, sellOrder.MakerId, taker.Id);
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
													var taker = human;
													taker.Income(pair.BaseAsset, transactionQuantity);
													taker.Income(pair.QuoteAsset, -amount);

													pair.AddTransaction(time, sellOrder.Price, transactionQuantity, sellOrder.MakerId, taker.Id);
													removeOpenOrders.Add(sellOrder);
													remainQuantity -= transactionQuantity;
												}
											}

											foreach (RicherOpenOrder order in removeOpenOrders)
											{
												pair.Orders.Remove(order);
											}

											if (remainQuantity > 0) // 수량이 남아있으면 나머지는 Maker
											{
												var amount = price.Value * remainQuantity;
												var maker = human;
												maker.Income(pair.QuoteAsset, -amount);
												pair.AddOrder(time, human.Id, orderSide, price.Value, remainQuantity);
											}
										}
									}
									break;

								case OrderSide.Sell:
									{
										if (human.Wallet.GetAssetQuantity(pair.BaseAsset) < quantity)
										{
											return $"Require {pair.BaseAsset}";
										}

										if (price > pair.OrderBook.TopBuyPrice) // Maker
										{
											var maker = human;
											maker.Income(pair.BaseAsset, -quantity);
											pair.AddOrder(time, human.Id, orderSide, price.Value, quantity);
										}
										else // Taker
										{
											var removeOpenOrders = new List<RicherOpenOrder>();
											var remainQuantity = quantity;
											for (var i = 0; i < pair.BuyOrders.Count; i++)
											{
												var buyOrder = pair.BuyOrders[i];
												if (buyOrder.Price < price)
												{
													break;
												}

												if (buyOrder.Remained > remainQuantity) // 체결 마지막 호가
												{
													var transactionQuantity = remainQuantity;
													var amount = buyOrder.Price * transactionQuantity;

													var maker = GetPlayer(buyOrder.MakerId);
													maker.Income(pair.BaseAsset, transactionQuantity);
													var taker = human;
													taker.Income(pair.BaseAsset, -transactionQuantity);
													taker.Income(pair.QuoteAsset, amount);

													pair.AddTransaction(time, buyOrder.Price, transactionQuantity, buyOrder.MakerId, taker.Id);
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
													var taker = human;
													taker.Income(pair.BaseAsset, -transactionQuantity);
													taker.Income(pair.QuoteAsset, amount);

													pair.AddTransaction(time, buyOrder.Price, transactionQuantity, buyOrder.MakerId, taker.Id);
													removeOpenOrders.Add(buyOrder);
													remainQuantity -= transactionQuantity;
												}
											}

											foreach (RicherOpenOrder order in removeOpenOrders)
											{
												pair.Orders.Remove(order);
											}

											if (remainQuantity > 0) // 수량이 남아있으면 나머지는 Maker
											{
												var maker = human;
												maker.Income(pair.BaseAsset, -remainQuantity);
												pair.AddOrder(time, human.Id, orderSide, price.Value, remainQuantity);
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
										if (human.Wallet.KrwQuantity * pair.MarketMaxRate < pair.OrderBook.TopSellPrice * quantity)
										{
											return "Require KRW";
										}

										var removeOpenOrders = new List<RicherOpenOrder>();
										var remainQuantity = quantity;
										for (var i = 0; i < pair.SellOrders.Count; i++)
										{
											var sellOrder = pair.SellOrders[i];
											if (sellOrder.Remained > remainQuantity) // 체결 마지막 호가
											{
												var transactionQuantity = remainQuantity;
												var amount = sellOrder.Price * transactionQuantity;

												var maker = GetPlayer(sellOrder.MakerId);
												maker.Income(pair.QuoteAsset, amount);
												var taker = human;
												taker.Income(pair.BaseAsset, transactionQuantity);
												taker.Income(pair.QuoteAsset, -amount);

												pair.AddTransaction(time, sellOrder.Price, transactionQuantity, sellOrder.MakerId, taker.Id);
												sellOrder.Filled += transactionQuantity;
												break;
											}
											else
											{
												var transactionQuantity = sellOrder.Remained;
												var amount = sellOrder.Price * transactionQuantity;

												var maker = GetPlayer(sellOrder.MakerId);
												maker.Income(pair.QuoteAsset, amount);
												var taker = human;
												taker.Income(pair.BaseAsset, transactionQuantity);
												taker.Income(pair.QuoteAsset, -amount);

												pair.AddTransaction(time, sellOrder.Price, transactionQuantity, sellOrder.MakerId, taker.Id);
												removeOpenOrders.Add(sellOrder);
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
										if (human.Wallet.GetAssetQuantity(pair.BaseAsset) < quantity)
										{
											return $"Require {pair.BaseAsset}";
										}

										var removeOpenOrders = new List<RicherOpenOrder>();
										var remainQuantity = quantity;
										for (var i = 0; i < pair.BuyOrders.Count; i++)
										{
											var buyOrder = pair.BuyOrders[i];
											if (buyOrder.Remained > remainQuantity) // 체결 마지막 호가
											{
												var transactionQuantity = remainQuantity;
												var amount = buyOrder.Price * transactionQuantity;

												var maker = GetPlayer(buyOrder.MakerId);
												maker.Income(pair.BaseAsset, transactionQuantity);
												var taker = human;
												taker.Income(pair.BaseAsset, -transactionQuantity);
												taker.Income(pair.QuoteAsset, amount);

												pair.AddTransaction(time, buyOrder.Price, transactionQuantity, buyOrder.MakerId, taker.Id);
												buyOrder.Filled += transactionQuantity;
												break;
											}
											else
											{
												var transactionQuantity = buyOrder.Remained;
												var amount = buyOrder.Price * transactionQuantity;

												var maker = GetPlayer(buyOrder.MakerId);
												maker.Income(pair.BaseAsset, transactionQuantity);
												var taker = human;
												taker.Income(pair.BaseAsset, -transactionQuantity);
												taker.Income(pair.QuoteAsset, amount);

												pair.AddTransaction(time, buyOrder.Price, transactionQuantity, buyOrder.MakerId, taker.Id);
												removeOpenOrders.Add(buyOrder);
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

		public static string HumanCancelOrder(RicherHuman human, string symbol, RicherOpenOrder order)
		{
			try
			{
				var pair = Exchange.Pairs.Find(x => x.Symbol.Equals(symbol));
				if (pair == null)
				{
					return "No Symbol";
				}

				var openOrder = pair.Orders.Find(x => x.Time.Equals(order.Time) && x.MakerId.Equals(order.MakerId));
				if (openOrder == null)
				{
					return "No Open Order";
				}

				var maker = human;
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
		#endregion

		#region AI
		public static string CreateAi(RicherAiType type, string symbol)
		{
			try
			{
				var pair = Exchange.Pairs.Find(x => x.Symbol.Equals(symbol));
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

				switch (type)
				{
					// 마스터: pair 물량 분배, 마스터는 1명 고정, 마스터는 매수 금지
					case RicherAiType.Master:
						{
							ai.Income(pair.BaseAsset, pair.TotalCount);
						}
						break;

					// 고래: pair 시총의 5%의 자금을 가지고 시작, 고래는 10명 고정
					case RicherAiType.Whale:
						{
							var initKrwQuantity = pair.MarketCap * 0.05m;
							ai.Income(pair.QuoteAsset, initKrwQuantity);
						}
						break;

						// 일반AI: pair 시총의 0.1% ~ 1%의 자금을 가지고 시작
					case RicherAiType.Commoner:
						{
							// 랜덤 모듈 구현해야지
							var initKrwQuantity = pair.MarketCap * 0.05m;
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
		#endregion

	}
}
