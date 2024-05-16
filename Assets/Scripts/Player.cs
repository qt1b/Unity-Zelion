using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bars;
using Unity.Netcode;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour, IHealth {
    // this file will be SPLITED into more files ! into the Player folder
    // putting all the player variables and all useful methods for ennemies here

    [Header("Speed")]
    public float inititialSpeed = 7f;
    public float currentSpeed;
    // would be better to get them by script
    [FormerlySerializedAs("ArrowRef")] [Header("Projectiles")]
    public GameObject arrowRef;
    private GameObject _arrowPreviewRef;
    [FormerlySerializedAs("PoisonBombRef")] public GameObject poisonBombRef;
    [FormerlySerializedAs("PoisonZoneRef")] public GameObject poisonZoneRef;
    private GameObject _poisonZonePreviewRef;

    Rigidbody2D _myRigidBody;
    public Vector3 change = Vector3.zero;
    public Vector3 notNullChange = new Vector3(0,1,0);




    // variables controlling the attack system
    // is our player already doing smthg
    // public as we will make ennemies / etc that make the player enter this state
    // capacities avalability
    bool _canSwordAttack = true;
    bool _canDash = true;
    bool _canShootArrow = true;
    bool _canThrowPoisonBomb = true;
    bool _canSlowDownTime = true;

    bool _isDashing = false; // so the stamina bar can use it
    // bool isWielding = false;
    bool _isAimingArrow = false;
    bool _isAimingBomb = false;
    private int _colorAcc; // color accumulator, to know the number of instances started
    private int _slowdownAcc; // same with slowdown
    // time control, to enable time effects
    // bc some bosses will slow down our controls, so this var has to be public

    // cooldown timers
    // the const property can be removed if we want to modify that stuff with the player's progression
    // BETTER : may be removed if not needed
    private const float SwordTime = 0.2f;
    private const float SwordAttackCooldown = 0.4f;
    // private const float SlowdownTimeDuration = 4f;
    float _dashCooldown = 1f;
    float _bowCooldown = 0.2f;
    float _poisonBombCooldown = 7f;


    
    private const float TotalSwordRot = 100f;
    // 'animation' timers
    float _dashTime = 0.12f;


    // variables for attack settings
    float _dashPower = 6f;
    float _attackSpeedNerf = 0.65f;
    float _maxBombDist = 7f;
    float _swordDist = 0.3f;


    // display variables
    Animator _animator;
    GameObject _swordHitzone;
    Collider2D _swordHitzoneCollider;
    Animator _swordHitzoneAnimator;
    [FormerlySerializedAs("Camera")] public Camera camera;

    [Header("Bars")]
    private HealthBar _healthBar;
    private StaminaBar _staminaBar;
    private ManaBar _manaBar;
    private Renderer _renderer;
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
        currentSpeed = inititialSpeed * TimeVariables.PlayerSpeed;
        _animator = GetComponent<Animator>();
        _myRigidBody = GetComponent<Rigidbody2D>();
        _animator.speed = TimeVariables.PlayerSpeed ;
        _swordHitzone = transform.GetChild(0).gameObject;
        _swordHitzoneCollider = _swordHitzone.GetComponent<Collider2D>();
        _swordHitzoneAnimator = _swordHitzone.GetComponent<Animator>();
        _swordHitzone.SetActive(false);
        _arrowPreviewRef = transform.GetChild(1).gameObject;
        _poisonZonePreviewRef = transform.GetChild(2).gameObject;
        _healthBar = FindObjectOfType<HealthBar>();
        _staminaBar = FindObjectOfType<StaminaBar>();
        _manaBar = FindObjectOfType<ManaBar>();
        // 'color' effects
        _renderer = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame 
    void Update()
    {    
        if(PauseMenu.GameIsPaused){
            return;
        }
        if (!_isDashing) {
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            change.Normalize();
            if (change != Vector3.zero) notNullChange = change;
            // one attack / 'normal' ability at a time
            if (_isAimingArrow) {
                if (!_arrowPreviewRef.activeSelf && _canShootArrow /* &&  _staminaBar.CanTakeDamages(2) */ ) _arrowPreviewRef.SetActive(true);
                PlacePreviewArrow();
                if(Input.GetKeyUp(KeyCode.Mouse0)){
                    if ( _canShootArrow && _staminaBar.TryTakeDamages(2) ) StartCoroutine(ShootArrow());
                    _isAimingArrow = false;
                    currentSpeed = inititialSpeed * TimeVariables.PlayerSpeed;
                    _arrowPreviewRef.SetActive(false);
                    _animator.SetBool("AimingBow",false);
                }
            }
            else if (_isAimingBomb) {
                if (!_poisonZonePreviewRef.activeSelf && _canThrowPoisonBomb && _manaBar.CanTakeDamages(10) ) _poisonZonePreviewRef.SetActive(true);
                PlacePreviewZone();
                if (Input.GetKeyUp(KeyCode.Mouse1) ) {
                    if ( _canThrowPoisonBomb && _manaBar.TryTakeDamages(10) ) StartCoroutine(ThrowPoisonBomb());
                    _isAimingBomb = false;
                    currentSpeed = inititialSpeed * TimeVariables.PlayerSpeed;
                    _animator.SetBool("AimingBomb",false);
                    _poisonZonePreviewRef.SetActive(false);
                }
            }
            else {
                // no cost in stamina for the sword
                if (_canSwordAttack && Input.GetKeyDown(KeyCode.Space)) {
                    StartCoroutine(SwordAttack());
                } 
                else if(_canDash && Input.GetKeyDown(KeyCode.LeftShift) && _staminaBar.TryTakeDamages(10)){
                    StartCoroutine(Dash());
                }
                else if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    _animator.SetBool("AimingBow",true);
                    _isAimingArrow = true;
                    currentSpeed = TimeVariables.PlayerSpeed * _attackSpeedNerf;
                    if ( _canShootArrow ) _arrowPreviewRef.SetActive(true);
                    // PoisonZonePreviewRef.SetActive(true);
                    PlacePreviewArrow();
                    // ArrowPreviewRef.transform.position = transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1)) {
                    _animator.SetBool("AimingBomb",true);
                    _isAimingBomb = true;
                    currentSpeed = TimeVariables.PlayerSpeed * _attackSpeedNerf;
                    if ( _canThrowPoisonBomb ) _poisonZonePreviewRef.SetActive(true);
                    PlacePreviewZone();
                }
                else if (Input.GetKeyDown(KeyCode.LeftControl)) {
                    if (_canSlowDownTime && _manaBar.TryTakeDamages(5)) {
                        StartCoroutine(SlowDownTimeFor(4f));
                    }
                    // else some visual and/or audio feedback telling us that we can
                }
            }
        }
        /* if (isWielding) {
            _swordHitzoneCollider.enabled = false;
            isWielding = false;
        } */
        UpdateAnimationAndMove();
    }

    IEnumerator SlowDownTimeFor(float duration) {
        // will be ennemy speed, using player speed to test the property
        // like color
        TimeVariables.PlayerSpeed = 0.5f;
        _slowdownAcc += 1;
        yield return new WaitForSeconds(duration);
        _slowdownAcc -= 1;
        if (_slowdownAcc == 0) {
            TimeVariables.PlayerSpeed = 1;
        }
    }

    Vector3 GetMouseRelativePos() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = camera.ScreenToWorldPoint(mousePosition);
        float y = (mousePosition.y - transform.position.y);
        float x = (mousePosition.x - transform.position.x);
        return new Vector3(x,y,0f).normalized;
    }

    void PlacePreviewArrow() {
        if (_canShootArrow) {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = camera.ScreenToWorldPoint(mousePosition);
        var position = transform.position;
        float y = (mousePosition.y - position.y);
        float x = (mousePosition.x - position.x);
        _animator.SetFloat("MouseX",x);
        _animator.SetFloat("MouseY",y);
        Vector3 pos = new Vector3(x,y,0f);
        _arrowPreviewRef.transform.position = position + pos.normalized;
        float teta = Mathf.Atan(y / x) * 180 / Mathf.PI - (mousePosition.x > position.x ? 90 : -90);
        _arrowPreviewRef.transform.eulerAngles = new Vector3(0f,0f,teta);
        }
    }

    void PlacePreviewZone() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = camera.ScreenToWorldPoint(mousePosition);
        var position = transform.position;
        float y = (mousePosition.y - position.y);
        float x = (mousePosition.x - position.x);
        Vector3 pos = new Vector3(x,y,0f);
        if (Vector3.Distance(pos, Vector3.zero) > _maxBombDist) {
            pos = _maxBombDist * pos.normalized;
        }
        _animator.SetFloat("MouseX",pos.x);
        _animator.SetFloat("MouseY",pos.y);
        _poisonZonePreviewRef.transform.position = new Vector3(position.x + pos.x, position.y + pos.y, 0f);
    }


    void UpdateAnimationAndMove() {
        if (change != Vector3.zero) {
            _myRigidBody.velocity = (change * ( 0.2f * currentSpeed * TimeVariables.PlayerSpeed));
            _animator.SetFloat("MoveX", change.x);
            _animator.SetFloat("MoveY", change.y);
            _animator.SetBool("IsMoving",true);
        }
        else {
            _myRigidBody.velocity = Vector3.zero;
            _animator.SetBool("IsMoving",false);
        }
    }

    public void ChangePlayerControlSpeed(float newSpeedControl) {
        TimeVariables.PlayerSpeed  = newSpeedControl;
        _animator.speed = TimeVariables.PlayerSpeed;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SwordAttack() {
        // wielding for 100 degrees
        _canSwordAttack = false;
        _swordHitzone.SetActive(true);
        // does not seem to work when the player has not yet moved
        float currentSwordRot = Mathf.Atan(notNullChange.y / notNullChange.x) * 180 / Mathf.PI + (notNullChange.x >= 0 ? 0 : 180);
        _swordHitzone.transform.eulerAngles = new Vector3(0f,0f,currentSwordRot);
        _swordHitzone.transform.position = transform.position + notNullChange * _swordDist;
        // _swordHitzoneCollider.enabled = true;
        currentSpeed *= _attackSpeedNerf;
        // isWielding = true;
        yield return new WaitForSeconds( SwordTime / TimeVariables.PlayerSpeed );
        _swordHitzone.SetActive(false);
        currentSpeed = inititialSpeed * TimeVariables.PlayerSpeed ;
        yield return new WaitForSeconds( SwordAttackCooldown / TimeVariables.PlayerSpeed );
        _canSwordAttack = true;
    }

    IEnumerator ShootArrow() {
        _canShootArrow = false;
        if (IsServer)
            SpawnArrowServer(GetMouseRelativePos());
        else
            SpawnArrowServerRPC(GetMouseRelativePos());
        //StartCoroutine(ChangeColorWait(new Color(1, 1, 0, 0.8f), 0.2f));
        yield return new WaitForSeconds( _bowCooldown / TimeVariables.PlayerSpeed );
        _canShootArrow = true;
    }

    void SpawnArrowServer(Vector3 mousePos)
    {
        Vector3 pos = mousePos;
        float teta = Mathf.Atan( pos.y / pos.x ) * 180 / Mathf.PI - (pos.x > 0 ? 90 : -90);
        Quaternion rot = Quaternion.Euler(0f,0f,teta);
        var obj = Instantiate(arrowRef, transform.position + pos, rot);
        obj.GetComponent<Projectile>().SetVelocity(pos, TimeVariables.PlayerSpeed);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }
    
    [ServerRpc]
    void SpawnArrowServerRPC(Vector3 mousePos)
    {
        SpawnArrowServer(mousePos);
    }
    

    // first throws the bomb, and then instanciates the poison bomb
    IEnumerator ThrowPoisonBomb() {
        _canThrowPoisonBomb = false;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = camera.ScreenToWorldPoint(mousePosition);
        var position = transform.position;
        StartCoroutine(ChangeColorWait(new Color(0.3f, 0.3f, 1, 0.8f), 0.2f));
        float y = (mousePosition.y - position.y);
        float x = (mousePosition.x - position.x);
        Vector3 pos = new Vector3(x,y,0f);
        if (Vector3.Distance(pos, Vector3.zero) > _maxBombDist) {
            pos = _maxBombDist * pos.normalized;
        }
        GameObject pZone = Instantiate(poisonZoneRef, new Vector3(position.x + pos.x, position.y + pos.y,0f) , new Quaternion() );
        yield return new WaitForSeconds( _poisonBombCooldown / TimeVariables.PlayerSpeed  );
        _canThrowPoisonBomb = true;
    }

    IEnumerator Dash(){
        // does not execute the dash if the player is not moving
        if (change.y != 0 || change.x != 0) {
            _canDash = false;
            _isDashing = true;
            currentSpeed *= _dashPower;
			StartCoroutine(ChangeColorWait(new Color(1, 1, 0.3f, 0.8f), 0.2f)); // yellow
            yield return new WaitForSeconds( _dashTime / TimeVariables.PlayerSpeed  );
            currentSpeed = inititialSpeed * TimeVariables.PlayerSpeed ;
            _isDashing = false;
            yield return new WaitForSeconds( _dashCooldown / TimeVariables.PlayerSpeed  );
            _canDash = true;
        }
    }
    public void TakeDamages(uint damage) {
        _healthBar.TakeDamages(damage);
        StartCoroutine(ChangeColorWait(new Color(1f, 0.3f, 0.3f, 0.8f), 0.2f));
    }

    public void Heal(uint heal) {
        _healthBar.Heal(heal);
        StartCoroutine(ChangeColorWait(new Color(0.3f, 1f, 0.3f, 0.8f), 0.2f));
    }

    IEnumerator ChangeColorWait(Color color, float time) {
        _colorAcc += 1;
        ChangeColor(color);
        yield return new WaitForSeconds(time);
        _colorAcc -= 1;
        if (_colorAcc == 0) {
            ChangeColor(new Color(1,1,1,1));
        }
    }
    // to be synced over network
    void ChangeColor(Color color) {
        // cannot manage to make it an effect on top of the sprite
        _renderer.material.SetColor("_Color",color);
    }
}
