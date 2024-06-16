using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Global;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using Player;
using TMPro;

// ALSO ACTS AS A LEVEL LOADER !

namespace PUN {
	public class GameManager : MonoBehaviourPunCallbacks {
		#region Public Fields
		public static GameManager Instance;
		public static bool WantToDisconnect;
		public TMP_Text NetworkStatusText; // TODO : DISABLE for public build
		public Animator LoaderAnim;
		private float LoadTime = 0.4f;
		private static readonly int Start1 = Animator.StringToHash("Start");

		#endregion

		#region MonoBehaviour

		private void Awake() {
			Instance = this;  //gameObject.GetComponent<GameManager>();
		}

		private void Start() {
			Debug.Log("Game Manager : Starting ...");
			// GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
			// decided not to keep the instance of the player between scenes for now
			// if (global::Player.Player.LocalPlayerInstance == null) {
			GlobalVars.PlayerList = new();
			if (GlobalVars.TimeStartedAt is null) Global.GlobalVars.TimeStartedAt = DateTime.UtcNow;
			Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
			// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
			PhotonNetwork.Instantiate("Prefabs/Player/Player", Vector3.zero, Quaternion.identity, 0);
			Instantiate(Resources.Load<GhostPlayer>("Prefabs/Player/GhostPlayer"));
			/*}
			else {
				Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
			}*/
			NetworkStatusText.SetText(GenerateNetworkStatusText());
		}

		public void LoadLevel(string levelName) {
			photonView.RPC("LoadLevelRpc",RpcTarget.AllBuffered,levelName);
		}

		IEnumerator LoadAnimRpc(string levelName) {
			LoaderAnim.SetBool(Start1,true);
			yield return new WaitForSeconds(LoadTime);
			if (PhotonNetwork.IsMasterClient) {
				// should be fine here, but can be dangerous bc of networking
				GlobalVars.CurrentLevelId += 1;
				GlobalVars.SaveId = 0;
				PhotonNetwork.LoadLevel(levelName);
			}
		}
		[PunRPC]
		private void LoadLevelRpc(string levelName) {
			StartCoroutine(LoadAnimRpc(levelName));
		}
		#endregion
		#region Photon Callbacks

		public override void OnDisconnected(DisconnectCause cause) {
			Debug.LogError("DISCONNECTED !!");
			if (PhotonNetwork.OfflineMode is true) {
				Debug.LogError("Offline mode Activated, trying to reconnect ??");
				// do not Disconnect !!!
				// idk if it works in offline mode
				PhotonNetwork.ReconnectAndRejoin();
			}
			else if (!WantToDisconnect) {
				PhotonNetwork.ReconnectAndRejoin();
			}
		}

		/*
		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		*/
		public override void OnLeftRoom() {
			WantToDisconnect = true;
			Debug.LogError("OnLeftRoom : Disconnected");
			SceneManager.LoadScene(0);
			global::Player.Player.LocalPlayerInstance = null;
			Destroy(gameObject); // destroys this very game manager
		}
		/*
		// are not being used ? idk
		*/
		public override void OnPlayerEnteredRoom(Photon.PhotonRealtime.Code.Player other)
		{
			Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
			NetworkStatusText.SetText(GenerateNetworkStatusText());
		}

		public override void OnPlayerLeftRoom(Photon.PhotonRealtime.Code.Player other)
		{
			Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
			NetworkStatusText.SetText(GenerateNetworkStatusText()); // to comment at the end
			if (GlobalVars.PlayerList.Count != 0 && GlobalVars.PlayerList.TrueForAll(p => p.isDead)) {
				PhotonNetwork.LoadLevel(GlobalVars.GameOverSceneName);
			}
			/*
			if (!allowJoining) {
				PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
			}*/
		}

		public override void OnMasterClientSwitched(Photon.PhotonRealtime.Code.Player newMasterClient) {
			if (PhotonNetwork.IsMasterClient) {
				PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
			}
		}
		#endregion

		#region Private Functions

		private string GenerateNetworkStatusText() {
			return
				$"RoomID: {PhotonNetwork.CurrentRoom.Name} | Players Connected: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers} | ID: {Global.GlobalVars.PlayerId} | Nickname: {PhotonNetwork.NickName}";
		}

		#endregion
	}
}