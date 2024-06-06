using System.Collections;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

namespace Ennemies
{
    public class ShootArrow : MonoBehaviourPunCallbacks
    {
        // @louis why use a list here ?
        [FormerlySerializedAs("ArrowRef")] public List<GameObject> arrowRef;
        [FormerlySerializedAs("ArrowSpeed")] public float arrowSpeed;

        public float startDistance;

        // Arrows do not despawn when the ennemy shoots them : WHY ???
        // Start is called before the first frame update
        public IEnumerator Shoot(int arrowIndex, Vector3 initialPos, Vector3 direction)
        {
            var rot = Quaternion.Euler(0f, 0f,
                Mathf.Atan(direction.y / direction.x) * 180 / Mathf.PI + (direction.x >= 0 ? 90 : -90));
            var arr = PhotonNetwork.Instantiate(
                "Prefabs/Projectiles/Arrow",
                initialPos - direction * startDistance, rot);
            var projectile = arr.GetComponent<Projectile>();
            projectile.SetVelocity(-direction);
            projectile.speed = arrowSpeed;
            yield break;
        }
    }
}