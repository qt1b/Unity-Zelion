using Global;
using Photon.PhotonUnityNetworking.Code;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
	public class GameOverMenu : MonoBehaviourPunCallbacks
	{
		public TMP_Text TimerText;
		void Awake() {
			GlobalVars.GameOverCount += 1;
			TimerText.text = TextValues.GameOverText(UIOperations.FormatTime());
		}
		// WARING : play From Last Checkpoint !
		public void PlayAgain() {
			photonView.RPC("PlayAgainRPC",RpcTarget.MasterClient);
		}
		[PunRPC]
		public void PlayAgainRPC() {
			// photonView.RPC("LoadDataRPC",RpcTarget.AllBuffered); //may not be necessary ?
			PhotonNetwork.LoadLevel(GlobalVars.LevelsName[GlobalVars.CurrentLevelId]);
		}
		[PunRPC]
		public void LoadDataRPC() {
			// GlobalVars.SaveId = 0;
			Player.Player.LocalPlayerInstance.GetComponent<Player.Player>().LoadSave();
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