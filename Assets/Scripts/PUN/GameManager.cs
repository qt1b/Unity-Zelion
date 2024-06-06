using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Global;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using TMPro;
using Player = Photon.PhotonRealtime.Code.Player;

namespace PUN {
	public class GameManager : MonoBehaviourPunCallbacks {
		#region Public Fields
		public static GameManager Instance;
		public TMP_Text NetworkStatusText;
		#endregion

		#region MonoBehaviour
		private void Start() {
			Debug.Log("Game Manager : Starting ...");
			Instance = this; // useless for now
			// GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
			// decided not to keep the instance of the player between scenes for now
			// if (global::Player.Player.LocalPlayerInstance == null) {
					GlobalVars.PlayerList = new();
					if (Global.GlobalVars.GameOverCount == 0) Global.GlobalVars.TimeStartedAt = DateTime.UtcNow;
					Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
					// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
					PhotonNetwork.Instantiate("Prefabs/Player/Player", Vector3.zero, Quaternion.identity, 0);
			/*}
			else {
				Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
			}*/
			NetworkStatusText.SetText(GenerateNetworkStatusText());
		}
		#endregion
		#region Photon Callbacks

		public override void OnDisconnected(DisconnectCause cause) {
			Debug.LogError("DISCONNECTED !!");
			if (PhotonNetwork.OfflineMode is true) {
				Debug.LogError("Offline mode Activated, trying to reconnect ??");
				// do not Disconnect !!!
				// idk if it works in offline mode
				PhotonNetwork.ReconnectAndRejoin();
			}
		}

		/*
		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		*/
		public override void OnLeftRoom()
		{
			Debug.LogError("OnLeftRoom : Disconnected");
			SceneManager.LoadScene(0);
			global::Player.Player.LocalPlayerInstance = null;
			Destroy(gameObject); // destroys this very game manager
		}
		/*
		// are not being used ? idk
		*/
		public override void OnPlayerEnteredRoom(Photon.PhotonRealtime.Code.Player other)
		{
			Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
			NetworkStatusText.SetText(GenerateNetworkStatusText());
		}

		public override void OnPlayerLeftRoom(Photon.PhotonRealtime.Code.Player other)
		{
			Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
			NetworkStatusText.SetText(GenerateNetworkStatusText());
			/*
			if (!allowJoining) {
				PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
			}*/
		}
		#endregion

		#region Private Functions

		private string GenerateNetworkStatusText() {
			return
				$"RoomID: {PhotonNetwork.CurrentRoom.Name} | Players Connected: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers} | ID: {Global.GlobalVars.PlayerId} | Nickname: {PhotonNetwork.NickName}";
		}

		#endregion
	}
}