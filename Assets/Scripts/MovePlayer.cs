using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float speed;
    private Rigidbody2D myRigidBody;
    private Vector3 change;

    //variables pour le dash
    private bool canDash = true;
    private bool isDashing = false;
    private float dashPower = 5f;
    private float dashTime = 0.2f;
    private float dashCooldown = 2f;


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    // 
    void Update()
    {
        if(isDashing){return;}
    
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        change.Normalize();

        // Debug.Log(change);
        if (change != Vector3.zero) {
            MoveCharacter();
        }

        if(Input.GetKey(KeyCode.LeftShift) && canDash==true)
        {
            StartCoroutine(Dash());
        }
        Debug.Log(change);

    }
    void MoveCharacter()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
            myRigidBody.MovePosition(transform.position + change * (speed*2) * Time.deltaTime);
        }
        else
        {
            myRigidBody.MovePosition(transform.position + change * speed * Time.deltaTime);
        }

    }

    // on ne peut pas dash vers le haut, probleme si on appuie plusieurs fois rapidements pour dash
    private IEnumerator Dash(){
        canDash = false;
        isDashing = true;
        if(change.y != 0 || change.x != 0)
        {
            myRigidBody.MovePosition(transform.position + change * dashPower);
        }
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

}
