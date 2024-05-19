using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * wth is this : this file will be used to load global variables, handle saves, and all of that stuff
 *
 *
 * 
 */

public class TimeVariables : NetworkBehaviour
{
	// to sync over network with NetworkVariable
    public static NetworkVariable<float> PlayerSpeed = new NetworkVariable<float>(1);
    public static NetworkVariable<float> EnnemySpeed = new NetworkVariable<float>(1);
}

public class SaveData {
	// to implement
}

public class GlobalReferences : MonoBehaviour {
	public static NetworkManager networkManager;
	private void Start() {
		networkManager = FindObjectOfType<NetworkManager>();
	}
}