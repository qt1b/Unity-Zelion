using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float speed;
    private Rigidbody2D myRigidBody;
    private Vector3 change;

    // display variables
    private Animator animator;
    public bool questionMarkActive = false;

    // variables controlling the attack system
    // is our player already doing smthg
    // public as we will make ennemies / etc that make the player enter this state
    public bool isBusy = false;
    

    // capacities avalability
    private bool canSwordAttack = true;
    private bool canDash = true;

    // cooldown timers
    private float SwordAttackCooldown = 1.2f;
    private float dashCooldown = 2.4f;

    // 'animation' timers
    private float dashTime = 0.2f;
    private float swordTime = 0.4f;

    //variables pour le dash
    private float dashPower = 5f;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame 
    void Update()
    {    
        // if (isBusy) return;

        // basic movement
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        change.Normalize();        
        UpdateAnimationAndMove();
        
        // one attack / 'normal' ability at a time
        if (!isBusy) {
            if (Input.GetKeyDown(KeyCode.Space) && canSwordAttack==true) {
                StartCoroutine(SwordAttack());
            } 
            else if(Input.GetKeyDown(KeyCode.LeftShift) && canDash==true){
                StartCoroutine(Dash());
            }
        }

        if {questionMarkActive} (
            // idk how to activate the question mark, it surely is needed to import it with a variable
            print("questionMarkActive");
        )

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
            myRigidBody.MovePosition(transform.position + change * (speed*2) * Time.deltaTime);
        }
        else
        {
            myRigidBody.MovePosition(transform.position + change * speed * Time.deltaTime);
        }

    }

    private IEnumerator SwordAttack() {
        canSwordAttack = false;
        isBusy = true;
        //animator.ResetTrigger("SwordAttack"); breaks the animation if actived ?
        animator.SetTrigger("SwordAttack");
        yield return new WaitForSeconds(swordTime);
        isBusy = false;
        yield return new WaitForSeconds(SwordAttackCooldown);
        canSwordAttack = true;
    }

    // on ne peut pas dash vers le haut, probleme si on appuie plusieurs fois rapidements pour dash
    private IEnumerator Dash(){
        // does not execute the dash if the player is not moving
        if(change.y != 0 || change.x != 0) {
            canDash = false;
            isBusy = true;
            myRigidBody.MovePosition(transform.position + change * dashPower);
            yield return new WaitForSeconds(dashTime);
            isBusy = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }

}
