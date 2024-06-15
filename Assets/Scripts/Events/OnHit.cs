using Interfaces;
using UnityEngine;

namespace Events {
	public class OnHit : MonoBehaviour, IHealth {
		private IAction _action;
		void Awake() {
			_action = gameObject.GetComponent<IAction>();
		}
		public void TakeDamages(ushort damage) {
			_action.Activate();
		}
		public void Heal(ushort heal) { }
	}
}