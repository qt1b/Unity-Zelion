using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonZone : MonoBehaviour
{
    public uint damage = 1;
    public float radius = 1.3f; // is 2.5 but the poison zone's scale is 2

    public float timeBetweenHits = .5f;
    public float duration = 5f;
    float _remaining;
    float _currentRemaining;
    float _currentTimeBetweenHits;


    void Awake() {
        _remaining = duration;
        ChangeTimeControl(1f);
        StartCoroutine(Main());
    }

    private IEnumerator Main()
    {
        while (_currentRemaining >= 0)
        {
            yield return new WaitForSeconds(_currentTimeBetweenHits);
            _currentRemaining -= _currentTimeBetweenHits;
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
        _currentRemaining = _remaining / timeControl;
        _currentTimeBetweenHits = timeBetweenHits / timeControl;
    }
}
