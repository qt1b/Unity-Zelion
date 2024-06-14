using System;
using Global;
using TMPro;
using Unity.VisualScripting;
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
			return String.Format((res.Hours > 0 ? res.Hours + " " + TextValues.Hour + (res.Hours > 1 && GlobalVars.Language != 2 ? "s ":" ") : "")
			                     + (res.Minutes > 0 ? res.Minutes + " " + TextValues.Minute + (res.Minutes > 1 && GlobalVars.Language != 2 ? "s":"") : "") 
			                     + (res.Minutes > 0 || res.Hours > 0 ? TextValues.And  + " ": "" )
								+ (res.Seconds > 0 ? res.Seconds + " " + TextValues.Second + (res.Seconds > 1 && GlobalVars.Language != 2 ? "s":"") : ""));
		}
	}
}