using Newtonsoft.Json;

namespace ErinWave.M5Server
{
	public class M5Field
	{
		[JsonIgnore]
		private Random random = new ();

		/// <summary>
		/// 보스 카드
		/// </summary>
		public string Boss { get; set; } = string.Empty;

		/// <summary>
		/// dg
		/// 2101 human-1
		/// 2102 human-2
		/// ...
		/// 2113 human-13
		/// 2114 monster-1
		/// ...
		/// 2125 monster-12
		/// 2126 obs-1
		/// ...
		/// 2140 obs-15
		/// 
		/// crisis
		/// 2201 event-1
		/// 2202 event-2
		/// 2203 event-3
		/// 2204 event-4
		/// 2205 mid-1
		/// 2206 mid-2
		/// 2207 mid-3
		/// 2208 mid-4
		/// 2209 mid-5
		/// 
		/// 던전 카드
		/// </summary>
		public List<string> Dungeons { get; set; } = [];

		/// <summary>
		/// 현재 던전 카드
		/// </summary>
		public string CurrentDungeon { get; set; } = string.Empty;

		/// <summary>
		/// 현재 올려져있는 아군카드
		/// </summary>
		public List<string> CurrentCards { get; set; } = [];

		/// <summary>
		/// 사용되어서 버려진 카드
		/// </summary>
		public List<string> Used { get; set; } = [];

		/// <summary>
		/// 남은 시간(초)
		/// </summary>
		public int RemainSeconds { get; set; } = -1;

		/// <summary>
		/// 게임 중인지
		/// </summary>
		public bool IsPlaying { get; set; } = false;
		
		/// <summary>
		/// 게임을 초기화하고 시작합니다.
		/// </summary>
		/// <param name="stage"></param>
		public void SetGame(int stage)
		{
			ClearDungeon();
			Boss = stage >= 5 ? "5" : stage.ToString();
			SetDungeon(15 + 5 * stage);
			ShuffleDungeons();
			CurrentDungeon = string.Empty;
			CurrentCards = [];
			Used = [];

			RemainSeconds = 300;
			IsPlaying = true;
		}

		/// <summary>
		/// Fisher-Yates Shuffle
		/// </summary>
		public void ShuffleDungeons()
		{
			var n = Dungeons.Count;
			while (n > 1)
			{
				n--;
				var k = random.Next(n + 1);
				(Dungeons[n], Dungeons[k]) = (Dungeons[k], Dungeons[n]);
			}
		}

		/// <summary>
		/// 던전 덱을 초기화합니다.
		/// </summary>
		public void ClearDungeon()
		{
			Dungeons.Clear();
		}

		public bool IsCurrentHuman() => int.Parse(CurrentDungeon) >= 2101 && int.Parse(CurrentDungeon) <= 2113;
		public bool IsCurrentMonster() => int.Parse(CurrentDungeon) >= 2114 && int.Parse(CurrentDungeon) <= 2125;
		public bool IsCurrentObstacle() => int.Parse(CurrentDungeon) >= 2126 && int.Parse(CurrentDungeon) <= 2140;
		public bool IsCurrentEvent() => int.Parse(CurrentDungeon) >= 2201 && int.Parse(CurrentDungeon) <= 2204;

		/// <summary>
		/// 던전 클리어
		/// </summary>
		public void WinDungeon()
		{
			Used.Add(CurrentDungeon);
			Used.AddRange(CurrentCards);
			CurrentCards = [];
			CurrentDungeon = string.Empty;

			if (Dungeons.Count == 0) // 보스 스테이지
			{
				CurrentDungeon = Boss;
			}
		}

		/// <summary>
		/// 보스 클리어
		/// </summary>
		public void WinBoss()
		{
			Used.Add(CurrentDungeon);
			Used.AddRange(CurrentCards);
			CurrentCards = [];
			CurrentDungeon = string.Empty;
		}

		/// <summary>
		/// 던전 클리어 후 다음 던전 카드 오픈
		/// </summary>
		public void OpenDungeon()
		{
			// 던전 카드가 이미 오픈되어 있으면 스킵
			if (CurrentDungeon != string.Empty)
			{
				return;
			}

			if (Dungeons.Count > 0) // 다음 던전
			{
				CurrentDungeon = Dungeons[0];
				Dungeons.RemoveAt(0);
			}
		}

