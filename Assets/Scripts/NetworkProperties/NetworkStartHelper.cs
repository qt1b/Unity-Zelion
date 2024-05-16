using UnityEngine;
using Unity.Netcode;

public class NetworkStartHelper : MonoBehaviour
{
    private NetworkManager _networkManager;
    // to test the scene more easily, without having to start the server everytime
    void Awake() {
        _networkManager = gameObject.GetComponent<NetworkManager>();
        if (!(_networkManager.IsHost || _networkManager.IsClient || _networkManager.IsServer)) _networkManager.StartHost();
    }
}
