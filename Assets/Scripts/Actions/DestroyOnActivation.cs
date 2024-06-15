using Interfaces;
using UnityEngine;

namespace Actions {
	public class DestroyOnActivation: MonoBehaviour, IAction {
		public void Activate() {
			Destroy(gameObject);
		}
	}
}