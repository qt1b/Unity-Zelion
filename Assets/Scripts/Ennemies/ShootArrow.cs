using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

namespace Ennemies {
    public class ShootArrow : NetworkBehaviour
    {
        // @louis why use a list here ?
        [FormerlySerializedAs("ArrowRef")] public List<GameObject> arrowRef;
        [FormerlySerializedAs("ArrowSpeed")] public float arrowSpeed;
        public float startDistance;
        // Arrows do not despawn when the ennemy shoots them : WHY ???
        // Start is called before the first frame update
        public void Shoot(int arrowIndex, Vector3 initialPos, Vector3 direction) {
            if (IsServer)
                ShootServer(arrowIndex, initialPos, direction);
            else
                ShootServerRpc(arrowIndex, initialPos, direction);
        }

        [ServerRpc]
        void ShootServerRpc(int arrowIndex, Vector3 initialPos, Vector3 direction) => ShootServer(arrowIndex, initialPos, direction);
        void ShootServer(int arrowIndex, Vector3 initialPos, Vector3 direction)
        {
            var rot = Quaternion.Euler(0f, 0f, 
                Mathf.Atan(direction.y / direction.x) * 180 / Mathf.PI + (direction.x >= 0 ? 90 : -90));
            var arr = Instantiate(arrowRef[arrowIndex], initialPos - direction * startDistance, rot);
            var projectile = arr.GetComponent<Projectile>();
            projectile.SetVelocity(-direction, 1);
            projectile.speed = arrowSpeed;
        }
    }
}
