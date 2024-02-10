using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomTranfer : MonoBehaviour
{
    public Vector2 cameraChangemin;
    public Vector2 cameraChangemax;
    public Vector3 playerChange;

    // script imports
    private CameraMovement cam;

    public bool needText;
    public string placeName;
    public GameObject text;
    public Text placeText;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            cam.minPosition += cameraChangemin;
            cam.maxPosition += cameraChangemax;
            other.transform.position += playerChange;
            if (needText) {
                StartCoroutine(placeNameCo());
            }
        }
    }

    // co for coroutine
    private IEnumerator placeNameCo() {
        text.SetActive(true);
        placeText.text = placeName;
        yield return new WaitForSeconds(4f);
        text.SetActive(false);
    }
}
