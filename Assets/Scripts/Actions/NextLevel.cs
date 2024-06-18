using Global;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using PUN;
using UnityEngine;

namespace Actions {
	public class NextLevel : MonoBehaviourPunCallbacks, IAction {
		public void Activate() {
			// TODO : Comment
			//Debug.Log("next level activated");
			//Debug.Log("current level id ="+GlobalVars.CurrentLevelId);
			//Debug.Log("next level id ="+GlobalVars.CurrentLevelId+1+", max level ID: "+GlobalVars.LevelsName.Length);
			if (GlobalVars.CurrentLevelId + 1 >= GlobalVars.LevelsName.Length) {
				GameManager.Instance.LoadLevel(GlobalVars.GameClearSceneName);
			}
			else {
				GameManager.Instance.LoadLevel(GlobalVars.LevelsName[GlobalVars.CurrentLevelId+1]);
			}
		}
	}
}