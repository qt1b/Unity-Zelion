using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using Photon.PhotonUnityNetworking.Code;
using Porperty_Attributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Weapons;


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

        [Tooltip("If ennemy can shoot arrows")]
        public bool isShooter;
        [SerializeField] private EnnemyShootParams ShootParams;

        [Tooltip("If ennemy do short distance attacks")]
        [HorizontalLine(Padding = 10f)] public bool isMelee;
        [SerializeField] private EnnemyMeleeParams MeleeParams;

        [Space] public NavMeshAgent Agent;

        private bool _canShoot;
        private bool _canMelee;
        private float LowerMargin => distanceFromPlayer - distanceFromPlayerMargin;
        private float HigherMargin => distanceFromPlayer + distanceFromPlayerMargin;
        public float speed => GlobalVars.EnnemySpeed;

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

        #region Attacks

        private IEnumerator Shoot(int arrowIndex, Vector3 initialPos, Vector3 direction)
        {
            _canShoot = false;
            var rot = Quaternion.Euler(0f, 0f,
                Mathf.Atan(direction.y / direction.x) * 180 / Mathf.PI + (direction.x < 0 ? 90 : -90));
            var arr = PhotonNetwork.Instantiate(
                "Prefabs/Projectiles/Arrow",
                initialPos + direction * ShootParams.startDistance, rot);
            var projectile = arr.GetComponent<Projectile>();
            projectile.damage = ShootParams.damage;
            projectile.SetVelocity(direction);
            projectile.speed = ShootParams.arrowSpeed;

            // animator for shooting here

            yield return new WaitForSeconds(ShootParams.shootFrequency / GlobalVars.EnnemySpeed);
            _canShoot = true;
        }

        private IEnumerator Attack(Player.Player player, Vector3 direciton)
        {
            _canMelee = false;
            player.TakeDamages(MeleeParams.meleeDamage);
            // animator for melee here
            yield return new WaitForSeconds(MeleeParams.meleeFrequency / GlobalVars.EnnemySpeed);
            _canMelee = true;
        }

        #endregion

        // Update is called once per frame
        private void Refresh()
        {
            List<Player.Player> players = new(GlobalVars.PlayerList);
            if (players.Count == 0)
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
        }
    }

    [Serializable]
    public class EnnemyShootParams
    {
        [SerializeAs("Arrow damage")] public uint damage;
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
        [SerializeAs("Melee damage")] public uint meleeDamage;
        [Tooltip("sec / hit")] public float meleeFrequency;
        public float meleeRange;
    }
}