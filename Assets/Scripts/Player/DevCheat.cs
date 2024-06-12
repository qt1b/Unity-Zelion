using System;
using Global;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;

namespace Player {
	public class DevCheat : MonoBehaviour {
		private void Update() {
			if (Input.GetKeyDown(KeyCode.Alpha0)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[4]);
			else if (Input.GetKeyDown(KeyCode.Alpha9)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[3]);
			else if (Input.GetKeyDown(KeyCode.Alpha8)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[2]);
			else if (Input.GetKeyDown(KeyCode.Alpha7)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[1]);
			else if (Input.GetKeyDown(KeyCode.Alpha6)) PhotonNetwork.LoadLevel(GlobalVars.LevelsName[0]);
		}
	}
}