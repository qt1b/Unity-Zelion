using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public Vector3 direction {get; set;} = Vector3.zero;
    public float controlSpeed {get; set;} = 1f;
    public string hitTag = "Damageable";
    public int damage = 3; 
    Rigidbody2D myRigidBody;

    
    void Awake() {
        myRigidBody = GetComponent<Rigidbody2D>();
        Destroy(gameObject,3f);
    }


    public void SetVelocity(Vector3 givenDirection, float givenControlSpeed) {
        direction = givenDirection;
        controlSpeed = givenControlSpeed;
        myRigidBody.velocity = direction * (speed * 0.2f * controlSpeed);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(hitTag))
        {
            print("hit " + other.name);
            other.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject,0.3f);
            myRigidBody.velocity = Vector3.zero;
        } else if (other.CompareTag($"Obstacle")) {
            Destroy(gameObject,0.3f);
            myRigidBody.velocity = Vector3.zero;
        }
    }
}