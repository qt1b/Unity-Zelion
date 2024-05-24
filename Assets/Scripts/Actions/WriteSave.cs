using System;
using System.IO;
using Global;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using Unity.Netcode;
using UnityEngine;
using File = UnityEngine.Windows.File;

namespace Actions {
	public class WriteSave: MonoBehaviourPunCallbacks, IAction {
		public byte SaveID;

		public void Activate() {
			Global.GlobalVars.SaveId = SaveID;
			if (PhotonNetwork.IsMasterClient) {
				Global.SaveManager.Save();
			}
			else photonView.RPC("WriteFile",RpcTarget.MasterClient);
		}

		[PunRPC]
		private void WriteFile() => System.IO.File.OpenWrite(Global.GlobalVars.SavePath).WriteByte(SaveID);
	}
}