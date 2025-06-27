using ErinWave.Richer.Enums;
using ErinWave.Richer.Models.Exchanges;
using ErinWave.Richer.Util;

using System.Windows;
using System.Windows.Controls;

namespace ErinWave.Richer
{
	/// <summary>
	/// SimpleMainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class SimpleMainWindow : Window
	{
		System.Timers.Timer timer;

		public SimpleMainWindow()
		{
			InitializeComponent();

			timer = new System.Timers.Timer(990);
			timer.Elapsed += Timer_Elapsed;
			timer.Start();

			RM.Init();
			RM.Load();
			RM.InitAfterLoad();

			RM.CreateAi(RicherAiType.Master, "TRCKRW");

			for (var i = 0; i < 10; i++)
			{
				RM.CreateAi(RicherAiType.Whale, "TRCKRW");
			}

			for (var i = 0; i < 100; i++)
			{
				RM.CreateAi(RicherAiType.Commoner, "TRCKRW");
			}
		}

		private void DrawOrderBook()
		{
			OrderBookGrid.Children.Clear();

			var orderBook = RM.Exchange.Pairs[0].OrderBook;
			for (int i = Math.Min(9, orderBook.SellTicks.Count - 1); i >= 0; i--)
			{
				var tick = orderBook.SellTicks[i];
				var priceTextBlock = new TextBlock()
				{
					Text = tick.Price.ToString()
				};
				var quantityTextBlock = new TextBlock()
				{
					Text = tick.Quantity.ToString()
				};
				OrderBookGrid.Children.Add(priceTextBlock);
				OrderBookGrid.Children.Add(quantityTextBlock);
			}

			var currentPriceTextBlock = new TextBlock()
			{
				Text = RM.Exchange.Pairs[0].Price.ToString()
			};
			OrderBookGrid.Children.Add(currentPriceTextBlock);
			var emptyTextBlock = new TextBlock();
			OrderBookGrid.Children.Add(emptyTextBlock);


			for (int i = 0; i < Math.Min(10, orderBook.BuyTicks.Count); i++)
			{
				var tick = orderBook.BuyTicks[i];
				var priceTextBlock = new TextBlock()
				{
					Text = tick.Price.ToString()
				};
				var quantityTextBlock = new TextBlock()
				{
					Text = tick.Quantity.ToString()
				};
				OrderBookGrid.Children.Add(priceTextBlock);
				OrderBookGrid.Children.Add(quantityTextBlock);
			}
		}

		private void DrawTransaction()
		{
			TransactionGrid.Children.Clear();

			var transactions = RM.Exchange.Pairs[0].Transactions.OrderByDescending(x => x.Time).Take(15);
			foreach (var transaction in transactions)
			{
				var priceTextBlock = new TextBlock()
				{
					Text = transaction.Price.ToString()
				};
				var quantityTextBlock = new TextBlock()
				{
					Text = transaction.Quantity.ToString()
				};
				TransactionGrid.Children.Add(priceTextBlock);
				TransactionGrid.Children.Add(quantityTextBlock);
			}
		}

		private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				RM.RicherTime = RM.RicherTime.AddSeconds(1);
				RM.ProcessAi(0.2);

				DispatcherService.Invoke(() =>
				{
					DrawOrderBook();
					DrawTransaction();
					AssetDataGrid.ItemsSource = null;
					AssetDataGrid.ItemsSource = RM.Human.Wallet.Assets;
					OrderDataGrid.ItemsSource = null;
					OrderDataGrid.ItemsSource = RM.Exchange.Pairs[0].Orders.Where(x => x.MakerId.Equals(RM.Human.Id)).ToList();
				});
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void LimitButton_Click(object sender, RoutedEventArgs e)
		{
			var price = decimal.Parse(PriceTextBox.Text);
			var quantity = decimal.Parse(QuantityTextBox.Text);
			var result = RM.PlaceOrder(RM.Human, "TRCKRW", OrderSide.Buy, OrderType.Limit, quantity, price);

			if (!string.IsNullOrEmpty(result))
			{
				MessageBox.Show(result);
			}
		}

		private void MarketButton_Click(object sender, RoutedEventArgs e)
		{
			var quantity = decimal.Parse(QuantityTextBox.Text);
			var result = RM.PlaceOrder(RM.Human, "TRCKRW", OrderSide.Buy, OrderType.Market, quantity);

			if (!string.IsNullOrEmpty(result))
			{
				MessageBox.Show(result);
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			RM.Save();
		}

		private void OrderCancelButton_Click(object sender, RoutedEventArgs e)
		{
			if (e.Source is not Button button)
			{
				return;
			}

			if (button.DataContext is not RicherOpenOrder order)
			{
				return;
			}

			RM.CancelOrder(RM.Human, "TRCKRW", order);
		}
	}
}
