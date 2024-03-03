namespace ErinWave.M5Server
{
	public class M5Manager : Worker
	{
		public static List<M5Player> Players = [];
		public static M5Field Field = new();
		public static int Stage = 0;

		public M5Manager()
		{
			Initialize(Work);
		}

		public void Work()
		{
			try
			{
				while (true)
				{
					if (Field.IsPlaying && Field.RemainSeconds > 0)
					{
						Field.RemainSeconds--;

						// 실시간으로 남은 시간을 전송
						Common.SendAll(new M5Packet("9", "", Field.RemainSeconds.ToString()));

						if(Field.RemainSeconds == 0)
						{
							Field.RemainSeconds = -1;
							Common.SendAll(new M5Packet("00", "", "타임 오버!"));
						}
					}

					Thread.Sleep(1000);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public static M5Player GetPlayer(string id) => Players.Find(x => x.Id.Equals(id)) ?? default!;

		public static M5Player GetOtherPlayer(string id) => Players.Find(x => !x.Id.Equals(id)) ?? default!;

		public static int GetRemainCardCount()
		{
			var p1 = Players[0];
			var p2 = Players[1];

			return p1.Deck.Count + p1.Hand.Count + p2.Deck.Count + p2.Hand.Count;
		}

		public static void UseCard(string playerId, int cardIndex)
		{
			Field.IsPlaying = true;

			var player = GetPlayer(playerId);
			var other = GetOtherPlayer(playerId);
			var card = player.Hand[cardIndex];
			player.Hand.RemoveAt(cardIndex);
			Field.CurrentCards.Add(card);

			switch (card)
			{
				case "1201": // 분기탱천
					player.Draw();
					player.Draw();
					player.Draw();
					other.Draw();
					other.Draw();
					other.Draw();
					break;

				case "1202": // 도약
					if (Field.IsCurrentObstacle())
					{
						Field.WinDungeon();
					}
					break;

				case "1203": // 화염구
					if (Field.IsCurrentMonster())
					{
						Field.WinDungeon();
					}
					break;

				//case "1204": // 마법폭탄
					//break;

				case "1205": // 취소
					if (Field.IsCurrentEvent())
					{
						Field.WinDungeon();
					}
					break;

				//case "1206": // 와일드카드
					//break;

				case "1207": // 약초
					other.DrawFromUsed();
					other.DrawFromUsed();
					other.DrawFromUsed();
					other.DrawFromUsed();
					break;

				case "1208": // 저격
					if (Field.IsCurrentHuman())
					{
						Field.WinDungeon();
					}
					break;

				case "1209": // 회복물약
					player.DrawFromUsedRandom();
					player.DrawFromUsedRandom();
					player.DrawFromUsedRandom();
					other.DrawFromUsedRandom();
					other.DrawFromUsedRandom();
					other.DrawFromUsedRandom();
					break;

				case "1210": // 치유
					other.Deck.InsertRange(0, other.Used);
					other.Used = [];
					break;

				case "1211": // 신성수호방패
					player.Draw();
					other.Draw();
					Field.IsPlaying = false;
					break;

				case "1212": // 강타
					if (Field.IsCurrentMonster())
					{
						Field.WinDungeon();
					}
					break;

				case "1213": // 성스러운수류탄
					Field.WinDungeon();
					break;

				case "1214": // 등에칼꽂기
					if (Field.IsCurrentHuman())
					{
						Field.WinDungeon();
					}
					break;

				case "1215": // 질주
					if (Field.IsCurrentObstacle())
					{
						Field.WinDungeon();
					}
					break;

				case "1216": // 기부
					other.Hand.AddRange(player.Hand);
					player.Hand = [];
					break;

				case "1217": // 도둑질
					player.Hand.AddRange(other.Hand);
					other.Hand = [];
					break;
			}
		}

		public static void ActivateCrisisEvent()
		{
			var p1 = Players[0];
			var p2 = Players[1];

			switch (Field.CurrentDungeon)
			{
				case "2201":
					p1.DiscardRandom();
					p2.DiscardRandom();
					break;

				case "2202":
					p1.DiscardRandom();
					p1.DiscardRandom();
					p1.DiscardRandom();
					p2.DiscardRandom();
					p2.DiscardRandom();
					p2.DiscardRandom();
					break;

				case "2203":
					p1.DiscardAll();
					p2.DiscardAll();
					break;

				case "2204":
					(p2.Hand, p1.Hand) = (p1.Hand, p2.Hand);
					break;
			}
		}

		public static bool UseAbility(string playerId)
		{
			var player = GetPlayer(playerId);
			var other = GetOtherPlayer(playerId);

			if(player.Hand.Count < 3)
			{
				return false;
			}
			player.DiscardRandom();
			player.DiscardRandom();
			player.DiscardRandom();

			switch (player.Job)
			{
				case "궁수":
					if (Field.IsCurrentHuman())
					{
						Field.WinDungeon();
					}
					break;

				case "바바리안":
					if (Field.IsCurrentMonster())
					{
						Field.WinDungeon();
					}
					break;

				case "검투사":
					if (Field.IsCurrentHuman())
					{
						Field.WinDungeon();
					}
					break;

				case "사냥꾼":
					other.Draw();
					other.Draw();
					other.Draw();
					other.Draw();
					break;

				case "마법사":
					Field.IsPlaying = false;
					break;

				case "닌자":
					if (Field.IsCurrentObstacle())
					{
						Field.WinDungeon();
					}
					break;

				case "성기사":
					if (Field.IsCurrentMonster())
					{
						Field.WinDungeon();
					}
					break;

				case "주술사":
					if (Field.IsCurrentObstacle())
					{
						Field.WinDungeon();
					}
					break;

				case "도적":
					player.Draw();
					player.Draw();
					player.Draw();
					player.Draw();
					player.Draw();
					break;

				case "발키리":
					player.Draw();
					player.Draw();
					other.Draw();
					other.Draw();
					break;

			}
			return true;
		}
	}
}
