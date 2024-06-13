iusing System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;

namespace Global {
	public class GlobalVars : MonoBehaviourPunCallbacks, IPunObservable {
		// to sync over network
		public static float PlayerSpeed = 1;
		public static float EnnemySpeed = 1;
		public static byte SaveId = 1; // useful for debugging, will be 0 in the end
		public static short DeathCount = 0;
		public static short GameOverCount = 0;
		// the INITIAL player count
		public static byte NbrOfPlayers = 1;
		// public static string RoomName; // not sure that is useful tbh
		// static
		// better to convert it into a 2-dimensional array if we're doing it like this ?
		public static readonly string SaveLookupData = "X Position; Y Position; Life; Stamina; Mana; Unlocked Sword; Unlocked Bow; Unlocked Poison; Unlocked Dash; Unlocked Slowdown; Unlocked TimeFreeze\n" +
		                                               "0;0;6;2;2;0;0;0;0;0;0\n" +
		                                               "0;0;12;16;16;1;1;1;1;1;1";
		// allows us to be sure there is the right amount of data
		public static readonly string[,] SaveLookupArray = new string[2,11] {
			{ "0", "0", "6", "2", "2", "0", "0", "0", "0", "0", "0" },
			{ "22", "-22", "12", "16", "16", "1", "1", "1", "1", "1", "1" }
		};
		public static DateTime TimeStartedAt;
		// not to be synced, maybe to put in player ?
		public static List<Player.Player> PlayerList = new List<Player.Player>();
		public static int PlayerId;
		public static bool Continue = false;
		#region Game Constants
		public static string GameVersion = "0.1";
		public static string FirstLevelName = "LVL1";
		// public static string SecondLevelName = ???; // if needed, maybe we'll keep everything into one scene
		public static string GameOverSceneName = "GameOver";
		public static string GameClearSceneName = "GameClear";
		public static string LobbySceneName = "Lobby";
		// 0 = en, 1 = fr, 2 = jp
		public static byte Language = 1;
		#endregion
		#region IPunObservable
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				stream.SendNext(PlayerSpeed);
				stream.SendNext(EnnemySpeed);
				//stream.SendNext(PlayerList);
				stream.SendNext(SaveId);
			}
			else {
				PlayerSpeed = (float)stream.ReceiveNext();
				EnnemySpeed = (float)stream.ReceiveNext();
				// cannot sync this, do we even really need it to be synced ?
				// PlayerList = (List<Player.Player>)stream.ReceiveNext();
				SaveId = (byte)stream.ReceiveNext();
				// PlayerId = (int)stream.ReceiveNext();
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