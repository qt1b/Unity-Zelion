using System.Collections;
using Global;
using TMPro;
using UnityEngine;
namespace Objects {
    public class Sign : MonoBehaviour {
        public GameObject dialogBox;
        private TMP_Text dialogText;
        private bool _validQuestionMark;
        private Animator _animator;
        public string dialog;
        private bool _playerInRange = false;
        private GameObject _questionMark;
        private static readonly int Interracting = Animator.StringToHash("interracting");
        public bool onlyOnce;
        private bool _interracted;
        // Start is called before the first frame update
        public void Start() {
            _animator = gameObject.GetComponent<Animator>();
            dialogText = dialogBox.GetComponentInChildren<TextMeshProUGUI>();
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && _playerInRange && (!onlyOnce || !_interracted)) {
                // playerControl.questionMarkActive = false;
                if (dialogBox.activeInHierarchy) {
                    _interracted = true;
                    dialogBox.SetActive(false);
                    _questionMark.SetActive(true);
                    _animator.SetBool(Interracting,false);
                }
                else if (_validQuestionMark) {
                    dialogBox.SetActive(true);
                    StartCoroutine(WriteText());
                    _questionMark.SetActive(false);
                    _animator.SetBool(Interracting,true);
                }
            }
        }

        IEnumerator WriteText() {
            string str = TextValues.DialogsDict[dialog][GlobalVars.Language];
            dialogText.text = "";
            foreach (char c in str) {
                dialogText.text += c;
                yield return new WaitForSeconds(0.03f * GlobalVars.PlayerSpeed);
                // audiomgr...
            }
        } 

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player") && (!onlyOnce || !_interracted)) {
                _playerInRange = true;
                _validQuestionMark = true;
                _questionMark = other.transform.GetChild(3).gameObject;
                _questionMark.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (_validQuestionMark && other.CompareTag("Player")) {
                _playerInRange = false;
                dialogBox.SetActive(false);
                _questionMark.SetActive(false);
                _animator.SetBool(Interracting,false);
            }
        }
    }
}