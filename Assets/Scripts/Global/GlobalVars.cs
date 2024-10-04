using System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;

namespace Global {
	public class GlobalVars : MonoBehaviourPunCallbacks, IPunObservable {
		public static float EnnemySpeed = 1f;
		public static float ProjectileSpeed = 1f;
		public static float ProjectileRefreshTime = .1f;
		public static byte SaveId = 0;
		public static short DeathCount = 0;
		public static short GameOverCount = 0;
		// the INITIAL player count
		public static byte NbrOfPlayers = 1;

		// first dim : Level
		// second dim : Xpos ,etc...
		public static readonly int[,][] SaveLookupArray2 = new int[8,11][] {
			{ // level 0 -- TESTING VALUES, for the TUTORIAL
				new [] {-15, -15}, // x pos
				new [] {0, 0}, // y pos
				new [] {30,30}, // Life
				new [] {25,25}, // stamina
				new [] {25,25}, // mana
				new [] {0,1}, // sword unlocked
				new [] {0,0}, // bow unlocked
				new [] {0,0}, // poison
				new [] {1,1}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
			{ // level 1 -- TESTING VALUES, for the FOREST // second save for unlocking the arc, positions should never be loaded
				new [] {-16,-16}, // x pos
				new [] {-5,-5}, // y pos
				new [] {30,30}, // Life
				new [] {25,25}, // stamina
				new [] {25,25}, // mana
				new [] {1,1}, // sword unlocked
				new [] {0,1}, // bow unlocked
				new [] {0,0}, // poison
				new [] {1,1}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
			{ // level 1.5 -- TESTING VALUES, for the FOREST BOSS
				new [] {-17}, // x pos
				new [] {0}, // y pos
				new [] {30}, // Life
				new [] {25}, // stamina
				new [] {25}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {0}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {0} // goBackInTime
			},
			{ // level 2 -- TESTING VALUES, for the MINE
				new [] {-67}, // x pos
				new [] {19}, // y pos
				new [] {40}, // Life
				new [] {35}, // stamina
				new [] {35}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {1}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {1} // goBackInTime
			},
			{ // level 2.5 -- for the BOSS of the MINE
				new [] {-4}, // x pos
				new [] {44}, // y pos
				new [] {40}, // Life
				new [] {35}, // stamina
				new [] {35}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {1}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {1} // goBackInTime
			},
			{ // level 3 -- TESTING VALUES, for the GARDEN (of the castle)
				new [] {-237}, // x pos
				new [] {-46}, // y pos
				new [] {50}, // Life
				new [] {45}, // stamina
				new [] {45}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {1}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {1} // goBackInTime
			},
			{ // level 3.2 -- TESTING VALUES, for the CASTLE
				new [] {-237}, // x pos
				new [] {-48}, // y pos
				new [] {50}, // Life
				new [] {45}, // stamina
				new [] {45}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {1}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {1} // goBackInTime
			},
			{ // level 3.5 -- for the FINAL BOSS
				new [] {0}, // x pos
				new [] {-2}, // y pos
				new [] {50}, // Life
				new [] {45}, // stamina
				new [] {45}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {1}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {1} // goBackInTime
			}
		};
		public static DateTime? TimeStartedAt = null;
		public static List<Player.Player> PlayerList = new List<Player.Player>();
		public static byte PlayerId;
		public static bool Continue = false;
		#region Game Constants
		public static string GameVersion = "0.2";
		// 1st level : Tutorial
		// 2nd : Forest
		// 3rd : Boss
		// 4th : Mine
		// 5th : Boss
		// 6th : Castle
		// 7th : Castle Part 2
		// 7th : FinalBoss
		public static string[] LevelsName = new[] {"IntroLevel","Forest","BossForest","Mine","BossMine", "Castle", "LastCastle","FinalBoss"};
		public static byte CurrentLevelId = 0;
		public static string GameOverSceneName = "GameOver";
		public static string GameClearSceneName = "GameClear";
		public static string LobbySceneName = "Lobby";
		// 0 = en, 1 = fr, 2 = jp
		public static byte Language = 0;
		#endregion

		public static void CleanUpVars() {
			GlobalVars.SaveId = 0; // from the beginning
			GlobalVars.DeathCount = 0;
			GlobalVars.NbrOfPlayers = (byte)PhotonNetwork.CurrentRoom.PlayerCount;
			GlobalVars.GameOverCount = 0;
			GlobalVars.CurrentLevelId = 0;
			GlobalVars.TimeStartedAt = null;

		}
		#region IPunObservable
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				stream.SendNext(EnnemySpeed);
				stream.SendNext(SaveId);
				stream.SendNext(CurrentLevelId);
			}
			else {
				EnnemySpeed = (float)stream.ReceiveNext();
				SaveId = (byte)stream.ReceiveNext();
				CurrentLevelId = (byte)stream.ReceiveNext();
			}
		}
		#endregion
		#region MonoBehaviour
		private void Awake() {
			DontDestroyOnLoad(this.gameObject);
		}
		#endregion
	}
}