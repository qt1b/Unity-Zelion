using Unity.Netcode;
using UnityEngine;

namespace Collectibles {
    public class Hearts : NetworkBehaviour {
        private uint healValue = 4;
        // private Collider2D _collider2D;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.TryGetComponent(out Player.Player player)) {
                player.Heal(healValue);
                DestroyGameObj();
            }
        }
        private void DestroyGameObj(float time = 0f) {
            if (IsServer) {
                DestroyServer(time);
            }
            else DestroyServerRpc(time);
        }

        private void DestroyServer(float time = 0f) {
            Destroy(gameObject,time);
        }

        [ServerRpc]
        private void DestroyServerRpc(float time = 0f) {
            DestroyServer(time);
        }
    }
}
