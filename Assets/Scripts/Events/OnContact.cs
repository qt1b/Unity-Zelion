using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

namespace Events {
	public class OnContact : MonoBehaviour{
		public List<Type> Types;
		public IAction Action;

		private void OnTriggerEnter2D(Collider2D other) {
			if (Types.Any(t => t == other.GetType())) {
				Action.Activate();
			}
		}
	}
}