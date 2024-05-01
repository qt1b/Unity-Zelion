using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public float speed = 30f;
    public Vector3 direction {get; set;} = Vector3.zero;
    public float controlSpeed {get; set;} = 1f;
    public string hitTag = "Damageable";
    public uint damage = 3; 
    Rigidbody2D myRigidBody;
    
    void Awake() {
        myRigidBody = GetComponent<Rigidbody2D>();
        if (IsServer)
        {
            myRigidBody.Sleep();
            Destroy(gameObject, 3f);
        }
        // _healthBar = GameObject.FindGameObjectWithTag($"PlayerHealth").GetComponent<HealthBar>();
    }
    
    public void SetVelocity(Vector3 givenDirection, float givenControlSpeed) {
        direction = givenDirection;
        controlSpeed = givenControlSpeed;
        myRigidBody.velocity = direction * (speed * 0.2f * controlSpeed);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(hitTag))
        {
            other.GetComponent<Health>().TakeDamages(damage);
            Destroy(gameObject,0.3f);
            myRigidBody.velocity = Vector3.zero;
        } else if (other.CompareTag($"Obstacle")) {
            Destroy(gameObject,0.3f);
            myRigidBody.velocity = Vector3.zero;
        } else if (other.CompareTag($"Player"))
        {
            /*if (other.gameObject.GetComponent<Player>().IsOwner)
                _healthBar.TakeDamagess(20); // should be moved the the player's health script */
            Destroy(gameObject,0.3f);
            myRigidBody.velocity = Vector3.zero;
        }
    }
}