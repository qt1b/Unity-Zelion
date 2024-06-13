using Global;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;

namespace Actions {
	public class NextLevel : MonoBehaviour, IAction {
		public void Activate() {
			Debug.Log("next level activated");
			GlobalVars.CurrentLevelId += 1;
			GlobalVars.SaveId = 0;
			Debug.Log("current level id ="+GlobalVars.CurrentLevelId);
			if (GlobalVars.CurrentLevelId == GlobalVars.LevelsName.Length) {
				PhotonNetwork.LoadLevel(GlobalVars.GameClearSceneName);
				return;
			}
			else {
				PhotonNetwork.LoadLevel(GlobalVars.LevelsName[GlobalVars.CurrentLevelId]);
				return;
			}
		}
	}
}