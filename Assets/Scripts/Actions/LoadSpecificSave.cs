using Interfaces;
using UnityEngine;

namespace Actions {
	public class LoadSpecificSave : MonoBehaviour, IAction {
		// WITHOUT changing the Var !
		public byte saveID;

		public void Activate() {
			Player.Player.LocalPlayerInstance.GetComponent<Player.Player>().LoadSpecificSave(saveID);
		}
	}
}