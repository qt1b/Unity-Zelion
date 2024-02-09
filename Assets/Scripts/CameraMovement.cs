using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovoment : MonoBehaviour
{
    // camera smoothness
    public Transform target;
    public float smoothValue;
    // camera bounding
    public Vector2 maxPosition;
    public Vector2 minPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // LastUpdate is called once per frame, after all updates were executed
    void LateUpdate()
    {
        if (transform.position != target.position) {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // forces the position to be between these two limits
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosition.x, maxPosition.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.x, minPosition.y, maxPosition.y);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothValue);
        }
    }
}
