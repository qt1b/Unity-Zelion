using System;
using System.Collections;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ennemies {
    public class Health : MonoBehaviourPunCallbacks, IPunObservable, IHealth
    {
        #region Fields
        public uint maxHealth;
        private uint _hp;
        public float deathDuration;
        private SpriteRenderer _spriteRenderer; // to change color when hit
        private uint _colorAcc;
        private static readonly int Death = Animator.StringToHash("Death");
        #endregion

        #region MonoBehaviours
        void Awake()
        {
            if (maxHealth > short.MaxValue) {
                // is necessary to avoid errors while syncing hp's value
                Debug.LogError("max value is too big, resizing it to short's max value");
                maxHealth = (uint)short.MaxValue;
            }
            _hp = maxHealth;
            if (_spriteRenderer is null)
                _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        #endregion

        #region Photon Observable
        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                // does not support uint type, so converting it to short
                // we should never reach hp values as big as 
                stream.SendNext((short)_hp);
            }
            else
            {
                // Network player, receive data
                this._hp = (ushort)(short)stream.ReceiveNext();
            }
        }
        #endregion

        public void TakeDamages(ushort damage){
            if (damage >= _hp) {
                //GetComponent<PhotonView>().RPC("SpawCollectiblesRPC", RpcTarget.AllBuffered);
                Die();
            }
            else _hp -= damage;
            photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,1f, 0.3f, 0.3f, 0.5f);
            // must add here some code to change the color for some frames: that way we will see when we make damages to an enemy/object
        }
        public void Heal(ushort heal)
        {
            if (heal + _hp >= maxHealth)
                _hp = maxHealth;
            else _hp += heal;
            // test if one or the other works
            photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,0.3f, 1f, 0.3f, 0.5f);
        }
        private void Die() {
            GetComponent<PhotonView>().RPC("DieRPC", RpcTarget.AllBuffered);
            // does not need to be master client ??
            CollectibleDrop.Activate(maxHealth,gameObject.transform.position);
        }

        [PunRPC]
        public void DieRPC() {
            if (gameObject.TryGetComponent(out Ennemy ennemy)) {
                ennemy.StopAllCoroutines();
                ennemy.enabled = false;
            }
            if (gameObject.TryGetComponent(out Collider2D collider2D)) {
                collider2D.enabled = false;
            }
            if (gameObject.TryGetComponent(out Animator animator)) {
                animator.SetBool(Death,true);
            }
            if (gameObject.TryGetComponent(out IAction action)) {
                action.Activate();
            }
            if (this.photonView.IsMine && PhotonNetwork.IsConnected) {
                StartCoroutine(DestroyAfterSecs(deathDuration));
            }
        }
        IEnumerator DestroyAfterSecs(float secs) {
            yield return new WaitForSeconds(secs);
            PhotonNetwork.Destroy(gameObject);
        }

        [PunRPC]
        public void ChangeColorWaitRpc(float r, float g, float b, float time) {
            StartCoroutine(ChangeColorWait(new Color(r,g,b), time));
        }
        [PunRPC]
        public void ChangeColorWAlphaWaitRpc(float r, float g, float b, float a, float time) {
            StartCoroutine(ChangeColorWait(new Color(r,g,b,a), time));
        }
        IEnumerator ChangeColorWait(Color color, float time) {
            Color baseColor = _spriteRenderer.color;
            _spriteRenderer.color = color;
            _colorAcc += 1;
            yield return new WaitForSeconds(time);
            _colorAcc -= 1;
            if (_colorAcc == 0) {
                _spriteRenderer.color = Color.white;
            }
            else if (baseColor != Color.white) _spriteRenderer.color = baseColor;
        }

        public bool IsAlive() => _hp > 0;
    }
}