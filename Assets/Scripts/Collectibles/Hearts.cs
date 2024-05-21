using Unity.Netcode;
using UnityEngine;

namespace Collectibles {
    public class Hearts : Collectible {
        internal override void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.TryGetComponent(out Player.Player player)) {
                player.Heal(HealValue);
                base.DestroyGameObj();
            }
        }
    }
}
