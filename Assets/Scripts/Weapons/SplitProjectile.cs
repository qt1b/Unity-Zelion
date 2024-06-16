using System;
using System.Collections;
using System.Collections.Generic;
using Global;
using JetBrains.Annotations;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Random = UnityEngine.Random;

namespace Weapons
{
    public class SplitProjectile : Projectile
    {
        [Tooltip("from projectiles")] public string subProjectilePath;
        public ushort nbSplitProjectiles;
        public ushort subDamage;
        public bool isSubRotated;


        void Awake()
        {
            
            _curSpeed = GlobalVars.ProjectileSpeed;
            _myRigidBody = GetComponent<Rigidbody2D>();
            // some arrows are not destroying ???
            StartCoroutine(DestroyAfterSecs(dieTime));
        }
        
        protected new IEnumerator DestroyAfterSecs(float secs)
        {
            yield return new WaitForSeconds(secs);
            var randAng = _random.Next(0, 360 / nbSplitProjectiles);
            for (int i = 0; i < nbSplitProjectiles; i++)
            {

                var rot = (360.0f * i) / nbSplitProjectiles + randAng;
                var proj = PhotonNetwork.Instantiate(
                    $"Prefabs/Projectiles/{subProjectilePath}",
                    transform.position,
                    UnityEngine.Quaternion.Euler(0, 0, isSubRotated ? rot : 0));
                var projectile = proj.GetComponent<Projectile>();
                projectile.SetVelocity(new Vector3(Mathf.Cos(rot * Mathf.Deg2Rad), Mathf.Sin(rot * Mathf.Deg2Rad)) * projectile.speed);
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }
}