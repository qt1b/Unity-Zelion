using System;
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
			TimerText.text = $"You cleared the game in {UIOperations.FormatTime()}.\n"+
			                 $"Number of deaths : {GlobalVars.DeathCount}" +
			                 $"Number of Game Overs : {GlobalVars.GameOverCount}"+
			                 $"Number of players : {GlobalVars.NbrOfPlayers}";
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
			GlobalVars.TimeStartedAt = DateTime.Now;
			photonView.RPC("LoadDataRPC",RpcTarget.AllBuffered);
			PhotonNetwork.LoadLevel(GlobalVars.FirstLevelName);
		}
		[PunRPC]
		public void LoadDataRPC() {
			// GlobalVars.SaveId = 0;
			Player.Player.LocalPlayerInstance.GetComponent<Player.Player>().LoadSave();
		}
		public void MainMenu(){
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene(0);
			Destroy(Player.Player.LocalPlayerInstance);
			Player.Player.LocalPlayerInstance = null;
		}
		public void ExitGame() {
			PhotonNetwork.Disconnect();
			Application.Quit();
		}
	}
}