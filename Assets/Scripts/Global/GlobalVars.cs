using System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;

namespace Global {
	public class GlobalVars : MonoBehaviourPunCallbacks, IPunObservable {
		// to sync over network
		public static float PlayerSpeed = 1f;
		public static float EnnemySpeed = 1f;
		public static float ProjectileSpeed = 1f;
		public static float ProjectileRefreshTime = .1f;
		public static byte SaveId = 0; // useful for debugging, will be 0 in the end
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
				new [] {20,20}, // Life
				new [] {20,20}, // stamina
				new [] {20,20}, // mana
				new [] {0,1}, // sword unlocked
				new [] {0,0}, // bow unlocked
				new [] {0,0}, // poison
				new [] {1,1}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
			{ // level 1 -- TESTING VALUES, for the FOREST // second save for unlocking the arc, positions should never be loaded
				new [] {-16,-998}, // x pos
				new [] {-3,-998}, // y pos
				new [] {20,20}, // Life
				new [] {20,20}, // stamina
				new [] {20,20}, // mana
				new [] {1,1}, // sword unlocked
				new [] {0,1}, // bow unlocked
				new [] {0,0}, // poison
				new [] {1,1}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
			{ // level 1.5 -- TESTING VALUES, for the FOREST BOSS
				new [] {5}, // x pos
				new [] {-13}, // y pos
				new [] {20}, // Life
				new [] {20}, // stamina
				new [] {20}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {0}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {0} // goBackInTime
			},
			{ // level 2 -- TESTING VALUES, for the MINE
				new [] {-67,-67}, // x pos
				new [] {30,-30}, // y pos
				new [] {20,20}, // Life
				new [] {20,20}, // stamina
				new [] {20,20}, // mana
				new [] {1,1}, // sword unlocked
				new [] {1,1}, // bow unlocked
				new [] {1,1}, // poison
				new [] {1,1}, // dash
				new [] {0,0}, // slowdown
				new [] {1,1} // goBackInTime
			},
			{ // level 2.5 -- for the BOSS of the MINE
				new [] {-4}, // x pos
				new [] {46}, // y pos
				new [] {20}, // Life
				new [] {20}, // stamina
				new [] {20}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {1}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {1} // goBackInTime
			},
			{ // level 3 -- TESTING VALUES, for the GARDEN (of the castle)
				new [] {-237,-237}, // x pos
				new [] {-46,-46}, // y pos
				new [] {20,20}, // Life
				new [] {20,20}, // stamina
				new [] {20,20}, // mana
				new [] {1,1}, // sword unlocked
				new [] {1,1}, // bow unlocked
				new [] {1,1}, // poison
				new [] {1,1}, // dash
				new [] {0,0}, // slowdown
				new [] {1,1} // goBackInTime
			},
			{ // level 3.2 -- TESTING VALUES, for the CASTLE
				new [] {-237}, // x pos
				new [] {-48}, // y pos
				new [] {20}, // Life
				new [] {20}, // stamina
				new [] {20}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {1}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {1} // goBackInTime
			},
			{ // level 3.5 -- for the FINAL BOSS
				new [] {-237}, // x pos
				new [] {-48}, // y pos
				new [] {6}, // Life
				new [] {2}, // stamina
				new [] {2}, // mana
				new [] {1}, // sword unlocked
				new [] {1}, // bow unlocked
				new [] {1}, // poison
				new [] {1}, // dash
				new [] {0}, // slowdown
				new [] {1} // goBackInTime
			}
		};
		public static DateTime? TimeStartedAt = null;
		// not to be synced, maybe to put in player ?
		public static List<Player.Player> PlayerList = new List<Player.Player>();
		public static byte PlayerId;
		public static bool Continue = false;
		#region Game Constants
		public static string GameVersion = "0.1";
		// 1st level : tuto, lore
		// 2nd level : forest
		// 3rd : Boss ?
		// 4th : Mine
		// 5th : Boss, again ?
		// 6th : Castle
		// 7th : FinalBoss
		//-public static string FirstLevelName = "Quentin6"; // will not delete it as it serves the same purpose and can be useful for debugging
		public static string[] LevelsName = new[] {"QuentinFirstLevelIntro","LVL1_Finale","BossForet","Mine","BossMine", "Chateau", "vrai_chateau","FinalBoss"};
		public static byte CurrentLevelId = 0;
		// public static string SecondLevelName = ???; // if needed, maybe we'll keep everything into one scene
		public static string GameOverSceneName = "GameOver";
		public static string GameClearSceneName = "GameClear";
		public static string LobbySceneName = "Lobby";
		// 0 = en, 1 = fr, 2 = jp
		public static byte Language = 0;
		#endregion
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