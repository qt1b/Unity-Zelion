using Interfaces;
using Photon.PhotonUnityNetworking.Code;

namespace Actions {
	public class ChangeScene : MonoBehaviourPunCallbacks, IAction {
		public string sceneName;

		public void Activate() {
			if (PhotonNetwork.IsMasterClient) {
				PhotonNetwork.LoadLevel(sceneName);
			}
			else photonView.RPC("ChangeSceneRPC",RpcTarget.MasterClient);
		}
		[PunRPC]
		private void ChangeSceneRPC() {
			PhotonNetwork.LoadLevel(sceneName);
		}
	}
}