using System;

namespace Global {
	public class TextValues {
		// we will put here all values used for the dialogues.
		// this will allow us to easily locate text and use multilingual values, etc
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
				else if (GlobalVars.Language == 1) return "Jeu Seul";
				else if (GlobalVars.Language == 2) return "一人";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string MultiPlayer {
			get {
				if (GlobalVars.Language == 0) return "Multiplayer";
				else if (GlobalVars.Language == 1) return "À Plusieurs";
				else if (GlobalVars.Language == 2) return "マルチプレイ";
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
				if (GlobalVars.Language == 0) return "Join";
				else if (GlobalVars.Language == 1) return "Rejoindre";
				else if (GlobalVars.Language == 2) return "ルームを参加する";
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
	}
}