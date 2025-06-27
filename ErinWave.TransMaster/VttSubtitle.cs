namespace ErinWave.TransMaster
{
	public class VttSubtitle(TimeSpan startTime, TimeSpan endTime, string text, string text2 = "")
	{
		/// <summary>
		/// 시작 타임스탬프
		/// </summary>
		public TimeSpan StartTime { get; set; } = startTime;

		/// <summary>
		/// 끝 타임스탬프
		/// </summary>
		public TimeSpan EndTime { get; set; } = endTime;

		/// <summary>
		/// 오리지널 텍스트
		/// </summary>
		public string Text { get; set; } = text;

		/// <summary>
		/// 번역된 텍스트
		/// </summary>
		public string Text2 { get; set; } = text2;
	}
}
