using System;
using System.Collections;
using Audio;
using Global;
using Interfaces;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Objects {
    public class Sign : MonoBehaviour {
        public GameObject dialogBox;
        private TMP_Text dialogText;
        private bool _validQuestionMark;
        public string dialog;
        private bool _playerInRange = false;
        private GameObject _questionMark;
            private static readonly int Interracting = Animator.StringToHash("interracting");
        public bool onlyOnce;
        private bool _interracted;
        private static string[] DialogSoundsArray = new[] { "dialogS1", "dialogS2", "dialogS3" };
        private static byte _unitsBeforeSound = 5;

        [CanBeNull]private Coroutine curText;
        // Start is called before the first frame update
        public void Start() {
            dialogText = dialogBox.GetComponentInChildren<TextMeshProUGUI>();
        }
        // Update is called once per frame
        void Update()
        { 
            if (Input.GetKeyDown(KeyCode.F) && _playerInRange && (!onlyOnce || !_interracted)) {
                // playerControl.questionMarkActive = false;
                if (dialogBox.activeInHierarchy) {
                    if (curText is not null) {
                        StopCoroutine(curText);
                        curText = null;
                        dialogText.text = TextValues.DialogsDict[dialog][GlobalVars.Language];
                    }
                    else {
                        DisableDialog();
                        if (gameObject.TryGetComponent(out IAction action)) {
                            action.Activate();
                        }
                        _questionMark.SetActive(!onlyOnce);
                        AudioManager.Instance.Play("closetxt");
                    }
                }
                else if (_validQuestionMark) {
                    ActivateDialog();
                }
            }
        }

        public void ActivateDialog() {
            dialogBox.SetActive(true);
            _playerInRange = true; // to enable it with ActivateSignByForce
            if (curText is not null)
                StopCoroutine(curText);
            curText = StartCoroutine(WriteText());
            _questionMark.SetActive(false);
            if (gameObject.TryGetComponent(out Animator animator)) animator.SetBool(Interracting,true);
        }

        private void DisableDialog() {
            _interracted = true;
            dialogBox.SetActive(false);
            if (curText is not null)
                StopCoroutine(curText);
            if (gameObject.TryGetComponent(out Animator animator)) animator.SetBool(Interracting,false);
        }
        public IEnumerator WriteText() {
            string[] dialogArray = TextValues.DialogsDict[dialog];
            dialogText.text = "";
            byte charCountBeforeSound = 0;
            foreach (char c in dialogArray[GlobalVars.Language]) {
                dialogText.text += c;
                if (charCountBeforeSound == 0) {
                    AudioManager.Instance.Play("text1"/*DialogSoundsArray[Random.Range(0, 3)]*/);
                    charCountBeforeSound = _unitsBeforeSound;
                }
                charCountBeforeSound--;
                yield return new WaitForSeconds(0.01f/* * GlobalVars.PlayerSpeed*/);
            }
            curText = null;
        } 

        void OnTriggerEnter2D(Collider2D other) {
            if (!other.isTrigger && other.CompareTag("Player") && (!onlyOnce || !_interracted) /*&& !dialogBox.activeInHierarchy*/) {
                _playerInRange = true;
                _validQuestionMark = true;
                _questionMark = other.transform.GetChild(3).gameObject;
                _questionMark.SetActive(true);
            }
        }


        private void OnTriggerExit2D(Collider2D other) {
            if (!other.isTrigger && _validQuestionMark && other.gameObject.CompareTag("Player")) {
                _playerInRange = false;
                if (dialogBox.activeInHierarchy) {
                    DisableDialog();
                }
                _questionMark.SetActive(false);
            }
        }
    }
}