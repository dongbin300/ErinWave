namespace ErinWave.M5
{
	public class LogQueue
	{
		public static Queue<string> Queue = new();
		public static int Count => Queue.Count;

		public static void Enqueue(string message)
		{
			Queue.Enqueue(message);
		}

		public static string Dequeue()
		{
			return Queue.Dequeue();
		}
	}
}
