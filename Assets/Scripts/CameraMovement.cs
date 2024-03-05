using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // camera smoothness
    public Transform target;
    // public float smoothValue;
    // the higher it is, more time it will take to follow the player
    public float smoothTime;
    private Vector3 velocity = Vector3.zero;
    // camera bounding
    // public Vector2 maxPosition;
    // public Vector2 minPosition;

    // LastUpdate is called once per frame, after all updates were executed
    void LateUpdate()
    {
        if (transform.position != target.position) {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // forces the position to be between these two limits
            // desiredPosition.x = /*Mathf.Clamp(*/desiredPosition.x;//, minPosition.x, maxPosition.x);
            // desiredPosition.y = /*Mathf.Clamp(*/desiredPosition.y;//, minPosition.y, maxPosition.y);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }
}
