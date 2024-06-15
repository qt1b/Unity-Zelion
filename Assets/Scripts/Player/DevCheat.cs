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
				GlobalVars.CurrentLevelId = 4;
				GlobalVars.SaveId = 0;
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[4]);
			}
			else if (Input.GetKeyDown(KeyCode.RightBracket)) {
				GlobalVars.CurrentLevelId = 3;
				GlobalVars.SaveId = 0;
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[3]);
			}
			else if (Input.GetKeyDown(KeyCode.LeftBracket)) {
				GlobalVars.CurrentLevelId = 2;
				GlobalVars.SaveId = 0;
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[2]);
			}
			else if (Input.GetKeyDown(KeyCode.P)) {
				GlobalVars.CurrentLevelId = 1;
				GlobalVars.SaveId = 0;
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[1]);
			}
			else if (Input.GetKeyDown(KeyCode.O)) {
				GlobalVars.CurrentLevelId = 0;
				GlobalVars.SaveId = 0;
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[0]);
			}
		}
	}
}