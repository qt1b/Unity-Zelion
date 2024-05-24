using System;
using Global;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
	public class TitleScreen : MonoBehaviour {
		public void StartSinglePlayer() {
			PhotonNetwork.OfflineMode = true;
			PhotonNetwork.JoinRandomOrCreateRoom();
			PhotonNetwork.NickName = Environment.UserName;
			PhotonNetwork.GameVersion = GlobalVars.GameVersion;
			PhotonNetwork.LoadLevel("Quentin5");
		}
		public void ExitGame() {
			Application.Quit();
		}

		// now unused, as the save will only be used for checkpoints
		//public void SetContinue() => Global.GlobalVars.Continue = true;
		//public void NewGame() => Global.GlobalVars.Continue = false;

		public void LoadLobby() {
			SceneManager.LoadScene("Scenes/Lobby");
		}
	}
}