using Global;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

// this SHOULD do the job for the checkpoints ? I hope so
namespace Actions {
	public class ActivateSavePoint : MonoBehaviourPunCallbacks, IAction {
		public byte saveID;

		void Start() {
			if (saveID <= GlobalVars.SaveId) {
				photonView.RPC("NetworkDestroyLightRpc", RpcTarget.AllBuffered);
				this.enabled = false;
			}
		}
		public void Activate() {
			if (GlobalVars.SaveId != saveID) {
				GlobalVars.SaveId = this.saveID;
				photonView.RPC("NetworkDestroyLightRpc", RpcTarget.AllBuffered);
				this.enabled = false;
			}
		}

		[PunRPC]
		public void NetworkDestroyLightRpc() {
			Debug.Log("Loading Save...");
			Destroy(gameObject.GetComponentInChildren<Light2D>().gameObject);
			// instantiates the light 2d and sets the current gameobject as its parent
			Instantiate(Resources.Load("Light2D/Light 2D Checkpoint After"), gameObject.transform);
			// loads the save, hopefully it is synced before this
			// just in case
			GlobalVars.SaveId = this.saveID;
			Player.Player.LocalPlayerInstance.GetComponent<Player.Player>().LoadSaveWithoutPos();
		}
	}
}