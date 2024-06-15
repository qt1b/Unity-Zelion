using System;
using Global;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;

namespace Player {
	public class DevCheat : MonoBehaviour {
		public bool enable;
		private void Awake() {
			Debug.developerConsoleEnabled = enable;
			this.enabled = enable;
		}
		private void Update() {
			if (Input.GetKeyDown(KeyCode.Backslash)) {
				GlobalVars.CurrentLevelId = 4;
				PhotonNetwork.LoadLevel(GlobalVars.LevelsName[4]);
			}
			else if (Input.GetKeyDown(KeyCode.RightBracket)) {
				GlobalVars.CurrentLevelId = 3;
				PhotonNetwork.LoadLevel(GlobalVars.LevelsName[3]);
			}
			else if (Input.GetKeyDown(KeyCode.LeftBracket)) {
				GlobalVars.CurrentLevelId = 2;
				PhotonNetwork.LoadLevel(GlobalVars.LevelsName[2]);
			}
			else if (Input.GetKeyDown(KeyCode.P)) {
				GlobalVars.CurrentLevelId = 1;
				PhotonNetwork.LoadLevel(GlobalVars.LevelsName[1]);
			}
			else if (Input.GetKeyDown(KeyCode.O)) {
				GlobalVars.CurrentLevelId = 0;
				PhotonNetwork.LoadLevel(GlobalVars.LevelsName[0]);
			}
		}
	}
}