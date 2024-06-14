using System;
using System.Collections.Generic;
using Global;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Demos.PunBasics_Tutorial.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// add settings ? if needed ?
namespace UI {
	public class GameClearMenu : MonoBehaviourPunCallbacks {
		public TMP_Text TimerText;
		void Awake() {
			TimerText.text = TextValues.GameClearText(UIOperations.FormatTime());
		}
		// WARNING : play From The Beginning !
		// behaves incorrectly
		public void PlayAgain() {
			photonView.RPC("PlayAgainRPC",RpcTarget.MasterClient);
		}
		[PunRPC]
		public void PlayAgainRPC() {
			GlobalVars.SaveId = 0; // from the beginning
			GlobalVars.DeathCount = 0;
			GlobalVars.NbrOfPlayers = (byte)PhotonNetwork.CurrentRoom.PlayerCount;
			GlobalVars.GameOverCount = 0;
			GlobalVars.TimeStartedAt = DateTime.UtcNow;
			// photonView.RPC("LoadDataRPC",RpcTarget.AllBuffered); // may not be necessary ??
			PhotonNetwork.LoadLevel(GlobalVars.LevelsName[GlobalVars.CurrentLevelId]);
		}
		[PunRPC]
		public void LoadDataRPC() {
			GlobalVars.SaveId = 0;
			// Player.Player.LocalPlayerInstance.GetComponent<Player.Player>().LoadSave();
		}
		public void MainMenu(){
			//PhotonNetwork.Destroy(Player.Player.LocalPlayerInstance);
			//Player.Player.LocalPlayerInstance = null;
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene(0);
		}
		public void ExitGame() {
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.Disconnect();
			Application.Quit();
		}
	}
}