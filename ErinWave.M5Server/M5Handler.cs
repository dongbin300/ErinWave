using Newtonsoft.Json;

using System.Net.Sockets;
using System.Text;

namespace ErinWave.M5Server
{
	public class M5Handler : Worker
	{
		private static Encoding BaseTextEncoding => Encoding.UTF8;
		private const int ResultBufferLength = 4 * 1024; // 단일 패킷 최대 4KB까지 허용
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
										SendSystemMessage("**직업\r\n바바리안\r\n손에 든 카드 3장을 랜덤으로 버리고 몬스터 하나를 처치합니다.\r\n\r\n검투사\r\n손에 든 카드 3장을 랜덤으로 버리고 인간 하나를 처치합니다.\r\n\r\n성기사\r\n손에 든 카드 3장을 랜덤으로 버리고 몬스터 하나를 처치합니다.\r\n\r\n발키리\r\n손에 든 카드 3장을 랜덤으로 버리고 모든 플레이어가 행동카드 2장씩 뽑습니다.\r\n\r\n궁수\r\n손에 든 카드 3장을 랜덤으로 버리고 인간 하나를 처치합니다.\r\n\r\n사냥꾼\r\n손에 든 카드 3장을 랜덤으로 버리고 상대방이 행동카드 4장을 뽑습니다.\r\n\r\n마법사\r\n손에 든 카드 3장을 랜덤으로 버리고 누군가 카드를 내려놓기 전까지, 시간을 잠시 멈춥니다.\r\n\r\n주술사\r\n손에 든 카드 3장을 랜덤으로 버리고 장애물 하나를 돌파합니다.\r\n\r\n도적\r\n손에 든 카드 3장을 랜덤으로 버리고 행동카드 5장을 뽑습니다.\r\n\r\n닌자\r\n손에 든 카드 3장을 랜덤으로 버리고 장애물 하나를 돌파합니다.");
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
												else if (M5Manager.GetOtherPlayer(Id).Job == string.Empty) // 아직 상대방이 직업을 안고른 경우
												{
													Player.Job = segment[1];
													SendSystemMessage($"직업 선택: {segment[1]}");
													SendGameStatus();
												}
												else // 모두 직업을 고른 경우
												{
													Player.Job = segment[1];
													SendSystemMessage("직업 선택 완료");
													SendSystemMessage("!시작 명령어로 게임을 시작해주세요.");
													SendGameStatus();
												}
												break;

