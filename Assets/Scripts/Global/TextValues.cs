using System;
using System.Collections.Generic;

namespace Global {
	public class TextValues {
		// we will put here all values used for the dialogues.
		// this will allow us to easily locate text and use multilingual values, etc
		public static Dictionary<string, string[]> DialogsDict = new Dictionary<string, string[]>() {
			{ "dialog1", new[] { "Dialog 1", "Dialogue 1", "会話１" } }
		};
		// Title Screen
		public static string Play {
			get {
				if (GlobalVars.Language == 0) return "Play";
				else if (GlobalVars.Language == 1) return "Jouer";
				else if (GlobalVars.Language == 2) return "始める";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Settings {
			get {
				if (GlobalVars.Language == 0) return "Settings";
				else if (GlobalVars.Language == 1) return "Paramètres";
				else if (GlobalVars.Language == 2) return "設定";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Exit {
			get {
				if (GlobalVars.Language == 0) return "Exit";
				else if (GlobalVars.Language == 1) return "Quitter";
				else if (GlobalVars.Language == 2) return "終了";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string SinglePlayer {
			get {
				if (GlobalVars.Language == 0) return "SinglePlayer";
				else if (GlobalVars.Language == 1) return "Partie Solo";
				else if (GlobalVars.Language == 2) return "一人で";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string MultiPlayer {
			get {
				if (GlobalVars.Language == 0) return "Multiplayer";
				else if (GlobalVars.Language == 1) return "Multijoueur";
				else if (GlobalVars.Language == 2) return "マルチプレイヤ";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Back {
			get {
				if (GlobalVars.Language == 0) return "Back";
				else if (GlobalVars.Language == 1) return "Retour";
				else if (GlobalVars.Language == 2) return "戻る";
				else throw new ArgumentException("invalid language value");
			}
		}
		// Lobby
		public static string JoinLobby {
			get {
				if (GlobalVars.Language == 0) return "Join";
				else if (GlobalVars.Language == 1) return "Rejoindre";
				else if (GlobalVars.Language == 2) return "ルームを参加する";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string CreateLobby {
			get {
				if (GlobalVars.Language == 0) return "Create";
				else if (GlobalVars.Language == 1) return "Créer";
				else if (GlobalVars.Language == 2) return "ルームを作る";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string RoomNameInput {
			get {
				if (GlobalVars.Language == 0) return "Room Name";
				else if (GlobalVars.Language == 1) return "Nom de la salle";
				else if (GlobalVars.Language == 2) return "ルームネーム";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Loading {
			get {
				if (GlobalVars.Language == 0) return "Loading...";
				else if (GlobalVars.Language == 1) return "Chargement...";
				else if (GlobalVars.Language == 2) return "ロード中";
				else throw new ArgumentException("invalid language value");
			}
		}
		// settings text
		public static string Resolution {
			get {
				if (GlobalVars.Language == 0) return "Resolution";
				else if (GlobalVars.Language == 1) return "Résolution";
				else if (GlobalVars.Language == 2) return "解像度";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string FullScreen {
			get {
				if (GlobalVars.Language == 0) return "FullScreen";
				else if (GlobalVars.Language == 1) return "Plein Écran";
				else if (GlobalVars.Language == 2) return "全画面";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Volume {
			get {
				if (GlobalVars.Language == 0) return "Volume";
				else if (GlobalVars.Language == 1) return "Volume";
				else if (GlobalVars.Language == 2) return "音量";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Language {
			get {
				if (GlobalVars.Language == 0) return "Language";
				else if (GlobalVars.Language == 1) return "Langue";
				else if (GlobalVars.Language == 2) return "言語";
				else throw new ArgumentException("invalid language value");
			}
		}
		// Pause Menu
		public static string Paused {
			get {
				if (GlobalVars.Language == 0) return "Paused";
				else if (GlobalVars.Language == 1) return "En Pause";
				else if (GlobalVars.Language == 2) return "ゲーム中断";// not sure for this one
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Resume {
			get {
				if (GlobalVars.Language == 0) return "Resume";
				else if (GlobalVars.Language == 1) return "Continuer";
				else if (GlobalVars.Language == 2) return "ゲーム再会";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string MainMenu {
			get {
				if (GlobalVars.Language == 0) return "Main Menu";
				else if (GlobalVars.Language == 1) return "Menu Principal";
				else if (GlobalVars.Language == 2) return "タイトルに戻る";
				else throw new ArgumentException("invalid language value");
			}
		}
	}
}