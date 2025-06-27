using Newtonsoft.Json;

namespace ErinWave.M5Server
{
	public class M5Player(string id)
	{
		[JsonIgnore]
		private Random random = new();

		/// <summary>
		/// 닉네임
		/// </summary>
		public string Id { get; set; } = id;

		/// <summary>
		/// 직업
		/// </summary>
		public string Job { get; set; } = string.Empty;

		/// <summary>
		/// 변경된 사항들
		/// - 직업 선택
		/// 2인 플레이 시, 직업 1개씩 고르고 덱은 추가로 원하는 색깔 하나를 더 가져올 수 있음
		/// -> 무조건 2인 플레이, 직업 1개씩 고르고 덱은 나머지 덱 3개 중에 랜덤으로 가져와짐
		/// 
		/// - 직업 능력
		/// 모든 능력은
		/// 손에 든 행동 카드를 3장 버리고
		/// -> 손에 든 카드 3장을 랜덤으로 버리고
		/// 
		/// 사냥꾼
		/// 다른 플레이어 1명을 골라 행동카드 4장을 뽑게 합니다.
		/// -> 상대방에게 행동카드 4장을 뽑게 합니다.
		/// 
		/// 발키리
		/// 자신을 제외한 모든 플레이어가 행동카드 2장씩 뽑습니다.
		/// -> 모든 플레이어가 행동카드 2장씩 뽑습니다.
		/// 
		/// - 위기
		/// 아야아야 해쪄
		/// 모든 플레이어는 카드 1장씩 버립니다.
		/// -> 모든 플레이어는 랜덤으로 카드 1장씩 버립니다.
		/// 
		/// 덜커덩
		/// 모든 플레이어는 카드 3장씩 버립니다.
		/// -> 모든 플레이어는 랜덤으로 카드 3장씩 버립니다.
		/// 
		/// 엉망진창
		/// 모든 플레이어는 손에 든 카드를 모두 다른 플레이어에게 줍니다.
		/// -> 서로 손에 든 카드를 모두 교환합니다.
		/// 
		/// 기습
		/// 카드 삭제.
		/// 
		/// - 기술
		/// 분기탱천
		/// 아무 플레이어 2명을 고릅니다. 그 2명은 각자 행동카드 3장씩 뽑습니다.
		/// -> 모든 플레이어는 각자 행동카드 3장을 뽑습니다.
		/// 
		/// 약초
		/// 아무 플레이어 1명을 고릅니다. 그 플레이어는 자신의 '버린 더미'에서 맨 위 카드 4장을 가져갑니다.
		/// -> 상대방 플레이어는 자신의 '버린 더미'에서 맨 위 카드 4장을 가져갑니다.
		/// 
		/// 회복 물약
		/// 모든 플레이어는 각자의 '버린 더미'에서 카드 3장씩 가져갑니다.
		/// -> 모든 플레이어는 각자의 '버린 더미'에서 랜덤으로 카드 3장씩 가져갑니다.
		/// 
		/// 치유
		/// 아무 플레이어 1명을 고릅니다. 그 플레이어는 자기 '버린 더미' 카드를 모두 가져와 자기 '행동 더미' 맨 위에 얹습니다.
		/// -> 상대방 플레이어는 자기 '버린 더미' 카드를 모두 가져와 자기 '행동 더미' 맨 위에 얹습니다.
		/// 
		/// 기부
		/// 자기 손에 든 카드를 전부 다른 플레이어 1명에게 줍니다.
		/// -> 자기 손에 든 카드를 전부 상대방에게 줍니다.
		/// 
		/// 도둑질
		/// 다른 플레이어 1명의 손에 든 카드를 전부 가져옵니다.
		/// -> 상대방 손에 든 카드를 전부 가져옵니다.
		/// 
		/// 카드 구분
		/// x___ 1: 아군 카드, 2: 적 카드
		/// 1x__ 1: 행동 카드, 2: 기술 카드
		/// 2x__ 1: 던전 카드, 2: 위기 카드
		/// 
		/// act
		/// 1101 sword
		/// 1102 sword2
		/// 1103 swordshield
		/// 1104 swordarrow
		/// 1105 swordscroll
		/// 1106 swordjump
		/// 1107 shield
		/// 1108 shield2
		/// 1109 arrow
		/// 1110 arrow2
		/// 1111 scroll
		/// 1112 scroll2
		/// 1113 jump
		/// 1114 jump2
		/// 
		/// skill
		/// 1201 bungi
		/// 1202 doyak
		/// 1203 fire
		/// 1204 magic
		/// 1205 cancel
		/// 1206 wildcard
		/// 1207 yakcho
		/// 1208 sniper
		/// 1209 heal
		/// 1210 chiyu
		/// 1211 suho
		/// 1212 gangta
		/// 1213 suryutan
		/// 1214 dk
		/// 1215 jilju
		/// 1216 gibu
		/// 1217 doduk
		/// 
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
		/// 카드 더미
		/// </summary>
		public List<string> Deck { get; set; } = [];

