using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Weapons {
    public class Projectile : NetworkBehaviour {
        public float speed = 30f;
        public Vector3 Direction { get; set; } = Vector3.zero;
        public float ControlSpeed { get; set; } = 1f;
        public uint damage = 3;
        Rigidbody2D _myRigidBody;

        // DOES NOT DESTROY CORRECTLY
        void Awake() {
            _myRigidBody = GetComponent<Rigidbody2D>();
            // some arrows are not destroying ???
            DestroyGameObj(4f);
            /*
         if (IsServer) {
            myRigidBody.Sleep();
            Destroy(gameObject);
        } */
            // _healthBar = GameObject.FindGameObjectWithTag($"PlayerHealth").GetComponent<HealthBar>();
        }

        public void SetVelocity(Vector3 givenDirection, float givenControlSpeed) {
            Direction = givenDirection;
            ControlSpeed = givenControlSpeed;
            _myRigidBody.velocity = Direction * (speed * 0.2f * ControlSpeed);
        }

        
        void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.TryGetComponent(out IHealth health)) {
                health.TakeDamages(damage);
            }
            _myRigidBody.velocity = Vector3.zero;
            DestroyGameObj(.2f);
        }

        // does not really work, is it bc it needs a rigidbody ?
        private void OnCollisionEnter2D(Collision2D other) {
            print("Collision Enter 2D!!");
            if (other.gameObject.TryGetComponent(out IHealth health))
                health.TakeDamages(damage);
            _myRigidBody.velocity = Vector3.zero;
            DestroyGameObj(.2f);
        }

        private void DestroyGameObj(float time = 0f) {
            if (IsServer) {
                DestroyServer(time);
            }
            else DestroyServerRPC(time);
        }

        private void DestroyServer(float time = 0f) {
            Destroy(gameObject,time);
        }

        [ServerRpc]
        private void DestroyServerRPC(float time = 0f) {
            DestroyServer(time);
        }
    }
}