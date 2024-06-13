using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Bars;
using Global;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using TMPro;
using UI;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Weapons;

namespace Player {
	public class Player : MonoBehaviourPunCallbacks, IHealth, IPunObservable {
		#region Public Fields

		[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
		public static GameObject LocalPlayerInstance;

		public float initialSpeed = 7f;
		[DoNotSerialize] public float speedModifier = 1;
		[DoNotSerialize] public Vector2 change = Vector2.zero;
		[DoNotSerialize] public Vector2 notNullChange = new Vector2(0, 1);
		[DoNotSerialize] public bool isDead = false;

		#endregion

		#region Capacity Unlocking Booleans

		private bool _swordUnlocked;
		private bool _bowUnlocked;
		private bool _poisonUnlocked;
		private bool _dashUnlocked;
		private bool _slowdownUnlocked;
		private bool _timeTravelUnlocked = true; // for testing

		#endregion

		#region Capacity Availability Booleans

		bool _canSwordAttack = true;
		bool _canDash = true;
		bool _canShootArrow = true;
		bool _canThrowPoisonBomb = true;
		bool _canSlowDownTime = true;
		bool _canTimeTravel = true;
		private bool _isDashing;
		bool _isAimingArrow;
		bool _isAimingBomb;

		#endregion

		#region Getters Combining if Capacity is Unloked and if it is available

		private bool CanSwordAttack => _swordUnlocked && _canSwordAttack;
		private bool CanDash => _dashUnlocked && _canDash;
		private bool CanShootArrow => _bowUnlocked && _canShootArrow;
		private bool CanPoison => _poisonUnlocked && _canThrowPoisonBomb;
		private bool CanSlowDownTime => _slowdownUnlocked && _canSlowDownTime;
		private bool CanTimeTravel => _timeTravelUnlocked && _canTimeTravel;

		#endregion

		#region Capacity Accumulators, To hopefully avoid having effects stacking up

		private uint _colorAcc; // color accumulator, to know the number of instances started
		private uint _slowdownAcc; // same with slowdown
		private uint _timeFreezeAcc;

		#endregion

		#region Set Variables

		private const float SwordTime = 0.2f;

		private const float SwordAttackCooldown = 0.4f;

		// private const float SlowdownTimeDuration = 4f;
		float _dashCooldown = 1f;
		float _bowCooldown = 0.2f;

		float _poisonBombCooldown = 7f;

		// private const float TotalSwordRot = 100f;
		float _dashTime = 0.12f;
		float _dashPower = 6f;
		float _attackSpeedNerf = 0.65f;
		float _maxBombDist = 7f;
		float _swordDist = 0.3f;

		#endregion

		#region Private References

		private Animator _animator;
		private Rigidbody2D _myRigidBody;
		private GameObject _arrowPreviewRef;
		private GameObject _poisonZonePreviewRef;
		private GameObject _swordHitzone;
		private HealthBar _healthBar;
		private StaminaBar _staminaBar;
		private ManaBar _manaBar;
		private Renderer _renderer;
		private SpriteRenderer _spriteRenderer;
		//private GameObject _poisonZoneRef;
		//private GameObject _arrowPrefab;
		[NonSerialized] public GhostPlayer ghostPlayer;
		public TMP_Text StatusText;
		#endregion

		#region Cached Values

		//private static readonly int Color1 = Shader.PropertyToID("_Color");
		private static readonly int IsMoving = Animator.StringToHash("IsMoving");
		private static readonly int MoveY = Animator.StringToHash("MoveY");
		private static readonly int MoveX = Animator.StringToHash("MoveX");
		private static readonly int MouseY = Animator.StringToHash("MouseY");
		private static readonly int MouseX = Animator.StringToHash("MouseX");
		private static readonly int AimingBomb = Animator.StringToHash("AimingBomb");
		private static readonly int AimingBow = Animator.StringToHash("AimingBow");

		#endregion

		#region Player Save management

/* The format of the lookup table:
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
		public void LoadSave() {
			byte saveID = GlobalVars.SaveId;
			_healthBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 2][saveID]);
			if (photonView.IsMine) {
				Vector3 pos = new Vector3(GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 0][saveID],
					GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 1][saveID] + GlobalVars.PlayerId, 0f);
				transform.position = pos;
				_staminaBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 3][saveID]);
				_manaBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 4][saveID]);
				_swordUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 5][saveID] == 1;
				_bowUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 6][saveID] == 1;
				_poisonUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 7][saveID] == 1;
				_dashUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 8][saveID] == 1;
				_slowdownUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 9][saveID] == 1;
				_timeTravelUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 10][saveID] == 1;
			}
			Debug.Log("Save loaded");

		}

		// next ????: should call a rpc to sync the healthbar's max value, etc..
		public void LoadSaveWithoutPos() {
			/*
			if (GlobalVars.SaveLookupArray.GetLength(0) > GlobalVars.SaveId) {
				_healthBar.ChangeMaxValue(ushort.Parse(GlobalVars.SaveLookupArray[GlobalVars.SaveId, 2]));
				if (photonView.IsMine) {
					_swordUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId, 5] == "1";
					_bowUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId, 6] == "1";
					_poisonUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId, 7] == "1";
					_dashUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId, 8] == "1";
					_slowdownUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId, 9] == "1";
					_timeTravelUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId, 10] == "1";
					_staminaBar.ChangeMaxValue(ushort.Parse(GlobalVars.SaveLookupArray[GlobalVars.SaveId, 3]));
					_manaBar.ChangeMaxValue(ushort.Parse(GlobalVars.SaveLookupArray[GlobalVars.SaveId, 4]));
				}
				Debug.Log("Loaded Global Save successfully, saveID=" + GlobalVars.SaveId);
			} */

			byte saveID = GlobalVars.SaveId;
			_healthBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 2][saveID]);
			if (photonView.IsMine) {
				_staminaBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 3][saveID]);
				_manaBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 4][saveID]);
				_swordUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 5][saveID] == 1;
				Debug.Log("loading specific : _sword unlocked =="+_swordUnlocked);
				_bowUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 6][saveID] == 1;
				_poisonUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 7][saveID] == 1;
				_dashUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 8][saveID] == 1;
				_slowdownUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 9][saveID] == 1;
				_timeTravelUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 10][saveID] == 1;
			}
			Debug.Log("Save without pos loaded");

		}
		
		public void LoadSpecificSave(byte saveID) {
			_healthBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 2][saveID]);
			if (photonView.IsMine) {
				_staminaBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 3][saveID]);
				_manaBar.ChangeMaxValue((ushort)GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 4][saveID]);
				_swordUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 5][saveID] == 1;
				Debug.Log("loading specific : _sword unlocked =="+_swordUnlocked);
				_bowUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 6][saveID] == 1;
				_poisonUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 7][saveID] == 1;
				_dashUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 8][saveID] == 1;
				_slowdownUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 9][saveID] == 1;
				_timeTravelUnlocked = GlobalVars.SaveLookupArray2[GlobalVars.CurrentLevelId, 10][saveID] == 1;
			}
			Debug.Log("Specific save loaded");
		}
		

	#endregion
		#region MonoBehaviour
		public CursorManager cursorManager; 
		void Awake() {
			//Debug.Log("player awake");
			GlobalVars.PlayerList.Add(this);
			_healthBar = GetComponentInChildren<HealthBar>();
			if (!photonView.IsMine) {
				//gameObject.GetComponentInChildren<Camera>().gameObject.SetActive(false);
				gameObject.GetComponentInChildren<Camera>().enabled = false;
				gameObject.GetComponentInChildren<CameraWork>().enabled = false;
				gameObject.GetComponentInChildren<AudioListener>().enabled = false;
				Destroy(gameObject.GetComponent<Rigidbody2D>());
				LoadSave();
				Debug.Log("player awake - is not mine, return");
				return;
			}
			// #Important
			// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
			Player.LocalPlayerInstance = this.gameObject;
			// #Critical
			// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
			// for now we don't use it
			// DontDestroyOnLoad(this.gameObject);

			// initializing all needed references
			_animator = GetComponent<Animator>();
			_myRigidBody = GetComponent<Rigidbody2D>();
			// _animator.speed = GlobalVars.PlayerSpeed;
			_swordHitzone = transform.GetChild(0).gameObject;
			_swordHitzone.SetActive(false);
			//_poisonZoneRef = Resources.Load<GameObject>("Prefabs/Projectiles/PoisonZone");
			//_arrowPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Arrow");
			_arrowPreviewRef = transform.GetChild(1).gameObject;
			_poisonZonePreviewRef = transform.GetChild(2).gameObject;
			_staminaBar = FindObjectOfType<StaminaBar>();
			_manaBar = FindObjectOfType<ManaBar>();
			_renderer = gameObject.GetComponent<Renderer>();
			_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			cursorManager = FindObjectOfType<CursorManager>();
			StatusText = FindObjectOfType<PauseMenu>().StatusText;
			LoadSave();
			gameObject.GetComponentInChildren<CameraWork>().OnStartFollowing();
		}

		/*
		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during initialization phase.
		/// </summary>
		void Start() {
			Debug.Log("player start");
			if (!photonView.IsMine) {
				Debug.Log("player start - is not mine, return");
				// this.enabled = false;
				return;
			}
			if (Camera.main is { } mainCamera && mainCamera.GetComponent<CameraWork>() is { } cameraWork) {
				cameraWork.OnStartFollowing();
			}
			else {
				Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component", this);
			}
		} */
		

		// Update is called once per frame
		// To Add : Sounds to indicate whether we can use the capacity or not
		void Update() {
			// Debug.Log("updating player ...");
			// && PhotonNetwork.IsConnected is for debugging purposes
			if ((!photonView.IsMine && PhotonNetwork.IsConnected) || PauseMenu.GameIsPaused) {
				/*Debug.LogWarning($"Update : Return !! \n" +
				                 $"photonView.IsMine={photonView.IsMine},\n" +
				                 $"PhotonNetwork.IsConnected={PhotonNetwork.IsConnected},\n" +
				                 $"PauseMenu.GameIsPaused={PauseMenu.GameIsPaused}");*/
				return;
			}
			// update with player input
			if (!_isDashing) {
				change = Vector2.zero;
				change.x = Input.GetAxisRaw("Horizontal");
				change.y = Input.GetAxisRaw("Vertical");
				change.Normalize();
				if (change != Vector2.zero) notNullChange = change;
				// one attack / 'normal' ability at a time
				//_ghostPlayer.Display(CanTimeTravel && _manaBar.CanTakeDamages(7));
				if (_isAimingArrow) {
					cursorManager.SetCursor(cursorManager.crosshairTexture, cursorManager.crosshairHotSpot); // is not opti
					if (!_arrowPreviewRef.activeSelf && _canShootArrow && _staminaBar.CanTakeDamages(2)) {
						_arrowPreviewRef.SetActive(true);
					}

					PlacePreviewArrow();
					if (Input.GetKeyUp(KeyCode.Mouse0)) {
						if (_canShootArrow && _staminaBar.TryTakeDamages(2)) StartCoroutine(ShootArrow());
						_isAimingArrow = false;
						speedModifier = 1;
						_arrowPreviewRef.SetActive(false);
						_animator.SetBool(AimingBow, false);
					}
				}
				else {
					cursorManager.SetCursor(cursorManager.cursorTexture, cursorManager.cursorHotSpot); // same
					if (_isAimingBomb) {
						if (!_poisonZonePreviewRef.activeSelf && _canThrowPoisonBomb && _manaBar.CanTakeDamages(10))
							_poisonZonePreviewRef.SetActive(true);
						PlacePreviewZone();
						if (Input.GetKeyUp(KeyCode.Mouse1)) {
							if (_canThrowPoisonBomb && _manaBar.TryTakeDamages(10)) {
								StartCoroutine(ThrowPoisonBomb());
							}
							else {
								AudioManager.Instance.Play("unauthorized");
							}
							_isAimingBomb = false;
							speedModifier = 1;
							_animator.SetBool(AimingBomb, false);
							_poisonZonePreviewRef.SetActive(false);
						}
					}
					else {
						// the sword has not any cost
						if (Input.GetKeyDown(KeyCode.Space)) {
							if (CanSwordAttack) {
								StartCoroutine(SwordAttack());
							}
							else {
								AudioManager.Instance.Play("unauthorized");
							}
						}
						else if ( Input.GetKeyDown(KeyCode.LeftShift) ) {
							if (CanDash && _staminaBar.TryTakeDamages(10)) {
								StartCoroutine(Dash());
							}
							else {
								AudioManager.Instance.Play("unauthorized");
							}
						}
						else if ( Input.GetKeyDown(KeyCode.Mouse0)) {
							if (_bowUnlocked) {
								_animator.SetBool(AimingBow, true);
								// bow aiming audio effect
								_isAimingArrow = true;
								speedModifier = _attackSpeedNerf;
								if (_canShootArrow) _arrowPreviewRef.SetActive(true);
								// PoisonZonePreviewRef.SetActive(true);
								PlacePreviewArrow();
								// ArrowPreviewRef.transform.position = transform.position;
							}
							else {
								AudioManager.Instance.Play("unauthorized");
							}
						}
						// poison zone: audio from the prefab
						else if (Input.GetKeyDown(KeyCode.Mouse1)) {
							if (_poisonUnlocked) {
								_animator.SetBool(AimingBomb, true);
								// poison aiming audio effect
								_isAimingBomb = true;
								speedModifier = _attackSpeedNerf;
								if (_canThrowPoisonBomb) _poisonZonePreviewRef.SetActive(true);
								PlacePreviewZone();
							}
							else {
								AudioManager.Instance.Play("unauthorized");
							}
						}
						else if ( Input.GetKeyDown(KeyCode.Q) ){
							if (CanSlowDownTime &&_manaBar.TryTakeDamages(5)) {
								AudioManager.Instance.Play("slowdownSpell");
								StartCoroutine(SlowDownTimeFor(4f));
							}
							else {
								AudioManager.Instance.Play("unauthorized");
							}
						}
						// to remove when we have some enemies
						else if (Input.GetKeyDown(KeyCode.M)) {
							this.TakeDamages(2);
						}
						else if ( Input.GetKeyDown(KeyCode.Z)) {
							if (CanTimeTravel && _manaBar.TryTakeDamages(7)) {
								AudioManager.Instance.Play("spellTp");
								GoBackInTime();
							}
							else {
								AudioManager.Instance.Play("unauthorized");
							}
						}
					}
				}
			}
			UpdateAnimationAndMove();
		}
		#endregion
		#region MonoBehaviour Callbacks
		/* // is not being used anymore, did not really work
		public void OnEvent(EventData photonEvent) {
			if (photonEvent.Code == NetworkArrowSpawnRef ) {
				object[] data = (object[]) photonEvent.CustomData;
				GameObject arrow = (GameObject) Instantiate(_arrowPrefab, (Vector3) data[0], (Quaternion) data[1]);
				arrow.GetComponent<Projectile>().SetVelocity((Vector3)data[2]);
				PhotonView photonView = arrow.GetComponent<PhotonView>();
				photonView.ViewID = (int) data[3];
			}
		} */
		// no callbacks in the player script
		#endregion
		// they may overlap
		IEnumerator SlowDownTimeFor(float duration) {
			// will be enemy speed, using player speed to test the property
			// like color
			// float oldVal = GlobalVars.PlayerSpeed.Value;
			GlobalVars.EnnemySpeed = 0.5f;
			// _animator.speed = GlobalVars.PlayerSpeed; // to remove if only slowing down enemies
			_slowdownAcc += 1;
			yield return new WaitForSeconds(duration);
			_slowdownAcc -= 1;
			if (_slowdownAcc == 0) {
				GlobalVars.EnnemySpeed = 1;
				_animator.speed = 1;
			}
		}
		/*
		IEnumerator TimeFreezeFor(float duration) {
			_timeFreezeAcc += 1;
			GlobalVars.EnnemySpeed = 0f;
			yield return new WaitForSeconds(duration);
			_timeFreezeAcc -= 1;
			if (_timeFreezeAcc == 0) {
				GlobalVars.EnnemySpeed = 1;
			}
		} */
		Vector3 GetMouseRelativePos() {
			Vector3 mousePosition = Input.mousePosition;
			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
			var position = transform.position;
			float y = (mousePosition.y - position.y);
			float x = (mousePosition.x - position.x);
			return new Vector3(x, y, 0f).normalized;
		}
		void PlacePreviewArrow() {
			if (_canShootArrow) {
				Vector3 mousePosition = Input.mousePosition;
				mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
				var position = transform.position;
				float y = (mousePosition.y - position.y);
				float x = (mousePosition.x - position.x);
				_animator.SetFloat(MouseX, x);
				_animator.SetFloat(MouseY, y);
				Vector3 pos = new Vector3(x, y, 0f);
				_arrowPreviewRef.transform.position = position + pos.normalized;
				float teta = Mathf.Atan(y / x) * 180 / Mathf.PI - (mousePosition.x > position.x ? 90 : -90);
				_arrowPreviewRef.transform.eulerAngles = new Vector3(0f, 0f, teta);
			}
		}
		void PlacePreviewZone() {
			Vector3 mousePosition = Input.mousePosition;
			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
			var position = transform.position;
			float y = (mousePosition.y - position.y);
			float x = (mousePosition.x - position.x);
			Vector3 pos = new Vector3(x, y, 0f);
			if (Vector3.Distance(pos, Vector3.zero) > _maxBombDist) {
				pos = _maxBombDist * pos.normalized;
			}
			_animator.SetFloat(MouseX, pos.x);
			_animator.SetFloat(MouseY, pos.y);
			_poisonZonePreviewRef.transform.position = new Vector3(position.x + pos.x, position.y + pos.y, 0f);
		}
		void UpdateAnimationAndMove() {
			if (change != Vector2.zero) {
				// 0.2 f ?
				_myRigidBody.velocity = (change * (0.2f * initialSpeed * speedModifier/* * GlobalVars.PlayerSpeed*/));
				_animator.SetFloat(MoveX, change.x);
				_animator.SetFloat(MoveY, change.y);
				_animator.SetBool(IsMoving, true);
			}
			else {
				_myRigidBody.velocity = Vector2.zero;
				_animator.SetBool(IsMoving, false);
			}
		}
		IEnumerator SwordAttack() {
			// wielding for 100 degrees
			_canSwordAttack = false;
			_swordHitzone.SetActive(true);
			// does not seem to work when the player has not yet moved
			float currentSwordRot = Mathf.Atan(notNullChange.y / notNullChange.x) * 180 / Mathf.PI +
			                        (notNullChange.x >= 0 ? 0 : 180);
			_swordHitzone.transform.eulerAngles = new Vector3(0f, 0f, currentSwordRot);
			_swordHitzone.transform.position = (Vector2)transform.position + notNullChange * _swordDist;
			// _swordHitzoneCollider.enabled = true;
			speedModifier = _attackSpeedNerf;
			// isWielding = true;
			yield return new WaitForSeconds(SwordTime /* GlobalVars.PlayerSpeed */);
			_swordHitzone.SetActive(false);
			speedModifier = 1;
			yield return new WaitForSeconds(SwordAttackCooldown /* GlobalVars.PlayerSpeed */);
			_canSwordAttack = true;
		}
		IEnumerator ShootArrow() {
			_canShootArrow = false;
			Vector3 pos = GetMouseRelativePos();
			float teta = Mathf.Atan(pos.y / pos.x) * 180 / Mathf.PI - (pos.x > 0 ? 90 : -90);
			Quaternion rot = Quaternion.Euler(0f, 0f, teta);
			GameObject arrow = PhotonNetwork.Instantiate("Prefabs/Projectiles/Arrow",transform.position+pos,rot);
			arrow.GetComponent<Projectile>().SetVelocity(pos);
			yield return new WaitForSeconds(_bowCooldown /* GlobalVars.PlayerSpeed*/ );
			_canShootArrow = true;
		}

		/* private void SpawnArrowCustomCall(Vector3 mousepos, Quaternion rotation) {
			GameObject arrow = (GameObject)Instantiate(_arrowPrefab,transform.position+mousepos,rotation);
			arrow.GetComponent<Projectile>().SetVelocity(mousepos);
			PhotonView photonView = arrow.GetComponent<PhotonView>();
			if (PhotonNetwork.AllocateViewID(photonView)) {
				object[] data = new object[] {
					arrow.transform.position, arrow.transform.rotation, mousepos, photonView.ViewID
				};

				RaiseEventOptions raiseEventOptions = new RaiseEventOptions {
					Receivers = ReceiverGroup.Others,
					CachingOption = EventCaching.AddToRoomCache
				};

				SendOptions sendOptions = new SendOptions {
					Reliability = true
				};

				PhotonNetwork.RaiseEvent(NetworkArrowSpawnRef, data, raiseEventOptions, sendOptions);
			}
			else {
				Debug.LogError("Failed to allocate a ViewId.");
				Destroy(arrow);
			}
		}*/
		// first throws the bomb, and then instantiates the poison bomb
		IEnumerator ThrowPoisonBomb() {
			_canThrowPoisonBomb = false;
			Vector3 mousePosition = Input.mousePosition;
			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
			var position = transform.position;
			photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,0.3f, 0.3f, 1f, 0.8f, 0.2f);
			float y = (mousePosition.y - position.y);
			float x = (mousePosition.x - position.x);
			Vector3 pos = new Vector3(x, y, 0f);
			if (Vector3.Distance(pos, Vector3.zero) > _maxBombDist) {
				pos = _maxBombDist * pos.normalized;
			}
			/*GameObject pZone =*/
			PhotonNetwork.Instantiate("Prefabs/Projectiles/PoisonZone", new Vector3(position.x + pos.x, position.y + pos.y, 0f), new Quaternion());
			yield return new WaitForSeconds(_poisonBombCooldown /* GlobalVars.PlayerSpeed*/ );
			_canThrowPoisonBomb = true;
		}

		IEnumerator Dash() {
			// does not execute the dash if the player is not moving
			// ends up executing it ? why ?
			if (change != Vector2.zero) {
				_canDash = false;
				_isDashing = true;
				speedModifier = _dashPower;
				photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,1f, 1f, 0.3f, 0.8f, 0.2f); // yellow
				yield return new WaitForSeconds(_dashTime /* GlobalVars.PlayerSpeed*/ );
				speedModifier = 1;
				_isDashing = false;
				yield return new WaitForSeconds(_dashCooldown /* GlobalVars.PlayerSpeed*/ );
				_canDash = true;
			}
		}

		private void GoBackInTime() {
			ghostPlayer.GoBackInTime();
		}

		// ADD SOUNDS HERE
		public void TakeDamages(ushort damage) {
			photonView.RPC("TakeDmgRPC",RpcTarget.AllBuffered,(short)damage);
		}

		[PunRPC]
		public void TakeDmgRPC(short damage) {
			if (_healthBar.TryTakeDamagesStrict((ushort)damage)) {
				photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,1f, 0.3f, 0.3f, 0.8f, 0.2f);
			}
			else {
				_healthBar.ChangeCurVal(0);
				// play some sound
				isDead = true;
				if (photonView.IsMine) GameOver();
				DisableOrEnablePlayer(false);
			}
		}
		public void Heal(ushort heal) {
			photonView.RPC("HealRPC",RpcTarget.AllBuffered,(short)heal);
		}
		[PunRPC]
		public void HealRPC(short heal) {
			if (_healthBar.curValue == 0) {
				DisableOrEnablePlayer(true);
				if (photonView.IsMine) Revive();
				isDead = false;
			}
			_healthBar.Heal((ushort)heal);
			photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,0.3f, 1f, 0.3f, 0.8f, 0.2f);
		}

		// these ones are not used over network
		// maybe change these colors ???
		public void TakeDamagesStamina(ushort damage) {
			_staminaBar.TakeDamages(damage);
			photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,1f, 0.3f, 0.3f, 0.8f, 0.2f);
		}

		public void HealStamina(ushort heal) {
			_staminaBar.Heal(heal);
			photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,0.3f, 1f, 0.3f, 0.8f, 0.2f);
		}

		public void TakeDamagesMana(ushort damage) {
			_manaBar.TakeDamages(damage);
			photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,1f, 0.3f, 0.3f, 0.8f, 0.2f);
		}

		public void HealMana(ushort heal) {
			_manaBar.Heal(heal);
			photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,0.3f, 1f, 0.3f, 0.8f, 0.2f);
		}

		private void GameOver() {
			List<Player> otherPlayers = new List<Player>();
			foreach (Player player in GlobalVars.PlayerList) {
				if (!player.photonView.IsMine && player.isActiveAndEnabled) otherPlayers.Add(player);
			}
			if (otherPlayers.Count > 0) {
				otherPlayers[0].GetComponentInChildren<Camera>().enabled = true;
				otherPlayers[0].GetComponentInChildren<AudioListener>().enabled = true;
				StatusText.gameObject.SetActive(true);
				StatusText.text = TextValues.YouDied;
			}
			// no more players are alive, game over screen and return to the title screen
			else {
				// all players SHOULD follow the scene transition, and go to game over screen
                photonView.RPC("LoadGameOverRPC",RpcTarget.MasterClient);
			}
		}

		[PunRPC]
		public void LoadGameOverRPC() {
			PhotonNetwork.LoadLevel("GameOver");
		}

		private void DisableOrEnablePlayer(bool val) {
			GetComponent<Collider2D>().enabled = val;
			GetComponent<SpriteRenderer>().enabled = val;
			if (photonView.IsMine) {
				GetComponentInChildren<Camera>().enabled = val;
				GetComponentInChildren<AudioListener>().enabled = val;
			}
			GetComponentInChildren<Light2D>().enabled = val;
			GetComponentInChildren<Canvas>().enabled = val;
			ghostPlayer.GetComponentInChildren<Light2D>().enabled = val;
			this.enabled = val;
		}

		private void Revive() {
			gameObject.GetComponentInChildren<Camera>().enabled = true;
			foreach (Player player in GlobalVars.PlayerList) {
				if (!photonView.IsMine) {
					player.GetComponentInChildren<Camera>().enabled = false;
					player.GetComponentInChildren<AudioListener>().enabled = false;
				}
			}
			StatusText.text = TextValues.Revived;
			StartCoroutine(FadeText());
			/*
			if (Camera.main is { } cam) {
				cam.GetComponent<CameraWork>()._player = this; //LocalPlayerInstance.GetComponent<Player>();
			}
			else Debug.LogError("CAMERA.main is null => no camera found ???");
			*/
		}

		IEnumerator FadeText() {
			float fadeLvl = 1f;
			for (int i = 0; i < 100; i++) {
				StatusText.color = new Color(1, 1, 1, fadeLvl);
				fadeLvl -= 0.01f;
				yield return new WaitForSeconds(0.02f);
			}
			StatusText.color = Color.white;
			StatusText.gameObject.SetActive(false);
		}

		IEnumerator ChangeColorWait(Color color, float time) {
			Color baseColor = _renderer.material.color;
			_colorAcc += 1;
			_spriteRenderer.color = (color);
			yield return new WaitForSeconds(time);
			_colorAcc -= 1;
			if (_colorAcc == 0) {
				_spriteRenderer.color = (Color.white);
			}
			else if (baseColor != Color.white) _spriteRenderer.color =(baseColor);
		}

		// must be changed to float values for the RPC to work
		[PunRPC]
		void ChangeColorWaitRpc(float r, float g, float b, float a, float time) {
			StartCoroutine(ChangeColorWait(new Color(r, g, b, a), time));
		}

		#region IPunObservable implementation
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				stream.SendNext((short)_healthBar.curValue);
				//stream.SendNext(isDead);
			}
			else {
				_healthBar.curValue = (ushort)(short)stream.ReceiveNext();
				//isDead = (bool)stream.ReceiveNext();
			}
		}
		#endregion
	}
}

#region Code TrashBin, some scraps that can be useful later in developement
/*void CalledOnLevelWasLoaded(int level)
{
    // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
    if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
    {
        transform.position = new Vector3(0f, 5f, 0f);
    }
}

#if UNITY_5_4_OR_NEWER
public override void OnDisable()
 {
     // Always call the base to remove callbacks
     base.OnDisable ();
     UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
 }
#endif

#endregion

#region Private Methods

#if UNITY_5_4_OR_NEWER
 void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
{
    this.CalledOnLevelWasLoaded(scene.buildIndex);
}
#endif


 #if UNITY_5_4_OR_NEWER
// Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
    UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
*/
#endregion