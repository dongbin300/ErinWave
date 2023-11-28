using System.Globalization;

namespace ErinWave.Tarinance.Views;

public partial class MarketListPage : ContentPage
{
    public MarketListPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        var nowPrice = TarinanceClient.GetNowPrice();

        foreach (var coin in nowPrice)
        {
            var price = decimal.Parse(coin.Price, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);
            var prevPrice = decimal.Parse(coin.Prev_price, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);
            var change = Math.Round((price / prevPrice - 1) * 100, 2);
            var changeString = change >= 0 ? $"+{change}%" : $"{change}%";

            Label label = new Label();
            label.Text = $"{coin.Name} {coin.Symbol} {price:#,###} {changeString}";

            MainLayout.Children.Add(label);
        }
    }
}