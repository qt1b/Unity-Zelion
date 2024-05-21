using Unity.Netcode;
using UnityEngine;

namespace Collectibles {
    public class ManaOrbs : Collectibles.Collectible {
        internal override void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.TryGetComponent(out Player.Player player)) {
                player.HealMana(HealValue);
                DestroyGameObj();
            }
        }
    }
}
