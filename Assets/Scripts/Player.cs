using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class Player : NetworkBehaviour, ITimeControl, IHealth
{
    // PLAYER MOVEMENT
    
    /*
    Notes : Player CANNOT be damageable, only ennemies and breakable objects should be tagged as "damageable"


    */


    // putting all the player variables and all useful methods for ennemies here

    /*
    int swordDamage = 3;
    int arrowDammage = 2;
    // POISON bomb, damage by 0.5 sec
    int bombDamage = 1;
    */

    [Header("Speed")]
    public float inititialSpeed = 7f;
    public float currentSpeed;
    float inititialWithControl;

    // variables related to time control : player speed should be modified by external functions
    [Header("Time Control")]
    public float controlSpeed = 1f;
    // float ennemySpeed = 1f; should not be used 

    // would be better to get them by script
    [Header("Projectiles")]
    public GameObject ArrowRef;
    private GameObject ArrowPreviewRef;
    public GameObject PoisonBombRef;
    public GameObject PoisonZoneRef;
    private GameObject PoisonZonePreviewRef;

    Rigidbody2D myRigidBody;
    public Vector3 change = Vector3.zero;
    public Vector3 notNullChange = new Vector3(0,1,0);




    // variables controlling the attack system
    // is our player already doing smthg
    // public as we will make ennemies / etc that make the player enter this state
    // capacities avalability
    bool canSwordAttack = true;
    bool canDash = true;
    bool canShootArrow = true;
    bool canThrowPoisonBomb = true;

    static public bool isDashing = false; // so the stamina bar can use it
    bool isWielding = false;
    bool isAimingArrow = false;
    bool isAimingBomb = false;

    // time control, to enable time effects
    // bc some bosses will slow down our controls, so this var has to be public

    // cooldown timers
    // should be used later to instanciate timers for capacities, 
    // allowing to see how much time we have before being able to use the capacity again
    float swordTime = 0.2f;
    float _swordAttackCooldown = 0.4f;
    float totalSwordRot = 100f;

    float dashCooldown = 1f;
    float bowCooldown = 0.2f;
    float poisonBombCooldown = 7f;

    // 'animation' timers
    float dashTime = 0.12f;


    // variables for attack settings
    float dashPower = 6f;
    float attackSpeedNerf = 0.65f;
    float maxBombDist = 7f;
    float swordDist = 0.3f;


    // display variables
    Animator animator;
    GameObject _swordHitzone;
    Collider2D _swordHitzoneCollider;
    Animator _swordHitzoneAnimator;
    public Camera Camera;

    private HealthBar _healthBar;
    private StaminaBar _staminaBar;
    private ManaBar _manaBar;
    private SpriteRenderer _spriteRenderer;
    // public uint staminaLevel
    // the best should be that StaminaBar manages stamina,
    // healthbar manages health and manabar manages mana
    

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }
    void Start()
    {
        inititialWithControl = inititialSpeed * controlSpeed;
        currentSpeed = inititialWithControl;
        animator = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();
        animator.speed = controlSpeed ;
        _swordHitzone = transform.GetChild(0).gameObject;
        _swordHitzoneCollider = _swordHitzone.GetComponent<Collider2D>();
        _swordHitzoneAnimator = _swordHitzone.GetComponent<Animator>();
        _swordHitzone.SetActive(false);
        ArrowPreviewRef = transform.GetChild(1).gameObject;
        PoisonZonePreviewRef = transform.GetChild(2).gameObject;
        _healthBar = FindObjectOfType<HealthBar>();
        _staminaBar = FindObjectOfType<StaminaBar>();
        _manaBar = FindObjectOfType<ManaBar>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame 
    void Update()
    {    
        if(PauseMenu.GameIsPaused){
            return;
        }
        if (!isDashing) {
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            change.Normalize();
            if (change != Vector3.zero) notNullChange = change;
            // one attack / 'normal' ability at a time
            if (isAimingArrow) {
                if (!ArrowPreviewRef.activeSelf && canShootArrow /* &&  _staminaBar.CanTakeDamages(2) */ ) ArrowPreviewRef.SetActive(true);
                PlacePreviewArrow();
                if(Input.GetKeyUp(KeyCode.Mouse0)){
                    if ( canShootArrow && _staminaBar.TryTakeDamages(2) ) StartCoroutine(ShootArrow());
                    isAimingArrow = false;
                    currentSpeed = inititialWithControl;
                    ArrowPreviewRef.SetActive(false);
                    animator.SetBool("AimingBow",false);
                }
            }
            else if (isAimingBomb) {
                if (!PoisonZonePreviewRef.activeSelf && canThrowPoisonBomb && _manaBar.CanTakeDamages(10) ) PoisonZonePreviewRef.SetActive(true);
                PlacePreviewZone();
                if (Input.GetKeyUp(KeyCode.Mouse1) ) {
                    if ( canThrowPoisonBomb && _manaBar.TryTakeDamages(10) ) StartCoroutine(ThrowPoisonBomb());
                    isAimingBomb = false;
                    currentSpeed = inititialWithControl;
                    animator.SetBool("AimingBomb",false);
                    PoisonZonePreviewRef.SetActive(false);
                }
            }
            else {
                // no cost in stamina for the sword
                if (canSwordAttack && Input.GetKeyDown(KeyCode.Space)) {
                    StartCoroutine(SwordAttack());
                } 
                else if(canDash && Input.GetKeyDown(KeyCode.LeftShift) && _staminaBar.TryTakeDamages(10)){
                    StartCoroutine(Dash());
                }
                else if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    animator.SetBool("AimingBow",true);
                    isAimingArrow = true;
                    currentSpeed = inititialWithControl * attackSpeedNerf;
                    if ( canShootArrow ) ArrowPreviewRef.SetActive(true);
                    // PoisonZonePreviewRef.SetActive(true);
                    PlacePreviewArrow();
                    // ArrowPreviewRef.transform.position = transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1)) {
                    animator.SetBool("AimingBomb",true);
                    isAimingBomb = true;
                    currentSpeed = inititialWithControl * attackSpeedNerf;
                    if ( canThrowPoisonBomb ) PoisonZonePreviewRef.SetActive(true);
                    PlacePreviewZone();
                }
            }
        }
        /* if (isWielding) {
            _swordHitzoneCollider.enabled = false;
            isWielding = false;
        } */
        UpdateAnimationAndMove();
    }

    Vector3 GetMouseRelativePos() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.ScreenToWorldPoint(mousePosition);
        float y = (mousePosition.y - transform.position.y);
        float x = (mousePosition.x - transform.position.x);
        return new Vector3(x,y,0f).normalized;
    }

    void PlacePreviewArrow() {
        if (canShootArrow) {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.ScreenToWorldPoint(mousePosition);
        var position = transform.position;
        float y = (mousePosition.y - position.y);
        float x = (mousePosition.x - position.x);
        animator.SetFloat("MouseX",x);
        animator.SetFloat("MouseY",y);
        Vector3 pos = new Vector3(x,y,0f);
        ArrowPreviewRef.transform.position = position + pos.normalized;
        float teta = Mathf.Atan(y / x) * 180 / Mathf.PI - (mousePosition.x > position.x ? 90 : -90);
        ArrowPreviewRef.transform.eulerAngles = new Vector3(0f,0f,teta);
        }
    }

    void PlacePreviewZone() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.ScreenToWorldPoint(mousePosition);
        var position = transform.position;
        float y = (mousePosition.y - position.y);
        float x = (mousePosition.x - position.x);
        Vector3 pos = new Vector3(x,y,0f);
        if (Vector3.Distance(pos, Vector3.zero) > maxBombDist) {
            pos = maxBombDist * pos.normalized;
        }
        animator.SetFloat("MouseX",pos.x);
        animator.SetFloat("MouseY",pos.y);
        PoisonZonePreviewRef.transform.position = new Vector3(position.x + pos.x, position.y + pos.y, 0f);
    }


    void UpdateAnimationAndMove() {
        if (change != Vector3.zero) {
            myRigidBody.velocity = (change * ( 0.2f * currentSpeed * controlSpeed));
            animator.SetFloat("MoveX", change.x);
            animator.SetFloat("MoveY", change.y);
            animator.SetBool("IsMoving",true);
        }
        else {
            myRigidBody.velocity = Vector3.zero;
            animator.SetBool("IsMoving",false);
        }
    }

    public void ChangeControlSpeed(float newSpeedControl) {
        controlSpeed  = newSpeedControl;
        inititialWithControl = inititialSpeed * controlSpeed;
        currentSpeed = inititialWithControl;
        // should change the speed of all the other players too, by calling this very same function
        animator.speed = controlSpeed;
    }

    public void ChangeTimeControl(float newSpeedControl) {
        // modifies all the IEltSpeed interfaces
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SwordAttack() {
        // wielding for 100 degrees
        canSwordAttack = false;
        _swordHitzone.SetActive(true);
        // does not seem to work when the player has not yet moved
        float currentSwordRot = Mathf.Atan(notNullChange.y / notNullChange.x) * 180 / Mathf.PI + (notNullChange.x >= 0 ? 0 : 180);
        _swordHitzone.transform.eulerAngles = new Vector3(0f,0f,currentSwordRot);
        _swordHitzone.transform.position = transform.position + notNullChange * swordDist;
        // _swordHitzoneCollider.enabled = true;
        currentSpeed *= attackSpeedNerf;
        // isWielding = true;
        yield return new WaitForSeconds( swordTime / controlSpeed );
        _swordHitzone.SetActive(false);
        currentSpeed = inititialWithControl ;
        yield return new WaitForSeconds( _swordAttackCooldown / controlSpeed );
        canSwordAttack = true;
    }

    IEnumerator ShootArrow() {
        canShootArrow = false;
        if (IsServer)
            SpawnArrowServer(GetMouseRelativePos());
        else
            SpawnArrowServerRPC(GetMouseRelativePos());
        yield return new WaitForSeconds( bowCooldown / controlSpeed  );
        canShootArrow = true;
    }

    void SpawnArrowServer(Vector3 mousePos)
    {
        Vector3 pos = mousePos;
        float teta = Mathf.Atan( pos.y / pos.x ) * 180 / Mathf.PI - (pos.x > 0 ? 90 : -90);
        Quaternion rot = Quaternion.Euler(0f,0f,teta);
        var obj = Instantiate(ArrowRef, transform.position + pos, rot);
        obj.GetComponent<Projectile>().SetVelocity(pos, controlSpeed);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }
    
    [ServerRpc]
    void SpawnArrowServerRPC(Vector3 mousePos)
    {
        SpawnArrowServer(mousePos);
    }
    

    // first throws the bomb, and then instanciates the poison bomb
    IEnumerator ThrowPoisonBomb() {
        canThrowPoisonBomb = false;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.ScreenToWorldPoint(mousePosition);
        var position = transform.position;
        float y = (mousePosition.y - position.y);
        float x = (mousePosition.x - position.x);
        Vector3 pos = new Vector3(x,y,0f);
        if (Vector3.Distance(pos, Vector3.zero) > maxBombDist) {
            pos = maxBombDist * pos.normalized;
        }
        GameObject pZone = Instantiate(PoisonZoneRef, new Vector3(position.x + pos.x, position.y + pos.y,0f) , new Quaternion() );
        yield return new WaitForSeconds( poisonBombCooldown / controlSpeed  );
        canThrowPoisonBomb = true;
    }

    IEnumerator Dash(){
        // does not execute the dash if the player is not moving
        if (change.y != 0 || change.x != 0) {
            canDash = false;
            isDashing = true;
            currentSpeed *= dashPower;
            yield return new WaitForSeconds( dashTime / controlSpeed  );
            currentSpeed = inititialSpeed * controlSpeed ;
            isDashing = false;
            yield return new WaitForSeconds( dashCooldown / controlSpeed  );
            canDash = true;
        }
    }
    
    public void TakeDamages(uint damage) {
        _healthBar.TakeDamages(damage);
        StartCoroutine(ChangeColorWait(new Color(255, 0, 0, 100), 0.5f));
    }

    public void Heal(uint heal) {
        _healthBar.Heal(heal);       
        StartCoroutine(ChangeColorWait(new Color(0, 255, 0, 100), 0.5f));
    }
    IEnumerator ChangeColorWait(Color color, float time) {
        Color baseColor = _spriteRenderer.color;
        ChangeColor(color);
        yield return new WaitForSeconds(time);
        ChangeColor(baseColor);
    }
    
    // to be synced over network
    void ChangeColor(Color color) {
        _spriteRenderer.color = color;
    }
}
