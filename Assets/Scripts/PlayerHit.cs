using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    int swordDamage = 3;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Damageable")) {
            other.GetComponent<Health>().TakeDamage(swordDamage);
        }
    }
}
