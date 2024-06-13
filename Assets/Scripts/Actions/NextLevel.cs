using Global;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using PUN;
using UnityEngine;

namespace Actions {
	public class NextLevel : MonoBehaviourPunCallbacks, IAction {
		public void Activate() {
			Debug.Log("next level activated");
			GlobalVars.CurrentLevelId += 1;
			GlobalVars.SaveId = 0;
			Debug.Log("current level id ="+GlobalVars.CurrentLevelId);
			if (GlobalVars.CurrentLevelId == GlobalVars.LevelsName.Length) {
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[GlobalVars.CurrentLevelId]);
			}
			else {
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[GlobalVars.CurrentLevelId]);
			}
		}
	}
}