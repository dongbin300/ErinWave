using System.Text;

namespace ErinWave.HelloAkiba.Models
{
	/// <summary>
	/// - kikai v1: t1 85%, t2 15%
	///	- v2: t1 70%, t2 30%
	///	- v3: t1 50%, t2 50%
	///	- v4: t1 30%, t2 65%, t3 5%
	///	- v5: t1 15%, t2 70%, t3 15%
	///	- v6: t2 65%, t3 30%, t4 5%
	///	- v7: t2 35%, t3 50%, t4 15%
	/// </summary>
	public class HaGenerator
	{
		public string Name { get; set; } = string.Empty;
		public int Version { get; set; } = 1;
		public decimal Cost => 5m;

		public string Generate(int count)
		{
			var builder = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				builder.AppendLine(Generate());
			}
			return builder.ToString().TrimEnd();
		}

		public string Generate()
		{
			if (HaSettings.Kane < Cost)
			{
				return "kane tarinai";
			}

			var type = Name switch
			{
				"kikai_1" => HaSettings.Random.Next(2) == 0 ? 1 : 2,
				_ => 0
			};

			var num = HaSettings.Random.Next(100);
			var tier = Version switch
			{
				1 => num < 85 ? 1 : 2,
				2 => num < 70 ? 1 : 2,
				3 => num < 50 ? 1 : 2,
				4 => num < 30 ? 1 : (num < 95 ? 2 : 3),
				5 => num < 15 ? 1 : (num < 85 ? 2 : 3),
				6 => num < 65 ? 2 : (num < 95 ? 3 : 4),
				7 => num < 35 ? 2 : (num < 85 ? 3 : 4),
				_ => 0
			};

			var result = HaSettings.GetItem(type, tier);

			if (result.Contains("teniireta"))
			{
				HaSettings.Kane -= Cost;
			}

			return result;
		}

		public string Upgrade()
		{
			var upgradeCost = Version switch
			{
				1 => 200m,
				2 => 350m,
				3 => 600m,
				4 => 900m,
				5 => 1500m,
				6 => 2100m,
				_ => 0
			};

			if (Version >= 7)
			{
				return "mou saikou shiteiru";
			}

			if (HaSettings.Kane < upgradeCost)
			{
				return $"kane tarinai ({upgradeCost:#,###})";
			}

			HaSettings.Kane -= upgradeCost;
			Version++;

			return $"saikou shita! (kikai v{Version})";
		}
	}
}