		/// <summary>
		/// 현재 던전을 클리어 할 수 있는지 확인
		/// </summary>
		public bool IsWinDungeon()
		{
			// 빨,노,초,파,보 순
			int[] t = CurrentDungeon switch
			{
				"1" => [2, 0, 2, 0, 3],
				"2" => [0, 3, 0, 7, 0],
				"3" => [4, 3, 0, 0, 3],
				"4" => [1, 1, 5, 0, 4],
				"5" => [3, 3, 3, 3, 0],
				"2101" => [1, 1, 1, 0, 0],
				"2102" => [0, 0, 1, 1, 1],
				"2103" => [2, 1, 0, 0, 0],
				"2104" => [0, 1, 2, 0, 0],
				"2105" => [0, 0, 0, 2, 1],
				"2106" => [0, 0, 0, 1, 2],
				"2107" => [1, 1, 0, 1, 0],
				"2108" => [2, 0, 0, 1, 0],
				"2109" => [0, 0, 1, 1, 0],
				"2110" => [0, 1, 1, 0, 0],
				"2111" => [2, 0, 1, 0, 0],
				"2112" => [0, 2, 1, 0, 0],
				"2113" => [0, 1, 1, 0, 1],
				"2114" => [0, 2, 0, 1, 0],
				"2115" => [0, 0, 2, 0, 1],
				"2116" => [1, 0, 0, 0, 1],
				"2117" => [1, 0, 1, 0, 0],
				"2118" => [3, 0, 0, 0, 0],
				"2119" => [0, 3, 0, 0, 0],
				"2120" => [0, 1, 0, 1, 0],
				"2121" => [2, 0, 0, 0, 0],
				"2122" => [0, 1, 0, 1, 1],
				"2123" => [1, 0, 2, 0, 0],
				"2124" => [0, 0, 1, 0, 1],
				"2125" => [0, 1, 0, 0, 1],
				"2126" => [0, 0, 0, 2, 0],
				"2127" => [0, 1, 0, 2, 0],
				"2128" => [0, 0, 0, 3, 0],
				"2129" => [0, 0, 0, 0, 3],
				"2130" => [0, 0, 0, 1, 1],
				"2131" => [0, 0, 0, 0, 2],
				"2132" => [1, 0, 0, 1, 1],
				"2133" => [0, 0, 2, 1, 0],
				"2134" => [0, 1, 0, 0, 2],
				"2135" => [0, 0, 1, 0, 2],
				"2136" => [1, 2, 0, 0, 0],
				"2137" => [1, 0, 1, 0, 1],
				"2138" => [1, 0, 0, 2, 0],
				"2139" => [0, 2, 0, 0, 1],
				"2140" => [1, 1, 0, 0, 1],
				"2205" => [3, 0, 0, 0, 3],
				"2206" => [2, 2, 2, 0, 0],
				"2207" => [0, 3, 3, 0, 0],
				"2208" => [1, 1, 1, 1, 1],
				"2209" => [0, 0, 0, 4, 2],

				"2201" => [9, 9, 9, 9, 9],
				"2202" => [9, 9, 9, 9, 9],
				"2203" => [9, 9, 9, 9, 9],
				"2204" => [9, 9, 9, 9, 9],

				_ => [0, 0, 0, 0, 0]
			};

			var wildcardCount = 0;
			foreach (var card in CurrentCards)
			{
				switch (card)
				{
					case "1101": t[0]--; break;
					case "1102": t[0]--; t[0]--; break;
					case "1103": t[0]--; t[1]--; break;
					case "1104": t[0]--; t[2]--; break;
					case "1105": t[0]--; t[3]--; break;
					case "1106": t[0]--; t[4]--; break;
					case "1107": t[1]--; break;
					case "1108": t[1]--; t[1]--; break;
					case "1109": t[2]--; break;
					case "1110": t[2]--; t[2]--; break;
					case "1111": t[3]--; break;
					case "1112": t[3]--; t[3]--; break;
					case "1113": t[4]--; break;
					case "1114": t[4]--; t[4]--; break;

					case "1204": t[0]--; t[1]--; t[2]--; t[3]--; t[4]--; break;
					case "1206": wildcardCount++; break;

					default: break;
				}
			}

			// -인 아이템은 0으로 고정
			for (int i = 0; i < t.Length; i++)
			{
				if (t[i] < 0)
				{
					t[i] = 0;
				}
			}

			return (t.Sum() - wildcardCount) <= 0;
		}

		/// <summary>
		/// 던전 카드를 설정합니다.
		/// 30장일 경우 위기 카드 4장 + 던전 카드 26장입니다.
		/// </summary>
		/// <param name="count"></param>
		public void SetDungeon(int count)
		{
			var dungeonDeck = new List<string>() { "2101", "2102", "2103","2104","2105", "2106", "2107", "2108", "2109", "2110", "2111", "2112", "2113", "2114", "2115", "2116", "2117", "2118", "2119", "2120", "2121", "2122", "2123", "2124", "2125", "2126", "2127", "2128", "2129", "2130", "2131", "2132", "2133", "2134", "2135", "2136", "2137", "2138", "2139", "2140", "2101", "2102", "2103", "2104", "2105", "2106", "2107", "2108", "2109", "2110", "2111", "2112", "2113", "2114", "2115", "2116", "2117", "2118", "2119", "2120", "2121", "2122", "2123", "2124", "2125", "2126", "2127", "2128", "2129", "2130", "2131", "2132", "2133", "2134", "2135", "2136", "2137", "2138", "2139", "2140" };
			var crisisDeck = new List<string>() { "2201", "2202", "2203", "2204", "2205", "2206", "2207", "2208", "2209" };
			var dungeonCount = count - 4;
			var crisisCount = 4;

			for (int i = 0; i < dungeonDeck.Count; i++)
			{
				var num = random.Next(i, dungeonDeck.Count);
				(dungeonDeck[num], dungeonDeck[i]) = (dungeonDeck[i], dungeonDeck[num]);
			}

			for (int i = 0; i < crisisDeck.Count; i++)
			{
				var num = random.Next(i, crisisDeck.Count);
				(crisisDeck[num], crisisDeck[i]) = (crisisDeck[i], crisisDeck[num]);
			}

			Dungeons.AddRange(dungeonDeck.Take(dungeonCount));
			Dungeons.AddRange(crisisDeck.Take(crisisCount));
		}
	}
}
