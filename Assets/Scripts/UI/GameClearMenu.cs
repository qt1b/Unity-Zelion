using System;
using Photon.PhotonUnityNetworking.Code;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
	public class GameClearMenu : MonoBehaviour {

		public TMP_Text TimerText;
		void Awake() {
			PhotonNetwork.Disconnect();
			TimerText.text = $"Took {FormatTime()}";
		}
		public void MainMenu(){
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene(0);
		}
		
		static string FormatTime()
		{
			TimeSpan res = DateTime.Now - Global.GlobalVars.TimeStartedAt;
			return res.ToString("hh':'mm':'ss"); // 00:03:48
		}
	}
}