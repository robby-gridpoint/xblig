using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public class TriggerObject
{
	public FighterObject triggeredBy = null;

	public TriggerManager.TriggerType type = TriggerManager.TriggerType.custom;

	public Rectangle rectZone = Definitions.EmptyRectangle;

	public DateTime activationTime = DateTime.MaxValue;

	public int ID = -1;

	public string uniqueName = "";

	public double runXSecondssAfterEvent = 0.0;

	public string runAfterEventNamed = "";

	public string activateEnemyNamed = "";

	public string activateSceneryNamed = "";

	public string activateObstacleNamed = "";

	public string activateWaveNamed = "";

	public string setEventNamed = "";

	public string activateSound = "";

	public string activateMusic = "";

	public string cutSceneName = "";

	public int cutSceneDurationInMS = 3;

	public QuickTimeEventsManager.QTEButtons QTEButton = QuickTimeEventsManager.QTEButtons.nulll;

	public string[] fightersToActivate;

	private int[] fighterIDsToActivate;

	private delegateCallbackFunction callbackFunction;

	public bool isActive = true;

	public int X
	{
		get
		{
			return rectZone.X;
		}
		set
		{
			rectZone.X = value;
		}
	}

	public int Y
	{
		get
		{
			return rectZone.Y;
		}
		set
		{
			rectZone.Y = value;
		}
	}

	public int width
	{
		get
		{
			return rectZone.Width;
		}
		set
		{
			rectZone.Width = value;
		}
	}

	public int height
	{
		get
		{
			return rectZone.Height;
		}
		set
		{
			rectZone.Height = value;
		}
	}

	public TriggerObject()
	{
	}

	public TriggerObject(Rectangle rect, int[] FighterIDsToActivate)
	{
		rect = rect;
		fighterIDsToActivate = FighterIDsToActivate;
	}

	public TriggerObject(Rectangle rect, delegateCallbackFunction CallbackFunction)
	{
		rect = rect;
		callbackFunction = CallbackFunction;
	}

	public TriggerObject(DateTime ActivationTime, int[] FighterIDsToActivate)
	{
		activationTime = ActivationTime;
		fighterIDsToActivate = FighterIDsToActivate;
	}

	public TriggerObject(DateTime ActivationTime, delegateCallbackFunction CallbackFunction)
	{
		activationTime = ActivationTime;
		callbackFunction = CallbackFunction;
	}

	public void onTrigger()
	{
		List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: false);
		if (humanPlayers.Count > 0)
		{
			onTrigger(humanPlayers[0].X, humanPlayers[0].Y, isRemotelyTriggered: false);
		}
	}

	public void onTrigger(int x, int y, bool isRemotelyTriggered)
	{
		if (!isActive)
		{
			return;
		}
		isActive = false;
		if (callbackFunction != null)
		{
			callbackFunction();
		}
		if (fighterIDsToActivate != null)
		{
			for (int i = 0; i < fighterIDsToActivate.Length; i++)
			{
				FighterManager.computerPlayers[i].PROPERTIES.isAlive = true;
			}
		}
		if (type != TriggerManager.TriggerType.custom)
		{
			switch (type)
			{
			case TriggerManager.TriggerType.ExitLevel:
				FighterManager.StopTimers();
				if (!RandomStaticGlobals.isPvPEnabled)
				{
					FileManager.SaveProgress(LevelManager.currentLevel.ToString());
					if (!RandomStaticGlobals.GameProgress.ContainsKey(LevelManager.currentLevel.ToString()))
					{
						RandomStaticGlobals.GameProgress.Add(LevelManager.currentLevel.ToString(), LevelManager.currentLevel.ToString());
					}
				}
				FileManager.SaveHighScores();
				SoundManager.StopMusic();
				if (cutSceneName != null && cutSceneName != "")
				{
					GraphicsManager.ShowCutSceneBtoContinue(cutSceneName, 0, isSkippable: false);
				}
				RandomStaticGlobals.currentlySelectedLevel++;
				ScreenManager.ShowMainMenu();
				break;
			case TriggerManager.TriggerType.LetterboxOff:
				GraphicsManager.IsInLetterBox = false;
				break;
			case TriggerManager.TriggerType.LetterboxOn:
				GraphicsManager.letterBox(DateTime.MaxValue);
				break;
			case TriggerManager.TriggerType.StopMusic:
				SoundManager.StopMusic();
				break;
			case TriggerManager.TriggerType.PauseMusic:
				SoundManager.PauseMusic();
				break;
			case TriggerManager.TriggerType.CheckForAwardment:
				AwardmentsManager.CheckForAwardments();
				break;
			case TriggerManager.TriggerType.VolumeRaise20p:
				RandomStaticGlobals.ChangeVolumeByPercent(20);
				break;
			case TriggerManager.TriggerType.VolumeLower20p:
				RandomStaticGlobals.ChangeVolumeByPercent(20);
				break;
			case TriggerManager.TriggerType.GhostEnemies:
				GraphicsManager.isDrawingEnemiesAsGhosts = true;
				break;
			case TriggerManager.TriggerType.ShowOverlay:
				GraphicsManager.ToggleOverlay(uniqueName);
				break;
			case TriggerManager.TriggerType.CutScene:
				GraphicsManager.ShowCutScene(cutSceneName, cutSceneDurationInMS, isSkippable: true);
				if (runAfterEventNamed.Contains("SUCCESS") || runAfterEventNamed.Contains("FAILED"))
				{
					isActive = true;
				}
				break;
			case TriggerManager.TriggerType.CutSceneUNSKIPPABLE:
				GraphicsManager.ShowCutScene(cutSceneName, cutSceneDurationInMS, isSkippable: false);
				if (runAfterEventNamed.Contains("SUCCESS") || runAfterEventNamed.Contains("FAILED"))
				{
					isActive = true;
				}
				break;
			case TriggerManager.TriggerType.QuickTimeEvent:
				QuickTimeEventsManager.AddQTE(uniqueName, cutSceneName, cutSceneDurationInMS, QTEButton);
				break;
			case TriggerManager.TriggerType.IfCoOp:
				if (FighterManager.humanPlayers.Count <= 1)
				{
					return;
				}
				break;
			case TriggerManager.TriggerType.IfHP80Plus:
			{
				FighterObject healthiestPlayer2 = FighterManager.GetHealthiestPlayer();
				if (isRemotelyTriggered || healthiestPlayer2 == null || healthiestPlayer2.PROPERTIES.healthPercentage < 0.8f || Definitions.Options.Difficulty <= 1)
				{
					return;
				}
				break;
			}
			case TriggerManager.TriggerType.IfHP50Plus:
			{
				FighterObject healthiestPlayer = FighterManager.GetHealthiestPlayer();
				if (isRemotelyTriggered || healthiestPlayer == null || healthiestPlayer.PROPERTIES.healthPercentage < 0.5f || Definitions.Options.Difficulty <= 1)
				{
					return;
				}
				break;
			}
			case TriggerManager.TriggerType.HPBoostToMax:
				FighterManager.BoostAllHumansHealth();
				break;
			case TriggerManager.TriggerType.HPLowerBy50p:
				FighterManager.AdjustAllHumanHealth(-0.5f);
				break;
			case TriggerManager.TriggerType.Death:
				FighterManager.AdjustAllHumanHealth(-10f);
				break;
			case TriggerManager.TriggerType.HighScoreAdd1:
				RandomStaticGlobals.ScoreCurrent++;
				isActive = true;
				break;
			case TriggerManager.TriggerType.HighScoreAdd10:
				RandomStaticGlobals.ScoreCurrent += 10;
				isActive = true;
				break;
			case TriggerManager.TriggerType.HighScoreAdd100:
				RandomStaticGlobals.ScoreCurrent += 100;
				isActive = true;
				break;
			case TriggerManager.TriggerType.IncreaseDifficulty:
				RandomStaticGlobals.BoostFlappySpeed();
				FighterManager.AdjustEveryonesRollSpeed(100.0, null, null);
				break;
			}
		}
		if (activateSound != "")
		{
			SoundManager.PlaySound(activateSound);
		}
		if (activateMusic != "")
		{
			SoundManager.PlayMusic(activateMusic, IsRepeating: true);
		}
		if (activateSceneryNamed != "")
		{
			SceneryManager.ActivateNamedObject(activateSceneryNamed);
		}
		if (activateEnemyNamed != "")
		{
			FighterManager.ActivateNamedObject(activateEnemyNamed);
		}
		if (activateObstacleNamed != "")
		{
			ObstacleManager.ActivateNamedObject(activateObstacleNamed, toggleOnOff: true);
		}
		if (setEventNamed != "")
		{
			TriggerManager.SetTriggerEvent(setEventNamed);
		}
		if (activateWaveNamed != "")
		{
			WaveManager.LoadWaves(activateWaveNamed, x, y);
		}
		if (!isRemotelyTriggered)
		{
			TriggerManager.BroadcastTriggerTriggered(ID, x, y);
		}
	}

	public TriggerObject Copy()
	{
		return (TriggerObject)MemberwiseClone();
	}
}
