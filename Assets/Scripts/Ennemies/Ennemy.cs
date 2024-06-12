using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using JetBrains.Annotations;
using Photon.PhotonUnityNetworking.Code;
using Porperty_Attributes;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Weapons;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;


namespace Ennemies
{
    public class Ennemy : MonoBehaviourPunCallbacks
    {
        [Header("Movement")] [Tooltip("From which distance the ennemy will be able to detect a player")]
        public float detectionDistance;

        [Tooltip("Distance from the player where the ennemy will not move")]
        public float distanceFromPlayer;

        [Tooltip("Margin of error for the distance from the player")]
        public float distanceFromPlayerMargin;

        [Space(5f)]
        [Tooltip("If the ennemy will distance himself from the player when too close")]
        public bool backWhenPlayerTooClose;

        [Tooltip("If the ennemy will close the distance to the player when too close")]
        public bool forwardWhenPlayerTooClose;

        [Tooltip("if the ennemy can charge")]public bool isCharger;

        [SerializeField] private EnnemyChargeParams _chargeParams;

        [Header("Attack Parameters")]
        [Tooltip("If ennemy can shoot arrows")]
        public bool isShooter;
        [SerializeField] private EnnemyShootParams ShootParams;

        [Tooltip("If ennemy do short distance attacks")]
        [HorizontalLine(Padding = 10f)] public bool isMelee;
        [SerializeField] private EnnemyMeleeParams MeleeParams;

        [Space] public NavMeshAgent Agent;
        public Rigidbody2D Rigidbody2D;

        private bool _isShooting;
        private bool _canShoot = true;
        private bool _isMeleeing;
        private bool _canMelee = true;
        private bool _isCharging;
        private bool _canCharge = true;

        private Random _random = new();
        private bool IsDoingAnAction => _isCharging || _isMeleeing || _isShooting;
        private float LowerMargin => distanceFromPlayer - distanceFromPlayerMargin;
        private float HigherMargin => distanceFromPlayer + distanceFromPlayerMargin;
        public float speed => GlobalVars.EnnemySpeed;

        private Coroutine chargeCoroutine;

