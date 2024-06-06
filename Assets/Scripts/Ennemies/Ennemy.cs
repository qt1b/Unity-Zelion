using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Global;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using UnityEngine.AI;


namespace Ennemies {
    public class Ennemy : MonoBehaviourPunCallbacks
    {
        private bool _isShooter;
        private bool _isMelee;

        private ShootArrow _shooter;
        private MeleeAttack _meleeAttack;
    
        public float speed => GlobalVars.EnnemySpeed;

        [Header("Movement variables")]
        public float detectionDistance;
        public float distanceFromPlayer;
        public float distanceFromPlayerMargin;
        public bool backWhenPlayerTooClose = false;
        private float LowerMargin => distanceFromPlayer - distanceFromPlayerMargin;
        private float HigherMargin => distanceFromPlayer + distanceFromPlayerMargin;
    
        [Header("Attacks variables")]
        public float shootFrequency;
        public float meleeFrequency;

        public float shootRangeMax;
        public float shootRangeMin;
        public float meleeRange;

        private bool _canShoot = true;
        private bool _canMelee;
        
        
        [Header("Movement variables")]
        public NavMeshAgent Agent;
    
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Enemy script : start");
            if (PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Enemy enabled");
                _shooter = GetComponent<ShootArrow>();
                _isShooter = _shooter is not null;

                _meleeAttack = GetComponent<MeleeAttack>();
                _isMelee = _meleeAttack is not null;
            }
            else
            {
                Debug.Log("Enemy disabled");
                enabled = false;
                GetComponent<NavMeshAgent>().enabled = false;
            }
        }

        public override void OnEnable()
        {
            Debug.Log("Enemy enabled");
            if (PhotonNetwork.IsMasterClient)
            {
                InvokeRepeating(nameof(Refresh), .1f, .1f);
            }
        }
        
        public override void OnDisable()
        {
            Debug.Log("Enemy disabled");
            CancelInvoke(nameof(Refresh));
        }

        public void OnDestroy() {
            Debug.Log("Enemy Destroyed");
            CancelInvoke(nameof(Refresh));
        }

        IEnumerator ResetShoot()
        {
            yield return new WaitForSeconds(shootFrequency / GlobalVars.EnnemySpeed);
            _canShoot = true;
        }
        
        IEnumerator ResetMelee()
        {
            yield return new WaitForSeconds(meleeFrequency / GlobalVars.EnnemySpeed);
            _canMelee = true;
        }
        // Update is called once per frame
        void Refresh()
        {
            List<Player.Player> players = new List<Player.Player>(GlobalVars.PlayerList);
            if (players.Count == 0)
                return;
            
            var pos = transform.position;
            players = players.OrderBy(g => (g.transform.position - pos).sqrMagnitude).ToList();
            var playerPos = players.First().transform.position;
            var nDirection = pos - playerPos;
            var distance = Mathf.Abs(nDirection.magnitude);
            
            if (!(distance <= detectionDistance)) return;
            bool bigMarg = distance > LowerMargin;
            bool smallMarg = distance < HigherMargin;

            if (bigMarg && !smallMarg)
                Agent.SetDestination(playerPos);
            else if (smallMarg && !bigMarg && backWhenPlayerTooClose)
                Agent.SetDestination(pos + nDirection);
            else
                Agent.ResetPath();
            if (_isShooter && distance >= shootRangeMin && distance <= shootRangeMax && _canShoot)
            {
                _canShoot = false;
                StartCoroutine(_shooter.Shoot(0, pos, nDirection.normalized));
                StartCoroutine(ResetShoot());
            }

            if (_isMelee && distance <= meleeRange && _canMelee)
            {
                _canMelee = false;
                StartCoroutine(_meleeAttack.Attack());
                StartCoroutine(ResetMelee());
            }
        }
    }
}
