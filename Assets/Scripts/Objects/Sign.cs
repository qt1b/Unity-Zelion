using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Objects {
    public class Sign : MonoBehaviour
    {
        public GameObject dialogBox;
        private TMP_Text dialogText;
        private bool _validQuestionMark;
        private Animator _animator;
        public string dialog;
        private bool _playerInRange = false;
        private GameObject _questionMark;
        private static readonly int Interracting = Animator.StringToHash("interracting");

        // script imports, for the player not being able to attack when in front of a sign
        // TODO : render the sign behind the player when y coordinates of the player are behind,
        // and in front of the player when the player is behind.

        // then we could use that on a lot of other elements like houses, trees...

        // Start is called before the first frame update
        public void Start() {
            _animator = gameObject.GetComponent<Animator>();
            dialogText = dialogBox.GetComponentInChildren<TextMeshProUGUI>();
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
                    _animator.SetBool(Interracting,false);
                }
                else if (_validQuestionMark) {
                    dialogBox.SetActive(true);
                    dialogText.text = dialog;
                    _questionMark.SetActive(false);
                    _animator.SetBool(Interracting,true);
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                _playerInRange = true;
                _validQuestionMark = true;
                _questionMark = other.transform.GetChild(3).gameObject;
                _questionMark.SetActive(true);
                // playerControl.questionMarkActive = true;
                // playerControl.isBusy = true; // may not be THAT great of an idea
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (_validQuestionMark && other.CompareTag("Player")) {
                _playerInRange = false;
                dialogBox.SetActive(false);
                _questionMark.SetActive(false);
                _animator.SetBool(Interracting,false);
                // playerControl.questionMarkActive = false;
                // playerControl.isBusy = false;
            }
        }
    }
}
