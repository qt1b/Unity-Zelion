using Unity.Netcode;
using UnityEngine;

namespace Collectibles {
    public class StaminaOrbs : Collectible {
        internal override void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.TryGetComponent(out Player.Player player)) {
                player.HealStamina(HealValue);
                DestroyGameObj();
            }
        }
    }
} 