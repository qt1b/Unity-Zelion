using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Actions {
	public class WriteSave: MonoBehaviour {
		public byte SaveID;

		public void Activate() {
			TimeVariables.PlayerList.ForEach(p=>p.saveID = SaveID);
			// how to activate this locally on each client ?
			//ActivateClientRpc();
		}

		/*[ClientRpc]
		private void ActivateClientRpc() {
			Global.SaveManager.Save(SaveID);
		} */
	}
}