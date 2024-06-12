using Global;
using Interfaces;
using UnityEngine;
using Objects;
using UnityEngine.Serialization;

namespace Actions {
	public class ActivateSignByForce: MonoBehaviour, IAction {
		public byte saveID;
		public string dialog;
		public void Start() {
			if (saveID == GlobalVars.SaveId && gameObject.TryGetComponent(out Sign sign)) {
				if (dialog != "") sign.dialog = this.dialog;
				sign.ActivateDialog();
			}
		}

		public void Activate() {
			Start();
		}
	}
}