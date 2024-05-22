using System.Collections;
using Interfaces;
using UnityEngine;

namespace Events {
	public class SurviveForSecs {
		public IAction Action;
		public float Secs = 30f;

		IEnumerator ActivateAfterSecs() {
			yield return new WaitForSeconds(Secs);
			Action.Activate();
		}
	}
}