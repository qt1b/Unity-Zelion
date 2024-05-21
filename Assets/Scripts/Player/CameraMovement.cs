using Photon.Pun;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player {
    public class CameraMovement : MonoBehaviourPunCallbacks {
        // the higher it is, more time it will take to follow the player
        public float smoothTime;
        private Vector3 _velocity = Vector3.zero;
        public float playerDistance;
        [FormerlySerializedAs("CameraHolder")] public GameObject cameraHolder;
        private Player _tarVelo;

        #region Mono Behaviours

        private void Awake() {
            _tarVelo = GetComponent<Player>();
        }
        void LateUpdate() {
            var position = transform.position;
            var position1 = cameraHolder.transform.position;
            if (position1 == position) return;
            Vector3 desiredPosition = new Vector3(position.x + _tarVelo.change.x * playerDistance, 
                position.y + _tarVelo.change.y * playerDistance, position1.z);
            // forces the position to be between these two limits
            // desiredPosition.x = /*Mathf.Clamp(*/desiredPosition.x;//, minPosition.x, maxPosition.x);
            // desiredPosition.y = /*Mathf.Clamp(*/desiredPosition.y;//, minPosition.y, maxPosition.y);
            position1 = Vector3.SmoothDamp(position1, desiredPosition, ref _velocity, smoothTime);
            cameraHolder.transform.position = position1;
        }
        #endregion

        #region PUN behaviours
        // could be moved to awake ? or not ?

        public override void OnConnected() {
            base.OnConnected();
            if (!photonView.IsMine) {
                cameraHolder.SetActive(false);
            }
        }

        #endregion
    }
}