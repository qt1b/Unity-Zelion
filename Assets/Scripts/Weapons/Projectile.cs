using System;
using System.Collections;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using Unity.Netcode;
using UnityEngine;

namespace Weapons {
    public class Projectile : MonoBehaviourPunCallbacks {
        public float speed = 30f;
        public Vector3 Direction { get; set; } = Vector3.zero;
        public ushort damage = 3;
        Rigidbody2D _myRigidBody;

        void Awake() {
            _myRigidBody = GetComponent<Rigidbody2D>();
            // some arrows are not destroying ???
            StartCoroutine(DestroyAfterSecs(4f));
            /*
         if (IsServer) {
            myRigidBody.Sleep();
            Destroy(gameObject);
        } */
            // _healthBar = GameObject.FindGameObjectWithTag($"PlayerHealth").GetComponent<HealthBar>();
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
        IEnumerator DestroyAfterSecs(float secs) {
            yield return new WaitForSeconds(secs);
            PhotonNetwork.Destroy(gameObject);
        }
        
        void OnTriggerEnter2D(Collider2D other) {
            if (photonView.IsMine && other.gameObject.TryGetComponent(out IHealth health)) {
                health.TakeDamages(damage);
            }
            _myRigidBody.velocity = Vector3.zero;
            StartCoroutine(DestroyAfterSecs(.2f));
            //PhotonNetwork.Destroy(gameObject);
            //DestroyGameObj();
        }

        // does not really work, is it bc it needs a rigidbody ?
        private void OnCollisionEnter2D(Collision2D other) {
            print("Collision Enter 2D!!");
            if (other.gameObject.TryGetComponent(out IHealth health))
                health.TakeDamages(damage);
            _myRigidBody.velocity = Vector3.zero;
            StartCoroutine(DestroyAfterSecs(.2f));
            //PhotonNetwork.Destroy(gameObject);
            //DestroyGameObj();
        }

        private void DestroyGameObj() {
            StartCoroutine(DestroyAfterSecs(.2f));
            //GetComponent<PhotonView>().RPC("NetworkDestroy", RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void NetworkDestroy() {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}