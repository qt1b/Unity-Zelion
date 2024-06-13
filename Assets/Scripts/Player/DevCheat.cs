using System;
using Global;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;

namespace Player {
	public class DevCheat : MonoBehaviour {
		// TODO : Do detach for the final game !!!
		private void Awake() {
			Debug.developerConsoleEnabled = false;
		}
		private void Update() {
			if (Input.GetKeyDown(KeyCode.Backslash)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[4]);
			else if (Input.GetKeyDown(KeyCode.RightBracket)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[3]);
			else if (Input.GetKeyDown(KeyCode.LeftBracket)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[2]);
			else if (Input.GetKeyDown(KeyCode.P)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[1]);
			else if (Input.GetKeyDown(KeyCode.O)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[0]);
		}
	}
}