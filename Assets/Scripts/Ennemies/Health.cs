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
        [FormerlySerializedAs("MaxHealth")] public uint maxHealth;
        // to NetworkVariable ??
        private uint _hp;
        public float deathDuration;
        private SpriteRenderer _spriteRenderer; // to change color when hit
        private uint _colorAcc;
        private static readonly int Death = Animator.StringToHash("Death");

        void Start()
        {
            if (maxHealth > short.MaxValue) {
                // is necessary to avoid errors while syncing hp's value
                Debug.LogError("max value is too big, resizing it to short's max value");
                maxHealth = (uint)short.MaxValue;
            }
            _hp = maxHealth;
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        
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
                this._hp = (uint)(short)stream.ReceiveNext();
            }
        }
        public void TakeDamages(uint damage){
            if (damage >= _hp) {
                //GetComponent<PhotonView>().RPC("SpawCollectiblesRPC", RpcTarget.AllBuffered);
                Die();
            }
            else _hp -= damage;
            StartCoroutine(ChangeColorWait(new Color(1, 0.3f, 0.3f, 1), 0.5f)); // red with transparency
            // must add here some code to change the color for some frames: that way we will see when we make damages to an enemy/object
        }
        public void Heal(uint heal)
        {
            if (heal + _hp >= maxHealth)
                _hp = maxHealth;
            else _hp += heal;
            // test if one or the other works
            photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,new Color(0.3f, 1, 0.3f, 1), 0.5f);
            //photonView.StartCoroutine(ChangeColorWait(new Color(0.3f, 1, 0.3f, 1), 0.5f));
        }
        // sync every function from the die function
        private void Die() {
            PhotonNetwork.Destroy(this.gameObject);
            //GetComponent<PhotonView>().RPC("DieRPC", RpcTarget.AllBuffered);
            CollectibleDrop.Activate(maxHealth,gameObject.transform.position);
        }
        IEnumerator DestroyAfterSecs(float secs) {
            yield return new WaitForSeconds(secs);
            PhotonNetwork.Destroy(gameObject);
        }
        /*[PunRPC]
        private void NetworkDestroy(float time = 0f)
        {
            Destroy(this.gameObject,time);
        } */

        [PunRPC]
        public void DieRPC() {
            if (gameObject.TryGetComponent(out Collider2D collider2D)) {
                collider2D.enabled = false;
            }
            if (gameObject.TryGetComponent(out Animator animator)) {
                animator.SetBool(Death,true);
            }
            StartCoroutine(DestroyAfterSecs(deathDuration));
        }
        [PunRPC]
        public void ChangeColorWaitRpc(Color color,float time) {
            StartCoroutine(ChangeColorWait(color, time));
        }
        IEnumerator ChangeColorWait(Color color, float time) {
            Color baseColor = _spriteRenderer.color;
            ChangeColorClientRpc(color);
            _colorAcc += 1;
            yield return new WaitForSeconds(time);
            _colorAcc -= 1;
            if (_colorAcc == 0) {
                ChangeColorClientRpc(Color.white);
            }
            else if (baseColor != Color.white) ChangeColorClientRpc(baseColor);
        }
        // to be synced over network
        void ChangeColorClientRpc(Color color) {
            _spriteRenderer.color = color;
        }
        /*
        void SpawnCollectibles() {
            GetComponent<PhotonView>().RPC("SpawCollectiblesRPC", RpcTarget.AllBuffered);
        } 
        
        
        //[ServerRpc(RequireOwnership = false)]
        [PunRPC]
        void SpawnCollectiblesRPC() {
            CollectibleDrop.Activate(maxHealth,gameObject.transform.position); // error here ?
        } */
        
    }
}