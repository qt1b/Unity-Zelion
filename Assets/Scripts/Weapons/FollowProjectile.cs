using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Weapons;

[Serializable]
public class FollowProjectile : Projectile
{
    [Tooltip("angular speed in 360º / sec")] public float angularSpeed;
    private static List<Player.Player> _players = GlobalVars.PlayerList;
    
    void Awake()
    {
        _curSpeed = GlobalVars.ProjectileSpeed;
        _myRigidBody = GetComponent<Rigidbody2D>();
        // some arrows are not destroying ???
        StartCoroutine(DestroyAfterSecs(dieTime));
        
    }
    void Update()
    {
        ModifySpeed();

        var closestPlayer = GlobalVars.PlayerList.Where(g => g.GetComponent<Player.Player>().IsAlive())
            .OrderBy(g => (g.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
        if (closestPlayer is null) return;
        
        Vector3 vel3 = _myRigidBody.velocity;
        vel3 = Vector3.RotateTowards(vel3, (closestPlayer.transform.position - transform.position).normalized * speed, 
            2 * Mathf.PI * angularSpeed * Time.deltaTime * _curSpeed, .0025f);


        _myRigidBody.velocity = vel3;
    }
}
