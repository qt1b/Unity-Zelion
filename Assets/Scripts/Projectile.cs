using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 change;
    public float speed;

    // Start is called before the first frame update
    void Awake() {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // change = Vector3.zero;
        change.x = mousePosition.x - transform.position.x;
        change.y = mousePosition.y - transform.position.y;
        change.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((transform.forward * speed * Time.deltaTime));
    }
}
