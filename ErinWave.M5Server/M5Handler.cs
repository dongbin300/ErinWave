using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.ComponentModel;
using System.Net.Sockets;
using System.Text;

namespace ErinWave.M5Server
{
	public class M5Handler : Worker
	{
		private static Encoding BaseTextEncoding => Encoding.UTF8;
		private const int ResultBufferLength = 1024; // 단일 패킷 최대 1KB까지 허용
		private const int IntervalWork = 25;
		private const string PacketDelimiter = "\r\n";

		public TcpClient Client = default!;
		private NetworkStream stream = default!;
		public bool IsInitialized { get; private set; }
		public bool IsRun { get; set; }
		public string Id = string.Empty;
		public M5Player Player => M5Manager.Players.Find(x => x.Id.Equals(Id)) ?? default!;

		public Random Random { get; set; }

		public M5Handler(TcpClient client)
		{
			Random = new Random();
			Client = client;
			Initialize(Work);
		}

		public void Work()
		{
			try
			{
				using (Client)
				{
					using (stream = Client.GetStream())
					{
						IsInitialized = true;
						IsRun = true;

						byte[] resultBuffer = new byte[ResultBufferLength];
						int bytesRead = 0;

						while (IsRun)
						{
							TryReceive(ref resultBuffer, ref bytesRead);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
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
			catch (IOException)
			{
				IsRun = false;
				Common.UserDisconnection(Id);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
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
						case "1001": // 유저 입장
							Id = packet.Data;
							M5Manager.Players.Add(new M5Player(Id));
							Common.SendAll(packet);
							//Console.WriteLine($"{packet.Data} 님이 입장하셨습니다.");
							break;

						case "1002": // 유저 퇴장
							M5Manager.Players.RemoveAll(x => x.Id == packet.Data);
							Common.SendAll(packet);
							//Console.WriteLine($"{packet.Data} 님이 퇴장하셨습니다.");
							break;

						case "2001": // 채팅 메시지
							Common.SendAll(packet);

							var message = packet.Data;
							var segment = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
							switch (segment[0])
							{
								case "!주사위":
									var value = Random.Next(1, 6);
									SendSystemMessage($"주사위를 굴려서 {value}이 나왔습니다!");
									break;

								case "!직업":
									if (segment.Length == 1)
									{
										SendSystemMessage("궁수, 바바리안, 검투사, 사냥꾼, 마법사, 닌자, 성기사, 주술사, 도적, 발키리 중 택 1");
									}
									else
									{
										switch (segment[1])
										{
											case "궁수":
											case "바바리안":
											case "검투사":
											case "사냥꾼":
											case "마법사":
											case "닌자":
											case "성기사":
											case "주술사":
											case "도적":
											case "발키리":
												if (Player.Job != string.Empty)
												{
													SendSystemMessage("이미 직업을 골랐습니다.");
												}
												else if (M5Manager.Players.Any(x => x.Job.Equals(segment[1])))
												{
													SendSystemMessage("상대가 고른 직업입니다.");
												}
												else
												{
													Player.Job = segment[1];
													SendAll("3101", Id, segment[1]);
													SendSystemMessage("직업 선택 완료");
												}
												break;

											default:
												SendSystemMessage("없는 직업입니다.");
												break;
										}
									}
									break;

								default:
									break;
							}
							break;

						default:
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public void SendPacket(string type, string source, string data)
		{
			SendPacket(new M5Packet(type, source, data));
		}

		public void SendPacket(M5Packet packet)
		{
			byte[] buffer = BaseTextEncoding.GetBytes(JsonConvert.SerializeObject(packet) + PacketDelimiter);

			if (stream.CanWrite)
			{
				stream.Write(buffer, 0, buffer.Length);
				Console.WriteLine(packet);
			}
		}

		private void SendAll(string type, string source, string data)
		{
			Common.SendAll(new M5Packet(type, source, data));
		}

		private void SendSystemMessage(string message)
		{
			SendAll("00", "", message);
		}
	}
}
