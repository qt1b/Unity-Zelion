using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTranfer : MonoBehaviour
{
    public Vector2 cameraChangemin;
    public Vector2 cameraChangemax;
    public Vector3 playerChange;
    private CameraMovement cam;

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
        }
    }
}
