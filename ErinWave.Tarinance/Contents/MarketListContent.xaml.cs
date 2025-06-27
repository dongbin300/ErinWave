using ErinWave.Tarinance.Models;

using System.Globalization;

namespace ErinWave.Tarinance.Contents;

public partial class MarketListContent : ContentView
{
	public MarketListContent()
	{
		InitializeComponent();
	}

	public void Init(TarinanceCoin coin)
	{
        var price = decimal.Parse(coin.Price, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);
        var prevPrice = decimal.Parse(coin.Prev_price, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);
        var changeValue = Math.Abs(price - prevPrice);
        var change = Math.Round((price / prevPrice - 1) * 100, 2);
        var changeString = change >= 0 ? $"+{change}%" : $"{change}%";

        NameText.Text = coin.Name;
        PriceText.Text = price.ToString("#,###.#####");
        ChangePerText.Text = changeString;
        SymbolText.Text = coin.Symbol;
        ChangeValueText.Text = changeValue.ToString("#,###.#####");

        NameText.TextColor = Color.FromArgb("EEEEEE");
        PriceText.TextColor = change >= 0 ? Color.FromArgb("3BCF86") : Color.FromArgb("ED3161");
        ChangePerText.TextColor = change >= 0 ? Color.FromArgb("3BCF86") : Color.FromArgb("ED3161");
        ChangeValueText.TextColor = change >= 0 ? Color.FromArgb("3BCF86") : Color.FromArgb("ED3161");
        StatusBar.Fill = change >= 0 ? Color.FromArgb("3BCF86") : Color.FromArgb("ED3161");
    }
}