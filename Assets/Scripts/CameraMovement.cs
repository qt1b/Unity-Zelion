using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovoment : MonoBehaviour
{
    // camera smoothing
    public Transform target;
    public float smoothing;
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
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // forces the position to be between these two limits
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
