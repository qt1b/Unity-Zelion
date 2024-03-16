using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // the only variable
    public float speed;
    public string hitTag;
    // private GameObject gameObject; 
    
    // should initialize this with the right speed and direction
    private Vector2 change;


    void Awake() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 direction = new Vector3(mousePosition.x - gameObject.transform.position.x, mousePosition.y - gameObject.transform.position.y, 0f);
        change = direction;
        gameObject.transform.up = direction;
        // transform.Rotate(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
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

    void Update() {
        transform.position = change * speed;
    }

}
