using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;

namespace Global {
	public class GlobalVars : MonoBehaviourPunCallbacks, IPunObservable {
		// to sync over network
		public static float PlayerSpeed = 1;
		public static float EnnemySpeed = 1;
		public static List<Player.Player> PlayerList = new List<Player.Player>();
		public static byte SaveId = 0;
		public static readonly string SavePath = "Zelion.sav";
		public static readonly string SaveLookupPath = "Assets/Resources/SaveLookupTable.csv";
		public static bool Continue = false;
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				stream.SendNext(PlayerSpeed);
				stream.SendNext(EnnemySpeed);
				stream.SendNext(PlayerList);
			}
			else {
				PlayerSpeed = (float)stream.ReceiveNext();
				EnnemySpeed = (float)stream.ReceiveNext();
				PlayerList = (List<Player.Player>)stream.ReceiveNext();
			}
			throw new System.NotImplementedException();
		}
	}
}