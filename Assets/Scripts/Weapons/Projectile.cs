using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

namespace Weapons {
    public class Projectile : MonoBehaviourPunCallbacks {
        public float speed = 1f;
        public Vector3 Direction { get; set; } = Vector3.zero;
        public ushort damage = 3;

        public float dieTime = 4f;
        protected Rigidbody2D _myRigidBody;

        public List<string> notDamageTags;
        protected float _curSpeed = 1f;
        

        protected static Random _random = new ();

        void Awake()
        {
            _curSpeed = GlobalVars.ProjectileSpeed;
            _myRigidBody = GetComponent<Rigidbody2D>();
            // some arrows are not destroying ???
            StartCoroutine(DestroyAfterSecs(dieTime));
        }

        public void SetVelocity(Vector3 givenDirection) {
            photonView.RPC("SetVelocityRPC",RpcTarget.AllBuffered,givenDirection);
        }

        [PunRPC]
        public void SetVelocityRPC(Vector3 givenDirection) {
            Direction = givenDirection;
            _myRigidBody.velocity = Direction * (speed * 0.2f * Global.GlobalVars.PlayerSpeed);
        }
        // may be destroyed on every instance ?
        protected IEnumerator DestroyAfterSecs(float secs) {
            yield return new WaitForSeconds(secs);
            PhotonNetwork.Destroy(gameObject);
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            if (notDamageTags.Any(g => other.gameObject.CompareTag(g)))
                return;
            
            if (other.gameObject.TryGetComponent(out IHealth health)) {
                if (photonView.IsMine) health.TakeDamages(damage);
                _myRigidBody.velocity = Vector3.zero;
                StartCoroutine(DestroyAfterSecs(.2f));
            }
            else if (!other.isTrigger && !other.CompareTag("LetProjectilesPass")) {
                _myRigidBody.velocity = Vector3.zero;
                StartCoroutine(DestroyAfterSecs(.2f));
            }
        }

        /*
        // does not really work, is it bc it needs a rigidbody ?
        private void OnCollisionEnter2D(Collision2D other) {
            print("Collision Enter 2D!!");
            if (photonView.IsMine && other.gameObject.TryGetComponent(out IHealth health)) {
                health.TakeDamages(damage);
            }
            _myRigidBody.velocity = Vector3.zero;
            StartCoroutine(DestroyAfterSecs(.2f));
            //PhotonNetwork.Destroy(gameObject);
            //DestroyGameObj();
        } */

        private void DestroyGameObj() {
            StartCoroutine(DestroyAfterSecs(.2f));
            //GetComponent<PhotonView>().RPC("NetworkDestroy", RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void NetworkDestroy() {
            PhotonNetwork.Destroy(this.gameObject);
        }

        protected void ModifySpeed()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_curSpeed != GlobalVars.ProjectileSpeed)
            {
                _myRigidBody.velocity = _myRigidBody.velocity / _curSpeed * GlobalVars.ProjectileSpeed;
                _curSpeed = GlobalVars.ProjectileSpeed;
            }
        }

        void Update()
        {
            ModifySpeed();
        }
    }
}