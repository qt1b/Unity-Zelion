using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using JetBrains.Annotations;
using Photon.PhotonUnityNetworking.Code;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Weapons;
using Random = System.Random;

namespace Ennemies{
    public class Eclipse : MonoBehaviourPunCallbacks
    {
        public float projStartDist;

        private delegate IEnumerator Attack(Vector2 direction);
        private List<Attack> _attacks;
        private Random _random = new ();

        [Serialize] private float minX = -6.94f;
        [Serialize] private float minY = 16.32f;
        [Serialize] private float maxX = 6.68f;
        [Serialize] private float maxY = 3.37f;


        private const string _projectileFolder = "FinalEnnemy/";

        private static string Basic => _projectileFolder +  "FireBall";
        private string Small => _projectileFolder + "SmallFireBall";
        private string Follow => _projectileFolder + "FollowBall";
        private string Split => _projectileFolder + "SplitBall";

        private const float tripleBasicReloadTime = 1f;
        private const float tripleBasicAngle = Mathf.PI / 6;
        private const float timeTripleBasic = 1f;

        private bool isIncanting = false;
        private void Awake()
        {
            gameObject.tag = "Boss";
            if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
            {
                enabled = false;
            }
            else
            {
                _attacks = new List<Attack> { TripleBasic };
                InvokeRepeating(nameof(Refresh), .1f, .1f);
            }
        }

        private void Tp()
        {
            float x = (float)(minX + _random.NextDouble() * (maxX - minX));
            float y = (float)(minY + _random.NextDouble() * (maxY - minY));
            transform.position = new Vector3(x, y, 0f);
        }

        private void Shoot(Vector2 direction, string name)
        {
            var initialPos = (Vector2)transform.position + direction.normalized * projStartDist;
            var rot = Quaternion.Euler(0f, 0f,
                Mathf.Atan(direction.y / direction.x) * 180 / Mathf.PI + (direction.x < 0 ? 90 : -90));
            var arr = PhotonNetwork.Instantiate(
                $"Prefabs/Projectiles/{name}",
                initialPos, rot);
            var projectile = arr.GetComponent<Projectile>();
            projectile.SetVelocity(direction);
            projectile.SetVelocityRPC(projectile.speed * direction.normalized);
        }

        private IEnumerator TripleBasic(Vector2 direction)
        {
            isIncanting = true;
            yield return new WaitForSeconds(tripleBasicReloadTime);
            Shoot(direction, Basic);
            Shoot(new Vector2(direction.x * Mathf.Cos(tripleBasicAngle) - direction.y * Mathf.Sin(tripleBasicAngle), 
                direction.x * Mathf.Sin(tripleBasicAngle) +  direction.y * Mathf.Cos(tripleBasicAngle)), Basic);
            Shoot(new Vector2(direction.x * Mathf.Cos(-tripleBasicAngle) - direction.y * Mathf.Sin(-tripleBasicAngle), 
                direction.x * Mathf.Sin(-tripleBasicAngle) +  direction.y * Mathf.Cos(-tripleBasicAngle)), Basic);
            yield return new WaitForSeconds(timeTripleBasic);
            Tp();
            isIncanting = false;
        }


        private void Refresh()
        {
            var player = GlobalVars.PlayerList.Where(g => g.GetComponent<Player.Player>().IsAlive())
                .OrderBy(g => (g.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
            if (isIncanting || player is null) return;

            StartCoroutine(_attacks[_random.Next(0, _attacks.Count - 1)](player.transform.position - transform.position));
        }
    }
}
