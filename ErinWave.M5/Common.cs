namespace ErinWave.M5
{
	public class Common
	{
		public static readonly string ImageResourceUrl = "pack://application:,,,/ErinWave.M5;component/Resources/Images/";

		public static bool IsExit = false;

		public static int RemainSeconds = 0;
		public static string RemainTimeString = string.Empty;

		public static string? MeJobImageSource = null;
		public static string? MeDeckImageSource = null;
		public static string? MeUsedImageSource = null;
		public static List<string?> MeHandImageSource = [];
		public static int MeDeckCount = 0;
		public static int MeUsedCount = 0;

		public static string? YouJobImageSource = null;
		public static string? YouDeckImageSource = null;
		public static string? YouUsedImageSource = null;
		public static List<string?> YouHandImageSource = [];
		public static int YouDeckCount = 0;
		public static int YouUsedCount = 0;

		public static string? FieldBossImageSource = null;
		public static string? FieldDungeonImageSource = null;
		public static string? FieldCurrentDungeonImageSource = null;
		public static List<string?> FieldCurrentCardImageSource = [];
		public static int FieldDungeonCount = 0;

		public static string? ToFileName(string cardId)
		{
			return cardId switch
			{
				"1101" => "act-sword",
				"1102" => "act-sword2",
				"1103" => "act-swordshield",
				"1104" => "act-swordarrow",
				"1105" => "act-swordscroll",
				"1106" => "act-swordjump",
				"1107" => "act-shield",
				"1108" => "act-shield2",
				"1109" => "act-arrow",
				"1110" => "act-arrow2",
				"1111" => "act-scroll",
				"1112" => "act-scroll2",
				"1113" => "act-jump",
				"1114" => "act-jump2",
				"1201" => "skill-bungi",
				"1202" => "skill-doyak",
				"1203" => "skill-fire",
				"1204" => "skill-magic",
				"1205" => "skill-cancel",
				"1206" => "skill-wildcard",
				"1207" => "skill-yakcho",
				"1208" => "skill-sniper",
				"1209" => "skill-heal",
				"1210" => "skill-chiyu",
				"1211" => "skill-suho",
				"1212" => "skill-gangta",
				"1213" => "skill-suryutan",
				"1214" => "skill-dk",
				"1215" => "skill-jilju",
				"1216" => "skill-gibu",
				"1217" => "skill-doduk",
				"2101" => "dg-human-1",
				"2102" => "dg-human-2",
				"2103" => "dg-human-3",
				"2104" => "dg-human-4",
				"2105" => "dg-human-5",
				"2106" => "dg-human-6",
				"2107" => "dg-human-7",
				"2108" => "dg-human-8",
				"2109" => "dg-human-9",
				"2110" => "dg-human-10",
				"2111" => "dg-human-11",
				"2112" => "dg-human-12",
				"2113" => "dg-human-13",
				"2114" => "dg-monster-1",
				"2115" => "dg-monster-2",
				"2116" => "dg-monster-3",
				"2117" => "dg-monster-4",
				"2118" => "dg-monster-5",
				"2119" => "dg-monster-6",
				"2120" => "dg-monster-7",
				"2121" => "dg-monster-8",
				"2122" => "dg-monster-9",
				"2123" => "dg-monster-10",
				"2124" => "dg-monster-11",
				"2125" => "dg-monster-12",
				"2126" => "dg-obs-1",
				"2127" => "dg-obs-2",
				"2128" => "dg-obs-3",
				"2129" => "dg-obs-4",
				"2130" => "dg-obs-5",
				"2131" => "dg-obs-6",
				"2132" => "dg-obs-7",
				"2133" => "dg-obs-8",
				"2134" => "dg-obs-9",
				"2135" => "dg-obs-10",
				"2136" => "dg-obs-11",
				"2137" => "dg-obs-12",
				"2138" => "dg-obs-13",
				"2139" => "dg-obs-14",
				"2140" => "dg-obs-15",
				"2201" => "crisis-event-1",
				"2202" => "crisis-event-2",
				"2203" => "crisis-event-3",
				"2204" => "crisis-event-4",
				"2205" => "crisis-mid-1",
				"2206" => "crisis-mid-2",
				"2207" => "crisis-mid-3",
				"2208" => "crisis-mid-4",
				"2209" => "crisis-mid-5",
				_ => null,
			};
		}
	}
}
