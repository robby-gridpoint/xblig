using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BunnyOfWar;

public static class RandomStaticGlobals
{
	public static int FlappySpeedUpDropDown = 0;

	public static int FlappySpeedUpScroll = 0;

	public static float FlappyPoopFuse = 0.45f;

	public static bool isShowingCutScene = false;

	public static bool GameIsStillRunningOnVita = true;

	public static int currentlySelectedLevel = -1;

	public static Vector2 RollVelocity = Vector2.Zero;

	public static Vector2 CameraRollVelocity = Vector2.Zero;

	public static int ScoreCurrent = 0;

	public static int ScoreAllTimeHigh = 0;

	private static bool isTrial = true;

	private static DateTime trialLastChecked = DateTime.MinValue;

	public static bool isDebugPlayActive = false;

	public static Dictionary<string, string> GameProgress = new Dictionary<string, string>();

	public static bool isPvPEnabled = false;

	public static bool isSkullSlingshotMode = false;

	public static Definitions.GameMode GameMode = Definitions.GameMode.none;

	public static Vector2[] SkullSlingshotOrigin = new Vector2[4]
	{
		new Vector2(300f, 650f),
		new Vector2(300f, 500f),
		new Vector2(300f, 350f),
		new Vector2(300f, 800f)
	};

	public static Vector2[] SkullSlingshotCurrentPosition = new Vector2[4]
	{
		new Vector2(300f, 650f),
		new Vector2(300f, 500f),
		new Vector2(300f, 350f),
		new Vector2(300f, 800f)
	};

	public static InputManagerObject InputManagerInstance = new InputManagerObject();

	public static ContentManager Content;

	public static ContentManager ContentTemporary;

	public static int CurrentFrameThisSecond = 1;

	public static DateTime UpdateAfterThisTime = DateTime.MinValue;

	public static DateTime UpdateNetworkAfterThisTime = DateTime.MinValue;

	public static bool isCounteringEnabled = true;

	public static Random RandomAI = new Random();

	public static string HelpTextForLevel = "";

	public static bool isGamePaused = false;

	public static void BoostFlappySpeed()
	{
		FlappySpeedUpScroll += 100;
		FlappySpeedUpDropDown++;
		FlappyPoopFuse -= 0.05f;
		if (FlappyPoopFuse <= 0.1f)
		{
			FlappyPoopFuse = 0.1f;
		}
	}

	public static void ResetFlappySpeed()
	{
		FlappySpeedUpDropDown = 0;
		FlappySpeedUpScroll = 0;
		FlappyPoopFuse = 0.45f;
	}

	public static void ResetAllGameDefaults()
	{
		FighterManager.localXboxPlayerID = 0;
		StopAllControllerVibrationRumbles();
		isShowingCutScene = false;
		NetworkGameplayManager.localPlayerIndex = PlayerIndex.Four;
		NetworkGameplayManager.localGamerTag = "";
		Networking.EndSession();
		Networking.StopGame();
	}

	public static bool IsTrial()
	{
		if (!Guide.SimulateTrialMode && !isTrial && trialLastChecked.AddSeconds(5.0) > DateTime.Now)
		{
			return isTrial;
		}
		isTrial = Guide.IsTrialMode;
		trialLastChecked = DateTime.Now;
		return isTrial;
	}

	public static void BuyMe(PlayerIndex pi)
	{
		if (IsTrial())
		{
			if (Gamer.SignedInGamers[pi] != null && Gamer.SignedInGamers[pi].Privileges.AllowPurchaseContent)
			{
				Guide.ShowMarketplace(pi);
			}
			else
			{
				GraphicsManager.Message("Sorry, this controller doesn't have permission to buy anything. Please try again with a controller that is signed in with an Xbox Live Gold account.");
			}
		}
	}

