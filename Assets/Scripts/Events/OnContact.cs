using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

namespace Events {
	public class OnContact : MonoBehaviour{
		public Type Type;
		public IAction Action;

		private void OnTriggerEnter2D(Collider2D other) {
			if (Type == other.GetType()) {
				Action.Activate();
			}
		}
	}
}