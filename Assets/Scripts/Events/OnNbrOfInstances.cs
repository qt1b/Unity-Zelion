using System;
using Interfaces;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Code.Interfaces;
using UnityEngine;

namespace Events {
	public class OnNbrOfInstances: MonoBehaviourPunCallbacks, IPunObservable {
		public ushort current;
		public ushort goal;
		private IAction _action;

		public void Awake() {
			if (current > short.MaxValue) current = (ushort)short.MaxValue;
			if (goal > short.MaxValue) goal = (ushort)short.MaxValue;
			_action = gameObject.GetComponent<IAction>();
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				stream.SendNext(current);
			}
			else {
				current = (ushort)(short)stream.ReceiveNext();
			}
		}

		public void Update() {
			if (current >= goal) {
				_action.Activate();
			}
		}
	}
}