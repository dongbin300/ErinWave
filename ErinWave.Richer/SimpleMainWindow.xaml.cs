using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ErinWave.Richer.Models;
using ErinWave.Richer.Models.Exchanges;
using ErinWave.Richer.Enums;
using System.Diagnostics;
using System.Collections.ObjectModel;
using ErinWave.Richer.Util;

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

			//RM.Ais.Add(new AI.RicherAi()
			//{
			//	Id = "AI_000001",
			//	Name = "AI_1번",
			//	Wallet = new RicherWallet()
			//});

			//RM.Ais.Add(new AI.RicherAi()
			//{
			//	Id = "AI_000002",
			//	Name = "AI_2번",
			//	Wallet = new RicherWallet()
			//});

			//RM.Exchange.Pairs[0].Orders = [];
			//RM.Exchange.Pairs[0].Orders.Add(new RicherOrder(new DateTime(2000, 1, 1), "1", OrderType.Buy, 99.9m, 3.3m));
			//RM.Exchange.Pairs[0].Orders.Add(new RicherOrder(new DateTime(2000, 1, 1), "1", OrderType.Buy, 99.8m, 4.2m));
			//RM.Exchange.Pairs[0].Orders.Add(new RicherOrder(new DateTime(2000, 1, 1), "1", OrderType.Buy, 99.6m, 5.3m));
			//RM.Exchange.Pairs[0].Orders.Add(new RicherOrder(new DateTime(2000, 1, 1), "1", OrderType.Buy, 99.9m, 6.1m));

			//RM.Exchange.Pairs[0].Orders.Add(new RicherOrder(new DateTime(2000, 1, 1), "1", OrderType.Sell, 100.1m, 6.1m));
			//RM.Exchange.Pairs[0].Orders.Add(new RicherOrder(new DateTime(2000, 1, 1), "1", OrderType.Sell, 100.2m, 3.4m));
			//RM.Exchange.Pairs[0].Orders.Add(new RicherOrder(new DateTime(2000, 1, 1), "1", OrderType.Sell, 101.0m, 12.5m));
			//RM.Exchange.Pairs[0].Orders.Add(new RicherOrder(new DateTime(2000, 1, 1), "1", OrderType.Sell, 100.1m, 6.1m));

		}

		private void DrawOrderBook()
		{
			OrderBookGrid.Children.Clear();

			var orderBook = RM.Exchange.Pairs[0].OrderBook;
			for (int i = orderBook.SellTicks.Count - 1; i >= 0; i--)
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


			for (int i = 0; i < orderBook.BuyTicks.Count; i++)
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

		private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			RM.RicherTime = RM.RicherTime.AddSeconds(1);

			DispatcherService.Invoke(() =>
			{
				DrawOrderBook();
				AssetDataGrid.ItemsSource = null;
				AssetDataGrid.ItemsSource = RM.Human.Wallet.Assets;
				OrderDataGrid.ItemsSource = null;
				OrderDataGrid.ItemsSource = RM.Exchange.Pairs[0].Orders.Where(x => x.MakerId.Equals(RM.Human.Id)).ToList();
			});
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
