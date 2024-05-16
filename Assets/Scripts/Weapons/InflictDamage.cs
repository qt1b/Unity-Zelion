using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflictDammage : MonoBehaviour
{
    public string hitTag = "Damageable";
    public uint damage = 3; 

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out Health health)) {
            health.TakeDamages(damage);
            // gameObject.SetActive(false);
        }
    }
}