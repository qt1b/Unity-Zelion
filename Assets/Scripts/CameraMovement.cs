using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // camera smoothness
    private GameObject target;
    // public float smoothValue;
    // the higher it is, more time it will take to follow the player
    public float smoothTime;
    private Vector3 velocity = Vector3.zero;

    public float playerDistance;

    private Player _tarVelo;
    // camera bounding
    // public Vector2 maxPosition;
    // public Vector2 minPosition;

    // LastUpdate is called once per frame, after all updates were executed
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        _tarVelo = target.GetComponent<Player>();
    }

    void LateUpdate()
    {
        var position = target.transform.position;
        var position1 = transform.position;
        if (position1 == position) return;
        Vector3 desiredPosition = new Vector3(position.x + _tarVelo.change.x * playerDistance, 
            position.y + _tarVelo.change.y * playerDistance, position1.z);

        // forces the position to be between these two limits
        // desiredPosition.x = /*Mathf.Clamp(*/desiredPosition.x;//, minPosition.x, maxPosition.x);
        // desiredPosition.y = /*Mathf.Clamp(*/desiredPosition.y;//, minPosition.y, maxPosition.y);

        position1 = Vector3.SmoothDamp(position1, desiredPosition, ref velocity, smoothTime);
        transform.position = position1;
    }
}
