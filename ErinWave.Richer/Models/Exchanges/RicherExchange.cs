namespace ErinWave.Richer.Models.Exchanges
{
	public class RicherExchange
	{
		public List<RicherPair> Pairs { get; set; } = [];

		public RicherPair? GetPair(string symbol) => Pairs.Find(x => x.Symbol.Equals(symbol));
	}
}
