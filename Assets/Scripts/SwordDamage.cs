using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDammage : MonoBehaviour
{
    public string hitTag;
    // private int SwordDammage = 

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(hitTag)) {
            other.GetComponent<Health>().TakeDamage(3);
        }
    }
}