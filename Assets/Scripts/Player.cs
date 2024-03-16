using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    // putting all the player variables and all useful methods for ennemies here

    int swordDamage = 3;
    int bowDammage = 2;
    // POISON bomb, damage by 0.5 sec
    int bombDamage = 2;

    // time variablse
    float inititialSpeed = 7f;
    float playerSpeed = 1f;
    float ennemySpeed = 1f;

    public GameObject Arrow;

    Rigidbody2D myRigidBody;
    Vector3 change;
    float currentSpeed;


    // display variables
    Animator animator;

    // variables controlling the attack system
    // is our player already doing smthg
    // public as we will make ennemies / etc that make the player enter this state
    // capacities avalability
    bool canSwordAttack = true;
    bool canDash = true;
    bool canShootArrow = true;
    bool isDashing = false;

    // time control, to enable time effects
    // bc some bosses will slow down our controls, so this var has to be public
    // public float playerSpeed = 1f;

    // cooldown timers
    // should be used later to instanciate timers for capacities, 
    // allowing to see how much time we have before being able to use the capacity again
    float SwordAttackCooldown = 0.4f;
    float dashCooldown = 1f;
    float bowCooldown = 0.8f;

    // 'animation' timers
    float dashTime = 0.12f;
    float swordTime = 0.4f;

    // variables for attack settings
    float dashPower = 6f;
    float attackSpeedNerf = 0.65f;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = inititialSpeed;
        animator = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame 
    void Update()
    {    
        if (!isDashing) {
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            change.Normalize();   
        }     
        UpdateAnimationAndMove();
        
        // one attack / 'normal' ability at a time
            if (canSwordAttack && Input.GetKeyDown(KeyCode.Space)) {
                StartCoroutine(SwordAttack());
            } 
            else if(canDash && Input.GetKeyDown(KeyCode.LeftShift)){
                StartCoroutine(Dash());
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl)) {
                animator.SetBool("UsingBow",true);
                currentSpeed *= attackSpeedNerf;
            }
            if (Input.GetKey(KeyCode.LeftControl)){
                animator.SetFloat("MouseX",Input.mousePosition.x - Screen.width / 2);
                animator.SetFloat("MouseY",Input.mousePosition.y - Screen.height / 2);
                if ( canShootArrow && Input.GetKeyDown(KeyCode.Mouse0)) {
                    StartCoroutine(ShootArrow());
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl)) {
                animator.SetBool("UsingBow",false);
                currentSpeed = inititialSpeed;
            }

        // time-related capacities should be an exception

        // Debug.Log(change);

    }

    void UpdateAnimationAndMove() {
        if (change != Vector3.zero) {
            MoveCharacter();
            animator.SetFloat("MoveX", change.x);
            animator.SetFloat("MoveY", change.y);
            animator.SetBool("IsMoving",true);
        }
        else {
            animator.SetBool("IsMoving",false);
        }
    }

    void MoveCharacter()
    {
        /*if(Input.GetKey(KeyCode.LeftControl))
        {
            myRigidBody.MovePosition(transform.position + change * (currentSpeed*2) * Time.deltaTime);
        }
        else*/
        myRigidBody.MovePosition(transform.position + change * currentSpeed * Time.deltaTime * playerSpeed);
    }

    public void ChangeSpeedControl(float newSpeedControl) {
        playerSpeed = newSpeedControl;
        animator.SetFloat("AnimationSpeed",newSpeedControl);
    }

    IEnumerator SwordAttack() {
        canSwordAttack = false;GetMouseDirection();
        //animator.ResetTrigger("SwordAttack"); breaks the animation if actived ?
        animator.SetTrigger("SwordAttack");
        currentSpeed *= attackSpeedNerf;
        yield return new WaitForSeconds( playerSpeed * swordTime );
        currentSpeed = inititialSpeed;
        yield return new WaitForSeconds( playerSpeed * SwordAttackCooldown );
        canSwordAttack = true;
    }

     (Vector3, Quaternion) GetMouseDirection() {
        //Vector3 mousePosition = Input.mousePosition;
        //mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // var teta = Mathf.Atan((mousePosition.y - transform.position.y) / (mousePosition.x - transform.position.x));
        var y = (Input.mousePosition.y - Screen.height / 2);
        var x = (Input.mousePosition.x - Screen.width / 2);
        var teta = Mathf.Atan(y / x);
        return (new Vector3(x, y, 0f), Quaternion.Euler(0f,0f,teta * 180 / Mathf.PI - (Input.mousePosition.x > Screen.width / 2 ? 90 : -90)));//Quaternion.Euler(transform.position, direction);
        // transform.up = direction;
        // transform.Rotate(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
    }

     IEnumerator ShootArrow() {
        canShootArrow = false;
        (Vector3 pos, Quaternion rot) = GetMouseDirection();
        GameObject arr = Instantiate(Arrow, transform.position, rot);
        pos.Normalize();
        arr.GetComponent<Projectile>().SetSpeedVector(pos);
        yield return new WaitForSeconds( playerSpeed * bowCooldown );
        canShootArrow = true;
    }

    // dash should be
     IEnumerator Dash(){
        // does not execute the dash if the player is not moving
        if(change.y != 0 || change.x != 0) {
            canDash = false;
            isDashing = true;
            currentSpeed *= dashPower;
            yield return new WaitForSeconds( playerSpeed * dashTime);
            currentSpeed = inititialSpeed;
            isDashing = false;
            yield return new WaitForSeconds( playerSpeed * dashCooldown);
            canDash = true;
        }
    }
}
