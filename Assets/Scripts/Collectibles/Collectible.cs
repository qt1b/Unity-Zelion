using Unity.Netcode;
using UnityEngine;

namespace Collectibles {
    public abstract class Collectible : NetworkBehaviour {
        public uint HealValue = 4;
        // private Collider2D _collider2D;

        internal abstract void OnTriggerEnter2D(Collider2D other);

        // or private ? private protected ?
        internal void DestroyGameObj(float time = 0f) {
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
