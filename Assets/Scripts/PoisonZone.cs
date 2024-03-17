using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonZone : MonoBehaviour
{
    public string hitTag = "Damageable";
    public int damage = 1; 
    public float controlSpeed {get; set;} = 1f;
    // Rigidbody2D myRigidBody;
    public float timeBetweenHits = .5f;
    float currentTime = 0f;
    public float duration = 5f;

    void Start() {
        // may not be the best way to do it, as it does not resist well to time changes
        // Destroy(gameObject,duration);
    }

    void Update()
    {
        currentTime += Time.deltaTime * controlSpeed;
        duration -= Time.deltaTime * controlSpeed;
        if (duration < 0) Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other) {
        print("detected Collision");
        if (currentTime >= timeBetweenHits && other.CompareTag(hitTag)) {
            other.GetComponent<Health>().TakeDamage(damage);
            currentTime = 0f;
            print("inflict dammage");
        }
    }
}
