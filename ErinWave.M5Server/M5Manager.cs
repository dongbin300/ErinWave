namespace ErinWave.M5Server
{
	public class M5Manager : Worker
	{
		public static List<M5Player> Players = new();

		public M5Manager()
		{

			//Initialize(Work);
		}

		public void Work()
		{
			try
			{

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
