using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraWork.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the Camera work to follow the player
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
// Heavily edited to work within the context of a 2d game, and to follow the player;

namespace Player { 
    public class CameraWork : MonoBehaviour
    {
        #region Private Fields

        // the player to follow. Set to public because it must change in case of gameOver
        [DoNotSerialize]
        public Player _player;
        [Tooltip("The multiplier applied to the player's")]
        [SerializeField]
        private float distance = 2.0f;
        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField]
        private bool followOnStart = false;
        [Tooltip("The Smoothing for the camera to follow the target")]
        [SerializeField]
        private float smoothSpeed = 0.125f;
        // velocity for Vector3.SmoothDamp
        private Vector3 _velocity = Vector3.zero;
        // cached transform of the camera
        private Transform _cameraTransform;
        // cached player, to get the value of its change vector
        // maintain a flag internally to reconnect if target is lost or camera is switched
        bool _isFollowing;
        #endregion

        #region MonoBehaviour Callbacks
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase
        /// </summary>
        void Start() {
            _player = Player.LocalPlayerInstance.GetComponent<Player>();
            // Start following the target if wanted.
            if (followOnStart)
            {
                OnStartFollowing();
            }
        }


        void LateUpdate()
        {
            // The transform target may not destroy on level load,
            // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
            if (_cameraTransform == null && _isFollowing)
            {
                OnStartFollowing();
            }

            // only follow is explicitly declared
            if (_isFollowing) {
                Follow ();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Raises the start following event.
        /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
        /// </summary>
        public void OnStartFollowing()
        {
            _cameraTransform = FindObjectOfType<Camera>().transform;
            _isFollowing = true;
            // we don't smooth anything, we go straight to the right camera shot
            Cut();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Follow the target smoothly
        /// </summary>
        void Follow()
        {
            var playPos = Player.LocalPlayerInstance.transform.position;
            var camPos = _cameraTransform.position;
            if (playPos == camPos) return;
            Vector3 desiredPosition = new Vector3(playPos.x + _player.change.x * distance, 
                playPos.y + _player.change.y * distance, camPos.z);
            // forces the position to be between these two limits
            // desiredPosition.x = /*Mathf.Clamp(*/desiredPosition.x;//, minPosition.x, maxPosition.x);
            // desiredPosition.y = /*Mathf.Clamp(*/desiredPosition.y;//, minPosition.y, maxPosition.y);
            _cameraTransform.position = Vector3.SmoothDamp(_cameraTransform.position, desiredPosition, ref _velocity,smoothSpeed);
        }


        void Cut() {
            var desiredPosition = transform.position;
            desiredPosition.z = -1;
            _cameraTransform.position = desiredPosition;
        }
        #endregion
    }
}