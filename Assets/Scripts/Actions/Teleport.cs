using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Actions {
    public class Teleport : MonoBehaviour, IAction
    {
        //[FormerlySerializedAs("X")] public float x;
        //[FormerlySerializedAs("Y")] public float y;
        public string hitTag;
        public GameObject player;
        public Vector3 goal;
        private Camera _camera;
        void Start() {
            _camera = Camera.main;
        }

        public void Activate() {
            player.transform.position = goal;
            _camera.transform.position = goal;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag(hitTag)) {
                other.transform.position = goal;
                _camera.transform.position = goal;
            }
        }
    }
}
