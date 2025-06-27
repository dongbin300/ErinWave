using System.IO;
using System.Text;

namespace ErinWave.TransMaster
{
	public class VttHelper
	{
		public static List<VttSubtitle> ParseVtt(string fileName)
		{
			var result = new List<VttSubtitle>();
			var data = File.ReadAllLines(fileName);
			var previousText = string.Empty;

			for (int i = 2; i < data.Length; i += 3)
			{
				var parts = data[i].Split(" --> ");
				var startTime = ParseTimestamp(parts[0]);
				var endTime = ParseTimestamp(parts[1]);
				var text = data[i + 1].Replace("…", "");

				if (text == previousText) // 이전 자막과 동일하면 건너뜀
				{
					continue;
				}

				var subtitle = new VttSubtitle(startTime, endTime, text);
				result.Add(subtitle);

				previousText = text;
			}

			return result;
		}

		public static void ApplyTranslatedText(List<VttSubtitle> subtitles, string translatedText)
		{
			var lines = translatedText.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

			var parsedTranslations = new Dictionary<(TimeSpan, TimeSpan), string>();
			for (int i = 0; i < lines.Length; i += 2)
			{
				var times = lines[i].Split(',');
				if (times.Length != 2) continue;

				if (TimeSpan.TryParse(times[0], out var startTime) && TimeSpan.TryParse(times[1], out var endTime))
				{
					parsedTranslations[(startTime, endTime)] = lines[i + 1];
				}
			}

			foreach (var subtitle in subtitles)
			{
				if (parsedTranslations.TryGetValue((subtitle.StartTime, subtitle.EndTime), out var translated))
				{
					subtitle.Text2 = translated;
				}
			}
		}

		public static void WriteVtt(string fileName, List<VttSubtitle> subtitles)
		{
			using var writer = new StreamWriter(fileName, false, Encoding.UTF8);
			writer.WriteLine("WEBVTT\n");

			foreach (var subtitle in subtitles)
			{
				writer.WriteLine($"{FormatTimestamp(subtitle.StartTime)} --> {FormatTimestamp(subtitle.EndTime)}");
				writer.WriteLine(subtitle.Text2);
				writer.WriteLine();
			}
		}

		private static string FormatTimestamp(TimeSpan time)
		{
			return time.ToString(@"hh\:mm\:ss\.fff");
		}

		private static TimeSpan ParseTimestamp(string input)
		{
			if (input.Count(c => c == ':') == 1) // "mm:ss.fff" 형식
				input = "00:" + input; // "hh:mm:ss.fff"로 맞춤

			return TimeSpan.ParseExact(input, @"hh\:mm\:ss\.fff", null);
		}
	}
}
