using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int hp = 1;
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
        // isDead = true;
        animator.SetBool("isDead", true);
        Destroy(gameObject,0.7f);
    }

    //called from 'die' animation
    /*void DestroyThisGameObject(){
        Destroy(gameObject);
    }*/
}
