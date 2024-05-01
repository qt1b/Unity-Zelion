using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonZone : MonoBehaviour , ITimeControl
{
    public uint damage = 1;
    public float radius = 1.3f; // is 2.5 but the poison zone's scale is 2

    public float timeBetweenHits = .5f;
    public float duration = 5f;
    float remaining;
    float currentRemaining;
    float currentTimeBetweenHits;


    void Awake() {
        remaining = duration;
        ChangeTimeControl(1f);
        StartCoroutine(Main());
    }

    private IEnumerator Main()
    {
        while (currentRemaining >= 0)
        {
            yield return new WaitForSeconds(currentTimeBetweenHits);
            currentRemaining -= currentTimeBetweenHits;
            DamageObjects();
        }
        DestroyObject();
    }

    // should be synced over network
    private void DamageObjects()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, radius);
        foreach(Collider2D col in colliders)
        {
            if (col.TryGetComponent(out Health health))
                health.TakeDamages(damage);
        }
    }

    // to be synced
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    // to by synced ? or maybe not ?
    public void ChangeTimeControl(float timeControl)
    {
        currentRemaining = remaining / timeControl;
        currentTimeBetweenHits = timeBetweenHits / timeControl;
    }
}
