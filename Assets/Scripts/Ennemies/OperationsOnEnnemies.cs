using UnityEngine;

namespace Ennemies {
	public class OperationsOnEnnemies : MonoBehaviour {
		public void SetEnnemyAnimatorSpeed() {
			foreach (var health in FindObjectsOfType<Health>()) {
				if (health.gameObject.TryGetComponent(out Animator animator)) {
					animator.speed = Global.GlobalVars.EnnemySpeed;
				}
			}
		}
	}
}