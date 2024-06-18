using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
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
		public TMP_InputField RoomIdInput;
		public TMP_Text RoomNameText;
		public Button StartGameButton;
		public GameObject BeforeLobby;
		public GameObject Loading;
		public GameObject InsideLobby;

		public TMP_Text BackButton1;
		public TMP_Text CreateLobby;
		public TMP_Text JoinLobby;
		public TMP_Text LoadingText;
		public TMP_Text BackButton2;
		public TMP_Text InputText;

		#endregion

		#region Private Fields
		private string _roomName = "";
		private bool _creatingRoom;
		private bool sound_done;
		#endregion
		#region MonoBehaviour
		private void Awake() {
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.OfflineMode = false;
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = GlobalVars.GameVersion;
		}

		public void Start() {
			BackButton1.text = TextValues.Back;
			BackButton2.text = TextValues.Back;
			CreateLobby.text = TextValues.CreateLobby;
			JoinLobby.text = TextValues.JoinLobby;
			InputText.text = TextValues.RoomName + "...";
			LoadingText.text = TextValues.Loading;
		}

		// after
		#endregion

		#region Public Functions

		public void Click() {
			AudioManager.Instance.Play("click2");
		}
		
		public static string GenerateRoomName() {
			string roomName = Random.Range(0,10000).ToString("0000");
			//Debug.Log($"generated RoomName:{roomName}");
			return roomName;
		}
		public void JoinRoom() {
			if (RoomIdInput.text.Length != 4 || !RoomIdInput.text.ToCharArray().All(char.IsDigit)) return;
			if (!sound_done) {
				AudioManager.Instance.Play("loading1");
				sound_done = true;
			}
			//Debug.Log("trying to join room, id:"+RoomIdInput.text);
			_roomName = RoomIdInput.text;
			PhotonNetwork.JoinRoom(_roomName);
			BeforeLobby.SetActive(false);
			Loading.SetActive(true);
		}
		// TODO : fix the bug that requires us to click on the button two times to create a room
		public void CreateRoom() {
			_creatingRoom = true;
			if (!sound_done) {
				AudioManager.Instance.Play("loading1");
				sound_done = true;
			}
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
				AudioManager.Instance.Play("loading2");
				PhotonNetwork.LoadLevel(GlobalVars.LevelsName[0]);
			}
		}
		public void ExitLobby() {
			// Debug.Log("exiting room");
			PhotonNetwork.LeaveRoom();
			//InsideLobby.SetActive(false);
			//Loading.SetActive(false);
			//BeforeLobby.SetActive(true);
			sound_done = false;
			_roomName = "";
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
			else if (_roomName.Length == 4) JoinRoom();
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
			GlobalVars.PlayerId = (byte)(PhotonNetwork.CurrentRoom.PlayerCount <= 0 ? 0 : PhotonNetwork.CurrentRoom.PlayerCount - 1);
			Debug.Log($"joining room with id:{RoomIdInput.text}");
			Debug.Log("Joined Room : current player ID:"+GlobalVars.PlayerId);
			PhotonNetwork.JoinRoom(RoomIdInput.text);
			Loading.SetActive(false);
			InsideLobby.SetActive(true);
			RoomNameText.SetText($"{TextValues.RoomName}:{PhotonNetwork.CurrentRoom.Name}");
			StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
			// depends on the language and the number of players
			if (GlobalVars.Language == 0) {
				StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text =
					$"Start a {PhotonNetwork.CurrentRoom.PlayerCount} player game";
			}
			else if (GlobalVars.Language == 1) {
				StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text =
					$"Jeu à {PhotonNetwork.CurrentRoom.PlayerCount} joueur" + (PhotonNetwork.CurrentRoom.PlayerCount > 1 ?"s":"");
			}
			else if (GlobalVars.Language == 2) {
				StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text =
					$"{PhotonNetwork.CurrentRoom.PlayerCount}人で始める。";
			}
			else throw new ArgumentException("invalid language value");
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