namespace ErinWave.Richer.Enums
{
	public enum RicherAiType
	{
		None,

		/// <summary>
		/// 초기에 코인물량을 파는 AI(이 AI는 매수 불가능)
		/// </summary>
		Master,

		/// <summary>
		/// 저가에 코인물량을 매집하고 고가에 매도하는 AI
		/// </summary>
		Whale,

		/// <summary>
		/// 일반적으로 매수 매도하는 AI
		/// </summary>
		Commoner
	}
}
