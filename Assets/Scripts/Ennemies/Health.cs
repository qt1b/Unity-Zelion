using System.Collections;
using Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ennemies {
    public class Health : NetworkBehaviour, IHealth
    {
        [FormerlySerializedAs("MaxHealth")] public uint maxHealth;
        // to NetworkVariable ??
        private NetworkVariable<uint> _hp;
        public float deathDuration;
        private SpriteRenderer _spriteRenderer; // to change color when hit
        private uint _colorAcc;
        private static readonly int Death = Animator.StringToHash("Death");

        void Start()
        {
            _hp = new NetworkVariable<uint>(maxHealth);
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        public void TakeDamages(uint damage){
            if (damage >= _hp.Value) Die();
            else _hp.Value -= damage;
            StartCoroutine(ChangeColorWait(new Color(1, 0.3f, 0.3f, 1), 0.5f)); // red with transparency
            // must add here some code to change the color for some frames: that way we will see when we make damages to an enemy/object
        }

        public void Heal(uint heal)
        {
            if (heal + _hp.Value >= maxHealth)
                _hp.Value = maxHealth;
            else _hp.Value += heal;
            StartCoroutine(ChangeColorWait(new Color(0.3f, 1, 0.3f, 1), 0.5f)); // green with transparency
        }

        
        // sync every function from the die function
        private void Die() {
            if (IsServer) {
                DieServer();
            }
            else DieServerRpc();
        }

        private void DieServer() {
            if (gameObject.TryGetComponent(out Collider2D collider2D)) {
                collider2D.enabled = false;
            }
            if (gameObject.TryGetComponent(out Animator animator)) {
                animator.SetTrigger(Death);
            }

            SpawnCollectibles();
            Destroy(gameObject,deathDuration);
        } 
            
        [ServerRpc] 
        private void DieServerRpc() {
            DieServer();
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
        [ClientRpc]
        void ChangeColorClientRpc(Color color) {
            _spriteRenderer.color = color;
        }

        void SpawnCollectibles() {
            if (IsServer) {
                SpawnCollectiblesServer();
            }
            else SpawnCollectiblesServerRpc();
        }

        void SpawnCollectiblesServer() {
            CollectibleDrop.Activate(maxHealth,gameObject.transform.position); // error here ?
            /*
             List<GameObject> toSpawn = CollectibleDrop.SpawnList(maxHealth,gameObject.transform.position);
            foreach (GameObject o in toSpawn) {
                var instanciated = Instantiate(o);
                instanciated.GetComponent<NetworkObject>().Spawn();
            }*/
        } 
        
        //[ServerRpc(RequireOwnership = false)]
        [ServerRpc]
        void SpawnCollectiblesServerRpc() {
            SpawnCollectiblesServer();
        }
        
    }
}