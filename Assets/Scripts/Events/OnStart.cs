using Interfaces;
using UnityEngine;

namespace Events {
	public class OnStart :MonoBehaviour {
		void Start() {
			if (gameObject.TryGetComponent(out IAction action)) {
				action.Activate();
			}
		}
	}
}