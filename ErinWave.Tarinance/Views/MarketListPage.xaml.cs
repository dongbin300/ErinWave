using ErinWave.Tarinance.Contents;

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

        MainLayout.Clear();
        foreach (var coin in nowPrice)
        {
            var content = new MarketListContent();
            content.Init(coin);

            MainLayout.Children.Add(content);
        }
    }
}