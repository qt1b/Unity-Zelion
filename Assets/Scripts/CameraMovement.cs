using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraMovement : NetworkBehaviour
{
    // the higher it is, more time it will take to follow the player
    public float smoothTime;
    private Vector3 velocity = Vector3.zero;

    public float playerDistance;

    public GameObject CameraHolder;

    private Player _tarVelo;
    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            CameraHolder.SetActive(false);
        }
    }
    private void Start()
    {
        
        _tarVelo = GetComponent<Player>();
    }

    void LateUpdate()
    {
        var position = transform.position;
        var position1 = CameraHolder.transform.position;
        if (position1 == position) return;
        Vector3 desiredPosition = new Vector3(position.x + _tarVelo.change.x * playerDistance, 
            position.y + _tarVelo.change.y * playerDistance, position1.z);

        // forces the position to be between these two limits
        // desiredPosition.x = /*Mathf.Clamp(*/desiredPosition.x;//, minPosition.x, maxPosition.x);
        // desiredPosition.y = /*Mathf.Clamp(*/desiredPosition.y;//, minPosition.y, maxPosition.y);

        position1 = Vector3.SmoothDamp(position1, desiredPosition, ref velocity, smoothTime);
        CameraHolder.transform.position = position1;
    }
}
