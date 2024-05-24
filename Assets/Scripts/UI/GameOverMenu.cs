using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class GameOverMenu : MonoBehaviour
    {
        public void MainMenu(){
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
