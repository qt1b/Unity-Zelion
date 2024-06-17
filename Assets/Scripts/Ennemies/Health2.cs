using System;
using System.Collections;
using Bars;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;

namespace Ennemies {
	// wraps around a health bar, for enemies with one
	public class Health2 : MonoBehaviourPunCallbacks, IPunObservable, IHealth {
		#region Fields
		private Bars.HealthBar _healthBar;
		public ushort maxHealth;
		public float deathDuration;
		private SpriteRenderer _spriteRenderer; // to change color when hit
		private uint _colorAcc;
		private static readonly int Death = Animator.StringToHash("Death");
		#endregion

		private void Start() {
			maxHealth = (ushort)(maxHealth * (PhotonNetwork.CurrentRoom.PlayerCount > 0 ? PhotonNetwork.CurrentRoom.PlayerCount : 1));
			_healthBar = GetComponentInChildren<HealthBar>();
			if (maxHealth > short.MaxValue) {
				// is necessary to avoid errors while syncing hp's value
				Debug.LogError("max value is too big, resizing it to short's max value");
				maxHealth = (ushort)short.MaxValue;
			}
			_healthBar.ChangeMaxValue(maxHealth);
			_spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
		}

		// should avoid doing any rpcs, need to see if this updates fast enough
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			// not any enemy should have HP superior to 32_000
			if (stream.IsWriting)
			{
				stream.SendNext((short)_healthBar.curValue);
			}
			else
			{
				// Network player, receive data
				ushort received = (ushort)(short)stream.ReceiveNext();
				if (received != _healthBar.curValue) _healthBar.ChangeCurVal(received);
			}
		}

		public void TakeDamages(ushort damage) {
			if (_healthBar.TryTakeDamagesStrict(damage)) {
				photonView.RPC("ChangeColorWaitRpc", RpcTarget.AllBuffered, 1f, 0.3f, 0.3f, 0.5f);
			}
			else Die();
		}

		private void Die() {
			//photonView.RPC("NetworkDestroy",RpcTarget.MasterClient);
			GetComponent<PhotonView>().RPC("DieRPC", RpcTarget.AllBuffered);
			// does not need to be master client ??
			CollectibleDrop.Activate(maxHealth,gameObject.transform.position);
		}

		[PunRPC]
		public void DieRPC() {
			if (gameObject.TryGetComponent(out Collider2D component)) {
				component.enabled = false;
			}
			if (gameObject.TryGetComponent(out Ennemy ennemy)) {
				ennemy.StopAllCoroutines();
				ennemy.Agent.destination = ennemy.transform.position;
				ennemy.enabled = false;
			}
			if (gameObject.TryGetComponent(out Animator animator)) {
				animator.SetBool(Death,true);
			}
			if (gameObject.TryGetComponent(out IAction action)) {
				action.Activate();
			}
			if (this.photonView.IsMine && PhotonNetwork.IsConnected) {
				StartCoroutine(DestroyAfterSecs(deathDuration));
			}
		}
		IEnumerator DestroyAfterSecs(float secs) {
			yield return new WaitForSeconds(secs);
			PhotonNetwork.Destroy(gameObject);
		}
		public void Heal(ushort heal) {
			_healthBar.Heal(heal);
			photonView.RPC("ChangeColorWaitRpc",RpcTarget.AllBuffered,0.3f, 1f, 0.3f, 0.5f);
		}

		[PunRPC]
		public void ChangeColorWaitRpc(float r, float g, float b, float time) {
			StartCoroutine(ChangeColorWait(new Color(r,g,b), time));
		}
		[PunRPC]
		public void ChangeColorWAlphaWaitRpc(float r, float g, float b, float a, float time) {
			StartCoroutine(ChangeColorWait(new Color(r,g,b,a), time));
		}
		IEnumerator ChangeColorWait(Color color, float time) {
			Color baseColor = _spriteRenderer.color;
			_spriteRenderer.color = color;
			_colorAcc += 1;
			yield return new WaitForSeconds(time);
			_colorAcc -= 1;
			if (_colorAcc == 0) {
				_spriteRenderer.color = Color.white;
			}
			else if (baseColor != Color.white) _spriteRenderer.color = baseColor;
		}
	}
}