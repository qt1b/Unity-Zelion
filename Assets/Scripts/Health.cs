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

    public void TakeDamages(uint damage){
        if (damage >= hp)
            StartCoroutine(Die());
        else hp -= damage;
        // must add here some code to change the color for some frames: that way we will see when we make damages to an enemy/object
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
        if (gameObject.TryGetComponent(out Animator animator)) {
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
}
