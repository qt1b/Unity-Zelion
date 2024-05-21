using Ennemies;
using Interfaces;
using UnityEngine;

namespace Weapons {
    public class InflictDammage : MonoBehaviour {
        public uint damage = 3; 
        // can friendly fire
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent(out IHealth health)) {
                health.TakeDamages(damage);
            }
        }
    }
}