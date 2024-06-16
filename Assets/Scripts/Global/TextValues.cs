using System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;

namespace Global {
	public class TextValues {
		// we will put here all values used for the dialogues.
		// this will allow us to easily locate text and use multilingual values, etc
		public static readonly Dictionary<string, string[]> DialogsDict = new Dictionary<string, string[]>() {
			{ "tuto1", new[] { "Welcome to Zelion ! Please use the arrow keys or WASD to move, and F to close this dialog and interact with elements.", "Bienvenue dans Zélion ! Utilise les flèches ou les touches ZQSD pour te déplacer, et la touche F pour fermer ce dialogue et interragir avec le décor.", "ZELIONへようこそ！矢印やWASDを使って動いて、Fキーを使ってこのダイアログを閉じて、人々と話せる。" } }
			, { "lore1", new[] { "6 Years ago, the Eclipse stole the sun and plunged this world into darkness. Everyone has put their hopes on you. C'mon, what are you waiting for!", "Il y a 6 ans, Eclipse a volé le soleil, et plongé notre monde dans la pénombre. Tout le monde crois en toi, pars, noble héros!", "６年前、エクリプスは太陽を奪え、この世界を深い闇にくらま。皆が君のことを信じているよ！さぁ、行きなさい！" } }
			, { "lore2", new[] { "Is that you, the hero who will bring back light to Masteria, our country?", "Est-ce donc toi, le héros qui va rétablir la lumière sur Masteria, notre pays ?", "君が勇者か？お願い、マステリア、僕らの国に、光を戻させてくれ！" } }
			, { "lore3", new[] { "Don't you should have a weapon ? Please talk to the blue guy on the store", "Ne serais tu pas mieux avec une arme ? Va parler à la personne au manteau bleu derrière le comptoir.", "ねぇ、武器を持った方が良くない？店員の青いやつに話してみてください。" } }
			, { "lore4", new[] { "Be wary, the dark forest is full of monsters !", "Sois prudent, la forêt tenebreuse est remplie de monstres !", "暗闇の森はモンスターにあふれている。気を付けてください！" } }
			, { "giveSword", new[] { "O, great hero, please take this sword along your quest. Please find the Zelion, beat Eclipses and bring back the light to these lands.", "Ô grand héros, prend cette épée avec toi au long de ta quête. Trouve le ZELION, et bat Éclipse afin de rétablir la lumière sur ces terres.", "勇者よ、この剣を取ってください。ZELIONを見つけ、エクリプスを倒してください。" } }
			, { "rmq1", new[] { "It seems that when playing in multiplayer, when one player goes to another zone, all the other players are teleported there ! At least that's what I heard...", "Il parait que lorsque l'on joue en multijoueur, quand un joueur change de zone, l'autre joueur se retrouve emporté avec lui ! Enfin... c'est ce que j'ai entendu.", "他のプレイヤーが次のエリアに行くと、君もそこに飛ばされるらしいぜ！聞いた話によると、ね..." } }
			, {"chest canvas" , new[] { "Content of Chest " , "Contenue du Coffre" , "宝箱の内容"}}
			, {"Empty_Chest" , new[] { "It looks empty." , "Il semble vide." , "空っぽに見える"}}
			, {"mine_south" , new[] { "Flee the south of the mine, there are strange noises and a horrible creature is rampaging..." , "Fuis le sud de la mine, il y a des bruits bizarres et une horrible créature se déchaîne..." , "鉱山の南から逃げて、奇妙な音が聞こえて、恐ろしい生き物が暴れている..."}}
			, {"Locked_chest" , new[] { "The chest is locked." , "le coffre est verouillé." , "鍵がかかっています"}}
			, {"Dialog_NPC_Red" , new[] { "Leaving already ?" , "Tu pars déjà?" , "もう行くのですか？"}}
			//,  {"Dialog_NPC_Green" , new[] { "Don't get lost in the dark, only God knows what's in there." , "Ne te perds pas dans l'obscurité, seul Dieu sait ce qu'il y a à l'intérieur." , "暗闇で迷わないで、そこに何があるかは神のみぞ知る。"}}
			, {"Dialog_NPC_Blue" , new[] { "The light of God will guide you out of this darkness, have faith in him." , "La lumière de Dieu vous guidera hors de cette obscurité, ayez foi en lui." , "神の光があなたをこの暗闇から導いてくれるでしょう。神を信じてください。"}}
			, {"Dialog_NPC_Green" , new[] { "If you ever find a bow, you can aim by maintaining the left click pressed, and shoot by and release the left click." , "Si jamais tu trouve un arc, tu peux viser avec en maintenant le clic droit, et tirer en le relachant." , "弓を見つかったら、左クリックを推して狙って、右クリックを放して射る事が出来る。"}}
			, {"giveBow" , new[] { "There is an old bow in the rusty chest, could be useful." , "Il y a un vieux arc dans le coffre rouillé, il pourrait être utile." , "錆びた箱の中に古い弓がある、役に立つかもしれない"}}
			, {"swordTutorial",new []{"You can wield a sword by pressing the SPACE button !","Tu peux utiliser l'épée en appuyant sur la barre ESPACE !","スペースキーを使って、剣を使うことが出来る。"}}
			, {"forestAhead",new []{"On your right, the Dark Forest.","Tu peux utiliser l'épée en appuyant sur la barre ESPACE !","スペースキーを使って、剣を使うことが出来る。"}}
		};
		public static string GameOverText(string time) {
			if (GlobalVars.Language == 0) return $"Time elapsed {time}.\nNumber of deaths : {GlobalVars.DeathCount}\nNumber of Game Over : {GlobalVars.GameOverCount}\nNumber of players : {GlobalVars.NbrOfPlayers}";
			else if (GlobalVars.Language == 1) return $"Temps écoulé: {time}.\nNombre de morts: {GlobalVars.DeathCount}\nNombre de Game Over : {GlobalVars.GameOverCount}\nNombre de joueurs: {GlobalVars.NbrOfPlayers}";
			else if (GlobalVars.Language == 2) return $"経過した時間：{time}.\n死の回数：{GlobalVars.DeathCount}\nゲームオーバ回数{GlobalVars.GameOverCount}\nプレイヤー数{GlobalVars.NbrOfPlayers}";
			else throw new ArgumentException("invalid language value");
		}
		public static string GameClearText(string time) {
			if (GlobalVars.Language == 0) return $"You took {time} to clear the game.\nNumber of deaths : {GlobalVars.DeathCount}\nNumber of Game Over : {GlobalVars.GameOverCount}\nNumber of players : {GlobalVars.NbrOfPlayers}";
			else if (GlobalVars.Language == 1) return $"Vous avez fini le jeu en : {time}.\nNombre de morts: {GlobalVars.DeathCount}\nNombre de Game Over : {GlobalVars.GameOverCount}\nNombre de joueurs: {GlobalVars.NbrOfPlayers}";
			else if (GlobalVars.Language == 2) return $"経過した時間：{time}.\n死の回数：{GlobalVars.DeathCount}\nゲームオーバ回数{GlobalVars.GameOverCount}\nプレイヤー数{GlobalVars.NbrOfPlayers}";
			else throw new ArgumentException("invalid language value");
		}
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
				else if (GlobalVars.Language == 2) return "主音量";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Music {
			get {
				if (GlobalVars.Language == 0) return "Music";
				else if (GlobalVars.Language == 1) return "Musique";
				else if (GlobalVars.Language == 2) return "音楽";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Sfx {
			get {
				if (GlobalVars.Language == 0) return "Sfx";
				else if (GlobalVars.Language == 1) return "Effets";
				else if (GlobalVars.Language == 2) return "効果音";
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

		public static string Revived {
			get {
				if (GlobalVars.Language == 0) return "Revived!";
				else if (GlobalVars.Language == 1) return "Réanimé!";
				else if (GlobalVars.Language == 2) return "生き返えた！";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string YouDied {
			get {
				if (GlobalVars.Language == 0) return "You Died";
				else if (GlobalVars.Language == 1) return "Vous êtes mort.";
				else if (GlobalVars.Language == 2) return "死んでしまった…";
				else throw new ArgumentException("invalid language value");
			}
		}

		public static string Hour {
			get {
				if (GlobalVars.Language == 0) return "hour";
				else if (GlobalVars.Language == 1) return "heure";
				else if (GlobalVars.Language == 2) return "時";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Minute {
			get {
				if (GlobalVars.Language == 0) return "minute";
				else if (GlobalVars.Language == 1) return "minute";
				else if (GlobalVars.Language == 2) return "分";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string Second {
			get {
				if (GlobalVars.Language == 0) return "second";
				else if (GlobalVars.Language == 1) return "seconde";
				else if (GlobalVars.Language == 2) return "秒";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string And {
			get {
				if (GlobalVars.Language == 0) return "and";
				else if (GlobalVars.Language == 1) return "et";
				else if (GlobalVars.Language == 2) return "と";
				else throw new ArgumentException("invalid language value");
			}
		}

		public static string Congratulations {
			get {
				if (GlobalVars.Language == 0) return "Congratulation";
				else if (GlobalVars.Language == 1) return "Félicitations";
				else if (GlobalVars.Language == 2) return "おめでとう！";
				else throw new ArgumentException("invalid language value");
			}
		}
		public static string GameOver {
			get {
				if (GlobalVars.Language == 0) return "Game Over";
				else if (GlobalVars.Language == 1) return "Game Over";
				else if (GlobalVars.Language == 2) return "ゲームオーバー";
				else throw new ArgumentException("invalid language value");
			}
		}
	}
}