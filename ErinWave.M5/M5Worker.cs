using Newtonsoft.Json;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
		public bool IsLocal => Dns.GetHostEntry(Dns.GetHostName()).AddressList.Any(x => x.ToString().Contains("192.168.219.101"));

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
					IsLocal ? "192.168.219.101" : "112.147.12.56"
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
				File.AppendAllText("log.txt", ex.ToString() + Environment.NewLine);
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
				File.AppendAllText("log.txt", ex.ToString() + Environment.NewLine);
			}
		}

		private void ParsePacket(string packetString)
		{
			try
			{
				File.AppendAllText("log.txt", "PACKET: " + packetString + Environment.NewLine);
				var packet = JsonConvert.DeserializeObject<M5Packet>(packetString) ?? default!;
				{
					switch (packet.Type)
					{
						case "00": // 시스템 메시지
							LogQueue.Enqueue($"[{DateTime.Now:HH:mm:ss}][SYSTEM] {packet.Data}");
							break;

						case "1001": // 유저 입장
							LogQueue.Enqueue($"[{DateTime.Now:HH:mm:ss}]{packet.Data} 님이 입장하셨습니다.");
							break;

						case "1002": // 유저 퇴장
							LogQueue.Enqueue($"[{DateTime.Now:HH:mm:ss}]{packet.Data} 님이 퇴장하셨습니다.");
							break;

						case "2001": // 채팅 메시지
							LogQueue.Enqueue($"[{DateTime.Now:HH:mm:ss}]{packet.Source}: {packet.Data}");
							break;

						case "0": // 현재 플레이어 상황
							if (packet.Source.Equals(Id)) // 자신
							{
								var data = JsonConvert.DeserializeObject<M5Player>(packet.Data) ?? default!;

								if (data.Job != string.Empty)
								{
									Common.MeJobImageSource = "job-" + data.Job switch
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
									};

									Common.MeDeckImageSource = data.Deck.Count > 0 ? "deck-" + data.Job switch
									{
										"바바리안" => "red",
										"검투사" => "red",
										"성기사" => "yellow",
										"발키리" => "yellow",
										"궁수" => "green",
										"사냥꾼" => "green",
										"마법사" => "blue",
										"주술사" => "blue",
										"닌자" => "purple",
										"도적" => "purple",
										_ => "red"
									} : null;

									Common.MeUsedImageSource = data.Used.Count > 0 ? Common.ToFileName(data.Used[0]) : null;

									Common.MeHandImageSource = [];
									foreach (var card in data.Hand)
									{
										var fileName = Common.ToFileName(card);
										Common.MeHandImageSource.Add(fileName);
									}

									Common.MeDeckCount = data.Deck.Count;
									Common.MeUsedCount = data.Used.Count;
								}
							}
							else // 상대
							{
								var data = JsonConvert.DeserializeObject<M5Player>(packet.Data) ?? default!;

								if (data.Job != string.Empty)
								{
									Common.YouJobImageSource = "job-" + data.Job switch
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
									};

									Common.YouDeckImageSource = data.Deck.Count > 0 ? "deck-" + data.Job switch
									{
										"바바리안" => "red",
										"검투사" => "red",
										"성기사" => "yellow",
										"발키리" => "yellow",
										"궁수" => "green",
										"사냥꾼" => "green",
										"마법사" => "blue",
										"주술사" => "blue",
										"닌자" => "purple",
										"도적" => "purple",
										_ => "red"
									} : null;

									Common.YouUsedImageSource = data.Used.Count > 0 ? Common.ToFileName(data.Used[0]) : null;

									Common.YouHandImageSource = [];
									foreach (var card in data.Hand)
									{
										var fileName = Common.ToFileName(card);
										Common.YouHandImageSource.Add(fileName);
									}

									Common.YouDeckCount = data.Deck.Count;
									Common.YouUsedCount = data.Used.Count;
								}
							}

							break;

						case "1": // 현재 필드 상황
							var fieldData = JsonConvert.DeserializeObject<M5Field>(packet.Data) ?? default!;

							// 보스
							Common.FieldBossImageSource = fieldData.Boss != string.Empty ? "boss" + fieldData.Boss : null;

							// 던전 덱 뒷면
							Common.FieldDungeonImageSource = fieldData.Dungeons.Count > 0 ? "card-monster" : null;

							// 현재 던전
							Common.FieldCurrentDungeonImageSource = Common.ToFileName(fieldData.CurrentDungeon);

							// 현재 올려져있는 카드
							Common.FieldCurrentCardImageSource = [];
							foreach (var card in fieldData.CurrentCards)
							{
								var fileName = Common.ToFileName(card);
								Common.FieldCurrentCardImageSource.Add(fileName);
							}

							Common.FieldDungeonCount = fieldData.Dungeons.Count;
							break;

						case "9": // 현재 남은 시간
							var seconds = int.Parse(packet.Data);
							Common.RemainSeconds = seconds;
							Common.RemainTimeString = $"{seconds / 60:00}:{seconds % 60:00}";
							break;

						default:
							break;
					}
				}
			}
			catch (Exception ex)
			{
				File.AppendAllText("log.txt", ex.ToString() + Environment.NewLine);
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

		public void SendDeckDrawEventPacket()
		{
			SendPacket("3001", Id, "");
		}

		public void SendUseCardEventPacket(int index)
		{
			SendPacket("3002", Id, index.ToString());
		}

		public void SendConfirmCurrentDungeonPacket()
		{
			SendPacket("3003", Id, "");
		}

		public void SendNextDungeonOpenPacket()
		{
			SendPacket("3004", Id, "");
		}

		public void SendConfirmBossPacket()
		{
			SendPacket("3005", Id, "");
		}

		public void SendUseAbilityPacket()
		{
			SendPacket("3010", Id, "");
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
