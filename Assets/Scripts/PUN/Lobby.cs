using System;
using Global;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Player = Photon.PhotonRealtime.Code.Player;

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
			Debug.Log("trying to join room, id:"+RoomIdInput.text);
			PhotonNetwork.JoinRoom(RoomIdInput.text);
		}
		// TODO : fix the bug that requires us to click on the button two times to create a room
		public void CreateRoom() {
			GenerateRoomName();
			Debug.Log("trying to join room, id:"+GlobalVars.RoomName);
			PhotonNetwork.CreateRoom(GlobalVars.RoomName,new RoomOptions(){MaxPlayers = 4});
		}
		public void LoadTitleScreen() {
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene("TitleScene5PUN");
		}
		public void StartGameMulti() {
			if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("Quentin5");
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

		public override void OnCreatedRoom() {
			Debug.Log($"created room with id:{PhotonNetwork.CurrentRoom.Name}");
			StartGameButton.gameObject.SetActive(true);
			StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text =
				$"Start a {PhotonNetwork.CurrentRoom.PlayerCount} player game";
		}

		public override void OnJoinedRoom() {
			PhotonNetwork.NickName = Environment.UserName;
			// assigns the right id to the right player
			GlobalVars.PlayerId = PhotonNetwork.CurrentRoom.PlayerCount-1;
			Debug.Log($"joining room with id:{RoomIdInput.text}");
			Debug.Log("Joined Room : current player ID:"+GlobalVars.PlayerId);
			PhotonNetwork.JoinRoom(RoomIdInput.text);
			BeforeLobby.SetActive(false);
			InsideLobby.SetActive(true);
			RoomNameText.SetText($"RoomID:{PhotonNetwork.CurrentRoom.Name}");
			if (PhotonNetwork.IsMasterClient) StartGameButton.gameObject.SetActive(true);
			else StartGameButton.gameObject.SetActive(false);
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

		public override void OnPlayerEnteredRoom(Photon.PhotonRealtime.Code.Player player) {
			Debug.Log("Other players joined the room.");
			if (PhotonNetwork.IsMasterClient) {
				StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text =
					$"Start a {PhotonNetwork.CurrentRoom.PlayerCount} player game";
			}
		}

		// public override void OnConnectedToMaster() { }
		#endregion
	}
}