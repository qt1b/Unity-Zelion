using System;
using System.Collections;
using System.Collections.Generic;
using Global;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.Demos.PunBasics_Tutorial.Scripts;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
	public class TitleScreen : MonoBehaviourPunCallbacks {
		// or start ?
		private bool _soloPlay = false;
		public Button PlayButton;
		public Button SettingsButton;
		public Button ExitButton;
		public Button SinglePlayerButton;
		public Button MultiPlayerButton;
		public Button BackButton;

		public void Start() {
			PlayButton.GetComponentInChildren<TextMeshProUGUI>().text = TextValues.Play;
			SettingsButton.GetComponentInChildren<TextMeshProUGUI>().text = TextValues.Settings;
			ExitButton.GetComponentInChildren<TextMeshProUGUI>().text = TextValues.Exit;
			SinglePlayerButton.GetComponentInChildren<TextMeshProUGUI>().text = TextValues.SinglePlayer;
			MultiPlayerButton.GetComponentInChildren<TextMeshProUGUI>().text = TextValues.MultiPlayer;
			BackButton.GetComponentInChildren<TextMeshProUGUI>().text = TextValues.Back;
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
			SceneManager.LoadScene("Scenes/Lobby");
		}

		public override void OnDisconnected(DisconnectCause cause) {
			if (_soloPlay) {
				PhotonNetwork.OfflineMode = true;
				PhotonNetwork.CreateRoom("soloGaming" /*DateTime.UtcNow.ToBinary().ToString()*/);
				PhotonNetwork.NickName = Environment.UserName;
				PhotonNetwork.GameVersion = GlobalVars.GameVersion;
				GlobalVars.PlayerList = new List<Player.Player>();
				GlobalVars.TimeStartedAt = DateTime.UtcNow;
				PhotonNetwork.LoadLevel(GlobalVars.FirstLevelName);
			}
		}
	}
}