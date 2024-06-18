using System;
using Bars;
using Global;
using Photon.PhotonUnityNetworking.Code;
using PUN;
using Unity.VisualScripting;
using UnityEngine;

namespace Player {
	public class DevCheat : MonoBehaviour {
		public bool enable;
		private void Awake() {
			Debug.developerConsoleEnabled = false;
			this.enabled = enable;
		}
		private void Update() {
			if (PhotonNetwork.IsConnectedAndReady) {
				if (Input.GetKeyDown(KeyCode.Backslash)) { // level 3.2 (6)
					GlobalVars.CurrentLevelId = 6;
					GameManager.Instance.LoadLevel(GlobalVars.LevelsName[6]);
				}
				else if (Input.GetKeyDown(KeyCode.RightBracket)) { // level 3 (5)
					GlobalVars.CurrentLevelId = 5;
					GameManager.Instance.LoadLevel(GlobalVars.LevelsName[5]);
				}
				else if (Input.GetKeyDown(KeyCode.Quote)) { // level 3.5 (7)
					GlobalVars.CurrentLevelId = 7;
					GameManager.Instance.LoadLevel(GlobalVars.LevelsName[7]);
				}
				else if (Input.GetKeyDown(KeyCode.LeftBracket)) { // level 2 (3) : mine
					GlobalVars.CurrentLevelId = 3;
					GameManager.Instance.LoadLevel(GlobalVars.LevelsName[3]);
				}
				else if (Input.GetKeyDown(KeyCode.Semicolon)) { // level 2.5 (4) : mine_boss
					GlobalVars.CurrentLevelId = 4;
					GameManager.Instance.LoadLevel(GlobalVars.LevelsName[4]);
				}
				else if (Input.GetKeyDown(KeyCode.O)) { // level 0: the tutorial
					GlobalVars.CurrentLevelId = 0; // game manager will +1 it
					GameManager.Instance.LoadLevel(GlobalVars.LevelsName[0]);
				}
				else if (Input.GetKeyDown(KeyCode.P)) { // level 1 : forest
					GlobalVars.CurrentLevelId = 1; // game manager will +1 it
					GameManager.Instance.LoadLevel(GlobalVars.LevelsName[1]);
				}
				else if (Input.GetKeyDown(KeyCode.L)) { // level 1.5 (2) : forest_boss
					GlobalVars.CurrentLevelId = 2;
					GameManager.Instance.LoadLevel(GlobalVars.LevelsName[2]);
				}
				else if (Input.GetKeyDown(KeyCode.Slash)) {
					var (oldsave, oldcurrent) = (GlobalVars.SaveId, GlobalVars.CurrentLevelId);
					GlobalVars.CurrentLevelId = 1;
					GlobalVars.SaveId = 1;
					Player.LocalPlayerInstance.GetComponent<Player>().LoadSaveWithoutPos();
					(GlobalVars.SaveId,GlobalVars.CurrentLevelId) = (oldsave,oldcurrent);
				}
				/*
				else if (Input.GetKeyDown(KeyCode.Slash)) {
					Player p = Player.LocalPlayerInstance.GetComponent<Player>();
					p.GetComponentInChildren<HealthBar>().ChangeMaxValue((ushort)short.MaxValue);
					p.GetComponentInChildren<StaminaBar>().ChangeMaxValue((ushort)short.MaxValue);
					p.GetComponentInChildren<ManaBar>().ChangeMaxValue((ushort)short.MaxValue);
				}
				else if (Input.GetKeyDown(KeyCode.Period)) {
					Player.LocalPlayerInstance.GetComponent<Player>().LoadSaveWithoutPos();
				}
				else if (Input.GetKeyDown(KeyCode.Minus)) {
					Player.LocalPlayerInstance.GetComponent<Player>().InstaKill(false);
				}
				else if (Input.GetKeyDown(KeyCode.Equals)) {
					Player.LocalPlayerInstance.GetComponent<Player>().InstaKill(true);
				} */
				/* else if (Input.GetKeyDown(KeyCode.O)) { // LEVEL 0 : NO HOTKEYS AS THE SAVE SYSTEM IS meh
	GameManager.Instance.LoadLevel(GlobalVars.LevelsName[0]);
	GlobalVars.CurrentLevelId = 0; // may load the values of the level 1, DO NOT USE IT
} */
			}
		}
	}
}