using Raylib_cs;

namespace ErinWave.Frame.Raylibs
{
	public class KoreanFontHelper
	{
		public static Font LoadFont(string fontPath, int fontSize)
		{
			if (!File.Exists(fontPath)) // 폰트 파일 없음
			{
				return default;
			}

			// codepoints 확장: ASCII + 한글 완성형 + 한글 자모 + 일부 특수기호
			var codepointLists = new List<int>();

			// 1. ASCII (0~127)
			for (int i = 0; i < 128; i++) codepointLists.Add(i);

			// 2. 한글 완성형 (AC00 ~ D7A3)
			for (int i = 0xAC00; i <= 0xD7A3; i++) codepointLists.Add(i);

			// 3. 한글 자모 (초성/중성/종성: 3131 ~ 318E)
			for (int i = 0x3131; i <= 0x318E; i++) codepointLists.Add(i);

			// 4. × 같은 특수기호 (필요한 거 추가, 예: U+00D7)
			//codepointLists.Add(0x00D7);  // ×
			//							 // 필요하면 더 추가: 예) 0x00B7 (·), 0x2013 (–) 등

			int[] codepoints = [.. codepointLists];
			int codepointCount = codepoints.Length;

			Font font = Raylib.LoadFontEx(fontPath, fontSize, codepoints, codepointCount);

			if (font.Texture.Id == 0) // 폰트 로드 실패
			{
				return default;
			}

			return font;
		}
	}
}
