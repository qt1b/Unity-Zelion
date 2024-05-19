using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class MainMenu : MonoBehaviour {
    
        private NetworkManager _networkManager;

        public void Start() {
            _networkManager = FindObjectOfType<NetworkManager>();
        }

        public void PlaySolo() {
            _networkManager.StartHost();
            // disable ports, etc
            LoadFirstScene();
        }

        public void StartHostLan() {
            // set ports to local
            _networkManager.StartHost();
            LoadFirstScene();
        }    
        public void StartClientLan() {
            // set ports to local
            _networkManager.StartClient();
            LoadFirstScene();
        }
        public void StartClientOnline() {
            // connect to an online service
            _networkManager.StartClient();
            LoadFirstScene();
        }
        public void StartHostOnline() {
            // connect to an online service
            _networkManager.StartHost();
            LoadFirstScene();
        }
        void LoadFirstScene() {
            SceneManager.LoadScene(1);
        }
        public void ExitGame() {
            Application.Quit();
        }
    }
}
