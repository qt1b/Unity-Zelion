using Interfaces;
using UnityEngine;

namespace Events {
	public class OnHit : MonoBehaviour, IHealth {
		public IAction Action;
		void Awake() {
			Action = gameObject.GetComponent<IAction>();
		}
		public void TakeDamages(uint damage) {
			Action.Activate();
		}

		// do nothing
		public void Heal(uint heal) { }
	}
}