        void Start()
        {
            if (PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient) return;
            enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
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

        #region Attacks

        
        private IEnumerator CanShootReset()
        {
            _canShoot = false;
            yield return new WaitForSeconds(ShootParams.shootFrequency / GlobalVars.EnnemySpeed);
            _canShoot = true;
        }
        private IEnumerator Shoot(int arrowIndex, Vector3 initialPos, Vector3 direction)
        {
            _isShooting = true;
            StartCoroutine(CanShootReset());
            // animator for shooting here
            var rot = Quaternion.Euler(0f, 0f,
                Mathf.Atan(direction.y / direction.x) * 180 / Mathf.PI + (direction.x < 0 ? 90 : -90));
            var arr = PhotonNetwork.Instantiate(
                "Prefabs/Projectiles/Arrow",
                initialPos + direction * ShootParams.startDistance, rot);
            var projectile = arr.GetComponent<Projectile>();
            projectile.damage = ShootParams.damage;
            projectile.SetVelocity(direction);
            projectile.speed = ShootParams.arrowSpeed;
            _isShooting = false;
            yield break;
        }
        
        private IEnumerator CanMeleeReset()
        {
            _canMelee = false;
            yield return new WaitForSeconds(ShootParams.shootFrequency / GlobalVars.EnnemySpeed);
            _canMelee = true;
        }

        private IEnumerator Attack(Player.Player player, Vector3 direction)
        {
            _isMeleeing = true;
            StartCoroutine(CanMeleeReset());
            // animator for melee here
            player.TakeDamages(MeleeParams.meleeDamage);
            _isMeleeing = false;
            yield break;
        }

        private IEnumerator CanChargeReset()
        {
            _canCharge = false;
            yield return new WaitForSeconds(_random.Next((int)(_chargeParams.Minfrequency * 100f),
                (int)(_chargeParams.maxFrequency * 100f)) / 100f / GlobalVars.EnnemySpeed);
            _canCharge = true;
        }

        private IEnumerator Charge(Player.Player player, Vector3 direction) {
            _isCharging = true;
            StartCoroutine(CanChargeReset());
            _chargeParams.target = player;
            Agent.ResetPath();
            Rigidbody2D.sleepMode = RigidbodySleepMode2D.StartAwake;
            Rigidbody2D.velocity = direction.normalized * _chargeParams.speed;
            yield return new WaitForSeconds(_chargeParams.time);
            Rigidbody2D.velocity = new UnityEngine.Vector2();
            _chargeParams.target = null;
            _isCharging = false;
        }

        #endregion

        private void Refresh()
        {
            List<Player.Player> players = new(GlobalVars.PlayerList);
            if (players.Count == 0 || IsDoingAnAction || PauseMenu.GameIsPaused || GlobalVars.EnnemySpeed == 0)
                return;

            var pos = transform.position;
            players = players.OrderBy(g => (g.transform.position - pos).sqrMagnitude).ToList();
            var player = players.First();
            var playerPos = player.transform.position;
            var direction = playerPos - pos;
            var distance = Mathf.Abs(direction.magnitude);

            if (!(distance <= detectionDistance)) return;
            var bigMarg = distance > LowerMargin;
            var smallMarg = distance < HigherMargin;
            
            if (bigMarg && !smallMarg)
                Agent.SetDestination(playerPos);
            else if (smallMarg && !bigMarg && backWhenPlayerTooClose)
                Agent.SetDestination(pos - direction);
            else
                Agent.ResetPath();
            
            
            if (isShooter && distance >= ShootParams.shootRangeMin && distance <= ShootParams.shootRangeMax &&
                _canShoot && (!forwardWhenPlayerTooClose || distance >= LowerMargin))
                StartCoroutine(Shoot(0, pos, direction.normalized));
            
            if (isMelee && distance <= MeleeParams.meleeRange && _canMelee)
                StartCoroutine(Attack(player, direction));
            
            if (_canCharge)
                StartCoroutine(Charge(player, direction));
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            if (_isCharging && other.gameObject.CompareTag("Player"))
            {
                if (_chargeParams.stopOnCollision)
                {
                    StopCoroutine(chargeCoroutine);
                    _isCharging = false;
                    Rigidbody2D.velocity = UnityEngine.Vector2.zero;
                }

                var playerRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
                var playerPos = other.gameObject.transform.position;
                var directionPlayer = (playerPos - gameObject.transform.position).normalized;
                var curDirection = Rigidbody2D.velocity.normalized;
                playerRigidbody.AddForce(curDirection.y * directionPlayer.y > curDirection.x * directionPlayer.x
                    ? new UnityEngine.Vector2(curDirection.y, -curDirection.x*.1f)
                    : new UnityEngine.Vector2(-curDirection.y, curDirection.x)*.1f);
                
                other.gameObject.GetComponent<Player.Player>().TakeDamages(_chargeParams.damage);
            }
        }
    }
    
    

    [Serializable]
    public class EnnemyShootParams
    {
        [SerializeAs("Arrow damage")] public ushort damage;
        [Tooltip("sec / arrow")] public float shootFrequency;

        [Tooltip("max distance where the ennemy will shoot arrows")]
        public float shootRangeMax;

        [Tooltip("min distance where the ennemy will shoot arrows")]
        public float shootRangeMin;

        public float arrowSpeed;

        [Tooltip("Distance from the ennemy's center where arrows will spawn")]
        public float startDistance;
    }

    [Serializable]
    public class EnnemyMeleeParams
    {
        [SerializeAs("Melee damage")] public ushort meleeDamage;
        [Tooltip("sec / hit")] public float meleeFrequency;
        public float meleeRange;
    }

    [Serializable]
    public class EnnemyChargeParams
    {
        [SerializeAs("Charging Speed")] public uint speed;
        [SerializeAs("Charge damage")] public ushort damage;

        [Tooltip("sec / charge")] [SerializeAs("Minimum charging frequency")]
        public float Minfrequency;

        [Tooltip("sec / charge")] [SerializeAs("Maximem charging frequency")]
        public float maxFrequency;

        [SerializeAs("Charge time")]public float time;

        [Tooltip("Stops to player")]
        public bool stopOnCollision;

        [NonSerialized] [CanBeNull] public Player.Player target;
    }
}