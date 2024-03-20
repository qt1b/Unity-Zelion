using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public float X;
    public float Y;
    public string hitTag;
    private Camera _camera;
    void Start() {
        _camera = Camera.main;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(hitTag)) {
            other.transform.position = new Vector3(X,Y,0f);
            _camera.transform.position = new Vector3(X,Y,_camera.transform.position.z);
        }
    }
}
