using System.Collections;
using Audio;
using Global;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using Objects;
using TMPro;
using UnityEngine.Serialization;

namespace Actions {
	public class ActivateDialog : MonoBehaviour, IAction {
		public string dialog;
		public GameObject dialogBox;
        private TMP_Text dialogText;
        private static byte _unitsBeforeSound = 5;
        [CanBeNull]private Coroutine curText;
        
        public void Start() {
            dialogText = dialogBox.GetComponentInChildren<TextMeshProUGUI>();
        }
		public void Activate() {
			activateDialog();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F)) {
                if (dialogBox.activeInHierarchy) {
                    if (curText is not null) {
                        StopCoroutine(curText);
                        curText = null;
                        dialogText.text = TextValues.DialogsDict[dialog][GlobalVars.Language];
                    }
                    else {
                        DisableDialog();
                        AudioManager.Instance.Play("closetxt");
                        Destroy(this.gameObject);
                    }
                }
            }
        }

        public void activateDialog() {
            dialogBox.SetActive(true);
            if (curText is not null)
                StopCoroutine(curText);
            curText = StartCoroutine(WriteText());
        }

        private void DisableDialog() {
            dialogBox.SetActive(false);
            if (curText is not null)
                StopCoroutine(curText);
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
                yield return new WaitForSeconds(0.01f * GlobalVars.PlayerSpeed);
            }
            curText = null;
        } 

        /*
        void OnTriggerEnter2D(Collider2D other) {
            if (!other.isTrigger && other.CompareTag("Player") && (!onlyOnce || !_interracted ) {
                _playerInRange = true;
            }
        }


        private void OnTriggerExit2D(Collider2D other) {
            if (!other.isTrigger && other.gameObject.CompareTag("Player")) {
                _playerInRange = false;
                if (dialogBox.activeInHierarchy) {
                    DisableDialog();
                }
            }
        } */
    }
}