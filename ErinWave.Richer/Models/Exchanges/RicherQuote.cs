using ErinWave.Richer.Enums;

namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherQuote
	{
		public DateTime Time { get; set; }
		public decimal Open { get; set; }
		public decimal High { get; set; }
		public decimal Low { get; set; }
		public decimal Close { get; set; }
		public decimal Volume { get; set; }
		public CandlestickType CandlestickType => Open < Close ? CandlestickType.Bullish : Open > Close ? CandlestickType.Bearish : CandlestickType.Doji;
        public decimal Change => (Close - Open) / Open * 100;
		public decimal BodyLength => Math.Abs(Change);

		public RicherQuote()
        {
            
        }

        public RicherQuote(DateTime time, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            Time = time;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }
    }
}
