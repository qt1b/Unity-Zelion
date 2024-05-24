using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Photon.PhotonUnityNetworking.Code;
using Player = Photon.PhotonRealtime.Code.Player;

namespace PUN {
	public class GameManager : MonoBehaviourPunCallbacks {
		#region Public Fields
		
		public static GameManager Instance;
		
		//[Tooltip("The prefab to use for representing the player")]
		//public GameObject playerPrefab; 
		
		#endregion

		#region MonoBehoviour
		private void Start() {
			Instance = this;
			GameObject playerPrefab = Resources.Load<GameObject>("RoguePUN");
			if (playerPrefab == null)
			{
				Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
			}
			else
			{
				if (global::Player.Player.LocalPlayerInstance == null)
				{
					Global.GlobalVars.TimeStartedAt = DateTime.Now;
					Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
					// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
					PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
				}
				else
				{
					Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
				} 
			}
		}
		#endregion
		
		#region Photon Callbacks
		/*
		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			SceneManager.LoadScene(0);
		}
		// are not being used ? idk
		public override void OnPlayerEnteredRoom(Photon.PhotonRealtime.Code.Player other)
		{
			Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

			if (PhotonNetwork.IsMasterClient)
			{
				Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

				LoadArena();
			}
		}

		public override void OnPlayerLeftRoom(Photon.PhotonRealtime.Code.Player other)
		{
			Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

			if (PhotonNetwork.IsMasterClient)
			{
				Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

				LoadArena();
			}
		}
		#endregion

		#region Public Methods

		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
		}

		#endregion
		#region Private Methods
		void LoadArena()
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
				return;
			}
			Debug.LogFormat("PhotonNetwork : Loading Level, Having {0} Players", PhotonNetwork.CurrentRoom.PlayerCount);
			// to modify this for Zelion
			//PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
			PhotonNetwork.LoadLevel("Quentin5");
		} */

		#endregion
	}
}