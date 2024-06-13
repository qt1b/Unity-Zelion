using System;
using System.Collections;
using Bars;
using Global;
using UnityEngine;

namespace Player {
	public class GhostPlayer : MonoBehaviour {
		private Animator _animator;
		private Animator _playeranim;
		private Player _player;
		private SpriteRenderer _spriteRenderer;
		private static readonly int IsMoving = Animator.StringToHash("IsMoving");
		private static readonly int MoveY = Animator.StringToHash("MoveY");
		private static readonly int MoveX = Animator.StringToHash("MoveX");
		private static readonly int MouseY = Animator.StringToHash("MouseY");
		private static readonly int MouseX = Animator.StringToHash("MouseX");
		private float _timeLag = 2f;
		//private bool _isOkToMove = true;
		public void Awake() {
			_player = Player.LocalPlayerInstance.GetComponent<Player>();
			_animator = gameObject.GetComponent<Animator>();
			_playeranim = Player.LocalPlayerInstance.GetComponent<Animator>();
			_player.ghostPlayer = this;
			transform.position = Player.LocalPlayerInstance.transform.position;
		}

		private void Update() {
			StartCoroutine(MoveAfterTimeLag());
			//if (_player.GetComponent<ManaBar>().CanTakeDamages(7))
		}

		IEnumerator MoveAfterTimeLag() {
			Vector3 playerPos = Player.LocalPlayerInstance.transform.position;
			float moveX = _playeranim.GetFloat(MoveX);
			float moveY = _playeranim.GetFloat(MoveY);
			bool isMoving = _playeranim.GetBool(IsMoving);
			//float animX = _playeranim.GetFloat(MoveX);
			//float animY = _playeranim.GetFloat(MoveY);
			yield return new WaitForSeconds(_timeLag);
			// if (_isOkToMove) {
				gameObject.transform.position = playerPos;
				_animator.SetFloat(MoveX, moveX);
				_animator.SetFloat(MoveY, moveY);
				_animator.SetBool(IsMoving, isMoving);
			//}
			/*else {
				_animator.SetBool(IsMoving,false);
			}*/
		}

		IEnumerator Cooldown() {
			yield return new WaitForSeconds(_timeLag);
			//_isOkToMove = true;
		}

		public void GoBackInTime() {
			Player.LocalPlayerInstance.transform.position = this.transform.position;
			//_isOkToMove = false;
			//StartCoroutine(Cooldown());
		}

		public void Display(bool val) {
			if (val) _spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
			else _spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
		}
	}
}