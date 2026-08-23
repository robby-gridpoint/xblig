using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public static class Definitions
{
	public enum GameMode
	{
		none,
		brawler,
		redbaron,
		angrybirds,
		runner,
		driver,
		swimmer,
		cutsceneORqte,
		helicopter,
		flappy,
		flappychase,
		zelda,
		shooter,
		space,
		gunsmoke
	}

	public enum AnalogDirection
	{
		up,
		down,
		left,
		right,
		none
	}

	public static class Options
	{
		private static float DefaultMasterVolume = 0.8f;

		private static float DefaultMusicVolume = 0.8f;

		private static float DefaultSoundsVolume = 1f;

		private static bool DefaultBloodOnOff = true;

		private static bool DefaultVibrationsOnOff = true;

		private static int DefaultDifficulty = 3;

		private static int DefaultTitleSafePercent = 10;

		private static bool DefaultPlayGameMusic = true;

		public static float masterVolume = DefaultMasterVolume;

		public static float MusicVolume = DefaultMusicVolume;

		public static float SoundsVolume = DefaultSoundsVolume;

		public static float MasterVolumeAdjustment = 0f;

		public static int TitleSafePercent = DefaultTitleSafePercent;

		public static bool BloodOnOff = DefaultBloodOnOff;

		public static bool VibrationsOnOff = DefaultVibrationsOnOff;

		public static bool MercyOnOff = false;

		public static int Difficulty = DefaultDifficulty;

		public static bool PlayGameMusic = DefaultPlayGameMusic;

		public static bool RegisteredForBonusContent = false;

		public static DateTime RegistryWhatsNewLastChecked = DateTime.MinValue;

		public static string RegistryKey = "";

		public static bool CheckWhatsNewAlot = false;

		public static bool isLoaded = false;

		public static float MasterVolume
		{
			get
			{
				if (masterVolume == 0f)
				{
					return 0f;
				}
				if (masterVolume + MasterVolumeAdjustment > 1f)
				{
					return 1f;
				}
				return masterVolume + MasterVolumeAdjustment;
			}
		}

		public static void Save()
		{
			string fileData = $"volume={masterVolume.ToString()};music={MusicVolume.ToString()};sounds={SoundsVolume.ToString()};blood={BloodOnOff.ToString()};vibe={VibrationsOnOff.ToString()};mercy={MercyOnOff.ToString()};titlesafe={TitleSafePercent.ToString()};registrykey={RegistryKey};difficulty={Difficulty.ToString()}";
			FileManager.BeSaved2("settings.txt", fileData, FileMode.CreateNew);
			SoundManager.UpdateVolumes();
		}

		public static void Load()
		{
			FileManager.delegateLoadFileCallBack delegated = Load;
			FileManager.ReadToMe("settings.txt", delegated);
		}

		public static void Load(string text)
		{
			if (text == null || text == "")
			{
				isLoaded = true;
				return;
			}
			string[] array = text.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					string[] array2 = array[i].Split('=');
					switch (array2[0])
					{
					case "volume":
						masterVolume = float.Parse(array2[1].Replace(",", "."));
						break;
					case "music":
						MusicVolume = float.Parse(array2[1].Replace(",", "."));
						break;
					case "sounds":
						SoundsVolume = float.Parse(array2[1].Replace(",", "."));
						break;
					case "blood":
						BloodOnOff = bool.Parse(array2[1]);
						break;
					case "vibe":
						VibrationsOnOff = bool.Parse(array2[1]);
						break;
					case "mercy":
						MercyOnOff = bool.Parse(array2[1]);
						break;
					case "difficulty":
						Difficulty = int.Parse(array2[i]);
						break;
					case "titlesafe":
						TitleSafePercent = int.Parse(array2[1]);
						setTitleSafe();
						break;
					case "registrykey":
						RegistryKey = array2[1];
						RegisteredForBonusContent = ValidateKey();
						break;
					}
				}
				catch (Exception)
				{
				}
			}
			isLoaded = true;
		}

		public static bool ValidateKey()
		{
			foreach (FighterObject humanPlayer in FighterManager.humanPlayers)
			{
				string text = GenerateXbox360Key(humanPlayer.PROPERTIES.GamerTag);
				if (RegistryKey == text)
				{
					return true;
				}
			}
			return false;
		}

		public static string GenerateXbox360Key(string gamerTag)
		{
			gamerTag = gamerTag.ToUpper().Replace(" ", "");
			string text = gamerTag + "FlappyMonkey" + gamerTag;
			string text2 = "";
			long num = 7L;
			for (int i = 0; i < text.Length; i++)
			{
				num += (int)text[i];
			}
			for (int i = 0; i < text.Length; i++)
			{
				num += (int)text[i];
				text2 += $"{num:x2}";
			}
			text2 = text2.ToUpper();
			text2 = text2.Replace("1", "").Replace("5", "").Replace("S", "")
				.Replace("7", "")
				.Replace("0", "")
				.Replace("I", "")
				.Replace("O", "")
				.Replace("D", "");
			return text2.Substring(0, 7);
		}

		public static void UpdateRegistrationKey(string key)
		{
			if (key != null && !(key == ""))
			{
				RegistryKey = key;
				RegisteredForBonusContent = ValidateKey();
				CheckWhatsNewAlot = false;
				Save();
			}
		}

		public static void ReValidateKey()
		{
			if (RegistryKey != null && !(RegistryKey == ""))
			{
				RegisteredForBonusContent = ValidateKey();
			}
		}

		public static string toggleTitleSafe()
		{
			TitleSafePercent += 5;
			if (TitleSafePercent > 10)
			{
				TitleSafePercent = 0;
			}
			setTitleSafe();
			if (TitleSafePercent == 0)
			{
				return "none";
			}
			return TitleSafePercent + "%";
		}

		private static void setTitleSafe()
		{
			GraphicsManager.TitleSafeTopLeft.X = 20 * TitleSafePercent;
			GraphicsManager.TitleSafeTopLeft.Y = 11 * TitleSafePercent;
		}

		public static string toggleVolume()
		{
			if (masterVolume == DefaultMasterVolume)
			{
				masterVolume = DefaultMasterVolume / 2f;
			}
			else if (MasterVolume < DefaultMasterVolume && masterVolume != 0f)
			{
				masterVolume = 0f;
			}
			else
			{
				masterVolume = DefaultMasterVolume;
			}
			return getWording(MasterVolume);
		}

		public static string toggleMusic()
		{
			if (MusicVolume == DefaultMusicVolume)
			{
				MusicVolume = DefaultMusicVolume / 2f;
			}
			else if (MusicVolume < DefaultMusicVolume && MusicVolume != 0f)
			{
				MusicVolume = 0f;
			}
			else
			{
				MusicVolume = DefaultMusicVolume;
			}
			return getWording(MusicVolume);
		}

		public static string toggleSounds()
		{
			if (SoundsVolume == DefaultSoundsVolume)
			{
				SoundsVolume = DefaultSoundsVolume / 2f - 0.001f;
			}
			else if (SoundsVolume < DefaultSoundsVolume && SoundsVolume != 0f)
			{
				SoundsVolume = 0f;
			}
			else
			{
				SoundsVolume = DefaultSoundsVolume;
			}
			return getWording(SoundsVolume);
		}

		public static string toggleVibrations()
		{
			VibrationsOnOff = !VibrationsOnOff;
			return getWording(VibrationsOnOff);
		}

		public static string toggleBlood()
		{
			BloodOnOff = !BloodOnOff;
			return getWording(BloodOnOff);
		}

		public static string toggleMercy()
		{
			Difficulty++;
			if (Difficulty > 3)
			{
				Difficulty = 1;
			}
			SetDifficultyModeSettings();
			return getDifficultyWording(Difficulty);
		}

		public static void SetDifficultyModeSettings()
		{
			if (Difficulty <= 1)
			{
				HumanCounterResponseWindowMS = HumanCounterResponseWindowMSEasy;
				HumanCounterDurationMS = HumanCounterDurationMSEasy;
				List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
				for (int i = 0; i < humanPlayers.Count; i++)
				{
					humanPlayers[i].PROPERTIES.healthMax = HumanHealthEasy;
				}
				CPUAttackSpeed = 1f;
			}
			else if (Difficulty == 2)
			{
				HumanCounterResponseWindowMS = HumanCounterResponseWindowMSNormal;
				HumanCounterDurationMS = HumanCounterDurationMSNormal;
				List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
				for (int i = 0; i < humanPlayers.Count; i++)
				{
					humanPlayers[i].PROPERTIES.healthMax = HumanHealth;
				}
				CPUAttackSpeed = 0.9f;
			}
			else
			{
				HumanCounterResponseWindowMS = HumanCounterResponseWindowMSHarder;
				HumanCounterDurationMS = HumanCounterDurationMSHarder;
				List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
				for (int i = 0; i < humanPlayers.Count; i++)
				{
					humanPlayers[i].PROPERTIES.healthMax = HumanHealth;
				}
				CPUAttackSpeed = 0.7f;
			}
		}

		public static string getWording(float f)
		{
			if (f == 0f)
			{
				return "OFF";
			}
			if (f < 0.5f)
			{
				return "LOW";
			}
			return "ON";
		}

		public static string getWording(bool b)
		{
			if (b)
			{
				return "ON";
			}
			return "OFF";
		}

		public static string getDifficultyWording(int i)
		{
			if (i <= 1)
			{
				return "EASY";
			}
			if (i == 2)
			{
				return "NORMAL";
			}
			return "SLIGHTLY HARDER";
		}
	}

	public enum FighterSpecialMoves
	{
		nulll,
		X,
		Y,
		swing,
		chop,
		rangedArrow,
		Hadouken
	}

	public enum facing
	{
		left,
		right
	}

	public static Rectangle EmptyRectangle = new Rectangle(0, 0, 0, 0);

	public static string InfoTextCopyright = "BabeWatch v1.0.0, © 2014 Awesome Enterprises";

	public static string InfoTextSupport = "support@awesome-enterprises.com";

	public static Rectangle rectBackButton = new Rectangle(400, 900, 50, 20);

	public static int MaximumWaitSaveSeconds = 5;

	public static string ContentRootDirectory = "FlappyContent";

	public static string MarketplaceAppCode = "";

	public static string URLtoRegister = "minotaur.bz/register360.aspx";

	public static string CONTENT_DIRECTORY_FOR_LEVEL_EDITOR = "F:\\CraigWorking\\Red Baron Game\\RedBaron360Content\\";

	public static int UpdatesPerSecond = 60;

	public static int NetworkUpdatesPerSecond = 15;

	public static float BloodSplatterSize = 1f;

	public static int SkullSlingshotSize = 50;

	public static float SkullSlingshotScale = 1f;

	public static int HPJessica = 100;

	public static int MoveSpeedSharkySHark = 750;

	public static int MoveSpeedHuman = 300;

	public static int MoveSpeedHumanBobcat = 180;

	public static float HPSlingshotFodder = 6f;

	public static float CPUAttackSpeed = 1f;

	public static int quickPunchDamage = 4;

	public static int slowPunchDamage = 10;

	public static int damageFromShark = 10;

	public static int HPSharkyShark = 1;

	public static int damageFromAlligator = 10;

	public static int HPAlligator = 2;

	public static int MoveSpeedAlligator = 300;

	public static int counterMoveDamage = 30;

	public static int SkullSlingMaxRange = 150;

	public static int DefaultJumpMaxAmount = 500;

	public static int DefaultJumpMaxAmountSecondTime = 1000;

	public static int DefaultJumpUpSpeed = 1500;

	public static int DefaultJumpFallSpeed = 1500;

	public static int DefaultJumpUnderwaterMaxAmount = 100;

	public static int DefaultJumpUnderwaterMaxAmountSecondTime = 100;

	public static int DefaultJumpUnderwaterUpSpeed = 500;

	public static int DefaultJumpUnderwaterFallSpeed = 200;

	public static int DamageToCPUonCollision = 0;

	public static int DamageToHumanOnCollision = 0;

	public static int MoveSpeedUFO = 500;

	public static int BulletDamageUFO = 1;

	public static int BombDamageUFO = 2;

	public static int HPUFO = 100;

	public static bool isDoubleJumpEnabled = false;

	public static float HumanRangedMaxShotsPerSecond = 1f;

	public static int ObstaclePixelsToFallPerFrame = 50;

	public static int ObstacleFallDamageAfterLanding = 30;

	public static int ObstacleMaxThrowingDistance = 150;

	public static int ObstacleGroundSmackDamage = 1000;

	public static int ObstacleSecondsToShowB = 2;

	public static int SpeedOfHadouken = 1;

	public static int GravityFallSpeed = 600;

	public static float HumanOnHumanBlockDamageLeakage = 0.35f;

	public static int HumanCounterResponseWindowMS = 7500;

	public static int HumanCounterResponseWindowMSHarder = 400;

	public static int HumanCounterResponseWindowMSNormal = 750;

	public static int HumanCounterResponseWindowMSEasy = 1500;

	public static int HumanCounterDurationMS = 1000;

	public static int HumanCounterDurationMSHarder = 750;

	public static int HumanCounterDurationMSNormal = 900;

	public static int HumanCounterDurationMSEasy = 1000;

	public static int HumanKickDurationMS = 500;

	public static int HumanCrouchDurationMS = 500;

	public static int HumanPushDistance = 350;

	public static Rectangle ScreenMaxRect = new Rectangle(-5000, -1000, 500000, 3000);

	public static int HumanHealth = 100;

	public static int HumanHealthEasy = 200;

	public static float ControllerRightThumbstickMax = 0.8f;

	public static float ControllerRightThumbstickMin = 0.1f;

	public static float ControllerLeftThumbstickMax = ControllerRightThumbstickMax;

	public static float ControllerLeftThumbstickMin = ControllerRightThumbstickMin;

	public static float LayerDepthForSky = 0.001f;

	public static float LayerDepthForGround = 0.01f;

	public static float LayerDepthForBlood = 0.011f;

	public static float LayerDepthFifthHighest = 0.996f;

	public static float LayerDepthFourthHighest = 0.997f;

	public static float LayerDepthThirdHighest = 0.998f;

	public static float LayerDepthSecondHighest = 0.999f;

	public static float LayerDepthTop = 1f;

	public static Combo[] CombosList = new Combo[0];

	public static bool IsContentPermanent(string path)
	{
		if (path.ToLower().StartsWith("cutscenes/") || path.ToLower().StartsWith("screens/unpermanent/") || path.ToLower().StartsWith("fighters/") || path.ToLower().StartsWith("scenery/"))
		{
			return false;
		}
		return true;
	}
}
