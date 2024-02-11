using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public string dialog;
    private bool dialogActive;
    private bool playerInRange = false;

    // script imports, for the player not being able to attack when in front of a sign
    public GameObject playerRef;
    private MovePlayer playerControl;

    // TODO : render the sign behind the player when y coordinates of the player are behind,
    // and in front of the player when the player is behind.

    // then we could use that on a lot of other elements like houses, trees...

    // Start is called before the first frame update
    void Start()
    {
        playerControl = playerRef.GetComponent<MovePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerInRange) {
            playerControl.questionMarkActive = false;
            if (dialogBox.activeInHierarchy) {
                dialogBox.SetActive(false);
            }
            else {
                dialogBox.SetActive(true);
                dialogText.text = dialog;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInRange = true;
            playerControl.questionMarkActive = true;
            playerControl.isBusy = true; // may not be THAT great of an idea
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInRange = false;
            dialogBox.SetActive(false);
            playerControl.questionMarkActive = false;
            playerControl.isBusy = false;
        }
    }
}
