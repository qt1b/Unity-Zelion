using System;
using System.Collections;
using System.IO;
using System.Linq;
using Bars;
using Interfaces;
using UI;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

namespace Player {
    public class Player : NetworkBehaviour, IHealth {
        // this file will be SPLITED into more files ! into the Player folder
        // putting all the player variables and all useful methods for ennemies here
        /* The format of the lookup table
         * 0 : X position (int)
         * 1 : Y pos
         * 2 : Life (max value, uint)
         * 3 : Stamina
         * 4 : Mana
         * 5 : Sword is unlocked (bool, formatted as 0 for false, 1 for true)
         * 6 : Bow
         * 7 : Poison
         * 8 : Dash
         * 9 : Slowdown
         * 10: TimeFreeze
         */
        // we will now list these variables here in the same order
        // initial position is done by the teleport action
        private bool _swordUnlocked;
        private bool _bowUnlocked;
        private bool _poisonUnlocked;
        private bool _dashUnlocked;
        private bool _slowdownUnlocked;
        private bool _timeFreezeUnlocked;
        
        [FormerlySerializedAs("inititialSpeed")] [Header("Speed")]
        public float initialSpeed = 7f;
        [DoNotSerialize] public float speedModifier;
        // would be better to get them by script
        private GameObject _arrowRef;
        private GameObject _arrowPreviewRef;
        private GameObject _poisonZoneRef;
        private GameObject _poisonZonePreviewRef;

        private Rigidbody2D _myRigidBody;
        [DoNotSerialize] public Vector2 change = Vector2.zero;
        [DoNotSerialize] public Vector2 notNullChange = new Vector2(0,1);
        // capacities availability
        bool _canSwordAttack = true;
        bool _canDash = true;
        bool _canShootArrow = true;
        bool _canThrowPoisonBomb = true;
        bool _canSlowDownTime = true;
        bool _canTimeFreeze = true;
        // getters
        private bool CanSwordAttack => _swordUnlocked && _canSwordAttack;
        private bool CanDash => _dashUnlocked && _canDash;
        private bool CanShootArrow => _bowUnlocked && _canShootArrow;
        private bool CanPoison => _poisonUnlocked && _canThrowPoisonBomb;
        private bool CanSlowDownTime => _slowdownUnlocked &&  _canSlowDownTime;
        private bool CanTimeFreeze => _timeFreezeUnlocked && _canTimeFreeze;

        bool _isDashing;
        bool _isAimingArrow;
        bool _isAimingBomb;
        private uint _colorAcc; // color accumulator, to know the number of instances started
        private uint _slowdownAcc; // same with slowdown
        private uint _timeFreezeAcc;
        [DoNotSerialize] public byte saveID;
        
        // cooldown timers
        // the const property can be removed if we want to modify that stuff with the player's progression
        // BETTER : may be removed if not needed
        private const float SwordTime = 0.2f;
        private const float SwordAttackCooldown = 0.4f;
        // private const float SlowdownTimeDuration = 4f;
        float _dashCooldown = 1f;
        float _bowCooldown = 0.2f;
        float _poisonBombCooldown = 7f;


    
        // private const float TotalSwordRot = 100f;
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
        // Collider2D _swordHitzoneCollider;
        // Animator _swordHitzoneAnimator;
        [FormerlySerializedAs("Camera")] public new Camera camera;

        private HealthBar _healthBar;
        private StaminaBar _staminaBar;
        private ManaBar _manaBar;
        private Renderer _renderer;
        //private SpriteRenderer _spriteRenderer;

        // for more efficient lookup
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MouseY = Animator.StringToHash("MouseY");
        private static readonly int MouseX = Animator.StringToHash("MouseX");
        private static readonly int AimingBomb = Animator.StringToHash("AimingBomb");
        private static readonly int AimingBow = Animator.StringToHash("AimingBow");

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
            speedModifier = 1;
            _animator = GetComponent<Animator>();
            _myRigidBody = GetComponent<Rigidbody2D>();
            _animator.speed = TimeVariables.PlayerSpeed.Value;
            _swordHitzone = transform.GetChild(0).gameObject;
            //_swordHitzoneCollider = _swordHitzone.GetComponent<Collider2D>();
            //_swordHitzoneAnimator = _swordHitzone.GetComponent<Animator>();
            _swordHitzone.SetActive(false);
            _arrowRef = Resources.Load<GameObject>("Prefabs/Projectiles/Arrow");
            _poisonZoneRef = Resources.Load<GameObject>("Prefabs/Projectiles/PoisonZone");
            _arrowPreviewRef = transform.GetChild(1).gameObject;
            _poisonZonePreviewRef = transform.GetChild(2).gameObject;
            _healthBar = FindObjectOfType<HealthBar>();
            _staminaBar = FindObjectOfType<StaminaBar>();
            _manaBar = FindObjectOfType<ManaBar>();
            // 'color' effects
            _renderer = gameObject.GetComponent<Renderer>();
            // _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (TimeVariables.PlayerList.Value.Count == 0) {
                // find if save exists, if it does loads it
                if (File.Exists("zelion.sav") && File.OpenRead("zelion.sav").ReadByte() is not -1 and/*is*/ {} readByte) {
                    saveID = (byte)readByte;
                }
                // else saveID = 0;
            }
            else {
                saveID = TimeVariables.PlayerList.Value.First().saveID;
                // OVERWRITES THE LAST SAVE !
            }

