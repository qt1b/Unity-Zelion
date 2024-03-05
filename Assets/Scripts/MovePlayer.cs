using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* public enum PlayerState {
    walk,
    attack,
    interact
} */

public class MovePlayer : MonoBehaviour
{
    public float inititialSpeed;
    private Rigidbody2D myRigidBody;
    private Vector3 change;
    private float currentSpeed;

    // public PlayerState currentState;

    // display variables
    private Animator animator;
    public bool questionMarkActive = false;

    // variables controlling the attack system
    // is our player already doing smthg
    // public as we will make ennemies / etc that make the player enter this state
    // capacities avalability
    private bool canSwordAttack = true;
    private bool canDash = true;
    private bool isDashing = false;

    // cooldown timers
    private float SwordAttackCooldown = 0.6f;
    private float dashCooldown = 2.4f;

    // 'animation' timers
    private float dashTime = 0.1f;
    private float swordTime = 0.4f;

    //variables pour le dash
    private float dashPower = 10f;
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

        if (questionMarkActive) {
            // idk how to activate the question mark, it surely is needed to import it with a variable
            print("questionMarkActive");
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
        if(Input.GetKey(KeyCode.LeftControl))
        {
            myRigidBody.MovePosition(transform.position + change * (currentSpeed*2) * Time.deltaTime);
        }
        else
        {
            myRigidBody.MovePosition(transform.position + change * currentSpeed * Time.deltaTime);
        }

    }

    private IEnumerator SwordAttack() {
        canSwordAttack = false;
        //animator.ResetTrigger("SwordAttack"); breaks the animation if actived ?
        animator.SetTrigger("SwordAttack");
        currentSpeed /= attackSpeedNerf;
        yield return new WaitForSeconds(swordTime);
        currentSpeed = inititialSpeed;
        yield return new WaitForSeconds(SwordAttackCooldown);
        canSwordAttack = true;
    }

    // dash should be
    private IEnumerator Dash(){
        // does not execute the dash if the player is not moving
        if(change.y != 0 || change.x != 0) {
            canDash = false;
            isDashing = true;
            currentSpeed *= dashPower;
            yield return new WaitForSeconds(dashTime);
            currentSpeed = inititialSpeed;
            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }

    
}
