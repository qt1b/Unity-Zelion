using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Actions {
	public class WriteSave: NetworkBehaviour {
		public byte SaveID;

		public void Activate() {
			TimeVariables.PlayerList.Value.ForEach(p=>p.saveID = SaveID);
			// how to activate this locally on each client ?
			ActivateClientRpc();
		}

		[ClientRpc]
		private void ActivateClientRpc() {
			Global.SaveManager.Save(SaveID);
		}
	}
}