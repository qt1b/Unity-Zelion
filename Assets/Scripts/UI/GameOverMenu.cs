using Global;
using Photon.PhotonUnityNetworking.Code;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class GameOverMenu : MonoBehaviour
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
            PhotonNetwork.LoadLevel("Quentin5");
        }
        public void MainMenu(){
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
        public void ExitGame() {
            PhotonNetwork.Disconnect();
            Application.Quit();
        }
    }
}
