using System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;

namespace Global {
	public class GlobalVars : MonoBehaviourPunCallbacks, IPunObservable {
		// to sync over network
		public static float PlayerSpeed = 1;
		public static float EnnemySpeed = 1;
		public static byte SaveId = 0;
		public static short DeathCount = 0;
		public static short GameOverCount = 0;
		public static byte NbrOfPlayers = 1;
		// public static string RoomName; // not sure that is useful tbh
		// static
		public static readonly string SavePath = "Zelion.sav";
		public static readonly string SaveLookupPath = "Assets/Resources/SaveLookupTable.csv";
		public static DateTime TimeStartedAt;
		// not to be synced, maybe to put in player ?
		public static List<Player.Player> PlayerList = new List<Player.Player>();
		public static int PlayerId;
		public static bool Continue = false;
		#region Game Constants
		public static string GameVersion = "0.1";
		public static string FirstLevelName = "Quentin5";
		// public static string SecondLevelName = ???; // if needed, maybe we'll keep everything into one scene
		public static string GameOverSceneName = "GameOver";
		public static string GameClearSceneName = "GameClear";
		public static string LobbySceneName = "Lobby";
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
				PlayerId = (int)stream.ReceiveNext();
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