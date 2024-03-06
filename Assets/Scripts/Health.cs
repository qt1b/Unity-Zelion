using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int hp;
    public float deathDuration;
    int maxHP;
    // bool isDead = false;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        maxHP = hp;
    }

    public void TakeDamage(int amount){
        hp -= amount;
        if (hp <= 0) Die();
    }

    void Die(){
        animator.SetTrigger("Death");
        Destroy(gameObject, deathDuration);
    }

    //called from 'die' animation
    /*void DestroyThisGameObject(){
        Destroy(gameObject);
    }*/
}