											default:
												SendSystemMessage("없는 직업입니다.");
												break;
										}
									}
									break;

								case "!시작":
									if (segment.Length == 1)
									{
										SendSystemMessage("!시작 3 => 3스테이지 게임을 시작합니다. (1~12 스테이지 구현되어 있습니다.)");
									}
									else
									{
										if (M5Manager.Players.Any(x => x.Job == string.Empty))
										{
											SendSystemMessage("플레이어 모두 직업을 선택해 주세요.");
										}
										else
										{
											var stage = int.Parse(segment[1]);

											if(stage > 12)
											{
												SendSystemMessage($"없는 스테이지입니다.");
											}
											else
											{
												M5Manager.Field.SetGame(stage);
												M5Manager.Stage = stage;

												DistributeDeck();

												if(stage > 5)
												{
													var additionalDeckCount = stage - 5;
													foreach (var player in M5Manager.Players)
													{
														player.GetAdditionalDeck(additionalDeckCount);
														player.ShuffleDeck();
													}
												}

												SendSystemMessage($"{stage} 스테이지가 시작되었습니다.");
												SendGameStatus();
											}

										}
									}
									break;


								case "!패치":
									SendSystemMessage("**변경된 사항들\r\n- 직업 선택\r\n2인 플레이 시, 직업 1개씩 고르고 덱은 추가로 원하는 색깔 하나를 더 가져올 수 있음\r\n-> 무조건 2인 플레이, 직업 1개씩 고르고 덱은 나머지 덱 3개 중에 랜덤으로 가져와짐\r\n\r\n- 직업 능력\r\n모든 능력은\r\n손에 든 행동 카드를 3장 버리고\r\n-> 손에 든 카드 3장을 랜덤으로 버리고\r\n\r\n사냥꾼\r\n다른 플레이어 1명을 골라 행동카드 4장을 뽑게 합니다.\r\n-> 상대방에게 행동카드 4장을 뽑게 합니다.\r\n\r\n발키리\r\n자신을 제외한 모든 플레이어가 행동카드 2장씩 뽑습니다.\r\n-> 모든 플레이어가 행동카드 2장씩 뽑습니다.\r\n\r\n- 위기\r\n아야아야 해쪄\r\n모든 플레이어는 카드 1장씩 버립니다.\r\n-> 모든 플레이어는 랜덤으로 카드 1장씩 버립니다.\r\n\r\n덜커덩\r\n모든 플레이어는 카드 3장씩 버립니다.\r\n-> 모든 플레이어는 랜덤으로 카드 3장씩 버립니다.\r\n\r\n엉망진창\r\n모든 플레이어는 손에 든 카드를 모두 다른 플레이어에게 줍니다.\r\n-> 서로 손에 든 카드를 모두 교환합니다.\r\n\r\n기습\r\n카드 삭제.\r\n\r\n- 기술\r\n분기탱천\r\n아무 플레이어 2명을 고릅니다. 그 2명은 각자 행동카드 3장씩 뽑습니다.\r\n-> 모든 플레이어는 각자 행동카드 3장을 뽑습니다.\r\n\r\n약초\r\n아무 플레이어 1명을 고릅니다. 그 플레이어는 자신의 '버린 더미'에서 맨 위 카드 4장을 가져갑니다.\r\n-> 상대방 플레이어는 자신의 '버린 더미'에서 맨 위 카드 4장을 가져갑니다.\r\n\r\n회복 물약\r\n모든 플레이어는 각자의 '버린 더미'에서 카드 3장씩 가져갑니다.\r\n-> 모든 플레이어는 각자의 '버린 더미'에서 랜덤으로 카드 3장씩 가져갑니다.\r\n\r\n치유\r\n아무 플레이어 1명을 고릅니다. 그 플레이어는 자기 '버린 더미' 카드를 모두 가져와 자기 '행동 더미' 맨 위에 얹습니다.\r\n-> 상대방 플레이어는 자기 '버린 더미' 카드를 모두 가져와 자기 '행동 더미' 맨 위에 얹습니다.\r\n\r\n기부\r\n자기 손에 든 카드를 전부 다른 플레이어 1명에게 줍니다.\r\n-> 자기 손에 든 카드를 전부 상대방에게 줍니다.\r\n\r\n도둑질\r\n다른 플레이어 1명의 손에 든 카드를 전부 가져옵니다.\r\n-> 상대방 손에 든 카드를 전부 가져옵니다.");
									break;

								default:
									break;
							}
							break;

						case "3001": // 자신의 덱에서 카드 드로우
							var p_2 = M5Manager.GetPlayer(packet.Source);
							if (p_2.Hand.Count < 5) // 손 카드 상한선 5
							{
								p_2.Draw();
							}
							SendGameStatus();
							break;

						case "3002": // 카드 사용
							M5Manager.UseCard(packet.Source, int.Parse(packet.Data));
							SendGameStatus();
							break;

						case "3003": // 던전 처치/위기 적용(던전카드 클릭)
							switch (M5Manager.Field.CurrentDungeon)
							{
								case "2201":
								case "2202":
								case "2203":
								case "2204": // 위기 적용
									M5Manager.ActivateCrisisEvent();
									M5Manager.Field.WinDungeon();
									break;

								default: // 던전 처치
									if (M5Manager.Field.IsWinDungeon())
									{
										M5Manager.Field.WinDungeon();
									}
									else
									{
										SendSystemMessage("카드가 부족합니다.");
									}
									break;
							}
							SendGameStatus();
							break;

						case "3004": // 던전 카드 오픈
							M5Manager.Field.OpenDungeon();
							SendGameStatus();
							break;

						case "3005": // 보스 처치
							switch (M5Manager.Field.CurrentDungeon)
							{
								case "1":
								case "2":
								case "3":
								case "4":
								case "5":
									if (M5Manager.Field.IsWinDungeon())
									{
										M5Manager.Field.WinBoss();
										M5Manager.Field.IsPlaying = false;
										var stage = M5Manager.Stage;
										var clearTime = 300 - M5Manager.Field.RemainSeconds;
										var remainCardCount = M5Manager.GetRemainCardCount();
										SendSystemMessage($"{stage} 스테이지 클리어!!");
										SendSystemMessage($"클리어 시간: {clearTime / 60}분 {clearTime % 60}초");
										SendSystemMessage($"남은 카드 수: {remainCardCount}");
									}
									else
									{
										SendSystemMessage("카드가 부족합니다.");
									}
									SendGameStatus();
									break;

								default:
									break;
							}
							break;

						case "3010": // 직업 능력 사용
							var id = packet.Source;
							if (M5Manager.UseAbility(id))
							{
								SendSystemMessage($"{id}님이 능력을 사용하였습니다.");
							}
							else
							{
								SendSystemMessage($"{id}님이 능력사용에 실패하였습니다.");
							}
							SendGameStatus();
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
				Console.WriteLine(Id + " << " + packet);
			}
		}

		/// <summary>
		/// 모두에게 패킷 전송
		/// </summary>
		/// <param name="type"></param>
		/// <param name="source"></param>
		/// <param name="data"></param>
		private void SendAll(string type, string source, string data)
		{
			Common.SendAll(new M5Packet(type, source, data));
		}

		/// <summary>
		/// 시스템 메시지 전송
		/// </summary>
		/// <param name="message"></param>
		private void SendSystemMessage(string message)
		{
			SendAll("00", "", message);
		}

		/// <summary>
		/// 현재 게임 상태 전송
		/// </summary>
		private void SendGameStatus()
		{
			// 플레이어 상태
			foreach (var player in M5Manager.Players)
			{
				var playerJson = JsonConvert.SerializeObject(player);
				SendAll("0", player.Id, playerJson);
			}

			// 필드 상태
			var fieldJson = JsonConvert.SerializeObject(M5Manager.Field);
			SendAll("1", "field", fieldJson);
		}

		private void DistributeDeck()
		{
			if (M5Manager.Players.Count == 2 && M5Manager.Players.Count(x => x.Job != string.Empty) == 2)
			{
				// Red, Yellow, Green, Blue, Purple 순
				bool[] decks = [true, true, true, true, true];

				foreach (var player in M5Manager.Players)
				{
					player.Deck = [];
					player.Hand = [];
					player.Used = [];

					switch (player.Job)
					{
						case "바바리안":
						case "검투사":
							decks[0] = false;
							player.GetRedDeck();
							break;

						case "성기사":
						case "발키리":
							decks[1] = false;
							player.GetYellowDeck();
							break;

						case "궁수":
						case "사냥꾼":
							decks[2] = false;
							player.GetGreenDeck();
							break;

						case "마법사":
						case "주술사":
							decks[3] = false;
							player.GetBlueDeck();
							break;

						case "닌자":
						case "도적":
							decks[4] = false;
							player.GetPurpleDeck();
							break;
					}
				}

				foreach (var player in M5Manager.Players)
				{
					var num = Random.Next(5);
					while (!decks[num])
					{
						num = Random.Next(5);
					}
					decks[num] = false;
					switch (num)
					{
						case 0:
							player.GetRedDeck();
							break;
						case 1:
							player.GetYellowDeck();
							break;
						case 2:
							player.GetGreenDeck();
							break;
						case 3:
							player.GetBlueDeck();
							break;
						case 4:
							player.GetPurpleDeck();
							break;
					}

					player.ShuffleDeck();
				}
				SendSystemMessage("플레이어에게 덱을 지급했습니다.");
			}
		}
	}
}
