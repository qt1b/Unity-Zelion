using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class UIOperations {
		public static void ActivateButton(Button button) {
			button.enabled = true;
			button.GetComponent<TMP_Text>().color = Color.white;
			// change colors and stuff
		}

		public static void DisableButton(Button button) {
			button.enabled = false;
			button.GetComponent<TMP_Text>().color = new Color(0.2f, 0.2f, 0.2f);
			// same
		}
	}
}