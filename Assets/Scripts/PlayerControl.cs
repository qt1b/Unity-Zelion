using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* public enum PlayerState {
    walk,
    attack,
    interact
} */

public class PlayerControl : MonoBehaviour
{
    public float inititialSpeed;
    private float currentSpeed;
    private Rigidbody2D myRigidBody;
    private Vector3 change;

    // display variables
    private Animator animator;

    // variables controlling the attack system
    // is our player already doing smthg
    // public as we will make ennemies / etc that make the player enter this state
    // capacities avalability
    private bool canSwordAttack = true;
    private bool canDash = true;
    private bool canShootArrow = true;
    private bool isDashing = false;

    // time control, to enable time effects
    // bc some bosses will slow down our controls, so this var has to be public
    public float TimeControl = 1f;

    // cooldown timers
    // should be used later to instanciate timers for capacities, 
    // allowing to see how much time we have before being able to use the capacity again
    private float SwordAttackCooldown = 0.6f;
    private float dashCooldown = 2.4f;
    private float bowCooldown = 0.8f;

    // 'animation' timers
    private float dashTime = 0.18f;
    private float swordTime = 0.4f;
    private float bowTime = 0.2f;

    // variables for attack settings
    private float dashPower = 6f;
    private float attackSpeedNerf = 0.65f;

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
        // if (isBusy) return;

        // basic movement
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
            else if (canShootArrow && Input.GetKeyDown(KeyCode.LeftControl)) {
                StartCoroutine(ShootArrow());
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
        myRigidBody.MovePosition(transform.position + change * currentSpeed * Time.deltaTime * TimeControl );

    }

    private IEnumerator SwordAttack() {
        canSwordAttack = false;
        //animator.ResetTrigger("SwordAttack"); breaks the animation if actived ?
        animator.SetTrigger("SwordAttack");
        currentSpeed /= attackSpeedNerf;
        yield return new WaitForSeconds( TimeControl * swordTime );
        currentSpeed = inititialSpeed;
        yield return new WaitForSeconds( TimeControl * SwordAttackCooldown );
        canSwordAttack = true;
    }

    private IEnumerator ShootArrow() {
        canShootArrow = false;
        animator.SetTrigger("BowAttack");
        currentSpeed /= attackSpeedNerf;
        yield return new WaitForSeconds( TimeControl * bowCooldown );
        currentSpeed = inititialSpeed;
        yield return new WaitForSeconds( TimeControl * bowCooldown );
        canShootArrow = true;
    }

    // dash should be
    private IEnumerator Dash(){
        // does not execute the dash if the player is not moving
        // if(change.y != 0 || change.x != 0) {
            canDash = false;
            isDashing = true;
            currentSpeed *= dashPower;
            yield return new WaitForSeconds( TimeControl * dashTime);
            currentSpeed = inititialSpeed;
            isDashing = false;
            yield return new WaitForSeconds( TimeControl * dashCooldown);
            canDash = true;
        // }
    }
}
