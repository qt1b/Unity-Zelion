using System;
using Global;
using Photon.PhotonUnityNetworking.Code;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// add settings ? if needed ?
namespace UI {
	public class GameClearMenu : MonoBehaviour {
		public TMP_Text TimerText;
		void Awake() {
			GlobalVars.GameOverCount += 1;
			TimerText.text = $"You cleared the game in {UIOperations.FormatTime()}.\n"+
			                 $"Number of deaths : {GlobalVars.DeathCount}" +
			                 $"Number of Game Overs : {GlobalVars.GameOverCount}"+
			                 $"Number of players : {GlobalVars.NbrOfPlayers}";
		}
		// WARNING : play From The Beginning !
		public void PlayAgain() {
			Global.GlobalVars.SaveId = 0; // from the beginning
			GlobalVars.DeathCount = 0;
			GlobalVars.NbrOfPlayers = (byte)PhotonNetwork.CurrentRoom.PlayerCount;
			GlobalVars.GameOverCount = 0;
			PhotonNetwork.LoadLevel("Quentin5");
		}
		public void MainMenu(){
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene(0);
		}
		public void ExitGame() {
			Application.Quit();
		}
	}
}