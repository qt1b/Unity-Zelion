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

        private delegate IEnumerator Attack(GameObject target);
        private List<Attack> _attacks;
        private Random _random = new ();

        [Serialize] private float minX = -6.94f;
        [Serialize] private float minY = 16.32f;
        [Serialize] private float maxX = 6.68f;
        [Serialize] private float maxY = 3.37f;


        private const string _projectileFolder = "Prefabs/Projectiles/FinalEnnemy/";

        private static string Basic => _projectileFolder +  "FireBall";
        private string Small => _projectileFolder + "SmallFireBall";
        private string Follow => _projectileFolder + "FollowBall";
        private string Split => _projectileFolder + "SplitBall";

        private const float tripleBasicReloadTime = 1f;
        private const float tripleBasicAngle = Mathf.PI / 6;
        private const float timeTripleBasic = 1f;
        
        private const float multipleSplitReloadTime = 2f;
        private const float multipleSplitTimeBetweenShoot = 2f;
        private const short multipleSplitNumber = 3;
        private const float multipleSplitEndTime = 2f;

        private const float vortexReloadTime = 2f;
        private const float vortexTimeBetweenShots = 0.2f;
        private const short vortexNumberOfShots = 27;
        private const float vortexRotation = 4 * Mathf.PI / 20;
        private const float vortexTimeEnd = 2f;

        private const float FollowReloadTime = 2f;
        private const float FollowTimeBetween = 0.5f;
        private const short FollowNumber = 10;
        private const float FollowTimeEnd = 2f;

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
                _attacks = new List<Attack> { FollowAttack };
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
            var arr = PhotonNetwork.Instantiate(name, initialPos, rot);
            var projectile = arr.GetComponent<Projectile>();
            projectile.SetVelocity(direction);
            projectile.SetVelocityRPC(projectile.speed * direction.normalized);
        }

        private Vector2 Rotate(Vector2 vector2, float rotation)
        {
            return new Vector2(vector2.x * Mathf.Cos(rotation) - vector2.y * Mathf.Sin(rotation),
                vector2.x * Mathf.Sin(rotation) + vector2.y * Mathf.Cos(rotation));
        }
        private IEnumerator TripleBasic(GameObject target)
        {
            isIncanting = true;
            yield return new WaitForSeconds(tripleBasicReloadTime);
            var direction = target.transform.position - transform.position;
            Shoot(direction, Basic);
            Shoot(Rotate(direction, tripleBasicAngle), Basic);
            Shoot(Rotate(direction, -tripleBasicAngle), Basic);
            yield return new WaitForSeconds(timeTripleBasic);
            Tp();
            isIncanting = false;
        }

        private IEnumerator MultipleSplit(GameObject target)
        {
            isIncanting = true;
            yield return new WaitForSeconds(multipleSplitReloadTime - multipleSplitTimeBetweenShoot);
            for (int i = 0; i < multipleSplitNumber; i++)
            {
                yield return new WaitForSeconds(multipleSplitTimeBetweenShoot);
                var direction = target.transform.position - transform.position;
                Shoot(direction, Split);
            }

            yield return new WaitForSeconds(multipleSplitEndTime);
            Tp();
            isIncanting = false;
        }

        private IEnumerator VortexAttack(GameObject target)
        {
            isIncanting = true;

            yield return new WaitForSeconds(vortexReloadTime - vortexTimeBetweenShots);

            var curDir = target.transform.position - transform.position;
            for (int i = 0; i < vortexNumberOfShots; i++)
            {
                yield return new WaitForSeconds(vortexTimeBetweenShots);
                curDir = Rotate(curDir, vortexRotation);
                Shoot(curDir, Basic);
            }

            yield return new WaitForSeconds(vortexTimeEnd);
            Tp();

            isIncanting = false;
        }


        private IEnumerator FollowAttack(GameObject target)
        {

            isIncanting = true;
            yield return new WaitForSeconds(FollowReloadTime - FollowTimeBetween);

            for (int i = 0; i < FollowNumber; i++)
            {
                yield return new WaitForSeconds(FollowTimeBetween);
                float x = (float)(minX + _random.NextDouble() * (maxX - minX));
                float y = (float)(minY + _random.NextDouble() * (maxY - minY));
                var pos = new Vector3(x, y);
                PhotonNetwork.Instantiate(Follow, pos, Quaternion.identity);
            }

            yield return new WaitForSeconds(FollowTimeEnd);
            Tp();
            isIncanting = false;

        }


        private void Refresh()
        {
            if (isIncanting) return;
            
            var player = GlobalVars.PlayerList.Where(g => g.GetComponent<Player.Player>().IsAlive())
                .OrderBy(g => (g.transform.position - transform.position).sqrMagnitude).FirstOrDefault();

            if (player is null) return;

            StartCoroutine(_attacks[_random.Next(0, _attacks.Count - 1)](player.gameObject));
        }
    }
}
