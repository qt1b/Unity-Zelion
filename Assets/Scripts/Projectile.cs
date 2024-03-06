using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // the only variable
    public float speed;
    
    // should initialize this with the right speed and direction
    private Vector2 change;

    void Awake() {
        transform.Rotate(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
    }

    // Start is called before the first frame update
    /*void Awake() {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // change = Vector3.zero;
        change.x = mousePosition.x - transform.position.x;
        change.y = mousePosition.y - transform.position.y;
        change.Normalize();
    }*/

    // is not doing what was expected
    void Update()
    {
        transform.Rotate(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        // transform.Translate((transform.forward * speed * Time.deltaTime));
    }
}
