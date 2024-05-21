using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ennemies {
    public class Ennemy : NetworkBehaviour {
        private bool _isShooter;
        private bool _isMelee;

        private ShootArrow _shooter;
        private MeleeAttack _meleeAttack;
    
        [FormerlySerializedAs("Speed")] public float speed;

        private List<GameObject> _player;
    
        public float shootFrequency;
        public float meleeFrequency;

        public float shootRangeMax;
        public float shootRangeMin;
        public float meleeRange;

        private float _remainingShootingTime;
        private float _remainingMeleeTime;
    
        // Start is called before the first frame update
        void Start()
        {
            if (IsServer)
            {
                _shooter = GetComponent<ShootArrow>();
                _isShooter = _shooter is not null;

                _meleeAttack = GetComponent<MeleeAttack>();
                _isMelee = _meleeAttack is not null;
                _player = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
                NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
            }
            else
                enabled = false;
        }

        public override void OnNetworkSpawn()
        {
        
        }

        private void ClientConnected(ulong u)
        {
            _player = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        }

        private async void ClientDisconnected(ulong u)
        {
            await Task.Yield();
            _player = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        }

        // Update is called once per frame
        void Update()
        {
            if (_player.Count == 0)
                return;
            var pos = transform.position;
            _player.Sort((g1, g2) => 
                Mathf.Abs((g1.transform.position - pos).magnitude) - 
                Mathf.Abs((g2.transform.position - pos).magnitude) >= 0f ? 1 : -1);
            var playerPos = _player[0].transform.position;
            var direction = (pos - playerPos);
            var distance = Mathf.Abs(direction.magnitude);
            //print(distance);
            if (_isShooter && distance >= shootRangeMin && distance <= shootRangeMax)
            {
                if (_remainingShootingTime <= 0)
                {
                    _remainingShootingTime = shootFrequency;
                    _shooter.Shoot(0, pos, direction.normalized);
                }
                else
                    _remainingShootingTime -= Time.deltaTime;
            }

            if (_isMelee && distance <= meleeRange)
            {
                if (_remainingMeleeTime <= 0)
                {
                    _remainingMeleeTime = meleeFrequency;
                    _meleeAttack.Attack();
                }
                else
                    _remainingMeleeTime -= Time.deltaTime;
            }
        }
    }
}
