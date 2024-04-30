using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatEnnemies : MonoBehaviour, IEvent{
    public IAction Action;
    public float Radius = 10f;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(BackgroundCheck());
    }

    // Update is called once per frame
    IEnumerator BackgroundCheck()
    {
        while (!CheckCond()) {
            yield return new WaitForSeconds(1f);
        }
        Action.Activate();
    }

    public bool CheckCond() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, Radius);
        foreach(Collider2D col in colliders)
        {
            if (col.CompareTag("Ennemy"))
                return false;
        }
        return true;
    }
}
