using Photon.Pun;
using UnityEngine;

namespace Collectibles {
    public class StaminaOrbs : MonoBehaviour {
        private uint HealValue = 4;
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.TryGetComponent(out Player.Player player)) {
                player.HealStamina(HealValue);
                if (this.GetComponent<PhotonView>().IsMine) {
                    PhotonNetwork.Destroy(this.gameObject);
                }
                else GetComponent<PhotonView>().RPC("NetworkDestroy", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        private void NetworkDestroy()
        {
            if (this.GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected) {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
} 