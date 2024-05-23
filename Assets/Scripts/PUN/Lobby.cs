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
		#region Public Fields
		public Button JoinButton;
		public TMP_InputField RoomIdInput;
		public TMPro.TMP_Text RoomNameText;
		public Button StartGameButton;
		public GameObject BeforeLobby;
		public GameObject InsideLobby;
		#endregion
		
		#region MonoBehaviour

		private void Awake() {
			PhotonNetwork.AutomaticallySyncScene = true;
		}

		// after
		#endregion

		#region Public Functions

		public static string GenerateRoomName() {
			string roomName = "";
			for (int i = 0; i < 4; i++) {
				roomName += Random.Range(0,11);
			}
			Global.GlobalVars.RoomName = roomName;
			return roomName;
		}
		public void JoinRoom() {
			if (RoomIdInput.text.Trim().Length != 4) return;
			PhotonNetwork.JoinRoom(RoomIdInput.text.Trim());
			BeforeLobby.SetActive(false);
			InsideLobby.SetActive(true);
			RoomNameText.text = $"RoomID:{PhotonNetwork.CurrentRoom.Name}";
		}
		public void CreateRoom() {
			PhotonNetwork.CreateRoom(GenerateRoomName(),new RoomOptions(){MaxPlayers = 4});
			BeforeLobby.SetActive(false);
			InsideLobby.SetActive(true);
			RoomNameText.text = $"RoomID:{PhotonNetwork.CurrentRoom.Name}";
		}

		public void LoadTitleScreen() {
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene("TitleScene5PUN");
		}
		public void StartGameMulti() {
			PhotonNetwork.LoadLevel("Quentin5");
		}

		public void ExitLobby() {
			Debug.Log("exiting lobby");
			PhotonNetwork.Disconnect();
		}

		public void TrimAndActivate(string str) {
			if (str.Length > 4) {
				RoomIdInput.text = str.Substring(0, 4);
				UI.UIOperations.ActivateButton(JoinButton);
			}
			else if (str.Length == 4) {
				RoomIdInput.text = str;
				UI.UIOperations.ActivateButton(JoinButton);
			}
			else {
				UI.UIOperations.DisableButton(JoinButton);
			}
		}
		
		#endregion

		#region Pun Callbacks

		public override void OnJoinedRoom() {
			PhotonNetwork.NickName = Environment.UserName;
			// hopefully assigns the right id to the right player
			GlobalVars.PlayerId = PhotonNetwork.CurrentRoom.PlayerCount;
			Debug.Log("current player ID:"+GlobalVars.PlayerId);
		}
		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
			Debug.Log($"return code:{returnCode}, message:{message}");
			BeforeLobby.SetActive(true);
			InsideLobby.SetActive(false);
		}
		#endregion
	}
}