using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public class Health : MonoBehaviour, IHealth
{
    [FormerlySerializedAs("MaxHealth")] public uint maxHealth;
    // to NetworkVariable ??
    private uint _hp;
    public float deathDuration;
    private SpriteRenderer _spriteRenderer; // to change color when hit

    void Start()
    {
        _hp = maxHealth;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void TakeDamages(uint damage){
        if (damage >= _hp)
            StartCoroutine(Die());
        else _hp -= damage;
        StartCoroutine(ChangeColorWait(new Color(1, 0.3f, 0.3f, 1), 0.5f)); // red with transparency
        // must add here some code to change the color for some frames: that way we will see when we make damages to an enemy/object
    }

    public void Heal(uint heal)
    {
        if (heal + _hp >= maxHealth)
            _hp = maxHealth;
        else _hp += heal;
        StartCoroutine(ChangeColorWait(new Color(0.3f, 1, 0.3f, 1), 0.5f)); // green with transparency
    }

    IEnumerator Die() {
        if (gameObject.TryGetComponent(out Collider2D collider2D))
            collider2D.enabled = false;
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

    // UNTESTED, should change the color
    IEnumerator ChangeColorWait(Color color, float time) {
        Color baseColor = _spriteRenderer.color;
        ChangeColor(color);
        yield return new WaitForSeconds(time);
        ChangeColor(baseColor);
    }
    // to be synced over network
    void ChangeColor(Color color) {
        _spriteRenderer.color = color;
    }
}