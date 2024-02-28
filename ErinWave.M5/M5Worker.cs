using Newtonsoft.Json;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace ErinWave.M5
{
	public class M5Worker : Worker
	{
		private static Encoding BaseTextEncoding => Encoding.UTF8;
		private const int ResultBufferLength = 4 * 1024; // 단일 패킷 최대 4KB까지 허용
		private const int IntervalWork = 25;
		private const string PacketDelimiter = "\r\n";

		private TcpClient client = default!;
		private NetworkStream stream = default!;
		public bool IsInitialized { get; private set; }
		public bool IsLocal => Dns.GetHostEntry(Dns.GetHostName()).AddressList.Any(x => x.ToString().Contains("172.30.1.95"));

		public string Id = string.Empty;

		public M5Worker(string id)
		{
			Id = id;
			Initialize(Work);
		}

		public void Work()
		{
			try
			{
				using (client = new TcpClient(
					IsLocal ? "172.30.1.95" : "175.212.176.45"
					, 45111))
				{
					LogQueue.Enqueue("접속 완료");
					using (stream = client.GetStream())
					{
						SendConnectionPacket();
						IsInitialized = true;

						byte[] resultBuffer = new byte[ResultBufferLength];
						int bytesRead = 0;

						while (!Common.IsExit)
						{
							TryReceive(ref resultBuffer, ref bytesRead);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void TryReceive(ref byte[] resultBuffer, ref int bytesRead)
		{
			try
			{
				bytesRead = stream.Read(resultBuffer, 0, resultBuffer.Length);
				string data = BaseTextEncoding.GetString(resultBuffer, 0, bytesRead);
				string[] packetStrings = data.Split(PacketDelimiter, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < packetStrings.Length; i++)
				{
					ParsePacket(packetStrings[i]);
				}

				Thread.Sleep(IntervalWork);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void ParsePacket(string packetString)
		{
			try
			{
				var packet = JsonConvert.DeserializeObject<M5Packet>(packetString) ?? default!;
				{
					switch (packet.Type)
					{
						case "00": // 시스템 메시지
							LogQueue.Enqueue($"[SYSTEM]: {packet.Data}");
							break;

						case "1001": // 유저 입장
							LogQueue.Enqueue($"{packet.Data} 님이 입장하셨습니다.");
							break;

						case "1002": // 유저 퇴장
							LogQueue.Enqueue($"{packet.Data} 님이 퇴장하셨습니다.");
							break;

						case "2001": // 채팅 메시지
							LogQueue.Enqueue($"{packet.Source}: {packet.Data}");
							break;

						case "0": // 현재 게임 상황
							if (packet.Source.Equals(Id)) // 자신
							{
								var data = JsonConvert.DeserializeObject<M5Player>(packet.Data) ?? default!;

								if (data.Job != string.Empty)
								{
									Common.MeJobImageSource = Common.ImageResourceUrl + "job-" + data.Job switch
									{
										"바바리안" => "baba",
										"검투사" => "glad",
										"성기사" => "pal",
										"발키리" => "val",
										"궁수" => "arc",
										"사냥꾼" => "hunter",
										"마법사" => "magi",
										"주술사" => "pow",
										"닌자" => "ninja",
										"도적" => "thief",
										_ => "baba"
									} + ".png";
								}
							}
							else // 상대
							{
								var data = JsonConvert.DeserializeObject<M5Player>(packet.Data) ?? default!;

								if (data.Job != string.Empty)
								{
									Common.YouJobImageSource = Common.ImageResourceUrl + "job-" + data.Job switch
									{
										"바바리안" => "baba",
										"검투사" => "glad",
										"성기사" => "pal",
										"발키리" => "val",
										"궁수" => "arc",
										"사냥꾼" => "hunter",
										"마법사" => "magi",
										"주술사" => "pow",
										"닌자" => "ninja",
										"도적" => "thief",
										_ => "baba"
									} + ".png";
								}
							}
									
							break;

						default:
							break;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void SendConnectionPacket()
		{
			SendPacket("1001", "system", Id);
		}

		public void SendDisconnectionPacket()
		{
			SendPacket("1002", "system", Id);
		}

		public void SendChatMessagePacket(string message)
		{
			SendPacket("2001", Id, message);
		}

		private void SendPacket(string type, string source, string data)
		{
			SendPacket(new M5Packet(type, source, data));
		}

		private void SendPacket(M5Packet packet)
		{
			byte[] buffer = BaseTextEncoding.GetBytes(JsonConvert.SerializeObject(packet) + PacketDelimiter);

			if (stream.CanWrite)
			{
				stream.Write(buffer, 0, buffer.Length);
			}
		}
	}
}
