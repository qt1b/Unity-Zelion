using System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;

namespace Global {
	public class GlobalVars : MonoBehaviourPunCallbacks, IPunObservable {
		// to sync over network
		// public static float PlayerSpeed = 1;
		public static float EnnemySpeed = 1;
		public static byte SaveId = 0; // useful for debugging, will be 0 in the end
		public static short DeathCount = 0;
		public static short GameOverCount = 0;
		// the INITIAL player count
		public static byte NbrOfPlayers = 1;

		// first dim : Level
		// second dim : Xpos ,etc...
		public static readonly int[,][] SaveLookupArray2 = new int[7,11][] {
			{ // level 0 -- TESTING VALUES, for the FOREST
				new [] {-15,-5}, // x pos
				new [] {-5,-5}, // y pos
				new [] {20,20}, // Life
				new [] {20,20}, // stamina
				new [] {20,20}, // mana
				new [] {1,1}, // sword unlocked
				new [] {1,1}, // bow unlocked
				new [] {1,1}, // poison
				new [] {1,1}, // dash
				new [] {1,1}, // slowdown
				new [] {1,1} // goBackInTime
			},
			{ // level 0 -- TESTING VALUES, for the MINE
				new [] {-5,-5}, // x pos
				new [] {-5,-5}, // y pos
				new [] {20,20}, // Life
				new [] {20,20}, // stamina
				new [] {20,20}, // mana
				new [] {1,1}, // sword unlocked
				new [] {1,1}, // bow unlocked
				new [] {1,1}, // poison
				new [] {1,1}, // dash
				new [] {1,1}, // slowdown
				new [] {1,1} // goBackInTime
			},
			{ // level 2
				new [] {0,0}, // x pos
				new [] {0,0}, // y pos
				new [] {6,6}, // Life
				new [] {2,2}, // stamina
				new [] {2,2}, // mana
				new [] {0,1}, // sword unlocked
				new [] {0,0}, // bow unlocked
				new [] {0,0}, // poison
				new [] {0,0}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
			{ // level 3
				new [] {0,0}, // x pos
				new [] {0,0}, // y pos
				new [] {6,6}, // Life
				new [] {2,2}, // stamina
				new [] {2,2}, // mana
				new [] {0,1}, // sword unlocked
				new [] {0,0}, // bow unlocked
				new [] {0,0}, // poison
				new [] {0,0}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
			{ // level 4
				new [] {0,0}, // x pos
				new [] {0,0}, // y pos
				new [] {6,6}, // Life
				new [] {2,2}, // stamina
				new [] {2,2}, // mana
				new [] {0,1}, // sword unlocked
				new [] {0,0}, // bow unlocked
				new [] {0,0}, // poison
				new [] {0,0}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
			{ // level 5
				new [] {0,0}, // x pos
				new [] {0,0}, // y pos
				new [] {6,6}, // Life
				new [] {2,2}, // stamina
				new [] {2,2}, // mana
				new [] {0,1}, // sword unlocked
				new [] {0,0}, // bow unlocked
				new [] {0,0}, // poison
				new [] {0,0}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
			{ // level 6
				new [] {0,0}, // x pos
				new [] {0,0}, // y pos
				new [] {6,6}, // Life
				new [] {2,2}, // stamina
				new [] {2,2}, // mana
				new [] {0,1}, // sword unlocked
				new [] {0,0}, // bow unlocked
				new [] {0,0}, // poison
				new [] {0,0}, // dash
				new [] {0,0}, // slowdown
				new [] {0,0} // goBackInTime
			},
		};
		public static DateTime TimeStartedAt;
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
		public static string[] LevelsName = new[] {  "LVL1_Finale","Mine","QuentinFirstLevelIntro",  "Quentin6"};
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