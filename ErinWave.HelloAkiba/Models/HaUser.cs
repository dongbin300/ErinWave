namespace ErinWave.HelloAkiba.Models
{
	public class HaUser(List<HaItem> requireItems, decimal rewardKane = 0m)
	{
		public List<HaItem> RequireItems { get; set; } = requireItems;
		public decimal RewardKane { get; set; } = rewardKane;

		public override string ToString()
		{
			var items = string.Join(", ", RequireItems.Select(i => i.Name));
			return $"req: {items}, kane: {RewardKane:#,###}";
		}
	}
}
