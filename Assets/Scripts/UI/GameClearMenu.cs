using System;
using System.Collections.Generic;
using Audio;
using Global;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Demos.PunBasics_Tutorial.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

// add settings ? if needed ?
namespace UI {
	public class GameClearMenu : MonoBehaviourPunCallbacks {
		public TMP_Text TimerText;
		public TMP_Text CongratsText;
		public TMP_Text RestartText;
		public TMP_Text MainMenuText;
		public TMP_Text ExitText;
		
		void Awake() {
			TimerText.text = TextValues.GameClearText(UIOperations.FormatTime());
		}

		void Start() {
			CongratsText.text = TextValues.Congratulations;
			RestartText.text = TextValues.RestartBeginning;
			MainMenuText.text = TextValues.MainMenu;
			ExitText.text = TextValues.Exit;
		}
		public void Click() {
			AudioManager.Instance.Play("click2");
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
			GlobalVars.TimeStartedAt = null;
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
			GlobalVars.TimeStartedAt = null;
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