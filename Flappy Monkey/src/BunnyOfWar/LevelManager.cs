using System.Collections.Generic;
using System.IO;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BunnyOfWar;

public static class LevelManager
{
	public static int enemiesRemaining = 10;

	public static int maxEnemiesAtOnce = 1;

	public static int bossesRemaining = 1;

	public static int currentLevel = 1;

	public static ContentManager Content;

	public static Rectangle viewportRect;

	public static Rectangle levelBoundaries = new Rectangle(0, 0, 4000, 2000);

	private static List<string> levelNames = new List<string>(100);

	private static List<string> levels = new List<string>(100);

	private static bool isPreloadedAlready = false;

	public static void init(ContentManager ContentX, Rectangle viewportRectX)
	{
		Content = ContentX;
		viewportRect = viewportRectX;
	}

	public static void LoadPvPLevel(int levelNumber)
	{
		LoadLevel("PvP" + levelNumber, isPvP: true);
	}

	public static void LoadPreloadData()
	{
		if (!isPreloadedAlready)
		{
			isPreloadedAlready = true;
			PreloadThread();
		}
	}

	private static void PreloadThread()
	{
		GraphicsManager.LoadTexture("scenery/floaters/cloud1", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud2", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud3", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud4", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud5", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud6", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud7", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud8", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud9", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud10", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud11", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/cloud12", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/tree1", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/tree2", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/tree3", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/tree4", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/tree5", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/tree6", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/tree7", cacheResult: true);
		GraphicsManager.LoadTexture("scenery/floaters/tree8", cacheResult: true);
		FighterObject fighterObject = FighterManager.createNewAlligator(RandomStaticGlobals.Content, new Rectangle(0, 0, 0, 0), 0, 0, isAlive: false, BunnyOfWar.AI.AI.modes.doNothing, 1f);
		fighterObject = FighterManager.createNewSomethingRed(RandomStaticGlobals.Content, new Rectangle(0, 0, 0, 0), 0, 0, isAlive: false, BunnyOfWar.AI.AI.modes.doNothing, 1f);
		FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 3000, 600, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "ChalkBoard", "v100");
		FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 2150, 600, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "Teacher", "v100");
		FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 0, 600, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "Vegetarian1", "v10");
		FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 0, 600, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "Vegetarian2", "v10");
		FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 0, 600, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "Vegetarian3", "v10");
		string level = GetLevel("EasyFlappy");
		SceneryManager.ImportData(level);
		FighterManager.ImportData(level);
		ObstacleManager.ImportData(level);
	}

	public static void LoadLevel(int levelNumber)
	{
		RandomStaticGlobals.isGamePaused = false;
		FileManager.Select360StorageDevice();
		currentLevel = levelNumber;
		LoadLevel(levelNumber.ToString(), isPvP: false);
	}

	public static void LoadLevel(string levelNumber, bool isPvP)
	{
		GraphicsManager.ClearTextureCache();
		RandomStaticGlobals.isPvPEnabled = isPvP;
		ResetLevelDefaults();
		GraphicsManager.IsInLetterBox = false;
		RandomStaticGlobals.isShowingCutScene = false;
		ProjectileManager.Clear();
		GraphicsManager.viewportRect = new Rectangle(0, 0, 1920, 1080);
		GraphicsManager.viewableArea = GraphicsManager.viewportRect;
		NetworkGameplayManager.Load();
		ScreenManager.UpdateLoadingStatus("Loading...");
		string level = GetLevel(levelNumber.ToString());
		ScreenManager.HidePlayerFailedScreen();
		ScreenManager.UpdateLoadingStatus("Loading Scenery");
		SceneryManager.ImportData(level);
		SceneryManager.BloodStainSceneryObjects.Clear();
		ScreenManager.UpdateLoadingStatus("Loading Fighters");
		FighterManager.ImportData(level);
		TriggerManager.ImportData(level);
		ScreenManager.UpdateLoadingStatus("Loading Obstacles");
		ObstacleManager.ImportData(level);
		if (FighterManager.humanPlayers.Count == 0)
		{
			FighterManager.ResetHumanPlayers();
			FighterManager.ResetHumanPlayersXY();
		}
		FighterManager.ResetHumanPlayers();
		CustomsManager.ImportData(level);
		if (FighterManager.humanPlayers[0].PROPERTIES.CustomAnimationName != CustomsManager.GetCustomPlayerAnimation())
		{
			FighterManager.RemakeHumanPlayersForCustoms();
		}
		FighterManager.ResetHumanPlayersXY();
		CustomsManager.ImportData(level);
		ScreenManager.UpdateLoadingStatus("done");
		FighterManager.ClearHighScores();
		TriggerManager.SetTriggerEvent("LevelStart");
		FighterManager.StartTimers();
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.flappy)
		{
			RandomStaticGlobals.enableCombo(Definitions.FighterSpecialMoves.X);
			RandomStaticGlobals.enableCombo(Definitions.FighterSpecialMoves.Y);
		}
		Definitions.Options.SetDifficultyModeSettings();
		if (GraphicsManager.messages != null && GraphicsManager.messages.Count > 0 && !RandomStaticGlobals.isShowingCutScene)
		{
			RandomStaticGlobals.isGamePaused = true;
		}
		else
		{
			RandomStaticGlobals.isGamePaused = false;
		}
		if (isPvP)
		{
			RandomStaticGlobals.isCounteringEnabled = false;
		}
		RandomStaticGlobals.isPvPEnabled = isPvP;
		ScreenManager.ShowBlank();
	}

	public static string GetLevel(string location)
	{
		string text = Definitions.ContentRootDirectory + "/levels/" + location;
		if (!text.EndsWith(".lvl"))
		{
			text += ".lvl";
		}
		if (levelNames.Contains(text))
		{
			return levels[levelNames.IndexOf(text)];
		}
		string text2 = "";
		using (Stream stream = TitleContainer.OpenStream(text))
		{
			StreamReader streamReader = new StreamReader(stream);
			text2 = streamReader.ReadToEnd();
			stream.Close();
		}
		levels.Add(text2);
		levelNames.Add(text);
		return text2;
	}

	public static void ResetLevelDefaults()
	{
		Definitions.Options.MasterVolumeAdjustment = 0f;
		Definitions.BloodSplatterSize = 1f;
		RandomStaticGlobals.isCounteringEnabled = true;
		RandomStaticGlobals.isSkullSlingshotMode = false;
		GraphicsManager.isDrawingEnemiesAsGhosts = false;
		GraphicsManager.ClearOverlays();
		GraphicsManager.ClearMessages();
		RandomStaticGlobals.HelpTextForLevel = "";
		RandomStaticGlobals.GameMode = Definitions.GameMode.brawler;
		RandomStaticGlobals.RollVelocity = Vector2.Zero;
		WaveManager.WaveQueue.Clear();
		TriggerManager.ClearData();
		CustomsManager.importCount = 0;
		RandomStaticGlobals.ScoreCurrent = 0;
	}
}
