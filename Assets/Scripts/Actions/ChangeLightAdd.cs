using Events;
using Global;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Actions {
	public class ChangeLightAdd : MonoBehaviourPun, IAction {
		public OnNbrOfInstances addEvent;
		private static readonly int Activate1 = Animator.StringToHash("activate");

		// could be better just to use an animator;
		public void Activate() {
			if (gameObject.TryGetComponent(out Animator animator) && !animator.GetBool(Activate1)) {
				animator.SetBool(Activate1, true);
				addEvent.current+=1;
			}
		}
	}
}