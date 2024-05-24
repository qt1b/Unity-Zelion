using Actions;
using Interfaces;
using UnityEngine;

namespace Events {
	public class OnContactTag: MonoBehaviour {
		public IAction Action;
		public string hitTag;

		void Awake() {
			Action = gameObject.GetComponent<IAction>();
		}
		private void OnTriggerEnter2D(Collider2D other) {
			if (other.CompareTag(hitTag)) {
				Action.Activate();
			}
		}
	}
}