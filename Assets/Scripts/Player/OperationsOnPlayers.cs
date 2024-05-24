using Photon.PhotonUnityNetworking.Code;
using UnityEngine;

namespace Player {
	public class OperationsOnPlayers {
		public static void SetRightAnimSpeed() =>
			Global.GlobalVars.PlayerList.ForEach(p => p.GetComponent<Animator>().speed = Global.GlobalVars.PlayerSpeed);
	}
}