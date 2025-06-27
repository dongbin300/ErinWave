namespace ErinWave.HelloAkiba.Models
{
    public class HaItem
    {
        public string Name { get; }
        public int Tier { get; set; } = 0;
        public int Type { get; set; } = 0;

		/// <summary>
		/// - 라벨, 버튼, 텍스트박스, 체크박스, 라디오버튼, 스택패널, 캔버스, 그리드, 콤보박스, 리스트박스, 리스트뷰, 트리뷰, 데이터그리드, 유저컨트롤, 커스텀컨트롤
		///	- 변수, 연산자, 조건문, 반복문, 리스트, 메서드, 구조체, 속성, 생성자, 상속, 오버로딩, 이벤트, 람다식, LINQ, 제네릭, 튜플, 비동기
		/// </summary>
		/// <param name="type"></param>
		/// <param name="tier"></param>
		public HaItem(int type, int tier)
		{
			Type = type;
			Tier = tier;
			Name = type switch
			{
				1 => tier switch
				{
					1 => "raberu",
					2 => "botan",
					3 => "tekisuto bokkusu",
					4 => "chekku bokkusu",
					5 => "rajio botan",
					6 => "sutakku paneru",
					7 => "kyanbasu",
					8 => "gurido",
					9 => "konbo bokkusu",
					10 => "risuto bokkusu",
					11 => "risutobyuu",
					12 => "toriibyuu",
					13 => "deetagurido",
					14 => "yuuzaa kontorooru",
					15 => "kasutamu kontorooru",
					_ => string.Empty
				},
				2 => tier switch
				{
					1 => "hensuu",
					2 => "enzanshi",
					3 => "jyoukenbun",
					4 => "ruupu",
					5 => "risuto",
					6 => "mesoddo",
					7 => "kouzoutai",
					8 => "zokusei",
					9 => "seiseisha",
					10 => "souzoku",
					11 => "ibento",
					12 => "ramudashiki",
					13 => "rinku",
					14 => "jenerikku",
					15 => "chuupuru",
					16 => "hidouki",
					_ => string.Empty
				},
				_ => string.Empty
			};
		}

		public override string ToString()
		{
			return $"{Name} ({Type},{Tier})";
		}
	}
}
