namespace ErinWave.Frame.Raylibs.Stats
{
	public class StatContainer
	{
		private readonly Dictionary<StatType, Stat> _stats = [];

		public void Add(StatType type, int max)
		{
			_stats[type] = new Stat(max);
		}

		public bool Has(StatType type)
		{
			return _stats.ContainsKey(type);
		}

		public Stat Get(StatType type)
		{
			return _stats[type];
		}
	}
}
