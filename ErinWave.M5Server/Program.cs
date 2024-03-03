using System.Net;
using System.Net.Sockets;

namespace ErinWave.M5Server
{
	internal class Program
	{
		static TcpListener listener = default!;
		static List<M5Handler> clients = [];

		static void Main(string[] args)
		{
			Common.UserDisconnection = (id) =>
			{
				clients.RemoveAll(x => !x.IsRun);
				M5Manager.Players.RemoveAll(x => x.Id == id);
				Console.WriteLine($"Client Disconnected [ {id} ]");
				SendAll("1002", "system", id);
			};

			Common.SendAll = SendAll;
			new M5Manager().Start();

			try
			{
				listener = new TcpListener(IPAddress.Any, 45111);
				listener.Start();
				Console.WriteLine($"Server 45111 ON");

				while (true)
				{
					var client = listener.AcceptTcpClient();
					var ipAddress = ((IPEndPoint)(client.Client.RemoteEndPoint ?? default!)).Address.ToString() ?? "고수";
					Console.WriteLine($"Client Connected [ {ipAddress} ]");

					var handler = new M5Handler(client);
					clients.Add(handler);
					handler.Start();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		static void SendAll(M5Packet packet)
		{
			SendAll(packet.Type, packet.Source, packet.Data);
		}

		static void SendAll(string type, string source, string data)
		{
			try
			{
				foreach (var client in clients)
				{
					client.SendPacket(type, source, data);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
