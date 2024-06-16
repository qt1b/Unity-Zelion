using System;
using Global;
using Photon.PhotonUnityNetworking.Code;
using PUN;
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
				GlobalVars.CurrentLevelId = 3;
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[4]);
			}
			else if (Input.GetKeyDown(KeyCode.RightBracket)) {
				GlobalVars.CurrentLevelId = 2;
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[3]);
			}
			else if (Input.GetKeyDown(KeyCode.LeftBracket)) {
				GlobalVars.CurrentLevelId = 1;
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[2]);
			}
			else if (Input.GetKeyDown(KeyCode.P)) {
				GlobalVars.CurrentLevelId = 0; // game manager will +1 it
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[1]);
			}
			else if (Input.GetKeyDown(KeyCode.O)) {
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[0]);
				GlobalVars.CurrentLevelId = 0; // may load the values of the level 1, DO NOT USE IT
			}
		}
	}
}