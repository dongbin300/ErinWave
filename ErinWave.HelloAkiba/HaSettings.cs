using ErinWave.HelloAkiba.Models;
using ErinWave.HelloAkiba.Serialization;

using Newtonsoft.Json;

using System.IO;
using System.Text;

namespace ErinWave.HelloAkiba
{
	public class HaSettings
	{
		public static readonly string SavePath = "data.json";
		public static decimal Kane { get; set; } = 100m;
		public static List<HaUser> Users { get; set; } = [];
		public static HaGenerator Generator1 { get; set; } = default!;
		public static List<HaItem> Items { get; set; } = [];
		public static SmartRandom Random { get; set; } = new SmartRandom(0x12345678);

		record SaveData(decimal Kane, List<HaUser> Users, HaGenerator Generator1, List<HaItem> Items);

		public static void Save()
		{
			var json = JsonConvert.SerializeObject(
				new SaveData(Kane, Users, Generator1, Items),
				Formatting.Indented, new JsonSerializerSettings { ContractResolver = new IgnoreReadOnlyResolver() });
			File.WriteAllText(SavePath, json);
		}

		public static void Load()
		{
			var jsonData = File.ReadAllText(SavePath);
			var data = JsonConvert.DeserializeObject<SaveData>(jsonData);

			if (data == null)
			{
				return;
			}

			Kane = data.Kane;
			Users = data.Users ?? [];
			Generator1 = data.Generator1;
			Items = data.Items ?? [];
		}

		public static string GetItem(int type, int tier, bool merge = false)
		{
			if (Items.Count >= 32)
			{
				return "kuukan tarinai";
			}

			var item = new HaItem(type, tier);
			Items.Add(item);

			return merge ? item.Name : $"{item.Name} teniireta";
		}

		public static string RemoveItem(int x)
		{
			if (x < 0 || x >= Items.Count)
			{
				return "nai";
			}

			Items.RemoveAt(x);

			return string.Empty;
		}

		public static string MergeItem(int x1, int x2)
		{
			if (x1 < 0 || x1 >= Items.Count || x2 < 0 || x2 >= Items.Count)
			{
				return "aitemu nai";
			}

			var item1 = Items[x1];
			var item2 = Items[x2];

			if (item1.Type != item2.Type || item1.Tier != item2.Tier)
			{
				return "taipuya tiaga chigau";
			}

			if (x1 > x2)
			{
				RemoveItem(x1);
				RemoveItem(x2);
			}
			else if (x2 > x1)
			{
				RemoveItem(x2);
				RemoveItem(x1);
			}
			else
			{
				return "onaji aitemu gouseidekinai";
			}

			var itemName = GetItem(item1.Type, item1.Tier + 1, true);

			return $"{itemName} gouseishita";
		}

		public static string AutoMerge(int type, int tier)
		{
			var builder = new StringBuilder();
			int mergeCount = 0;
			while (true)
			{
				var indices = Items
					.Select((item, idx) => new { item, idx })
					.Where(x => x.item.Type == type && x.item.Tier == tier)
					.Select(x => x.idx)
					.Take(2)
					.ToList();

				if (indices.Count < 2)
					break;

				var result = MergeItem(indices[1], indices[0]);
				mergeCount++;

				builder.AppendLine(result);

				if (!result.Contains("gouseishita"))
					break;
			}

			if (mergeCount == 0)
				return "gouseidekiru aitemu nai";
			else
				return builder.ToString().TrimEnd();
		}

		public static string AddUser(int userTier = 1)
		{
			if (Users.Count >= 5)
			{
				return "yuuzaa tarinai";
			}

			List<HaItem> requireItems = [];
			var requireItemCount = Random.Next(1, 4);
			for (int i = 0; i < requireItemCount; i++)
			{
				var type = Random.Next(1, 3);
				var tier = Random.Next(0 + userTier, 3 + userTier);
				var item = new HaItem(type, tier);
				requireItems.Add(item);
			}

			decimal rewardKane = 0m;
			foreach (var item in requireItems)
			{
				rewardKane += 5m * (decimal)Math.Pow(2.3, item.Tier - 1);
			}
			rewardKane *= 1.25m;
			rewardKane *=
				requireItemCount == 2 ? 1.2m :
				requireItemCount == 3 ? 1.5m :
				1.0m;
			rewardKane = Math.Round(rewardKane, 0);

			var user = new HaUser(requireItems, rewardKane);
			Users.Add(user);

			return "yuuzaa dekita";
		}

		public static string RemoveUser(int userIndex)
		{
			if (userIndex < 0 || userIndex >= Users.Count)
			{
				return "yuuzaa nai";
			}

			Users.RemoveAt(userIndex);
			return "yuuzaa kieta";
		}

		public static string GiveToUser(int userIndex)
		{
			if (userIndex < 0 || userIndex >= Users.Count)
			{
				return "yuuzaa nai";
			}

			var user = Users[userIndex];
			var requireItems = user.RequireItems;

			var tempItems = new List<HaItem>(Items);
			foreach (var req in requireItems)
			{
				var found = tempItems.FirstOrDefault(x => x.Type == req.Type && x.Tier == req.Tier);
				if (found == null)
				{
					return "aitemu tarinai";
				}
				tempItems.Remove(found); // 중복 방지
			}

			foreach (var req in requireItems)
			{
				var idx = Items.FindIndex(x => x.Type == req.Type && x.Tier == req.Tier);
				if (idx >= 0)
					Items.RemoveAt(idx);
			}

			Kane += user.RewardKane;
			Users.RemoveAt(userIndex);

			return $"kane {user.RewardKane:#,###} itadaki.";
		}
	}
}
