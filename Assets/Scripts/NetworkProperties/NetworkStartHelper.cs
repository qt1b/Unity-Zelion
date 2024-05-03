using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class NetworkStartHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake() {
        NetworkManager networkManager = gameObject.GetComponent<NetworkManager>();
        networkManager.StartHost();
    }


}
