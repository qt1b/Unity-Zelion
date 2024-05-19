using UnityEngine;
using UnityEngine.UI;

namespace Objects {
    public class Sign : MonoBehaviour
    {
        public GameObject dialogBox;
        public Text dialogText;
        public string dialog;
        private bool _playerInRange = false;
        private GameObject _questionMark;

        // script imports, for the player not being able to attack when in front of a sign
        // TODO : render the sign behind the player when y coordinates of the player are behind,
        // and in front of the player when the player is behind.

        // then we could use that on a lot of other elements like houses, trees...

        // Start is called before the first frame update
        void Start()
        {
            _questionMark = new GameObject();
            // Instantiate(questionMark);
            // QuestionMark questionMark = gameObject.AddComponent<QuestionMark>() as QuestionMark;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && _playerInRange) {
                // playerControl.questionMarkActive = false;
                if (dialogBox.activeInHierarchy) {
                    dialogBox.SetActive(false);
                    _questionMark.SetActive(true);
                }
                else {
                    dialogBox.SetActive(true);
                    dialogText.text = dialog;
                    _questionMark.SetActive(false);
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                _playerInRange = true;
                _questionMark = other.transform.GetChild(3).gameObject;
                _questionMark.SetActive(true);
                // playerControl.questionMarkActive = true;
                // playerControl.isBusy = true; // may not be THAT great of an idea
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                _playerInRange = false;
                dialogBox.SetActive(false);
                _questionMark.SetActive(false);
                // playerControl.questionMarkActive = false;
                // playerControl.isBusy = false;
            }
        }
    }
}
