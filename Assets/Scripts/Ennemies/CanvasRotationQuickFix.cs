using System;
using UnityEngine;

namespace Ennemies {
	public class CanvasRotationQuickFix : MonoBehaviour {
		private Quaternion _rot = Quaternion.Euler(-90,0,0);
		private void Update() {
			gameObject.transform.rotation = _rot;
		}
	}
}