using System;
using Global;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PUN {
	public class Lobby : MonoBehaviourPunCallbacks {
		// TODO : change more UI elements, in function of if the player is masterClient, what it can do, etc
		// does not use photon's lobby system, may be worth the try ?
		#region Public Fields
		public Button JoinButton;
		public TMP_InputField RoomIdInput;
		public TMP_Text RoomNameText;
		public Button StartGameButton;
		public GameObject BeforeLobby;
		public GameObject InsideLobby;
		#endregion
		#region MonoBehaviour
		private void Awake() {
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = GlobalVars.GameVersion;
		}
		// after
		#endregion

		#region Public Functions
		public static string GenerateRoomName() {
			string roomName = Random.Range(0,10000).ToString("0000");
			GlobalVars.RoomName = roomName;
			Debug.Log(roomName);
			return roomName;
		}
		public void JoinRoom() {
			if (RoomIdInput.text.Length != 4) return;
			Debug.Log($"joining room with id:{RoomIdInput.text}");
			PhotonNetwork.JoinRoom(RoomIdInput.text);
			BeforeLobby.SetActive(false);
			InsideLobby.SetActive(true);
			RoomNameText.SetText($"RoomID:{PhotonNetwork.CurrentRoom.Name}");
			StartGameButton.gameObject.SetActive(false);
		}
		public void CreateRoom() {
			GenerateRoomName();
			PhotonNetwork.CreateRoom(GlobalVars.RoomName,new RoomOptions(){MaxPlayers = 4});
			BeforeLobby.SetActive(false);
			InsideLobby.SetActive(true);
			Debug.Log($"creating room with id:{PhotonNetwork.CurrentRoom.Name}");
			RoomNameText.SetText($"RoomID:{PhotonNetwork.CurrentRoom.Name}");
			StartGameButton.gameObject.SetActive(true);
		}
		public void LoadTitleScreen() {
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene("TitleScene5PUN");
		}
		public void StartGameMulti() {
			if (PhotonNetwork.IsConnected) PhotonNetwork.LoadLevel("Quentin5");
		}
		public void ExitLobby() {
			Debug.Log("exiting lobby");
			PhotonNetwork.LeaveRoom();
		}

		public void TrimAndActivate(string str) {
			if (str.Length > 4) {
				str = str[new Range(new Index(0), new Index(4))];
			}
			RoomIdInput.text = str.Trim();
			//UI.UIOperations.DisableButton(JoinButton);
		}
		#endregion

		#region Pun Callbacks
		public override void OnJoinedRoom() {
			PhotonNetwork.NickName = Environment.UserName;
			// assigns the right id to the right player
			GlobalVars.PlayerId = PhotonNetwork.CurrentRoom.PlayerCount-1;
			Debug.Log("Joined Room : current player ID:"+GlobalVars.PlayerId);
		}
		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
			Debug.Log($"return code:{returnCode}, message:{message}");
			BeforeLobby.SetActive(true);
			InsideLobby.SetActive(false);
		}

		public override void OnCreateRoomFailed(short returnCode, string message) {
			Debug.Log("PUN Basics Tutorial/Launcher:OnCreateRoomFailed() was called by PUN. Retrying...");
			Debug.Log($"return code:{returnCode}, message:{message}");
			CreateRoom();
		}

		// public override void OnConnectedToMaster() { }
		#endregion
	}
}