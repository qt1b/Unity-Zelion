using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Actions {
    public class Teleport : MonoBehaviour, IAction
    {
        //[FormerlySerializedAs("X")] public float x;
        //[FormerlySerializedAs("Y")] public float y;
        public string hitTag;
        private GameObject _player;
        public Vector3 goal;
        private Camera _camera;
        void Start() {
            _camera = Camera.main;
            _player = Player.Player.LocalPlayerInstance;
        }

        public void Activate() {
            _player.transform.position = goal;
            _camera.transform.position = new Vector3(goal.x,goal.y,-1);
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag(hitTag)) {
                other.transform.position = goal;
                _camera.transform.position = new Vector3(goal.x,goal.y,-1);
            }
        }
    }
}
