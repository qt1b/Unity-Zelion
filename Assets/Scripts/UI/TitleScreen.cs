using System;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
	public class TitleScreen : MonoBehaviour {
		public void StartSinglePlayer() {
			PhotonNetwork.OfflineMode = true;
			PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 1});
			PhotonNetwork.NickName = Environment.UserName;
			PhotonNetwork.LoadLevel("Quentin5");
		}
		public void ExitGame() {
			Application.Quit();
		}

		public void SetContinue() => Global.GlobalVars.Continue = true;
		public void NewGame() => Global.GlobalVars.Continue = false;

		public void LoadLobby() {
			SceneManager.LoadScene("Scenes/Lobby");
		}
	}
}