		/// <summary>
		/// 손에 들고 있는 카드
		/// </summary>
		public List<string> Hand { get; set; } = [];

		/// <summary>
		/// 버린 더미
		/// </summary>
		public List<string> Used { get; set; } = [];

		/// <summary>
		/// 덱에서 카드 한 장을 드로우합니다.
		/// </summary>
		public void Draw()
		{
			if (Deck.Count == 0)
			{
				return;
			}

			Hand.Add(Deck[0]);
			Deck.RemoveAt(0);
		}

		/// <summary>
		/// 버린 더미에서 카드 한 장을 드로우합니다.
		/// </summary>
		public void DrawFromUsed()
		{
			if (Used.Count == 0)
			{
				return;
			}

			Hand.Add(Used[0]);
			Used.RemoveAt(0);
		}

		/// <summary>
		/// 버린 더미에서 카드 한 장을 랜덤으로 드로우합니다.
		/// </summary>
		public void DrawFromUsedRandom()
		{
			if (Used.Count == 0)
			{
				return;
			}

			var num = random.Next(Used.Count);
			Hand.Add(Used[num]);
			Used.RemoveAt(num);
		}


		/// <summary>
		/// 손에 든 카드 중에서 한 장을 랜덤으로 버린 더미로 버립니다.
		/// </summary>
		public void DiscardRandom()
		{
			if (Hand.Count == 0)
			{
				return;
			}

			var num = random.Next(Hand.Count);
			Used.Insert(0, Hand[num]);
			Hand.RemoveAt(num);
		}

		/// <summary>
		/// 손에 든 카드 전부를 버린 더미로 버립니다.
		/// </summary>
		public void DiscardAll()
		{
			if (Hand.Count == 0)
			{
				return;
			}

			Used.InsertRange(0, Hand);
			Hand = [];
		}

		/// <summary>
		/// Fisher-Yates Shuffle
		/// </summary>
		public void ShuffleDeck()
		{
			var n = Deck.Count;
			while (n > 1)
			{
				n--;
				var k = random.Next(n + 1);
				(Deck[n], Deck[k]) = (Deck[k], Deck[n]);
			}
		}

		/// <summary>
		/// 덱을 초기화합니다.
		/// </summary>
		public void ClearDeck()
		{
			Deck.Clear();
		}

		public void GetRedDeck() => Deck.AddRange([.. "1101,1101,1101,1101,1101,1102,1102,1103,1103,1104,1104,1105,1105,1106,1106,1107,1107,1107,1107,1107,1107,1107,1109,1109,1109,1109,1109,1111,1111,1111,1113,1113,1113,1113,1113,1113,1201,1201,1202,1202".Split(',')]);

		public void GetYellowDeck() => Deck.AddRange([.. "1101,1101,1101,1101,1101,1101,1107,1107,1107,1107,1107,1107,1107,1107,1107,1108,1108,1109,1109,1109,1109,1109,1109,1111,1111,1111,1111,1111,1111,1111,1111,1113,1113,1113,1209,1210,1211,1211,1212,1213".Split(",")]);

		public void GetGreenDeck() => Deck.AddRange([.. "1101,1101,1101,1101,1107,1107,1107,1109,1109,1109,1109,1109,1109,1109,1109,1109,1110,1110,1111,1111,1111,1111,1113,1113,1113,1113,1113,1113,1113,1206,1206,1206,1206,1206,1206,1206,1206,1207,1207,1208".Split(",")]);

		public void GetBlueDeck() => Deck.AddRange([.. "1101,1101,1101,1107,1107,1107,1107,1107,1109,1109,1109,1109,1109,1109,1109,1111,1111,1111,1111,1111,1111,1111,1111,1111,1112,1112,1113,1113,1113,1113,1113,1113,1203,1203,1203,1203,1204,1204,1204,1205".Split(",")]);

		public void GetPurpleDeck() => Deck.AddRange([.. "1101,1101,1101,1101,1101,1101,1101,1107,1107,1107,1107,1107,1109,1109,1109,1111,1111,1111,1111,1111,1111,1113,1113,1113,1113,1113,1113,1113,1114,1114,1114,1214,1214,1214,1215,1215,1215,1216,1217,1217".Split(",")]);

		public void GetAdditionalDeck(int count)
		{
			for (int i = 0; i < count; i++)
			{
				Deck.AddRange([.. "1101,1107,1109,1111,1113".Split(",")]);
			}
		}
	}
}
