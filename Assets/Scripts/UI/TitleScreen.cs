using System;
using System.Collections.Generic;
using Global;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Demos.PunBasics_Tutorial.Scripts;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
	public class TitleScreen : MonoBehaviourPunCallbacks {
		// or start ?
		private void Awake() {
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.OfflineMode = false;
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = GlobalVars.GameVersion;
		}

		private bool _soloPlay = false;
		public void StartSinglePlayer() {
			_soloPlay = true;
			PhotonNetwork.Disconnect();
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

		public override void OnDisconnected(DisconnectCause cause) {
			if (_soloPlay) {
				PhotonNetwork.OfflineMode = true;
				PhotonNetwork.CreateRoom("soloGaming" /*DateTime.UtcNow.ToBinary().ToString()*/);
				PhotonNetwork.NickName = Environment.UserName;
				PhotonNetwork.GameVersion = GlobalVars.GameVersion;
				GlobalVars.PlayerList = new List<Player.Player>();
				GlobalVars.TimeStartedAt = DateTime.UtcNow;
				PhotonNetwork.LoadLevel(GlobalVars.FirstLevelName);
			}
			else Awake();
		}
	}
}