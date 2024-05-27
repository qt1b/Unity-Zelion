using System;
using Global;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Objects {
	public class CheckPoint : MonoBehaviourPunCallbacks {
		private Light2D _light2DBefore;
		private Light2D _light2DAfter;
		public byte SaveID;

		private void Start() {
			_light2DBefore = Resources.Load<Light2D>("Light2D/Light 2D Checkpoint Before");
			_light2DBefore = Resources.Load<Light2D>("Light2D/Light 2D Checkpoint After");
		}

		// next : sync the new light, etc...
		private void OnTriggerEnter2D(Collider2D other) {
			if (other.TryGetComponent(out Player.Player player)) {
				photonView.RPC("PlayerLoadRPC", RpcTarget.AllBuffered);
			}
		}

		private void PlayerLoadRPC(Player.Player player) {
			GlobalVars.SaveId = SaveID;
			// next : load without position ?
			player.LoadSaveWithoutPos();
		}
}
}