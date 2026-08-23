using System;
using System.Collections.Generic;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public static class SceneryManager
{
	public static List<SceneryObject> BackSkySceneryObjects = new List<SceneryObject>(100);

	public static List<SceneryObject> BackGroundSceneryObjects = new List<SceneryObject>(100);

	public static List<SceneryObject> FloatingSceneryObjects = new List<SceneryObject>(500);

	public static List<SceneryObject> BloodStainSceneryObjects = new List<SceneryObject>(100);

	public static List<string> imagesPath = new List<string>(500);

	public static List<Texture2D> imagesTexture = new List<Texture2D>(500);

	public static ContentManager Content;

	private static DateTime dtProcessAfter = DateTime.MinValue;

	public static void Sort()
	{
		BackSkySceneryObjects.Sort(delegate(SceneryObject so, SceneryObject so2)
		{
			int num = so.X.CompareTo(so2.X);
			return (num != 0) ? num : so.Y.CompareTo(so2.Y);
		});
		BackGroundSceneryObjects.Sort(delegate(SceneryObject so, SceneryObject so2)
		{
			int num = so.X.CompareTo(so2.X);
			return (num != 0) ? num : so.Y.CompareTo(so2.Y);
		});
		FloatingSceneryObjects.Sort(delegate(SceneryObject so, SceneryObject so2)
		{
			int num = so.X.CompareTo(so2.X);
			return (num != 0) ? num : so.Y.CompareTo(so2.Y);
		});
	}

	public static void SortBlood()
	{
		BloodStainSceneryObjects.Sort(delegate(SceneryObject so, SceneryObject so2)
		{
			int num = so.name.CompareTo(so2.name);
			return (num != 0) ? num : so.X.CompareTo(so2.X);
		});
	}

	public static void AddBloodStain(int x, int y, int width, int height, ObstacleObject OptionalExclusionObstacle)
	{
		if (CustomsManager.IsBloodEnabled())
		{
			if (height < 20)
			{
				height = 20;
			}
			Random random = new Random(DateTime.Now.Millisecond);
			int num = random.Next(GraphicsManager.bloodStainList.Count);
			int num2 = y;
			num2 += height / 2;
			int num3 = height;
			num3 /= 4;
			int num4 = 0;
			while (!AmIOnSolidGround(new Rectangle(x, num2 - 100, width, 5), OptionalExclusionObstacle) && num4 < 1000)
			{
				num2 += height / 10;
				num4++;
			}
			num2 += 10;
			BloodStainSceneryObjects.Add(new SceneryObject(BloodStainSceneryObjects.Count, num.ToString(), x, num2, width, num3, visible: true));
			SortBlood();
		}
	}

	public static void DrawScenerySkyAndGround()
	{
		for (int i = 0; i < BackSkySceneryObjects.Count; i++)
		{
			SpriteEffects effects = SpriteEffects.None;
			if (BackSkySceneryObjects[i].isFlippedHorizontally)
			{
				effects = SpriteEffects.FlipHorizontally;
			}
			if (BackSkySceneryObjects[i].isFlippedVertically)
			{
				effects = SpriteEffects.FlipVertically;
			}
			if (BackSkySceneryObjects[i].isVisible && BackSkySceneryObjects[i].AImode == NonFighterAI.modes.ScrollRatioToPlayer)
			{
				GraphicsManager.Draw(GraphicsManager.LoadTexture("scenery/sky/" + BackSkySceneryObjects[i].name), GraphicsManager.getAdjustedRectangleForSlowScroller(BackSkySceneryObjects[i].rect, BackSkySceneryObjects[i].AIAmountSpeed), null, Color.White, 0f, Vector2.Zero, effects, Definitions.LayerDepthForSky);
			}
			else if (BackSkySceneryObjects[i].isVisible && BackSkySceneryObjects[i].rect.Intersects(GraphicsManager.viewableArea))
			{
				GraphicsManager.Draw(GraphicsManager.LoadTexture("scenery/sky/" + BackSkySceneryObjects[i].name), GraphicsManager.getAdjustedRectangle(BackSkySceneryObjects[i].rect), null, Color.White, 0f, Vector2.Zero, effects, Definitions.LayerDepthForSky);
			}
		}
		for (int i = 0; i < BackGroundSceneryObjects.Count; i++)
		{
			if (BackGroundSceneryObjects[i].isVisible)
			{
				SpriteEffects effects = SpriteEffects.None;
				if (BackGroundSceneryObjects[i].isFlippedHorizontally)
				{
					effects = SpriteEffects.FlipHorizontally;
				}
				if (BackGroundSceneryObjects[i].isFlippedVertically)
				{
					effects = SpriteEffects.FlipVertically;
				}
				if (BackGroundSceneryObjects[i].AImode == NonFighterAI.modes.ScrollRatioToPlayer)
				{
					GraphicsManager.Draw(GraphicsManager.LoadTexture("scenery/ground/" + BackGroundSceneryObjects[i].name), GraphicsManager.getAdjustedRectangleForSlowScroller(BackGroundSceneryObjects[i].rect, BackGroundSceneryObjects[i].AIAmountSpeed), null, Color.White, 0f, Vector2.Zero, effects, Definitions.LayerDepthForGround);
				}
				else if (BackGroundSceneryObjects[i].rect.Intersects(GraphicsManager.viewableArea))
				{
					GraphicsManager.Draw(GraphicsManager.LoadTexture("scenery/ground/" + BackGroundSceneryObjects[i].name), GraphicsManager.getAdjustedRectangle(BackGroundSceneryObjects[i].rect), null, Color.White, 0f, Vector2.Zero, effects, Definitions.LayerDepthForGround);
				}
			}
		}
	}

	public static void DrawSceneryFloaters()
	{
		bool flag = true;
		bool flag2 = false;
		float num = 0f;
		for (int i = 0; i < FloatingSceneryObjects.Count; i++)
		{
			if ((FloatingSceneryObjects[i].isVisible && FloatingSceneryObjects[i].X + 5000f > (float)GraphicsManager.viewableArea.X && FloatingSceneryObjects[i].X < (float)(GraphicsManager.viewableArea.X + 5000)) || FloatingSceneryObjects[i].AImode == NonFighterAI.modes.ScrollRatioToPlayer)
			{
				flag2 = true;
				num += 1E-07f;
				SpriteEffects effects = SpriteEffects.None;
				if (FloatingSceneryObjects[i].isFlippedHorizontally)
				{
					effects = SpriteEffects.FlipHorizontally;
				}
				if (FloatingSceneryObjects[i].isFlippedVertically)
				{
					effects = SpriteEffects.FlipVertically;
				}
				if (FloatingSceneryObjects[i].AImode == NonFighterAI.modes.ScrollRatioToPlayer && GraphicsManager.viewportRect.Intersects(GraphicsManager.getAdjustedRectangleForSlowScroller(FloatingSceneryObjects[i].rect, FloatingSceneryObjects[i].AIAmountSpeed)))
				{
					if (FloatingSceneryObjects[i].AIAmountSpeed != 0)
					{
						GraphicsManager.Draw(GraphicsManager.LoadTexture("scenery/floaters/" + FloatingSceneryObjects[i].name), GraphicsManager.getAdjustedRectangleForSlowScroller(FloatingSceneryObjects[i].rect, FloatingSceneryObjects[i].AIAmountSpeed), null, Color.White, 0f, Vector2.Zero, effects, Definitions.LayerDepthForSky + num);
					}
				}
				else if (GraphicsManager.viewableArea.Intersects(FloatingSceneryObjects[i].rect))
				{
					GraphicsManager.Draw(GraphicsManager.LoadTexture("scenery/floaters/" + FloatingSceneryObjects[i].name), GraphicsManager.getAdjustedRectangle(FloatingSceneryObjects[i].rect), null, Color.White, 0f, Vector2.Zero, effects, FloatingSceneryObjects[i].getLayerDepth() + num);
				}
			}
			else if (flag2)
			{
				flag = false;
			}
		}
		Color white = Color.White;
		white.A = 200;
		for (int i = 0; i < BloodStainSceneryObjects.Count; i++)
		{
			if (!BloodStainSceneryObjects[i].rect.Intersects(GraphicsManager.viewableArea))
			{
				continue;
			}
			if (Definitions.Options.BloodOnOff)
			{
				if (GraphicsManager.bloodStainList != null && GraphicsManager.bloodStainList.Count != 0)
				{
					GraphicsManager.Draw(GraphicsManager.bloodStainList[int.Parse(BloodStainSceneryObjects[i].name)], GraphicsManager.getAdjustedRectangle(BloodStainSceneryObjects[i].rect), null, white, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthForBlood);
				}
			}
			else
			{
				GraphicsManager.Draw(GraphicsManager.bloodStainListGREEN[int.Parse(BloodStainSceneryObjects[i].name)], GraphicsManager.getAdjustedRectangle(BloodStainSceneryObjects[i].rect), null, white, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthForBlood);
			}
		}
	}

	public static SceneryObject makeSceneryObjectFromPath(int ID, string path)
	{
		try
		{
			string text = path.Substring(path.LastIndexOf("Content"));
			text = text.Replace("\\", "/");
			text = text.Substring(text.IndexOf("/") + 1);
			if (!path.StartsWith("C:\\"))
			{
				text = text.Replace("ContentPhone/", "");
				text = text.Replace("ContentPhone\\", "");
				text = text.Replace("Content/", "");
				text = text.Replace("Content\\", "");
			}
			text = text.Substring(0, text.LastIndexOf("."));
			Texture2D texture2D = GraphicsManager.LoadTexture(text);
			SceneryObject sceneryObject = new SceneryObject(ID);
			if (text.Contains("death"))
			{
				sceneryObject.name = text.Replace("scenery/floaters/", "").Replace("scenery/sky", "").Replace("scenery/ground/", "");
			}
			else
			{
				sceneryObject.name = text.Replace("scenery/floaters/", "").Replace("scenery/sky", "").Replace("scenery/ground/", "");
			}
			if (texture2D != null)
			{
				sceneryObject.width = texture2D.Width;
				sceneryObject.height = texture2D.Height;
			}
			else
			{
				sceneryObject.width = -1;
				sceneryObject.height = -1;
			}
			sceneryObject.X = 0f;
			sceneryObject.Y = 0f;
			return sceneryObject;
		}
		catch (Exception ex)
		{
			string text2 = ex.Message + ID + path;
		}
		return null;
	}

	public static string ExportData()
	{
		string text = "";
		foreach (SceneryObject backSkySceneryObject in BackSkySceneryObjects)
		{
			text += string.Format("type=SkyScenery;x={0};y={1};w={2};h={3};name={4};visible={5};uniqueName={6};xRoll={7};yRoll={8};DPS={9};isDPSFeetOnly={10};AI={11};AIspeed={12};AIdistance={13};isFlippedHorizontally={14};isFlippedVertically={15};Z={16}" + Environment.NewLine, backSkySceneryObject.X, backSkySceneryObject.Y, backSkySceneryObject.width, backSkySceneryObject.height, backSkySceneryObject.name, backSkySceneryObject.isVisible, backSkySceneryObject.uniqueName, backSkySceneryObject.xRoll, backSkySceneryObject.yRoll, backSkySceneryObject.DPS, backSkySceneryObject.isDPSFeetOnly, backSkySceneryObject.AImode.ToString(), backSkySceneryObject.AIAmountSpeed.ToString(), backSkySceneryObject.AIAmountDistance.ToString(), backSkySceneryObject.isFlippedHorizontally, backSkySceneryObject.isFlippedVertically, backSkySceneryObject.Z);
		}
		foreach (SceneryObject backGroundSceneryObject in BackGroundSceneryObjects)
		{
			text += string.Format("type=GroundScenery;x={0};y={1};w={2};h={3};name={4};visible={5};uniqueName={6};xRoll={7};yRoll={8};DPS={9};isDPSFeetOnly={10};AI={11};AIspeed={12};AIdistance={13};isFlippedHorizontally={14};isFlippedVertically={15};Z={16}" + Environment.NewLine, backGroundSceneryObject.X, backGroundSceneryObject.Y, backGroundSceneryObject.width, backGroundSceneryObject.height, backGroundSceneryObject.name, backGroundSceneryObject.isVisible, backGroundSceneryObject.uniqueName, backGroundSceneryObject.xRoll, backGroundSceneryObject.yRoll, backGroundSceneryObject.DPS, backGroundSceneryObject.isDPSFeetOnly, backGroundSceneryObject.AImode.ToString(), backGroundSceneryObject.AIAmountSpeed.ToString(), backGroundSceneryObject.AIAmountDistance.ToString(), backGroundSceneryObject.isFlippedHorizontally, backGroundSceneryObject.isFlippedVertically, backGroundSceneryObject.Z);
		}
		foreach (SceneryObject floatingSceneryObject in FloatingSceneryObjects)
		{
			text += string.Format("type=FloatingScenery;x={0};y={1};w={2};h={3};name={4};visible={5};uniqueName={6};xRoll={7};yRoll={8};DPS={9};isDPSFeetOnly={10};AI={11};AIspeed={12};AIdistance={13};isFlippedHorizontally={14};isFlippedVertically={15};Z={16}" + Environment.NewLine, floatingSceneryObject.X, floatingSceneryObject.Y, floatingSceneryObject.width, floatingSceneryObject.height, floatingSceneryObject.name, floatingSceneryObject.isVisible, floatingSceneryObject.uniqueName, floatingSceneryObject.xRoll, floatingSceneryObject.yRoll, floatingSceneryObject.DPS, floatingSceneryObject.isDPSFeetOnly, floatingSceneryObject.AImode.ToString(), floatingSceneryObject.AIAmountSpeed.ToString(), floatingSceneryObject.AIAmountDistance.ToString(), floatingSceneryObject.isFlippedHorizontally, floatingSceneryObject.isFlippedVertically, floatingSceneryObject.Z);
		}
		return text;
	}

	public static List<string> ParseTexturePaths(string levelData)
	{
		return null;
	}

	public static void ImportData(string data)
	{
		ClearData();
		string[] array = data.Split(Environment.NewLine.ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].StartsWith("type=SkyScenery"))
			{
				BackSkySceneryObjects.Add(convertTextLineToSceneryObject(BackSkySceneryObjects.Count, array[i]));
				BackSkySceneryObjects[BackSkySceneryObjects.Count - 1].ID = BackSkySceneryObjects.Count - 1;
				GraphicsManager.LoadTexture("scenery/sky/" + BackSkySceneryObjects[BackSkySceneryObjects.Count - 1].name);
			}
			else if (array[i].StartsWith("type=GroundScenery"))
			{
				BackGroundSceneryObjects.Add(convertTextLineToSceneryObject(BackGroundSceneryObjects.Count, array[i]));
				BackGroundSceneryObjects[BackGroundSceneryObjects.Count - 1].ID = BackGroundSceneryObjects.Count - 1;
				GraphicsManager.LoadTexture("scenery/ground/" + BackGroundSceneryObjects[BackGroundSceneryObjects.Count - 1].name);
			}
			else if (array[i].StartsWith("type=FloatingScenery"))
			{
				FloatingSceneryObjects.Add(convertTextLineToSceneryObject(FloatingSceneryObjects.Count, array[i]));
				FloatingSceneryObjects[FloatingSceneryObjects.Count - 1].ID = FloatingSceneryObjects.Count - 1;
				GraphicsManager.LoadTexture("scenery/floaters/" + FloatingSceneryObjects[FloatingSceneryObjects.Count - 1].name);
			}
			if (i % 10 == 0)
			{
				ScreenManager.UpdateLoadingStatus("");
			}
		}
		Sort();
	}

	public static void ClearData()
	{
		BackGroundSceneryObjects.Clear();
		BackSkySceneryObjects.Clear();
		FloatingSceneryObjects.Clear();
	}

	private static SceneryObject convertTextLineToSceneryObject(int ID, string s)
	{
		s = s.Trim();
		SceneryObject sceneryObject = new SceneryObject(ID);
		string[] array = s.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('=');
			switch (array2[0])
			{
			case "x":
				sceneryObject.X = int.Parse(array2[1]);
				break;
			case "y":
				sceneryObject.Y = int.Parse(array2[1]);
				break;
			case "w":
				sceneryObject.width = int.Parse(array2[1]);
				break;
			case "h":
				sceneryObject.height = int.Parse(array2[1]);
				break;
			case "name":
				sceneryObject.name = array2[1];
				break;
			case "uniqueName":
				sceneryObject.uniqueName = array2[1];
				break;
			case "visible":
				if (array2[1] == "True")
				{
					sceneryObject.isVisible = true;
				}
				else if (array2[1] == "False")
				{
					sceneryObject.isVisible = false;
				}
				break;
			case "xRoll":
				sceneryObject.xRoll = int.Parse(array2[1]);
				break;
			case "yRoll":
				sceneryObject.yRoll = int.Parse(array2[1]);
				break;
			case "DPS":
				sceneryObject.DPS = int.Parse(array2[1]);
				break;
			case "isDPSFeetOnly":
				if (array2[1] == "True")
				{
					sceneryObject.isDPSFeetOnly = true;
				}
				else if (array2[1] == "False")
				{
					sceneryObject.isDPSFeetOnly = false;
				}
				break;
			case "AI":
				sceneryObject.AImode = (NonFighterAI.modes)Enum.Parse(typeof(NonFighterAI.modes), array2[1], ignoreCase: true);
				break;
			case "AIspeed":
				sceneryObject.AIAmountSpeed = int.Parse(array2[1]);
				break;
			case "AIdistance":
				sceneryObject.AIAmountDistance = int.Parse(array2[1]);
				break;
			case "isFlippedVertically":
				if (array2[1] == "True")
				{
					sceneryObject.isFlippedVertically = true;
				}
				else if (array2[1] == "False")
				{
					sceneryObject.isFlippedVertically = false;
				}
				break;
			case "isFlippedHorizontally":
				if (array2[1] == "True")
				{
					sceneryObject.isFlippedHorizontally = true;
				}
				else if (array2[1] == "False")
				{
					sceneryObject.isFlippedHorizontally = false;
				}
				break;
			case "Z":
				sceneryObject.Z = float.Parse(array2[1].Replace(",", "."));
				break;
			}
		}
		return sceneryObject;
	}

	public static void ActivateNamedObject(string name)
	{
		for (int i = 0; i < BackSkySceneryObjects.Count; i++)
		{
			if (BackSkySceneryObjects[i].uniqueName == name)
			{
				BackSkySceneryObjects[i].isVisible = BackSkySceneryObjects[i].isVisible;
			}
		}
		for (int i = 0; i < BackGroundSceneryObjects.Count; i++)
		{
			if (BackGroundSceneryObjects[i].uniqueName == name)
			{
				BackGroundSceneryObjects[i].isVisible = !BackGroundSceneryObjects[i].isVisible;
			}
		}
		for (int i = 0; i < FloatingSceneryObjects.Count; i++)
		{
			if (FloatingSceneryObjects[i].uniqueName == name)
			{
				FloatingSceneryObjects[i].isVisible = !FloatingSceneryObjects[i].isVisible;
			}
		}
	}

	public static bool AmIOnSolidGround(Rectangle rect, ObstacleObject OptionalExclusionObstacle)
	{
		foreach (SceneryObject backGroundSceneryObject in BackGroundSceneryObjects)
		{
			if (backGroundSceneryObject.isVisible && (backGroundSceneryObject.rect.Intersects(rect) || backGroundSceneryObject.rect.Contains(rect)))
			{
				return true;
			}
		}
		rect.Height += 10;
		foreach (ObstacleObject obstacle in ObstacleManager.Obstacles)
		{
			if (obstacle.isActive && (OptionalExclusionObstacle == null || obstacle.ID != OptionalExclusionObstacle.ID) && (obstacle.rect.Intersects(rect) || obstacle.rect.Contains(rect)))
			{
				return true;
			}
		}
		return false;
	}

	public static int AmITakingSceneryDPS(Rectangle rect, Rectangle feet, bool isCrouching, bool isKicking)
	{
		foreach (SceneryObject floatingSceneryObject in FloatingSceneryObjects)
		{
			if (floatingSceneryObject.DPS == 0 || !floatingSceneryObject.isVisible)
			{
				continue;
			}
			if (isKicking && floatingSceneryObject.AImode == NonFighterAI.modes.ImDestructible)
			{
				if (floatingSceneryObject.rect.Intersects(feet) || floatingSceneryObject.rect.Contains(rect) || rect.Contains(floatingSceneryObject.rect))
				{
					floatingSceneryObject.isVisible = false;
				}
				continue;
			}
			if (feet.Width > 0 && feet.Height > 0 && floatingSceneryObject.rect.Intersects(feet))
			{
				return floatingSceneryObject.DPS;
			}
			if (!floatingSceneryObject.isDPSFeetOnly && !isCrouching && (floatingSceneryObject.rect.Intersects(rect) || floatingSceneryObject.rect.Contains(rect) || rect.Contains(floatingSceneryObject.rect)))
			{
				return floatingSceneryObject.DPS;
			}
		}
		foreach (SceneryObject backGroundSceneryObject in BackGroundSceneryObjects)
		{
			if (backGroundSceneryObject.DPS != 0 && backGroundSceneryObject.isVisible)
			{
				if (backGroundSceneryObject.rect.Intersects(rect) && !backGroundSceneryObject.isDPSFeetOnly)
				{
					return backGroundSceneryObject.DPS;
				}
				if (backGroundSceneryObject.rect.Intersects(feet))
				{
					return backGroundSceneryObject.DPS;
				}
			}
		}
		foreach (SceneryObject backSkySceneryObject in BackSkySceneryObjects)
		{
			if (backSkySceneryObject.DPS != 0 && backSkySceneryObject.isVisible)
			{
				if (backSkySceneryObject.rect.Intersects(rect) && !backSkySceneryObject.isDPSFeetOnly)
				{
					return backSkySceneryObject.DPS;
				}
				if (backSkySceneryObject.rect.Intersects(feet))
				{
					return backSkySceneryObject.DPS;
				}
			}
		}
		foreach (ObstacleObject obstacle in ObstacleManager.Obstacles)
		{
			if (obstacle.DPS != 0 && obstacle.isActive && (obstacle.rect.Intersects(rect) || obstacle.rect.Intersects(feet)))
			{
				return obstacle.DPS;
			}
		}
		return 0;
	}

	public static void ProcessScenery()
	{
		foreach (SceneryObject floatingSceneryObject in FloatingSceneryObjects)
		{
			DriftScenery(floatingSceneryObject);
			DoSceneryAIStuff(floatingSceneryObject);
		}
		foreach (SceneryObject backGroundSceneryObject in BackGroundSceneryObjects)
		{
			DriftScenery(backGroundSceneryObject);
			DoSceneryAIStuff(backGroundSceneryObject);
		}
		foreach (SceneryObject backSkySceneryObject in BackSkySceneryObjects)
		{
			DriftScenery(backSkySceneryObject);
			DoSceneryAIStuff(backSkySceneryObject);
		}
	}

	public static void DoSceneryAIStuff(SceneryObject so)
	{
		if (so.AImode != NonFighterAI.modes.none && so.AImode == NonFighterAI.modes.MoveUpDown)
		{
			so = NonFighterAI.moveUpDown(so);
		}
	}

	private static void DriftScenery(SceneryObject o)
	{
		if (o.isVisible && o.isVisible && (o.xRoll != 0 || o.yRoll != 0))
		{
			o.X += (float)o.xRoll / (float)Definitions.UpdatesPerSecond;
			o.Y += (float)o.yRoll / (float)Definitions.UpdatesPerSecond;
			if (o.xRollFighters != 0 || o.yRollFighters != 0)
			{
				FighterManager.MoveFighters(o.rect, (float)o.xRollFighters / (float)Definitions.UpdatesPerSecond, (float)o.yRollFighters / (float)Definitions.UpdatesPerSecond);
			}
			if (!Definitions.ScreenMaxRect.Contains(o.rect))
			{
				o.xRoll = 0;
				o.yRoll = 0;
			}
		}
	}
}