            saveID = 1;
            LoadSave();
            TimeVariables.PlayerList.Value.Add(this);
        }

        public void LoadSave() {
            // Reads from the save Id common to all instances
            var lookupTable = File.ReadLines(SaveData.SaveLookupPath).Skip(1).ToArray();
            if (lookupTable.Length > saveID) {
                string[] args = lookupTable[saveID].Split(';');
                if (args.Length != 11) throw new ArgumentException("the save lookup table is not formatted as expected");
                else {
                    Actions.Teleport.Activate(gameObject, new Vector3(int.Parse(args[0]), int.Parse(args[1]),0f));
                    _healthBar.ChangeMaxValue(uint.Parse(args[2]));
                    _staminaBar.ChangeMaxValue(uint.Parse(args[3]));
                    _manaBar.ChangeMaxValue(uint.Parse(args[4]));
                    _swordUnlocked = args[5] == "1";
                    _bowUnlocked = args[6] == "1";
                    _poisonUnlocked = args[7] == "1";
                    _dashUnlocked = args[8] == "1";
                    _slowdownUnlocked = args[9] == "1";
                    _timeFreezeUnlocked = args[10] == "1";
                    print("read successfully ?");
                }
            }
            else throw new NotImplementedException("unsupported save");
        }

        // Update is called once per frame
        // To Add : Sounds to indicate whether we can use the capacity or not
        void Update()
        {
            if(PauseMenu.GameIsPaused){
                return;
            }
            if (!_isDashing) {
                change = Vector2.zero;
                change.x = Input.GetAxisRaw("Horizontal");
                change.y = Input.GetAxisRaw("Vertical");
                change.Normalize();
                if (change != Vector2.zero) notNullChange = change;
                // one attack / 'normal' ability at a time
                if (_isAimingArrow) {
                    if (!_arrowPreviewRef.activeSelf && _canShootArrow &&  _staminaBar.CanTakeDamages(2) ) _arrowPreviewRef.SetActive(true);
                    PlacePreviewArrow();
                    if(Input.GetKeyUp(KeyCode.Mouse0)){
                        if ( _canShootArrow && _staminaBar.TryTakeDamages(2) ) StartCoroutine(ShootArrow());
                        _isAimingArrow = false;
                        speedModifier = 1;
                        _arrowPreviewRef.SetActive(false);
                        _animator.SetBool(AimingBow,false);
                    }
                }
                else if (_isAimingBomb) {
                    if (!_poisonZonePreviewRef.activeSelf && _canThrowPoisonBomb && _manaBar.CanTakeDamages(10) ) _poisonZonePreviewRef.SetActive(true);
                    PlacePreviewZone();
                    if (Input.GetKeyUp(KeyCode.Mouse1) ) {
                        if ( _canThrowPoisonBomb && _manaBar.TryTakeDamages(10) ) StartCoroutine(ThrowPoisonBomb());
                        _isAimingBomb = false;
                        speedModifier = 1;
                        _animator.SetBool(AimingBomb,false);
                        _poisonZonePreviewRef.SetActive(false);
                    }
                }
                else {
                    // no cost in stamina for the sword
                    if (CanSwordAttack && Input.GetKeyDown(KeyCode.Space)) {
                        StartCoroutine(SwordAttack());
                    } 
                    else if(CanDash && Input.GetKeyDown(KeyCode.LeftShift) && _staminaBar.TryTakeDamages(10)){
                        StartCoroutine(Dash());
                    }
                    else if (_bowUnlocked && Input.GetKeyDown(KeyCode.Mouse0)) {
                        _animator.SetBool(AimingBow,true);
                        // bow aiming audio effect
                        _isAimingArrow = true;
                        speedModifier = _attackSpeedNerf;
                        if ( _canShootArrow ) _arrowPreviewRef.SetActive(true);
                        // PoisonZonePreviewRef.SetActive(true);
                        PlacePreviewArrow();
                        // ArrowPreviewRef.transform.position = transform.position;
                    }
                    // poison zone: audio from the prefab
                    else if (_poisonUnlocked && Input.GetKeyDown(KeyCode.Mouse1)) {
                        _animator.SetBool(AimingBomb,true);
                        // poison aiming audio effect
                        _isAimingBomb = true;
                        speedModifier = _attackSpeedNerf;
                        if ( _canThrowPoisonBomb ) _poisonZonePreviewRef.SetActive(true);
                        PlacePreviewZone();
                    }
                    else if (CanSlowDownTime && Input.GetKeyDown(KeyCode.LeftControl) && _manaBar.TryTakeDamages(5)) {
                        // slow down audio effect
                        StartCoroutine(SlowDownTimeFor(4f));
                    }
                        // else some visual and/or audio feedback telling us that we can
                    else if (CanTimeFreeze && Input.GetKeyDown(KeyCode.V) && _manaBar.TryTakeDamages(14)) {
                        // time freeze audio effect
                        print("time freeze");
                    }
                    else if (Input.GetKeyDown(KeyCode.M)) {
                        this.TakeDamages(2);
                    }
                }
            }
            UpdateAnimationAndMove();
        }

        // they may overlap
        IEnumerator SlowDownTimeFor(float duration) {
            // will be enemy speed, using player speed to test the property
            // like color
            // float oldVal = TimeVariables.PlayerSpeed.Value;
            TimeVariables.PlayerSpeed.Value = 0.5f;
            _animator.speed = TimeVariables.PlayerSpeed.Value; // to remove if only slowing down ennemies
            _slowdownAcc += 1;
            yield return new WaitForSeconds(duration);
            _slowdownAcc -= 1;
            if (_slowdownAcc == 0) {
                TimeVariables.PlayerSpeed.Value = 1;
                _animator.speed = 1;
            }
        }

        IEnumerator TimeFreezeFor(float duration) {
            _timeFreezeAcc += 1;
            TimeVariables.PlayerSpeed.Value = 0f;
            yield return new WaitForSeconds(duration);
            _timeFreezeAcc -= 1;
            if (_timeFreezeAcc == 0) {
                TimeVariables.PlayerSpeed.Value = 1;
            }
        }

        Vector3 GetMouseRelativePos() {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = camera.ScreenToWorldPoint(mousePosition);
            var position = transform.position;
            float y = (mousePosition.y - position.y);
            float x = (mousePosition.x - position.x);
            return new Vector3(x,y,0f).normalized;
        }

        void PlacePreviewArrow() {
            if (_canShootArrow) {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition = camera.ScreenToWorldPoint(mousePosition);
                var position = transform.position;
                float y = (mousePosition.y - position.y);
                float x = (mousePosition.x - position.x);
                _animator.SetFloat(MouseX,x);
                _animator.SetFloat(MouseY,y);
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
            _animator.SetFloat(MouseX,pos.x);
            _animator.SetFloat(MouseY,pos.y);
            _poisonZonePreviewRef.transform.position = new Vector3(position.x + pos.x, position.y + pos.y, 0f);
        }


        void UpdateAnimationAndMove() {
            if (change != Vector2.zero) {
                // 0.2 f ?
                _myRigidBody.velocity = (change * (0.2f * initialSpeed * speedModifier * TimeVariables.PlayerSpeed.Value));
                _animator.SetFloat(MoveX, change.x);
                _animator.SetFloat(MoveY, change.y);
                _animator.SetBool(IsMoving,true);
            }
            else {
                _myRigidBody.velocity = Vector2.zero;
                _animator.SetBool(IsMoving,false);
            }
        }

        public void ChangePlayerControlSpeed(float newSpeedControl) {
            // TimeVariables.PlayerSpeed.Value  = newSpeedControl;
            _animator.speed = TimeVariables.PlayerSpeed.Value;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        IEnumerator SwordAttack() {
            // wielding for 100 degrees
            _canSwordAttack = false;
            _swordHitzone.SetActive(true);
            // does not seem to work when the player has not yet moved
            float currentSwordRot = Mathf.Atan(notNullChange.y / notNullChange.x) * 180 / Mathf.PI + (notNullChange.x >= 0 ? 0 : 180);
            _swordHitzone.transform.eulerAngles = new Vector3(0f,0f,currentSwordRot);
            _swordHitzone.transform.position = (Vector2)transform.position + notNullChange * _swordDist;
            // _swordHitzoneCollider.enabled = true;
            speedModifier = _attackSpeedNerf;
            // isWielding = true;
            yield return new WaitForSeconds( SwordTime / TimeVariables.PlayerSpeed.Value );
            _swordHitzone.SetActive(false);
            speedModifier = 1 ;
            yield return new WaitForSeconds( SwordAttackCooldown / TimeVariables.PlayerSpeed.Value );
            _canSwordAttack = true;
        }

        IEnumerator ShootArrow() {
            _canShootArrow = false;
            if (IsServer)
                SpawnArrowServer(GetMouseRelativePos());
            else
                SpawnArrowServerRPC(GetMouseRelativePos());
            //StartCoroutine(ChangeColorWait(new Color(1, 1, 0, 0.8f), 0.2f));
            yield return new WaitForSeconds( _bowCooldown / TimeVariables.PlayerSpeed.Value );
            _canShootArrow = true;
        }

        void SpawnArrowServer(Vector3 mousePos)
        {
            Vector3 pos = mousePos;
            float teta = Mathf.Atan( pos.y / pos.x ) * 180 / Mathf.PI - (pos.x > 0 ? 90 : -90);
            Quaternion rot = Quaternion.Euler(0f,0f,teta);
            var obj = Instantiate(_arrowRef, transform.position + pos, rot);
            obj.GetComponent<Projectile>().SetVelocity(pos, TimeVariables.PlayerSpeed.Value);
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
            /*GameObject pZone =*/ Instantiate(_poisonZoneRef, new Vector3(position.x + pos.x, position.y + pos.y,0f) , new Quaternion() );
            yield return new WaitForSeconds( _poisonBombCooldown / TimeVariables.PlayerSpeed.Value  );
            _canThrowPoisonBomb = true;
        }

        IEnumerator Dash(){
            // does not execute the dash if the player is not moving
            if (change.y != 0 || change.x != 0) {
                _canDash = false;
                _isDashing = true;
                speedModifier = _dashPower;
                StartCoroutine(ChangeColorWait(new Color(1, 1, 0.3f, 0.8f), 0.2f)); // yellow
                yield return new WaitForSeconds( _dashTime / TimeVariables.PlayerSpeed.Value  );
                speedModifier = 1 ;
                _isDashing = false;
                yield return new WaitForSeconds( _dashCooldown / TimeVariables.PlayerSpeed.Value  );
                _canDash = true;
            }
        }
    
        // ADD SOUNDS HERE
        public void TakeDamages(uint damage) {
            if (_healthBar.TryTakeDamages(damage)) {
                StartCoroutine(ChangeColorWait(new Color(1f, 0.3f, 0.3f, 0.8f), 0.2f));
            }
            else GameOver();
        }

        // healing collectibles are not healing and idk why
        public void Heal(uint heal) {
            _healthBar.Heal(heal);
            StartCoroutine(ChangeColorWait(new Color(0.3f, 1f, 0.3f, 0.8f), 0.2f));
        }
        // maybe change these colors ???
        public void TakeDamagesStamina(uint damage) {
            _staminaBar.TakeDamages(damage);
            StartCoroutine(ChangeColorWait(new Color(1f, 0.3f, 0.3f, 0.8f), 0.2f));
        }

        public void HealStamina(uint heal) {
            _staminaBar.Heal(heal);
            StartCoroutine(ChangeColorWait(new Color(0.3f, 1f, 0.3f, 0.8f), 0.2f));
        }
        public void TakeDamagesMana(uint damage) {
            _manaBar.TakeDamages(damage);
            StartCoroutine(ChangeColorWait(new Color(1f, 0.3f, 0.3f, 0.8f), 0.2f));
        }

        public void HealMana(uint heal) {
            _manaBar.Heal(heal);
            StartCoroutine(ChangeColorWait(new Color(0.3f, 1f, 0.3f, 0.8f), 0.2f));
        }

        public void GameOver() {
            print("gameover");
        }

        IEnumerator ChangeColorWait(Color color, float time) {
            Color baseColor = _renderer.material.color;
            _colorAcc += 1;
            ChangeColorClientRpc(color);
            yield return new WaitForSeconds(time);
            _colorAcc -= 1;
            if (_colorAcc == 0) {
                ChangeColorClientRpc(Color.white);
            }
            else if (baseColor != Color.white) ChangeColorClientRpc(baseColor);
        }

        // to be synced over network
        [ClientRpc]
        void ChangeColorClientRpc(Color color) {
            // cannot manage to make it an effect on top of the sprite
            _renderer.material.SetColor(Color1,color);
        }
    }
}
