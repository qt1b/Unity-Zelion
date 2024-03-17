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

    void Update() {
        myRigidBody.MovePosition(transform.position + direction * speed * Time.deltaTime * controlSpeed);
        // transform.position += vecSpeed * Time.deltaTime * controlSpeed;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(hitTag)) {
            other.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        // error : collision is not defined
        } 
        else {
            Destroy(gameObject);
        }
    }
}