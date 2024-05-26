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
	public class TitleScreen : MonoBehaviour {
		// public static GameObject GlobalVarsGo;
		void Start() {
			// idk if it even is useful, to try without ?
			/*if (GlobalVarsGo == null) {
				GlobalVarsGo = Instantiate(Resources.Load("Prefabs/Player/GlobalVars")) as GameObject;
				DontDestroyOnLoad(GlobalVarsGo);
			}*/
		}
		public void StartSinglePlayer() {
			PhotonNetwork.OfflineMode = true;
			//PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.CreateRoom("soloGaming"/*DateTime.UtcNow.ToBinary().ToString()*/);
			PhotonNetwork.NickName = Environment.UserName;
			PhotonNetwork.GameVersion = GlobalVars.GameVersion;
			GlobalVars.PlayerList = new List<Player.Player>();
			GlobalVars.TimeStartedAt = DateTime.UtcNow;
			PhotonNetwork.LoadLevel(GlobalVars.FirstLevelName);
			// Player.Player.LocalPlayerInstance = PhotonNetwork.Instantiate("Prefabs/Player/Player",Vector3.zero,Quaternion.identity);
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