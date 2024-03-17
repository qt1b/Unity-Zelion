using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    // putting all the player variables and all useful methods for ennemies here

    int swordDamage = 3;
    int arrowDammage = 2;
    // POISON bomb, damage by 0.5 sec
    int bombDamage = 1;

    // time variablse
    public float inititialSpeed = 7f;
    public float currentSpeed;

    // variables related to time control : player speed should be modified by external functions
    public float playerControlSpeed = 1f;
    // float ennemySpeed = 1f; should not be used 

    // would be better to get them by script
    public GameObject ArrowRef;
    public GameObject PoisonBombRef;
    public GameObject PoisonZoneRef;
    public GameObject PoisonZonePreviewRef;

    Rigidbody2D myRigidBody;
    public Vector3 change;


    // display variables
    Animator animator;

    // variables controlling the attack system
    // is our player already doing smthg
    // public as we will make ennemies / etc that make the player enter this state
    // capacities avalability
    bool canSwordAttack = true;
    bool canDash = true;
    bool canShootArrow = true;
    bool canThrowPoisonBomb = true;
    
    bool isDashing = false;
    bool isAiming = false;

    // time control, to enable time effects
    // bc some bosses will slow down our controls, so this var has to be public
    // public float playerControlSpeed = 1f;

    // cooldown timers
    // should be used later to instanciate timers for capacities, 
    // allowing to see how much time we have before being able to use the capacity again
    float SwordAttackCooldown = 0.4f;
    float dashCooldown = 1f;
    float bowCooldown = 0.4f;
    float poisonBombCooldown = 9f;

    // 'animation' timers
    float dashTime = 0.12f;
    float swordTime = 0.4f;

    // variables for attack settings
    float dashPower = 6f;
    float attackSpeedNerf = 0.65f;
    float maxBombDist = 4f;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = inititialSpeed;
        animator = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();
        animator.speed = playerControlSpeed;
    }

    // Update is called once per frame 
    void Update()
    {    
        if (!isDashing) {
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            change.Normalize();
            // one attack / 'normal' ability at a time
            if (!isAiming) {
                if (canSwordAttack && Input.GetKeyDown(KeyCode.Space)) {
                    StartCoroutine(SwordAttack());
                } 
                else if(canDash && Input.GetKeyDown(KeyCode.LeftShift)){
                    StartCoroutine(Dash());
                }
                else if (Input.GetKeyDown(KeyCode.LeftControl)) {
                    animator.SetBool("UsingBow",true);
                    isAiming = true;
                    currentSpeed *= attackSpeedNerf;
                }
            }
            else 
            {
                animator.SetFloat("MouseX",Input.mousePosition.x - Screen.width / 2);
                animator.SetFloat("MouseY",Input.mousePosition.y - Screen.height / 2);
                if ( canShootArrow && Input.GetKeyDown(KeyCode.Mouse0)) {
                    StartCoroutine(ShootArrow());
                }
                else if (canThrowPoisonBomb & Input.GetKeyDown(KeyCode.Mouse1)) {
                    StartCoroutine(ThrowPoisonBomb());
                }
                if (Input.GetKeyUp(KeyCode.LeftControl)) {
                    animator.SetBool("UsingBow",false);
                    isAiming = false;
                    currentSpeed = inititialSpeed * playerControlSpeed;
                }
            }
        }
        UpdateAnimationAndMove();
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
        myRigidBody.MovePosition(transform.position + change * (currentSpeed * Time.deltaTime * playerControlSpeed));
    }

    public void ChangePlayerControlSpeedControl(float newSpeedControl) {
        playerControlSpeed = newSpeedControl;
        currentSpeed = playerControlSpeed * currentSpeed;
        // should change the speed of all the other players too, by calling this very same function
        animator.speed = playerControlSpeed;
    }

    public void ChangeEltSpeedControl(float newSpeedControl) {
        // modifies all the IEltSpeed interfaces
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SwordAttack() {
        canSwordAttack = false;GetMouseDirection();
        //animator.ResetTrigger("SwordAttack"); breaks the animation if actived ?
        animator.SetTrigger("SwordAttack");
        currentSpeed *= attackSpeedNerf;
        yield return new WaitForSeconds( swordTime / playerControlSpeed);
        currentSpeed = inititialSpeed * playerControlSpeed;
        yield return new WaitForSeconds( SwordAttackCooldown / playerControlSpeed );
        canSwordAttack = true;
    }

    (Vector3, Quaternion) GetMouseDirection() {
        /* version taking the player's position */
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        var y = (mousePosition.y - transform.position.y);
        var x = (mousePosition.x - transform.position.x);
        var teta = Mathf.Atan(y / x);        
        /* version taking the middle of the screen as reference
        var y = (Input.mousePosition.y - Screen.height / 2);
        var x = (Input.mousePosition.x - Screen.width / 2);
        var teta = Mathf.Atan(y / x);
        */
        return (new Vector3(x, y, 0f),
            Quaternion.Euler(0f, 0f, teta * 180 / Mathf.PI - (Input.mousePosition.x > Screen.width / 2 ? 90 : -90)));   
    }

    IEnumerator ShootArrow() {
        canShootArrow = false;
        (Vector3 pos, Quaternion rot) = GetMouseDirection();
        //GameObject arr = Instantiate(Resources.Load("Prefabs"+"Arrow"), transform.position, rot);
        GameObject arr = Instantiate(ArrowRef, transform.position, rot);
        // pos.Normalize();
        Projectile projectile= arr.GetComponent<Projectile>();
        projectile.direction = pos.normalized;
        projectile.controlSpeed = playerControlSpeed;
        yield return new WaitForSeconds( bowCooldown / playerControlSpeed );
        canShootArrow = true;
    }

    IEnumerator ThrowPoisonBomb() {
        canThrowPoisonBomb = false;
        (Vector3 pos, Quaternion rot) = GetMouseDirection();
        GameObject pBomb = Instantiate(PoisonBombRef, transform.position, rot);
        yield return new WaitForSeconds( poisonBombCooldown / playerControlSpeed );
        canThrowPoisonBomb = true;
    }

    IEnumerator Dash(){
        // does not execute the dash if the player is not moving
        if (change.y != 0 || change.x != 0) {
            canDash = false;
            isDashing = true;
            currentSpeed *= dashPower;
            yield return new WaitForSeconds( dashTime / playerControlSpeed );
            currentSpeed = inititialSpeed * playerControlSpeed;
            isDashing = false;
            yield return new WaitForSeconds( dashCooldown / playerControlSpeed );
            canDash = true;
        }
    }
}
