using System;
using System.Collections.Generic;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public static class CustomsManager
{
	public enum Customizations
	{
		undefined,
		BigBlood,
		DisableCounters,
		SkullSlingshotMode,
		GhostEnemies,
		HelpText,
		Player2IsALog,
		ZeroGravity,
		RedBaron,
		Gigantor,
		Runner,
		YoIisUnderWater,
		Driver,
		CustomPlayerAnimation,
		MoveSpeed,
		CutSceneOrQTE,
		MeteorsEasy,
		MeteorsHard,
		HelicopterMode,
		FlappyMode,
		FlappyPoliceMode,
		FlappyDifficulty,
		ShooterMode,
		SpaceMode,
		GunSmokeMode,
		CameraRoll
	}

	public static int importCount = 0;

	public static Dictionary<Customizations, string> LevelCustomizations = new Dictionary<Customizations, string>();

	public static string GetCustomPlayerAnimation()
	{
		if (LevelCustomizations.ContainsKey(Customizations.CustomPlayerAnimation))
		{
			return LevelCustomizations[Customizations.CustomPlayerAnimation].ToString();
		}
		return "";
	}

	public static bool IsBloodEnabled()
	{
		return false;
	}

	public static bool GetIsCollidableWithCPUs()
	{
		return false;
	}

	public static bool GetIsUnderWater()
	{
		if (LevelCustomizations.ContainsKey(Customizations.YoIisUnderWater))
		{
			return true;
		}
		return false;
	}

	public static Customizations ConvertFromString(string s)
	{
		return (Customizations)Enum.Parse(typeof(Customizations), s, ignoreCase: true);
	}

	public static string ExportData()
	{
		string text = "";
		foreach (Customizations key in LevelCustomizations.Keys)
		{
			text += string.Format("type=customization;name={0};value={1}" + Environment.NewLine, key.ToString(), LevelCustomizations[key]);
		}
		return text;
	}

	public static void ImportData(string data)
	{
		ClearData();
		importCount++;
		string[] array = data.Split(Environment.NewLine.ToCharArray());
		Customizations customizations = Customizations.undefined;
		string value = "";
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].ToString().Trim();
			if (!array[i].StartsWith("type=customization"))
			{
				continue;
			}
			string[] array2 = array[i].Split(';');
			for (int j = 0; j < array2.Length; j++)
			{
				string[] array3 = array2[j].Split('=');
				if (array3[0] == "name")
				{
					customizations = (Customizations)Enum.Parse(typeof(Customizations), array3[1], ignoreCase: true);
				}
				else if (array3[0] == "value")
				{
					value = array3[1];
				}
			}
			try
			{
				if (customizations != Customizations.undefined)
				{
					LevelCustomizations.Add(customizations, value);
				}
			}
			catch (Exception)
			{
			}
		}
		ProcessCustoms();
	}

	private static void ProcessCustoms()
	{
		foreach (Customizations key in LevelCustomizations.Keys)
		{
			switch (key)
			{
			case Customizations.BigBlood:
			{
				string text = LevelCustomizations[key];
				Definitions.BloodSplatterSize = float.Parse(text.Replace(",", "."));
				break;
			}
			case Customizations.DisableCounters:
				RandomStaticGlobals.isCounteringEnabled = false;
				break;
			case Customizations.SkullSlingshotMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.angrybirds;
				RandomStaticGlobals.isSkullSlingshotMode = true;
				break;
			case Customizations.HelpText:
				RandomStaticGlobals.HelpTextForLevel = LevelCustomizations[key];
				break;
			case Customizations.GhostEnemies:
				GraphicsManager.isDrawingEnemiesAsGhosts = true;
				break;
			case Customizations.Gigantor:
			{
				float scale = float.Parse(LevelCustomizations[key].ToString().Replace(",", "."));
				for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
				{
					FighterManager.humanPlayers[i].PROPERTIES.scale = scale;
				}
				break;
			}
			case Customizations.RedBaron:
				RandomStaticGlobals.GameMode = Definitions.GameMode.redbaron;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.Runner:
				RandomStaticGlobals.GameMode = Definitions.GameMode.runner;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetJumpSpeeds(Definitions.GameMode.runner);
				break;
			case Customizations.CutSceneOrQTE:
				RandomStaticGlobals.GameMode = Definitions.GameMode.cutsceneORqte;
				break;
			case Customizations.YoIisUnderWater:
				RandomStaticGlobals.GameMode = Definitions.GameMode.swimmer;
				FighterManager.SetJumpSpeeds(Definitions.GameMode.swimmer);
				break;
			case Customizations.HelicopterMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.helicopter;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.FlappyMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.flappy;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.FlappyPoliceMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.flappychase;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.ShooterMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.shooter;
				FighterManager.SetHumanRandomThings(null, int.Parse(LevelCustomizations[key].ToString().Replace(",", ".")));
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.SpaceMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.space;
				FighterManager.SetHumanRandomThings(null, int.Parse(LevelCustomizations[key].ToString().Replace(",", ".")));
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.GunSmokeMode:
			{
				RandomStaticGlobals.GameMode = Definitions.GameMode.gunsmoke;
				int value = int.Parse(LevelCustomizations[key].ToString().Replace(",", "."));
				FighterManager.SetHumanRandomThings(null, 0.0, value);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			}
			case Customizations.CameraRoll:
			{
				string[] array = LevelCustomizations[key].ToString().Split(',');
				RandomStaticGlobals.CameraRollVelocity = new Vector2(0f, 0f);
				if (array[0] != null)
				{
					RandomStaticGlobals.CameraRollVelocity.X = float.Parse(array[0]);
				}
				if (array[1] != null)
				{
					RandomStaticGlobals.CameraRollVelocity.Y = float.Parse(array[1]);
				}
				break;
			}
			case Customizations.FlappyDifficulty:
			{
				if (importCount > 1)
				{
					return;
				}
				RandomStaticGlobals.ResetFlappySpeed();
				int num3 = 1100000;
				if (RandomStaticGlobals.IsTrial())
				{
					num3 = 120000;
				}
				string[] array2 = LevelCustomizations[key].ToString().Split(',');
				int num4 = 7;
				num4 = ((!(array2[0] != "R")) ? DateTime.Now.Millisecond : int.Parse(array2[0]));
				int num5 = int.Parse(array2[1]);
				int num6 = int.Parse(array2[2]);
				int num7 = 300;
				int num8 = 100;
				int num9 = 3;
				int num10 = 0;
				bool flag = false;
				Random random2 = new Random(num4);
				int maxValue = 12;
				int maxValue2 = 8;
				for (int i = 0; i < num3; i += 10000)
				{
					SceneryObject sceneryObject = new SceneryObject(SceneryManager.FloatingSceneryObjects.Count, "scenery/floaters/", "cloud" + random2.Next(1, maxValue), i + random2.Next(0, 750), -500 + random2.Next(0, 200));
					sceneryObject.AImode = NonFighterAI.modes.ScrollRatioToPlayer;
					sceneryObject.AIAmountSpeed = random2.Next(40, 60);
					SceneryManager.FloatingSceneryObjects.Add(sceneryObject);
				}
				for (int i = 0; i < num3; i += 3000)
				{
					SceneryObject sceneryObject = new SceneryObject(SceneryManager.FloatingSceneryObjects.Count, "scenery/floaters/", "cloud" + random2.Next(1, maxValue), i + random2.Next(0, 100), -100 + random2.Next(0, 200));
					sceneryObject.AImode = NonFighterAI.modes.ScrollRatioToPlayer;
					sceneryObject.AIAmountSpeed = random2.Next(10, 15);
					sceneryObject.width /= 2;
					sceneryObject.height /= 2;
					sceneryObject.Z = Definitions.LayerDepthForSky + 0.0001f;
					SceneryManager.FloatingSceneryObjects.Add(sceneryObject);
				}
				for (int i = 0; i < num3; i += 2000)
				{
					SceneryObject sceneryObject = new SceneryObject(SceneryManager.FloatingSceneryObjects.Count, "scenery/floaters/", "tree" + random2.Next(1, maxValue2), i + random2.Next(0, 750), 500 + random2.Next(0, 200));
					sceneryObject.AImode = NonFighterAI.modes.ScrollRatioToPlayer;
					sceneryObject.AIAmountSpeed = random2.Next(10, 20);
					sceneryObject.Z = Definitions.LayerDepthForGround + 0.002f;
					SceneryManager.FloatingSceneryObjects.Add(sceneryObject);
				}
				for (int i = 0; i < num3; i += 1000)
				{
					SceneryObject sceneryObject = new SceneryObject(SceneryManager.FloatingSceneryObjects.Count, "scenery/floaters/", "tree" + random2.Next(1, maxValue2), i + random2.Next(0, 100), 800 + random2.Next(0, 100));
					sceneryObject.AImode = NonFighterAI.modes.none;
					sceneryObject.Z = Definitions.LayerDepthForGround + 0.001f;
					SceneryManager.FloatingSceneryObjects.Add(sceneryObject);
				}
				FighterManager.AddComputerPlayer(FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 3000, 650, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "ChalkBoard", "v100"));
				FighterManager.AddComputerPlayer(FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 2150, 650, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "Teacher", "v100"));
				bool flag2 = true;
				for (int i = 4000; i < num3; i += num5)
				{
					int y = random2.Next(300, 700);
					int num11 = random2.Next(500, num5 - 500);
					if (i % 120000 < 80000)
					{
						ObstacleManager.AddCrocogator(i, y - num6 / 2 - 900, 250, 900, flippedH: false, flippedV: false);
						ObstacleManager.AddCrocogator(i, y + num6 / 2, 250, 1100 - y, flippedH: true, flippedV: true);
						if (i % num5 == 0)
						{
							num10++;
							switch (num10)
							{
							case 1:
								ObstacleManager.AddCoin(i + num5 / 2, y - 200, 150, 150, 0, 0);
								break;
							case 2:
								ObstacleManager.AddSomethingToShoot(i + num5 / 2, y - 200, 256, 175, 0, 0);
								ObstacleManager.AddCoin(i + num5 / 2, y + 200, 150, 150, 0, 0);
								break;
							case 3:
							{
								num9++;
								int num12 = num9 % 3 + 1;
								FighterManager.AddComputerPlayer(FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, i + num11, 675, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "Vegetarian" + num12, "v10"));
								num10 = 0;
								break;
							}
							}
						}
						flag = false;
						continue;
					}
					if (i % 120000 > 110000 && !flag)
					{
						FighterManager.AddComputerPlayer(FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, i + 5000, 650, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "ChalkBoard", "v100"));
						FighterManager.AddComputerPlayer(FighterManager.createNewPooTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, i + 5000 - 1200, 650, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 1f, "Teacher", "v100"));
						flag = true;
					}
					if (!flag)
					{
						if (flag2)
						{
							ObstacleManager.AddSomethingToShoot(i + num5 / 6, 200, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 2, 200, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 3, 200, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 4, 200, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 5, 200, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 6, 200, 256, 175, 0, 0);
						}
						else
						{
							ObstacleManager.AddSomethingToShoot(i + num5 / 6, 800, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 2, 800, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 3, 800, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 4, 800, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 5, 800, 256, 175, 0, 0);
							ObstacleManager.AddSomethingToShoot(i + num5 / 6 * 6, 800, 256, 175, 0, 0);
						}
						flag2 = !flag2;
					}
					num7 += num8;
					ObstacleManager.AddCoin(i + num5 / 4, num7, 150, 150, 0, 0);
					if (num7 > 600)
					{
						num8 = -100;
					}
					if (num7 < 300)
					{
						num8 = 100;
					}
					num7 += num8;
					ObstacleManager.AddCoin(i + num5 / 4 * 2, num7, 150, 150, 0, 0);
					if (num7 > 600)
					{
						num8 = -100;
					}
					if (num7 < 300)
					{
						num8 = 100;
					}
					num7 += num8;
					ObstacleManager.AddCoin(i + num5 / 4 * 3, num7, 150, 150, 0, 0);
					if (num7 > 600)
					{
						num8 = -100;
					}
					if (num7 < 300)
					{
						num8 = 100;
					}
					num7 += num8;
					ObstacleManager.AddCoin(i + num5 / 4 * 4, num7, 150, 150, 0, 0);
					if (num7 > 600)
					{
						num8 = -100;
					}
					if (num7 < 300)
					{
						num8 = 100;
					}
				}
				ObstacleManager.Sort();
				SceneryManager.Sort();
				break;
			}
			case Customizations.MoveSpeed:
				FighterManager.SetHumanRandomThings(null, int.Parse(LevelCustomizations[key].ToString().Replace(",", ".")));
				break;
			case Customizations.Driver:
				RandomStaticGlobals.GameMode = Definitions.GameMode.driver;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				break;
			case Customizations.MeteorsEasy:
			{
				if (importCount > 1)
				{
					return;
				}
				int num = int.Parse(LevelCustomizations[key].ToString().Replace(",", "."));
				Random random = new Random(3);
				for (int i = 2000; i < num; i += 1000)
				{
					int y = random.Next(100, 900);
					int num2 = random.Next(250, 500);
					int xRoll = random.Next(10, 500) * -1;
					ObstacleManager.AddMeteorInSpace(i, y, num2, num2, xRoll, 0);
				}
				break;
			}
			case Customizations.MeteorsHard:
			{
				if (importCount > 1)
				{
					return;
				}
				int num = int.Parse(LevelCustomizations[key].ToString().Replace(",", "."));
				Random random = new Random(3);
				for (int i = 2000; i < num; i += 500)
				{
					int y = random.Next(100, 900);
					int num2 = random.Next(250, 500);
					int xRoll = random.Next(100, 500) * -1;
					ObstacleManager.AddMeteorInSpace(i, y, num2, num2, xRoll, 0);
				}
				break;
			}
			}
		}
	}

	public static void ClearData()
	{
		LevelCustomizations.Clear();
	}
}
