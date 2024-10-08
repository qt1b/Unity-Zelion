using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using Interfaces;
using JetBrains.Annotations;
using Photon.PhotonUnityNetworking.Code;
using Porperty_Attributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Weapons;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;


namespace Ennemies
{
    public class Golem : MonoBehaviourPunCallbacks
    {
        [Header("Movement")] [Tooltip("From which distance the ennemy will be able to detect a player")]
        public float detectionDistance;

        [Tooltip("Distance from the player where the ennemy will not move")]
        public float distanceFromPlayer;

        [Tooltip("Margin of error for the distance from the player")]
        public float distanceFromPlayerMargin;

        [Space(5f)] [Tooltip("If the ennemy will distance himself from the player when too close")]
        public bool backWhenPlayerTooClose;

        [Tooltip("If the ennemy will close the distance to the player when too close")]
        public bool forwardWhenPlayerTooClose;

        [Header("Attack Parameters")]

        [Space(5f)]
        [Tooltip("If ennemy do short distance attacks")]
        [NonSerialized]public bool isMelee = true;

        [SerializeField] private GolemMeleeParams _melee = new GolemMeleeParams(){damage = 5, range = 5, maxFrequency = 5, minFrequency = 4, meleeTime = 1, nbDirections = 4};
        [Space(3f)]
        [NonSerialized] public bool isCharger = true;

        [SerializeField] private GolemChargeParams _charge = new GolemChargeParams(){stopOnCollision = true, time = 4f};

        [Space] public NavMeshAgent Agent;
        public Rigidbody2D Rigidbody2D;
        [SerializeField] private Animator _animator;

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

        private Coroutine currentAction;

        private static readonly int IsMovingLeft = Animator.StringToHash("IsMovingLeft");
        private static readonly int IsMovingRight = Animator.StringToHash("IsMovingRight");
        private static readonly int IsDrawing = Animator.StringToHash("IsDrawing");
        private static readonly int IsCharging = Animator.StringToHash("IsCharging");
        private static readonly int IsMeleeing = Animator.StringToHash("IsMeleeing");

        private void Start()
        {
            if (PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient) return;
            enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            if (_animator is null)
                _animator = GetComponent<Animator>();
        }

        public override void OnEnable()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (isMelee)
                {
                    _melee.possibleDirections = new float[_melee.nbDirections];
                    for (int i = 0; i < _melee.nbDirections; i++)
                    {
                        _melee.possibleDirections[i] = i * 360f / _melee.nbDirections;
                    }
                }
                InvokeRepeating(nameof(Refresh), .1f, .1f);
            }
        }

        public override void OnDisable()
        {
            CancelInvoke(nameof(Refresh));
        }

        public void OnDestroy()
        {
            CancelInvoke(nameof(Refresh));
        }

        #region Attacks

        private IEnumerator CanMeleeReset()
        {
            _canMelee = false;
            yield return new WaitForSeconds(_random.Next((int)(_melee.minFrequency * 100f),
                (int)(_melee.maxFrequency * 100f)) / 100f / GlobalVars.EnnemySpeed);
            _canMelee = true;
        }


        private IEnumerator Attack(Player.Player player, Vector3 direciton)
        {
            _isMeleeing = true;
            _animator.SetBool(IsMeleeing, true);
            StartCoroutine(CanMeleeReset());
            // animator for melee here
            yield return new WaitForSeconds(_melee.meleeTime);

            foreach (var player1 in GlobalVars.PlayerList.Where(g => (g.transform.position- transform.position).sqrMagnitude <= _melee.range))
                player1.TakeDamages(_melee.damage);

            yield return new WaitForSeconds(1);


            _isMeleeing = false;
            _animator.SetBool(IsMeleeing, false);
        }

        private IEnumerator CanChargeReset()
        {
            _canCharge = false;
            yield return new WaitForSeconds(_random.Next((int)(_charge.Minfrequency * 100f),
                (int)(_charge.maxFrequency * 100f)) / 100f / GlobalVars.EnnemySpeed);
            _canCharge = true;
        }

        private IEnumerator Charge(Player.Player player, Vector3 direction)
        {
            _isCharging = true;
            
            UpdateAnimator();
            StartCoroutine(CanChargeReset());
            Agent.ResetPath();
            Rigidbody2D.sleepMode = RigidbodySleepMode2D.StartAwake;
            Rigidbody2D.velocity = direction.normalized * _charge.speed;
            yield return new WaitForSeconds(_charge.time);
            Rigidbody2D.velocity = new UnityEngine.Vector2();
            _isCharging = false;
            UpdateAnimator();
        }

        #endregion

        public void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (_isCharging)
                {
                    var playerRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
                    var playerPos = other.gameObject.transform.position;
                    var directionPlayer = (playerPos - gameObject.transform.position).normalized;
                    var curDirection = Rigidbody2D.velocity.normalized;
                    if (_charge.stopOnCollision) // always true, else it instant-kills
                    {
                        StopCoroutine(currentAction);
                        _isCharging = false;
                        Rigidbody2D.velocity = UnityEngine.Vector2.zero;
                    }

                    other.gameObject.GetComponent<Player.Player>().TakeDamages(_charge.damage);
                }
                else
                {
                    Agent.ResetPath();
                }
            }
        }

        public void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Agent.ResetPath();
            }
        }

        private void UpdateAnimator()
        {
            _animator.SetBool(IsMovingLeft, _isCharging ?Rigidbody2D.velocity.x <= 0 : Agent.velocity.x <= 0);
            _animator.SetBool(IsMovingRight, _isCharging ? Rigidbody2D.velocity.x > 0 || (Rigidbody2D.velocity.y != 0 && Rigidbody2D.velocity.x == 0) : 
                Agent.velocity.x > 0 || (Agent.velocity.y != 0 && Agent.velocity.x == 0));
        }

        private void Refresh()
        {
            var players = GlobalVars.PlayerList.Where(g => g.GetComponent<Player.Player>().IsAlive());
            if (!players.Any() || IsDoingAnAction)
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

            var mel = _random.Next(0, 2) == 0;
            
            if (mel)
                currentAction = StartCoroutine(Attack(player, direction));

            else
                currentAction = StartCoroutine(Charge(player, direction));
            
            UpdateAnimator();
        }
    }
    

    [Serializable]
    public class GolemMeleeParams
    {
        [SerializeAs("Melee damage")] public ushort damage;
        [Tooltip("sec / hit")] public float minFrequency;
        [Tooltip("sec / hit")] public float maxFrequency;
        [SerializeAs("Melee time")] public float meleeTime;
        public float range;

        [Tooltip("Number of different attack direction (only powers of 2)")] [SerializeAs("Number of melee directions")]
        public ushort nbDirections;

        [NonSerialized] public float[] possibleDirections;
    }

    [Serializable]
    public class GolemChargeParams
    {
        [SerializeAs("Charging Speed")] public uint speed;
        [SerializeAs("Charge damage")] public ushort damage;

        [Tooltip("sec / charge")] [SerializeAs("Minimum charging frequency")]
        public float Minfrequency;

        [Tooltip("sec / charge")] [SerializeAs("Maximem charging frequency")]
        public float maxFrequency;

        [SerializeAs("Charge time")] public float time;

        //[Tooltip("Stops to player")] public bool stopOnCollision;
        [NonSerialized]public bool stopOnCollision = true; // else it instant-kills the player
    }
}