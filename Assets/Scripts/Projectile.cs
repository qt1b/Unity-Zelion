using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 vecSpeed = new Vector3();
    public float playerSpeed = 1f;
    public string hitTag = "Damageable";
    public int damage = 3; 
    Rigidbody2D myRigidBody;

    
    void Awake() {
        myRigidBody = GetComponent<Rigidbody2D>();
        Destroy(gameObject,5f);
    }

    public void SetSpeedVector(/*float sp,*/ Vector3 vec) {
        // speed = sp;
        vecSpeed = vec * speed;
    }

    void Update() {
        myRigidBody.MovePosition(transform.position + vecSpeed * Time.deltaTime * playerSpeed);
        // transform.position += vecSpeed * Time.deltaTime * playerSpeed;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(hitTag)) {
            other.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        // error : collision is not defined
        } /*else if (other.CompareTag("Collision")) {
            Destroy(gameObject);
        } */
    }
}