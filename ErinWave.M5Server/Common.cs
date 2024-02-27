using System.Net.Sockets;

namespace ErinWave.M5Server
{
	public class Common
	{
		public static Action<string> UserDisconnection = default!;
		public static Action<M5Packet> SendAll = default!;
	}
}
