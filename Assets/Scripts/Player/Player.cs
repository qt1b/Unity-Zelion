using System;
using System.Collections;
using System.IO;
using System.Linq;
using Actions;
using Bars;
using ExitGames.Client.Photon;
using Global;
using Interfaces;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;
using Object = System.Object;

namespace Player {
	
	
	public class Player : MonoBehaviourPunCallbacks, IHealth {
		#region Public Fields

		[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
		public static GameObject LocalPlayerInstance;

		public float initialSpeed = 7f;
		[DoNotSerialize] public float speedModifier = 1;
		[DoNotSerialize] public Vector2 change = Vector2.zero;
		[DoNotSerialize] public Vector2 notNullChange = new Vector2(0, 1);
		[DoNotSerialize] public bool isDead;

		#endregion

		#region Capacity Unlocking Booleans

		private bool _swordUnlocked;
		private bool _bowUnlocked;
		private bool _poisonUnlocked;
		private bool _dashUnlocked;
		private bool _slowdownUnlocked;
		private bool _timeFreezeUnlocked;

		#endregion

		#region Capacity Availability Booleans

		bool _canSwordAttack = true;
		bool _canDash = true;
		bool _canShootArrow = true;
		bool _canThrowPoisonBomb = true;
		bool _canSlowDownTime = true;
		bool _canTimeFreeze = true;
		bool _isDashing;
		bool _isAimingArrow;
		bool _isAimingBomb;

		#endregion

		#region Getters Combining if Capacity is Unloked and if it is available

		private bool CanSwordAttack => _swordUnlocked && _canSwordAttack;
		private bool CanDash => _dashUnlocked && _canDash;
		private bool CanShootArrow => _bowUnlocked && _canShootArrow;
		private bool CanPoison => _poisonUnlocked && _canThrowPoisonBomb;
		private bool CanSlowDownTime => _slowdownUnlocked && _canSlowDownTime;
		private bool CanTimeFreeze => _timeFreezeUnlocked && _canTimeFreeze;

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

		private Rigidbody2D _myRigidBody;
		private GameObject _arrowPreviewRef;
		private GameObject _poisonZoneRef;
		private GameObject _poisonZonePreviewRef;
		private Animator _animator;
		private GameObject _swordHitzone;
		private HealthBar _healthBar;
		private StaminaBar _staminaBar;
		private ManaBar _manaBar;
		private Renderer _renderer;
		private GameObject _arrowPrefab;
		#endregion

		#region Cached Values

		private static readonly int Color1 = Shader.PropertyToID("_Color");
		private static readonly int IsMoving = Animator.StringToHash("IsMoving");
		private static readonly int MoveY = Animator.StringToHash("MoveY");
		private static readonly int MoveX = Animator.StringToHash("MoveX");
		private static readonly int MouseY = Animator.StringToHash("MouseY");
		private static readonly int MouseX = Animator.StringToHash("MouseX");
		private static readonly int AimingBomb = Animator.StringToHash("AimingBomb");
		private static readonly int AimingBow = Animator.StringToHash("AimingBow");

		#endregion

		#region Network Callback References
		// private byte NetworkArrowSpawnRef = 245;
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
			// Reads from the save Id common to all instances
			/*
			var lookupTable = GlobalVars.SaveLookupData.Split('\n').Skip(1).ToArray();
			if (lookupTable.Length > GlobalVars.SaveId) {
				string[] args = lookupTable[GlobalVars.SaveId].Split(';');
				if (args.Length != 11)
					throw new ArgumentException("the save lookup table is not formatted as expected; saveID=" + GlobalVars.SaveId);
				else {
					// should work ? may be better to use transforms manually
					Vector3 pos =  new Vector3(int.Parse(args[0]), int.Parse(args[1]), 0f);
					gameObject.transform.position = pos;
					Camera.main.transform.position = pos;
					_healthBar.ChangeMaxValue(uint.Parse(args[2]));
					_staminaBar.ChangeMaxValue(uint.Parse(args[3]));
					_manaBar.ChangeMaxValue(uint.Parse(args[4]));
					_swordUnlocked = args[5] == "1";
					_bowUnlocked = args[6] == "1";
					_poisonUnlocked = args[7] == "1";
					_dashUnlocked = args[8] == "1";
					_slowdownUnlocked = args[9] == "1";
					_timeFreezeUnlocked = args[10] == "1";
					Debug.Log("Loaded Save successfully, saveID=" + GlobalVars.SaveId);
				}
			}
			else throw new NotImplementedException("Unsupported saveID : " + GlobalVars.SaveId);
			*/
			// ver 2
			if (GlobalVars.SaveLookupArray.GetLength(0) > GlobalVars.SaveId) {
				Vector3 pos =  new Vector3(int.Parse(GlobalVars.SaveLookupArray[GlobalVars.SaveId,0]), int.Parse(GlobalVars.SaveLookupArray[GlobalVars.SaveId,1]), 0f);
				gameObject.transform.position = pos;
				Camera.main.transform.position = new Vector3(pos.x,pos.y,-1f);
				_healthBar.ChangeMaxValue(uint.Parse(GlobalVars.SaveLookupArray[GlobalVars.SaveId,2]));
				_staminaBar.ChangeMaxValue(uint.Parse(GlobalVars.SaveLookupArray[GlobalVars.SaveId,3]));
				_manaBar.ChangeMaxValue(uint.Parse(GlobalVars.SaveLookupArray[GlobalVars.SaveId,4]));
				_swordUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId,5] == "1";
				_bowUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId,6] == "1";
				_poisonUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId,7] == "1";
				_dashUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId,8] == "1";
				_slowdownUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId,9] == "1";
				_timeFreezeUnlocked = GlobalVars.SaveLookupArray[GlobalVars.SaveId,10] == "1";
				Debug.Log("Loaded Save successfully, saveID=" + GlobalVars.SaveId);
			}
		}

		#endregion
		#region MonoBehaviour
		public CursorManager cursorManager; 
		void Awake() {
			GlobalVars.PlayerList.Add(this);
			if (!photonView.IsMine) {
				return;
			}
			// #Important
			// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
			Player.LocalPlayerInstance = this.gameObject;
			// #Critical
			// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
			DontDestroyOnLoad(this.gameObject);

			// initializing all needed references
			_animator = GetComponent<Animator>();
			_myRigidBody = GetComponent<Rigidbody2D>();
			_animator.speed = GlobalVars.PlayerSpeed;
			_swordHitzone = transform.GetChild(0).gameObject;
			_swordHitzone.SetActive(false);
			_poisonZoneRef = Resources.Load<GameObject>("Prefabs/Projectiles/PoisonZone");
			_arrowPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Arrow");
			_arrowPreviewRef = transform.GetChild(1).gameObject;
			_poisonZonePreviewRef = transform.GetChild(2).gameObject;
			_healthBar = FindObjectOfType<HealthBar>();
			_staminaBar = FindObjectOfType<StaminaBar>();
			_manaBar = FindObjectOfType<ManaBar>();
			_renderer = gameObject.GetComponent<Renderer>();
			LoadSave();
		}

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during initialization phase.
		/// </summary>
		void Start() {
			if (!photonView.IsMine) {
				// this.enabled = false;
				return;
			}
			if (Camera.main is { } mainCamera && mainCamera.GetComponent<CameraWork>() is { } cameraWork) {
				cameraWork.OnStartFollowing();
			}
			else {
				Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component", this);
			}
		}

		// Update is called once per frame
		// To Add : Sounds to indicate whether we can use the capacity or not
		void Update() {
			
		
			// && PhotonNetwork.IsConnected is for debugging purposes
			if ((!photonView.IsMine && PhotonNetwork.IsConnected) || PauseMenu.GameIsPaused) {
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
				if (_isAimingArrow)
				{ cursorManager.SetCursor(cursorManager.crosshairTexture, cursorManager.crosshairHotSpot); }   
	


				if (_isAimingArrow) {
					if (!_arrowPreviewRef.activeSelf && _canShootArrow && _staminaBar.CanTakeDamages(2))
						_arrowPreviewRef.SetActive(true);
					PlacePreviewArrow();
					if (Input.GetKeyUp(KeyCode.Mouse0)) {
						if (_canShootArrow && _staminaBar.TryTakeDamages(2)) StartCoroutine(ShootArrow());
						_isAimingArrow = false;
						speedModifier = 1;
						_arrowPreviewRef.SetActive(false);
						_animator.SetBool(AimingBow, false);
					}
				}
				else if (_isAimingBomb) {
					if (!_poisonZonePreviewRef.activeSelf && _canThrowPoisonBomb && _manaBar.CanTakeDamages(10))
						_poisonZonePreviewRef.SetActive(true);
					PlacePreviewZone();
					if (Input.GetKeyUp(KeyCode.Mouse1)) {
						if (_canThrowPoisonBomb && _manaBar.TryTakeDamages(10)) StartCoroutine(ThrowPoisonBomb());
						_isAimingBomb = false;
						speedModifier = 1;
						_animator.SetBool(AimingBomb, false);
						_poisonZonePreviewRef.SetActive(false);
					}
				}
				else {
					// the sword has not any cost
					if (CanSwordAttack && Input.GetKeyDown(KeyCode.Space)) {
						StartCoroutine(SwordAttack());
					}
					else if (CanDash && Input.GetKeyDown(KeyCode.LeftShift) && _staminaBar.TryTakeDamages(10)) {
						StartCoroutine(Dash());
					}
					else if (_bowUnlocked && Input.GetKeyDown(KeyCode.Mouse0)) {
						_animator.SetBool(AimingBow, true);
						// bow aiming audio effect
						_isAimingArrow = true;
						speedModifier = _attackSpeedNerf;
						if (_canShootArrow) _arrowPreviewRef.SetActive(true);
						// PoisonZonePreviewRef.SetActive(true);
						PlacePreviewArrow();
						// ArrowPreviewRef.transform.position = transform.position;
					}
					// poison zone: audio from the prefab
					else if (_poisonUnlocked && Input.GetKeyDown(KeyCode.Mouse1)) {
						_animator.SetBool(AimingBomb, true);
						// poison aiming audio effect
						_isAimingBomb = true;
						speedModifier = _attackSpeedNerf;
						if (_canThrowPoisonBomb) _poisonZonePreviewRef.SetActive(true);
						PlacePreviewZone();
					}
					else if (CanSlowDownTime && Input.GetKeyDown(KeyCode.LeftControl) &&
					         _manaBar.TryTakeDamages(5)) {
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
			GlobalVars.PlayerSpeed = 0.5f;
			_animator.speed = GlobalVars.PlayerSpeed; // to remove if only slowing down enemies
			_slowdownAcc += 1;
			yield return new WaitForSeconds(duration);
			_slowdownAcc -= 1;
			if (_slowdownAcc == 0) {
				GlobalVars.PlayerSpeed = 1;
				_animator.speed = 1;
			}
		}
		IEnumerator TimeFreezeFor(float duration) {
			_timeFreezeAcc += 1;
			GlobalVars.PlayerSpeed = 0f;
			yield return new WaitForSeconds(duration);
			_timeFreezeAcc -= 1;
			if (_timeFreezeAcc == 0) {
				GlobalVars.PlayerSpeed = 1;
			}
		}
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
				_myRigidBody.velocity = (change * (0.2f * initialSpeed * speedModifier * GlobalVars.PlayerSpeed));
				_animator.SetFloat(MoveX, change.x);
				_animator.SetFloat(MoveY, change.y);
				_animator.SetBool(IsMoving, true);
			}
			else {
				_myRigidBody.velocity = Vector2.zero;
				_animator.SetBool(IsMoving, false);
			}
		}
		public void ChangePlayerControlSpeed(float newSpeedControl) {
			// GlobalVars.PlayerSpeed.Value  = newSpeedControl;
			_animator.speed = GlobalVars.PlayerSpeed;
		}
		// ReSharper disable Unity.PerformanceAnalysis
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
			yield return new WaitForSeconds(SwordTime / GlobalVars.PlayerSpeed);
			_swordHitzone.SetActive(false);
			speedModifier = 1;
			yield return new WaitForSeconds(SwordAttackCooldown / GlobalVars.PlayerSpeed);
			_canSwordAttack = true;
		}
		IEnumerator ShootArrow() {
			_canShootArrow = false;
			Vector3 pos = GetMouseRelativePos();
			float teta = Mathf.Atan(pos.y / pos.x) * 180 / Mathf.PI - (pos.x > 0 ? 90 : -90);
			Quaternion rot = Quaternion.Euler(0f, 0f, teta);
			GameObject arrow = PhotonNetwork.Instantiate("Prefabs/Projectiles/Arrow",transform.position+pos,rot);
			arrow.GetComponent<Projectile>().SetVelocity(pos);
			yield return new WaitForSeconds(_bowCooldown / GlobalVars.PlayerSpeed);
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
			StartCoroutine(ChangeColorWait(new Color(0.3f, 0.3f, 1, 0.8f), 0.2f));
			float y = (mousePosition.y - position.y);
			float x = (mousePosition.x - position.x);
			Vector3 pos = new Vector3(x, y, 0f);
			if (Vector3.Distance(pos, Vector3.zero) > _maxBombDist) {
				pos = _maxBombDist * pos.normalized;
			}
			/*GameObject pZone =*/
			PhotonNetwork.Instantiate("Prefabs/Projectiles/PoisonZone", new Vector3(position.x + pos.x, position.y + pos.y, 0f), new Quaternion());
			yield return new WaitForSeconds(_poisonBombCooldown / GlobalVars.PlayerSpeed);
			_canThrowPoisonBomb = true;
		}

		IEnumerator Dash() {
			// does not execute the dash if the player is not moving
			if (change.y != 0 || change.x != 0) {
				_canDash = false;
				_isDashing = true;
				speedModifier = _dashPower;
				StartCoroutine(ChangeColorWait(new Color(1, 1, 0.3f, 0.8f), 0.2f)); // yellow
				yield return new WaitForSeconds(_dashTime / GlobalVars.PlayerSpeed);
				speedModifier = 1;
				_isDashing = false;
				yield return new WaitForSeconds(_dashCooldown / GlobalVars.PlayerSpeed);
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

		// TODO: change to 
		public void GameOver() {
			isDead = true;
			if (GlobalVars.PlayerList.Any(p => !p.isDead)) {
				if (Camera.main is { } cam) {
					cam.GetComponent<CameraWork>()._player = GlobalVars.PlayerList.First(p => !p.isDead);
				}
				else Debug.LogError("CAMERA.main is null => no camera found ???");
			}
			// no more players are alive, game over screen and return to the title screen
			else {
				// all players SHOULD follow the scene transition, and go to game over screen
                PhotonNetwork.LoadLevel("GameOver");
			}

			// display a indicative text

			// should be used in case where all players are dead and do not decide to replay
			// PUN.GameManager.Instance.LeaveRoom();
		}

		public void Revive() {
			isDead = false;
			if (Camera.main is { } cam) {
				cam.GetComponent<CameraWork>()._player = this; //LocalPlayerInstance.GetComponent<Player>();
			}
			else Debug.LogError("CAMERA.main is null => no camera found ???");
		}

		IEnumerator ChangeColorWait(Color color, float time) {
			Color baseColor = _renderer.material.color;
			_colorAcc += 1;
			photonView.RPC("ChangeColorClientRpc", RpcTarget.AllBuffered, color);
			yield return new WaitForSeconds(time);
			_colorAcc -= 1;
			if (_colorAcc == 0) {
				photonView.RPC("ChangeColorClientRpc", RpcTarget.AllBuffered, Color.white);
			}
			else if (baseColor != Color.white) photonView.RPC("ChangeColorClientRpc", RpcTarget.AllBuffered, baseColor);
		}

		// must be changed to float values for the RPC to work
		[PunRPC]
		void ChangeColorClientRpc(Color color) {
			// cannot manage to make it an effect on top of the sprite
			_renderer.material.SetColor(Color1, color);
		}
	}
}

#region Code TrashBin, some scraps that can be useful later in developement

/* #region IPunObservable implementation
 // does not work because bars are not set for every player
void Photon.Pun.IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        // We own this player: send the others our data
        stream.SendNext(_healthBar.curValue);
    }
    else
    {
        // Network player, receive data
        this._healthBar.curValue = (uint)stream.ReceiveNext();
    }
}
#endregion */
// healing collectibles are not healing and idk why
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