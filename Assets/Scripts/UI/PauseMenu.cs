using System.Collections;
using System.Collections.Generic;
using Global;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI {
    // todo : sync the pause menu over the network
    public class PauseMenu : MonoBehaviourPunCallbacks {
        public static bool GameIsPaused = false;
        [FormerlySerializedAs("GameObjectUI")] public GameObject gameObjectUI;
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsAimingBow = Animator.StringToHash("IsAimingBow");

        public TMP_Text PausedTxt;
        public TMP_Text ResumeTxt;
        public TMP_Text SettingsTxt;
        public TMP_Text MainMenuTxt;
        
        public void Start() {
            PausedTxt.text = TextValues.Paused;
            ResumeTxt.text = TextValues.Resume;
            SettingsTxt.text = TextValues.Settings;
            MainMenuTxt.text = TextValues.MainMenu;
        }
        
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                if(GameIsPaused){
                    Resume();
                }
                else{
                    Pause();
                }
            }
        }
        
        public void Resume() {
            photonView.RPC("ResumeRpc", RpcTarget.AllBuffered);
        }
        [PunRPC]
        public void ResumeRpc() {
            GameIsPaused = false;
            gameObjectUI.SetActive(false);
            if (PhotonNetwork.OfflineMode) {
                Time.timeScale = 1f; // wth this existed all along
            }
            else {
                Player.Player.LocalPlayerInstance.GetComponent<Animator>().speed = GlobalVars.PlayerSpeed;
            }
        }
        
        public void Pause() {
            photonView.RPC("PauseRpc",RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void PauseRpc() {
            GameIsPaused = true;
            gameObjectUI.SetActive(true);
            if (PhotonNetwork.OfflineMode) {
                Time.timeScale = 0f;
            }
            else {
                Animator animator = Player.Player.LocalPlayerInstance.GetComponent<Animator>();
                animator.speed = 0f;
                animator.SetBool(IsMoving,false);
                animator.SetBool(IsAimingBow,false);
                Player.Player.LocalPlayerInstance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
        public void MainMenu(){
            Time.timeScale = 1f; // wth this existed all along
            GameIsPaused = false;
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }

        public void Settings(){
            Debug.Log("settings ");
        }
    }
}
