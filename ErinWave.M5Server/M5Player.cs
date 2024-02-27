namespace ErinWave.M5Server
{
	public class M5Player(string id)
	{
		public string Id { get; set; } = id;
		public string Job { get; set; } = string.Empty;

		/// <summary>
		/// 카드 구분
		/// x___ 1: 아군 카드, 2: 적 카드
		/// 1x__ 1: 행동 카드, 2: 기술 카드
		/// 2x__ 1: 던전 카드, 2: 위기 카드
		/// 
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
		/// 1201
		/// 
		/// 2101 
		/// 
		/// 2201 
		///
		/// </summary>
		public List<string> Deck { get; set; } = [];
		public List<string> Used { get; set; } = [];
	}
}
