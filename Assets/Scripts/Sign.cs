using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour
{
    public GameObject dialogBox;
    public GameObject questionMark;
    public Text dialogText;
    public string dialog;
    private bool playerInRange = false;

    // script imports, for the player not being able to attack when in front of a sign
    // TODO : render the sign behind the player when y coordinates of the player are behind,
    // and in front of the player when the player is behind.

    // then we could use that on a lot of other elements like houses, trees...

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(questionMark);
        // QuestionMark questionMark = gameObject.AddComponent<QuestionMark>() as QuestionMark;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerInRange) {
            // playerControl.questionMarkActive = false;
            if (dialogBox.activeInHierarchy) {
                dialogBox.SetActive(false);
                questionMark.SetActive(true);
            }
            else {
                dialogBox.SetActive(true);
                dialogText.text = dialog;
                questionMark.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInRange = true;
            questionMark.SetActive(true);
            // playerControl.questionMarkActive = true;
            // playerControl.isBusy = true; // may not be THAT great of an idea
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInRange = false;
            dialogBox.SetActive(false);
            questionMark.SetActive(false);
            // playerControl.questionMarkActive = false;
            // playerControl.isBusy = false;
        }
    }
}
