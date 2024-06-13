using Actions;
using Interfaces;
using UnityEngine;

namespace Events {
	public class OnContactTag: MonoBehaviour {
		public string hitTag;
		
		private bool called;
		private void OnTriggerEnter2D(Collider2D other) {
			if (!called && other.CompareTag(hitTag) && gameObject.TryGetComponent(out IAction action)) {
				called = true;
				action.Activate();
				Debug.Log("+level");
			}
			this.enabled = false;
		}
	}
}