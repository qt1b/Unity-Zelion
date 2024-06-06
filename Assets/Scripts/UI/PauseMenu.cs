using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI {
    // todo : sync the pause menu over the network
    public class PauseMenu : MonoBehaviourPunCallbacks
    {
        public static bool GameIsPaused = false;
        [FormerlySerializedAs("GameObjectUI")] public GameObject gameObjectUI;
       /* private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsAimingBow = Animator.StringToHash("IsAimingBow");
        private static readonly int IsAimingBomb = Animator.StringToHash("IsAimingBomb"); */
        // Update is called once per frame
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
        public void Resume(){
            photonView.RPC("ResumeRpc",RpcTarget.AllBuffered);
        }
        [PunRPC]
        public void ResumeRpc(){
            gameObjectUI.SetActive(false);
            Time.timeScale = 1f; // wthhhhhhhh this existed all along
            GameIsPaused = false;
        }
        public void Pause() {
            photonView.RPC("PauseRpc",RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void PauseRpc() {
            /* Animator animator = Player.Player.LocalPlayerInstance.GetComponent<Animator>();
            animator.SetBool(IsMoving,false);
            animator.SetBool(IsAimingBow,false);
            animator.SetBool(IsAimingBomb,false); */
            gameObjectUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
        public void MainMenu(){
            //PhotonNetwork.Destroy(Player.Player.LocalPlayerInstance);
            //Player.Player.LocalPlayerInstance = null;
            // no way do "destroy" a room
            Time.timeScale = 1f; // wthhhhhhhh this existed all along
            GameIsPaused = false;
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
            // GameIsPaused = false;
        }

        public void Settings(){
            Debug.Log("settings ");
        }
    }
}
