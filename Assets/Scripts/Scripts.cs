using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

// TO Move and split elsewhere !

public class TimeVariables : NetworkBehaviour {
	// to sync over network with NetworkVariable
    public static float PlayerSpeed = 1;
    public static float EnnemySpeed = 1;
    public static List<Player.Player> PlayerList = new List<Player.Player>();
}

public class SaveData : NetworkBehaviour {
	// not sure about this network variable
	public static NetworkVariable<byte> SaveId = new NetworkVariable<byte>(0);
	public static string SavePath = "Zelion.sav";
	public static string SaveLookupPath = "Assets/Resources/SaveLookupTable.csv";
}

public class GlobalReferences : MonoBehaviour {
	public static NetworkManager NetworkManager;
	private void Start() {
		NetworkManager = FindObjectOfType<NetworkManager>();
	}
}