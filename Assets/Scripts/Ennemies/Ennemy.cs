using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
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
    public class Ennemy : MonoBehaviourPunCallbacks
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

        [Header("Attack Parameters")] [Tooltip("If ennemy can shoot arrows")]
        public bool isShooter;

        [SerializeField] private EnnemyShootParams _shoot;

        [Tooltip("If ennemy do short distance attacks")]
        public bool isMelee;

        [SerializeField] private EnnemyMeleeParams _melee;
        
        [Tooltip("if the ennemy can charge")] public bool isCharger;

        [SerializeField] private EnnemyChargeParams _charge;

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

        private Coroutine currentAction;
        public bool IsMoving => Agent.velocity != Vector3.zero || _isCharging;

        private void Start()
        {
            if (PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient) return;
            enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }

        public override void OnEnable()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (isMelee)
                {
                    _melee.hitbox.GetComponentInChildren<InflictDammage>().damage = _melee.damage;
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

        private IEnumerator CanShootReset()
        {
            _canShoot = false;
            yield return new WaitForSeconds(_random.Next((int)(_shoot.minFrequency * 100f),
                (int)(_shoot.maxFrequency * 100f)) / 100f / GlobalVars.EnnemySpeed);
            _canShoot = true;
        }

        private IEnumerator ShootAux(Vector3 initialPos, Vector3 direction)
        {
            var rot = Quaternion.Euler(0f, 0f,
                Mathf.Atan(direction.y / direction.x) * 180 / Mathf.PI + (direction.x < 0 ? 90 : -90));
            var arr = PhotonNetwork.Instantiate(
                $"Prefabs/Projectiles/{_shoot.projectileName}",
                initialPos + direction * _shoot.startDistance, rot);
            var projectile = arr.GetComponent<Projectile>();
            projectile.damage = _shoot.damage;
            projectile.SetVelocity(direction);
            projectile.speed = _shoot.speed;
            yield break;
        }

        private IEnumerator Shoot(Vector3 initialPos, Vector3 direction)
        {
            _isShooting = true;
            StartCoroutine(CanShootReset());
            // animator for shooting here
            yield return new WaitForSeconds(_shoot.drawingTime);
            StartCoroutine(ShootAux(initialPos, direction));
            _isShooting = false;
            yield break;
        }

        private IEnumerator CanMeleeReset()
        {
            _canMelee = false;
            yield return new WaitForSeconds(_random.Next((int)(_melee.minFrequency * 100f),
                (int)(_melee.maxFrequency * 100f)) / 100f / GlobalVars.EnnemySpeed);
            _canMelee = true;
        }

        private float GetHitboxRotation(Vector3 direction)
        {
            var angle = Mathf.Atan(direction.y / direction.x) * 180 / Mathf.PI + (direction.x >= 0 ? 0 : 180);
            return _melee.possibleDirections.Aggregate((x, y) => Math.Abs(x - angle) < Math.Abs(y - angle) ? x : y);
        }

        private IEnumerator Attack(Player.Player player, Vector3 direciton)
        {
            _isMeleeing = true;
            StartCoroutine(CanMeleeReset());

            _melee.hitbox.transform.eulerAngles = new Vector3(0, 0, GetHitboxRotation(direciton));
            _melee.hitbox.SetActive(true);
            // animator for melee here


            yield return new WaitForSeconds(_melee.meleeTime);
            _melee.hitbox.SetActive(false);

            _isMeleeing = false;
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
            StartCoroutine(CanChargeReset());
            Agent.ResetPath();
            Rigidbody2D.sleepMode = RigidbodySleepMode2D.StartAwake;
            Rigidbody2D.velocity = direction.normalized * _charge.speed;
            yield return new WaitForSeconds(_charge.time);
            Rigidbody2D.velocity = new UnityEngine.Vector2();
            _isCharging = false;
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
                    if (_charge.stopOnCollision)
                    {
                        StopCoroutine(currentAction);
                        _isCharging = false;
                        Rigidbody2D.velocity = UnityEngine.Vector2.zero;
                        playerRigidbody.AddForce(-curDirection * .1f);
                    }
                    else
                    {
                        playerRigidbody.AddForce(curDirection.y * directionPlayer.y > curDirection.x * directionPlayer.x
                            ? new UnityEngine.Vector2(curDirection.y, -curDirection.x * .1f)
                            : new UnityEngine.Vector2(-curDirection.y, curDirection.x) * .1f);
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

        private void Refresh()
        {
            List<Player.Player> players = new(GlobalVars.PlayerList);
            if (players.Count == 0 || IsDoingAnAction)
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


            if (isShooter && distance >= _shoot.rangeMin && distance <= _shoot.rangeMax &&
                _canShoot && (!forwardWhenPlayerTooClose || distance >= LowerMargin))
                currentAction = StartCoroutine(Shoot(pos, direction.normalized));

            if (isMelee && distance <= _melee.range && _canMelee)
                currentAction = StartCoroutine(Attack(player, direction));

            if (isCharger && _canCharge)
                currentAction = StartCoroutine(Charge(player, direction));
        }
    }


    [Serializable]
    public class EnnemyShootParams
    {
        [SerializeAs("Projectile's prefab name")]
        public string projectileName;

        [SerializeAs("Arrow damage")] public ushort damage;

        [SerializeAs("Max shoot frequency")] [Tooltip("sec / arrow")]
        public float maxFrequency;

        [SerializeAs("Min shoot frequency")] [Tooltip("sec / arrow")]
        public float minFrequency;

        public float drawingTime;

        [Tooltip("max distance where the ennemy will shoot arrows")]
        public float rangeMax;

        [Tooltip("min distance where the ennemy will shoot arrows")]
        public float rangeMin;

        [SerializeAs("Arrow speed")] public float speed;

        [Tooltip("Distance from the ennemy's center where arrows will spawn")]
        public float startDistance;
    }

    [Serializable]
    public class EnnemyMeleeParams
    {
        [SerializeAs("Melee damage")] public ushort damage;
        [Tooltip("sec / hit")] public float minFrequency;
        [Tooltip("sec / hit")] public float maxFrequency;
        [SerializeAs("Melee time")] public float meleeTime;
        public float range;

        [Tooltip("Number of different attack direction (only powers of 2)")] [SerializeAs("Number of melee directions")]
        public ushort nbDirections;

        public GameObject hitbox;

        [NonSerialized] public float[] possibleDirections;
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

        [SerializeAs("Charge time")] public float time;

        [Tooltip("Stops to player")] public bool stopOnCollision;
    }
}