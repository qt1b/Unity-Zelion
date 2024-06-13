using System;
using System.Collections;
using Global;
using UnityEngine;

namespace Player {
	public class GhostPlayer : MonoBehaviour {
		private Animator _animator;
		private Animator _playeranim;
		private static readonly int IsMoving = Animator.StringToHash("IsMoving");
		private static readonly int MoveY = Animator.StringToHash("MoveY");
		private static readonly int MoveX = Animator.StringToHash("MoveX");
		private static readonly int MouseY = Animator.StringToHash("MouseY");
		private static readonly int MouseX = Animator.StringToHash("MouseX");
		private float _timeLag = 2f;
		private bool _isOkToMove = true;
		public void Awake() {
			_animator = gameObject.GetComponent<Animator>();
			_playeranim = Player.LocalPlayerInstance.GetComponent<Animator>();
		}

		private void Update() {
			StartCoroutine(MoveAfterTimeLag());
		}

		IEnumerator MoveAfterTimeLag() {
			Vector3 playerPos = Player.LocalPlayerInstance.transform.position;
			float moveX = _playeranim.GetFloat(MoveX);
			float moveY = _playeranim.GetFloat(MoveY);
			bool isMoving = _playeranim.GetBool(IsMoving);
			//float animX = _playeranim.GetFloat(MoveX);
			//float animY = _playeranim.GetFloat(MoveY);
			yield return new WaitForSeconds(_timeLag);
			if (_isOkToMove) {
				gameObject.transform.position = playerPos;
				_animator.SetFloat(MoveX, moveX);
				_animator.SetFloat(MoveY, moveY);
				_animator.SetBool(IsMoving, isMoving);
			}
			else {
				_animator.SetBool(IsMoving,false);
			}
		}

		IEnumerator Cooldown() {
			yield return new WaitForSeconds(_timeLag);
			_isOkToMove = true;
		}

		public void GoBackInTime() {
			Player.LocalPlayerInstance.transform.position = this.transform.position;
			_isOkToMove = false;
		}
	}
}