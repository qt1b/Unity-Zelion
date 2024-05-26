using System;
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
		public static string FormatTime()
		{
			TimeSpan res = DateTime.UtcNow - Global.GlobalVars.TimeStartedAt;
			// idk if the format is valid, may be worth to do some checks to cleanup the output
			return String.Format("{0:%h} hours {0:%m} minutes and {0:%s} seconds", res);
			// return res.ToString("hh':'mm':'ss"); // 00:03:48
		}
	}
}