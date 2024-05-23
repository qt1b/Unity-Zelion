using System;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PUN {
	public class Lobby : MonoBehaviour{
		#region Public Fields
		public Button JoinButton;
		public InputField RoomIdInput;
		#endregion
		
		#region MonoBehaviour
		// after
		#endregion

		#region Public Functions

		public static string GenerateRoomName() {
			string roomName = "";
			for (int i = 0; i < 4; i++) {
				roomName += Random.Range(0,11);
			}
			Global.GlobalVars.RoomName = roomName;
			return roomName;
		}
		public void JoinRoom() {
			if (RoomIdInput.text.Length != 4) return;
			PhotonNetwork.JoinRoom(RoomIdInput.text);
		}
		public void SetRoomName(string value) {
			PhotonNetwork.CreateRoom(GenerateRoomName());
			// could be customized, but eh
			PhotonNetwork.NickName = Environment.UserName;
		}

		#endregion
	}
}