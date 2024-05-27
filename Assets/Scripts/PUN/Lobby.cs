using System;
using System.Collections.Generic;
using System.Linq;
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
		public GameObject Loading;
		public GameObject InsideLobby;
		#endregion

		#region Private Fields
		private string _roomName;
		private bool _creatingRoom;
		#endregion
		#region MonoBehaviour
		private void Awake() {
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.OfflineMode = false;
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = GlobalVars.GameVersion;
		}

		// after
		#endregion

		#region Public Functions
		public static string GenerateRoomName() {
			string roomName = Random.Range(0,10000).ToString("0000");
			//Debug.Log($"generated RoomName:{roomName}");
			return roomName;
		}
		public void JoinRoom() {
			if (RoomIdInput.text.Length != 4 || !RoomIdInput.text.ToCharArray().All(char.IsDigit)) return;
			Debug.Log("trying to join room, id:"+RoomIdInput.text);
			PhotonNetwork.JoinRoom(RoomIdInput.text);
			BeforeLobby.SetActive(false);
			Loading.SetActive(true);
		}
		// TODO : fix the bug that requires us to click on the button two times to create a room
		public void CreateRoom() {
			_creatingRoom = true;
			_roomName = GenerateRoomName();
			Debug.Log("trying to join room, id:"+_roomName);
			PhotonNetwork.CreateRoom(_roomName,new RoomOptions(){MaxPlayers = 4});
			BeforeLobby.SetActive(false);
			Loading.SetActive(true);
		}
		public void LoadTitleScreen() {
			PhotonNetwork.Disconnect(); // should also work without it
			SceneManager.LoadScene(0);
		}
		public void StartGameMulti() {
			if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
				PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
				PhotonNetwork.LoadLevel(GlobalVars.FirstLevelName);
			}
		}
		public void ExitLobby() {
			Debug.Log("exiting room");
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

		// dirty but works
		public override void OnConnectedToMaster() {
			Debug.Log("Lobby: OnConnectedToMaster");
			if (_creatingRoom) CreateRoom();
			else JoinRoom();
		}

		public override void OnCreatedRoom() {
			Debug.Log("Lobby: OnCreatedRoom");
			Debug.Log($"created room with id:{PhotonNetwork.CurrentRoom.Name}");
			_creatingRoom = false;
		}

		public override void OnJoinedRoom() {
			Debug.Log("Lobby: OnJoinedRoom");
			PhotonNetwork.NickName = Environment.UserName;
			// assigns the right id to the right player
			GlobalVars.PlayerId = PhotonNetwork.CurrentRoom.PlayerCount-1;
			Debug.Log($"joining room with id:{RoomIdInput.text}");
			Debug.Log("Joined Room : current player ID:"+GlobalVars.PlayerId);
			PhotonNetwork.JoinRoom(RoomIdInput.text);
			Loading.SetActive(false);
			InsideLobby.SetActive(true);
			RoomNameText.SetText($"RoomID:{PhotonNetwork.CurrentRoom.Name}");
			StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
			StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text =
				$"Start a {PhotonNetwork.CurrentRoom.PlayerCount} player game";
		}
		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.Log("Lobby: OnJoinRoomFailed()");
			Debug.Log($"return code:{returnCode}, message:{message}");
			BeforeLobby.SetActive(true);
			Loading.SetActive(false);
			InsideLobby.SetActive(false);
		}

		public override void OnCreateRoomFailed(short returnCode, string message) {
			Debug.Log("Lobby: OnCreateRoomFailed(). Retrying...");
			Debug.Log($"return code:{returnCode}, message:{message}");
			CreateRoom();
		}

		// allows us to change something when on player joins the room
		public override void OnPlayerEnteredRoom(Photon.PhotonRealtime.Code.Player player) {
			Debug.Log("Lobby: OnPlayerEnteredRoom()");
			Debug.Log("Other players joined the room.");
			if (PhotonNetwork.IsMasterClient) {
				StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text =
					$"Start a {PhotonNetwork.CurrentRoom.PlayerCount} player game";
			}
		}

		#endregion

		/*
		#region Pun RPC

		public void StartGameRPC() {
			// is not a good idea, could be done from gameManager
			PhotonNetwork.Instantiate("Prefabs/Player/Player", Vector3.zero, Quaternion.identity);
			GlobalVars.TimeStartedAt = DateTime.UtcNow;
			GlobalVars.PlayerList = new List<Player.Player>();
			GlobalVars.SaveId = 0; // no save if we start from the lobby
		}
		#endregion
		*/
	}
}