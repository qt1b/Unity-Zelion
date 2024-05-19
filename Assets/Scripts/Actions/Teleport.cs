using UnityEngine;
using UnityEngine.Serialization;

namespace Actions {
    public class Teleport : MonoBehaviour
    {
        [FormerlySerializedAs("X")] public float x;
        [FormerlySerializedAs("Y")] public float y;
        public string hitTag;
        private Camera _camera;
        void Start() {
            _camera = Camera.main;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag(hitTag)) {
                other.transform.position = new Vector3(x,y,0f);
                _camera.transform.position = new Vector3(x,y,_camera.transform.position.z);
            }
        }
    }
}