	public static byte[] StringToAscii(string s)
	{
		byte[] array = new byte[s.Length];
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (c <= '\u007f')
			{
				array[i] = (byte)c;
			}
			else
			{
				array[i] = 63;
			}
		}
		return array;
	}

	public static void enableCombo(Definitions.FighterSpecialMoves attack)
	{
		for (int i = 0; i < Definitions.CombosList.Length; i++)
		{
			if (Definitions.CombosList[i].SpecialMove == attack && !Definitions.CombosList[i].enabled)
			{
				Definitions.CombosList[i].enabled = true;
			}
		}
	}

	public static void StartStopMusic()
	{
		if (MediaPlayer.State != MediaState.Paused)
		{
			SoundManager.PauseMusic();
		}
		else
		{
			SoundManager.ResumeMusic();
		}
	}

	public static bool IsThisControllerInputValid(PlayerIndex pi)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		try
		{
			if (Networking.SessionState.HasValue && (int)Networking.SessionState.Value != 1)
			{
				return true;
			}
			if (!Networking.SessionState.HasValue)
			{
				return true;
			}
		}
		catch (Exception)
		{
			return true;
		}
		if (FighterManager.humanPlayers == null || FighterManager.humanPlayers.Count == 0)
		{
			return true;
		}
		if (isTrial && ScreenManager.isShowingBuyMeScreen)
		{
			return true;
		}
		if (ScreenManager.CurrentScreen == ScreenManager.screens.Blank || ScreenManager.CurrentScreen == ScreenManager.screens.Options || ScreenManager.CurrentScreen == ScreenManager.screens.WorldMap || ScreenManager.CurrentScreen == ScreenManager.screens.PauseMenu)
		{
			foreach (FighterObject humanPlayer in FighterManager.humanPlayers)
			{
				if (humanPlayer.PROPERTIES.isLocal && humanPlayer.PROPERTIES.PlayerIndexControllerNumber.HasValue && humanPlayer.PROPERTIES.PlayerIndexControllerNumber == pi)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void StopAllControllerVibrationRumbles()
	{
		try
		{
			foreach (FighterObject humanPlayer in FighterManager.humanPlayers)
			{
				if (humanPlayer.PROPERTIES.PlayerIndexControllerNumber.HasValue && humanPlayer.PROPERTIES.PlayerIndexControllerNumber.HasValue)
				{
					GamePad.SetVibration(humanPlayer.PROPERTIES.PlayerIndexControllerNumber.Value, 0f, 0f);
				}
				humanPlayer.PROPERTIES.isStunned = false;
				humanPlayer.PROPERTIES.stunExpires = DateTime.MinValue;
			}
		}
		catch (Exception ex)
		{
			string text = ex.ToString();
		}
	}

	public static void pauseButtonPressed(bool broadcastThis)
	{
		InputManager.ClearPreviousInputs();
		StopAllControllerVibrationRumbles();
		if (ScreenManager.isShowingPlayerFailedScreen)
		{
			ScreenManager.HidePlayerFailedScreen();
			return;
		}
		isGamePaused = !isGamePaused;
		if (isGamePaused)
		{
			FighterManager.StopTimers();
			if (MediaPlayer.State == MediaState.Playing)
			{
				SoundManager.PauseMusic();
			}
			ScreenManager.ShowPauseMenu();
			NetworkGameplayManager.SendPlayerStats();
		}
		else
		{
			FighterManager.StartTimers();
			MediaPlayer.Volume = Definitions.Options.MasterVolume * Definitions.Options.MusicVolume;
			if (MediaPlayer.State == MediaState.Paused)
			{
				SoundManager.ResumeMusic();
			}
			ScreenManager.HideHighScoress();
			ScreenManager.HidePlayerFailedScreen();
			ScreenManager.hideNetworkBusy();
			ScreenManager.ShowBlank();
		}
		if (broadcastThis)
		{
			NetworkGameplayManager.SendPauseState();
		}
	}

	public static float makePositive(float f)
	{
		if (f == 0f)
		{
			return f;
		}
		if (f < 0f)
		{
			f *= -1f;
		}
		return f;
	}

	public static float getLayerDepth(int Y, int height)
	{
		float num = (float)(Y + height) / ((float)GraphicsManager.viewableArea.Height * 2f);
		if (num >= 1f)
		{
			return Definitions.LayerDepthFourthHighest;
		}
		if (num < 0f)
		{
			return 0f;
		}
		return num;
	}

	public static void ChangeVolumeByPercent(int percent)
	{
		Definitions.Options.MasterVolumeAdjustment += (float)percent / 200f;
		if (Definitions.Options.MasterVolumeAdjustment < 0.1f)
		{
			Definitions.Options.MasterVolumeAdjustment = 0.1f;
		}
		if (Definitions.Options.MasterVolumeAdjustment > 1f)
		{
			Definitions.Options.MasterVolumeAdjustment = 1f;
		}
		MediaPlayer.Volume = Definitions.Options.MasterVolume * Definitions.Options.MusicVolume;
	}

	public static string GetMemoryUsage()
	{
		long num = 0L;
		if (num == 0)
		{
			return "unknown";
		}
		return num + "MB";
	}

	public static string GetTimeFromSeconds(int seconds)
	{
		return string.Format("{0} : {1,00}", seconds / 60, (seconds % 60).ToString("D2"));
	}

	public static bool DoRectsCollide(Rectangle r1, Rectangle r2)
	{
		if (DoesRectContain(r1, new Vector2(r2.X, r2.Y)))
		{
			return true;
		}
		if (DoesRectContain(r1, new Vector2(r2.X + r2.Width, r2.Y)))
		{
			return true;
		}
		if (DoesRectContain(r1, new Vector2(r2.X, r2.Y + r2.Height)))
		{
			return true;
		}
		if (DoesRectContain(r1, new Vector2(r2.X + r2.Width, r2.Y + r2.Height)))
		{
			return true;
		}
		if (DoesRectContain(r2, new Vector2(r1.X, r1.Y)))
		{
			return true;
		}
		if (DoesRectContain(r2, new Vector2(r1.X + r1.Width, r1.Y)))
		{
			return true;
		}
		if (DoesRectContain(r2, new Vector2(r1.X, r1.Y + r1.Height)))
		{
			return true;
		}
		if (DoesRectContain(r2, new Vector2(r1.X + r1.Width, r1.Y + r1.Height)))
		{
			return true;
		}
		return false;
	}

	public static bool DoesRectContain(Rectangle r, Vector2 v)
	{
		if (r.Contains((int)v.X, (int)v.Y))
		{
			return true;
		}
		return false;
	}

	public static Vector2 GetCPUBaitVector2()
	{
		Vector2 one = Vector2.One;
		if (ObstacleManager.Obstacles == null || ObstacleManager.Obstacles.Count == 0)
		{
			return one;
		}
		for (int i = 0; i < ObstacleManager.Obstacles.Count; i++)
		{
			if (ObstacleManager.Obstacles[i].uniqueName.ToLower() == "bait")
			{
				one.X = ObstacleManager.Obstacles[i].X + (float)(ObstacleManager.Obstacles[i].width / 2);
				one.Y = ObstacleManager.Obstacles[i].Y + (float)(ObstacleManager.Obstacles[i].height / 2);
				return one;
			}
		}
		return one;
	}
}
