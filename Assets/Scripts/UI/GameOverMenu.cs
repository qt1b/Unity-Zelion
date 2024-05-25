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
			TimerText.text = $"Time elapsed {UIOperations.FormatTime()}.\n"+
			                 $"Number of deaths : {GlobalVars.DeathCount}" +
			                 $"Number of Game Overs : {GlobalVars.GameOverCount}"+
			                 $"Number of players : {GlobalVars.NbrOfPlayers}";
		}
		// WARING : play From Last Checkpoint !
		public void PlayAgain() {
			photonView.RPC("PlayAgainRPC",RpcTarget.MasterClient);
		}
		[PunRPC]
		public void PlayAgainRPC() {
			photonView.RPC("LoadDataRPC",RpcTarget.AllBuffered);
			PhotonNetwork.LoadLevel("Quentin5");
		}
		[PunRPC]
		public void LoadDataRPC() {
			// GlobalVars.SaveId = 0;
			Player.Player.LocalPlayerInstance.GetComponent<Player.Player>().LoadSave();
		}
		public void MainMenu(){
			PhotonNetwork.Destroy(Player.Player.LocalPlayerInstance);
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