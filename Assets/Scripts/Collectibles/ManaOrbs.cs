using Unity.Netcode;
using UnityEngine;

namespace Collectibles {
    public class ManaOrbs : NetworkBehaviour
    {
        [Header("Values")]
        public uint HealValue;
        // private Collider2D _collider2D;
        
        // destruction should be done over network
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.TryGetComponent(out Player.Player player)) {
                player.HealMana(HealValue);
                DestroyGameObj();

            }
        }
        private void DestroyGameObj(float time = 0f) {
            if (IsServer) {
                DestroyServer(time);
            }
            else DestroyServerRPC(time);
        }

        private void DestroyServer(float time = 0f) {
            Destroy(gameObject,time);
        }

        [ServerRpc]
        private void DestroyServerRPC(float time = 0f) {
            DestroyServer(time);
        }

    }
}
