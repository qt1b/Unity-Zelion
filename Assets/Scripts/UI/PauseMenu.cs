using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI {
    public class PauseMenu : MonoBehaviour
    {
        static public bool GameIsPaused = false;
        [FormerlySerializedAs("GameObjectUI")] public GameObject gameObjectUI;
    

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape)){
                if(GameIsPaused){
                    Resume();
                }
                else{
                    Pause();
                }
            }
        }

        public void Resume(){
            gameObjectUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
        public void Pause(){
            gameObjectUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }

        public void MainMenu(){
            SceneManager.LoadScene(0);
            GameIsPaused = false;
        }

        public void Settings(){
            Debug.Log("settings ");
        }
    }
}
