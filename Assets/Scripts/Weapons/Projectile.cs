using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : NetworkBehaviour {
    public float speed = 30f;
    public Vector3 direction { get; set; } = Vector3.zero;
    public float controlSpeed { get; set; } = 1f;
    public uint damage = 3;
    Rigidbody2D myRigidBody;

    void Awake() {
        myRigidBody = GetComponent<Rigidbody2D>();
        // some arrows are not destroying ???
        DestroyAfterSecs(4f);
        /*
         if (IsServer) {
            myRigidBody.Sleep();
            Destroy(gameObject);
        } */
        // _healthBar = GameObject.FindGameObjectWithTag($"PlayerHealth").GetComponent<HealthBar>();
    }

    public void SetVelocity(Vector3 givenDirection, float givenControlSpeed) {
        direction = givenDirection;
        controlSpeed = givenControlSpeed;
        myRigidBody.velocity = direction * (speed * 0.2f * controlSpeed);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.TryGetComponent(out IHealth health))
            health.TakeDamages(damage);
        myRigidBody.velocity = Vector3.zero;
        DestroyAfterSecs(.2f);
    }

    void DestroyAfterSecs(float secs) {
        if (IsServer) 
            DestrowAfterSecsServer(secs);
        else
            DestroyAfterSecsServerRPC(secs);
    }
    
    void DestrowAfterSecsServer(float secs) {
        Destroy(gameObject, secs);
    }

    [ServerRpc]
    void DestroyAfterSecsServerRPC(float secs) {
        DestrowAfterSecsServer(secs);
    }
}