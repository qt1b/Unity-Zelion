using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IHealth
{
    public uint MaxHealth;
    private uint hp;
    public float deathDuration;

    void Start()
    {
        hp = MaxHealth;
    }

    public void TakeDamage(uint damage){
        if (damage >= hp)
            StartCoroutine(Die());
        else hp -= damage;
    }

    public void Heal(uint heal)
    {
        if (heal + hp >= MaxHealth)
            hp = MaxHealth;
        else hp += heal;
    }

    IEnumerator Die() {
        if (gameObject.TryGetComponent(out Collider2D collider))
            collider.enabled = false;
        if (gameObject.TryGetComponent(out Animator animator))
        {
            animator.SetTrigger("Death");
            // int deathDuration = animator.GetInteger("DeathDuration");
        }
        yield return new WaitForSeconds(deathDuration);
        DestroyGameObject();
    }

    // sync every function from the die function



    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
    //called from 'die' animation
    /*void DestroyThisGameObject(){
        Destroy(gameObject);
    }*/
}
