using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Global;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
	public class TitleScreen : MonoBehaviourPunCallbacks {
		// or start ?
		private bool _soloPlay = false;
		public TMP_Text PlayTxt;
		public TMP_Text SettingsTxt;
		public TMP_Text ExitTxt;
		public TMP_Text SinglePlayerTxt;
		public TMP_Text MultiPlayerTxt;
		public TMP_Text BackTxt;

		void Awake() {
			GlobalVars.CurrentLevelId = 0;
		}
		public void Start() {
			PlayTxt.text = TextValues.Play;
			SettingsTxt.text = TextValues.Settings;
			ExitTxt.text = TextValues.Exit;
			SinglePlayerTxt.text = TextValues.SinglePlayer;
			MultiPlayerTxt.text = TextValues.MultiPlayer;
			BackTxt.text = TextValues.Back;
		}

		public void StartSinglePlayer() {
			//PhotonNetwork.NetworkClientState is ClientState.ConnectedToMasterServer) {
			if (PhotonNetwork.IsConnected) {
				_soloPlay = true;
				PhotonNetwork.Disconnect();
			}
			else {
				_soloPlay = true;
				OnDisconnected(DisconnectCause.None);
				//OnDisconnected(DisconnectCause.None);
			}
			AudioManager.Instance.Play("loading1");
			//else StartCoroutine(WaitAndRestartSingle());
		}

		IEnumerator WaitAndRestartSingle() {
			yield return new WaitForSeconds(0.5f);
			StartSinglePlayer();
		}
		public void ExitGame() {
			Application.Quit();
		}

		// now unused, as the save will only be used for checkpoints
		//public void SetContinue() => Global.GlobalVars.Continue = true;
		//public void NewGame() => Global.GlobalVars.Continue = false;

		public void LoadLobby() {
			SceneManager.LoadScene(GlobalVars.LobbySceneName);
		}

		public override void OnDisconnected(DisconnectCause cause) {
			if (_soloPlay) {
				PhotonNetwork.OfflineMode = true;
				PhotonNetwork.CreateRoom("soloGaming" /*DateTime.UtcNow.ToBinary().ToString()*/);
				PhotonNetwork.NickName = Environment.UserName;
				PhotonNetwork.GameVersion = GlobalVars.GameVersion;
				GlobalVars.PlayerList = new List<Player.Player>();
				GlobalVars.TimeStartedAt = DateTime.UtcNow;
				PhotonNetwork.LoadLevel(GlobalVars.LevelsName[0]);
			}
		}
	}
}