using System;
using System.Collections.Generic;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public static class ObstacleManager
{
	public static List<ObstacleObject> Obstacles = new List<ObstacleObject>(10);

	public static void Sort()
	{
		Obstacles.Sort(delegate(ObstacleObject so, ObstacleObject so2)
		{
			int num = so.X.CompareTo(so2.X);
			if (num != 0)
			{
				return num;
			}
			num = so.Y.CompareTo(so2.Y);
			return (num != 0) ? num : so.width.CompareTo(so2.width);
		});
	}

	public static Rectangle? doCollisionCheck(Rectangle rectBody, Rectangle rectFeet, Rectangle? rectPickupZone, ObstacleObject optionalExcludedObject, ObstacleObject optionalExcludedObject2)
	{
		foreach (ObstacleObject obstacle in Obstacles)
		{
			if (obstacle.isActive && (optionalExcludedObject == null || obstacle.ID != optionalExcludedObject.ID) && (optionalExcludedObject2 == null || obstacle.ID != optionalExcludedObject2.ID) && obstacle.pixelsInTheAir <= 0 && obstacle.AImode != NonFighterAI.modes.ImActuallyScenery && !obstacle.isReallyScenery)
			{
				if (rectPickupZone.HasValue && obstacle.rect.Intersects(rectPickupZone.Value))
				{
					obstacle.isInPickupableRange = true;
					obstacle.dtLastTimeInPickupableRange = DateTime.Now;
				}
				if (obstacle.isOnGround && obstacle.rect.Intersects(rectFeet))
				{
					return obstacle.rect;
				}
				if (obstacle.rect.Intersects(rectBody))
				{
					return obstacle.rect;
				}
			}
		}
		return null;
	}

	public static List<ObstacleObject> getPickupableObjects(Rectangle? rectOptional)
	{
		List<ObstacleObject> list = new List<ObstacleObject>(0);
		foreach (ObstacleObject obstacle in Obstacles)
		{
			if (!obstacle.isPickupable)
			{
				continue;
			}
			if (rectOptional.HasValue)
			{
				if (rectOptional.Value.Intersects(obstacle.rect))
				{
					list.Add(obstacle);
				}
			}
			else
			{
				list.Add(obstacle);
			}
		}
		return list;
	}

	public static bool IsThisMoveDangerous(Rectangle r)
	{
		foreach (ObstacleObject obstacle in Obstacles)
		{
			if (obstacle.isActive && (obstacle.AImode == NonFighterAI.modes.FallingMeteorite || obstacle.AImode == NonFighterAI.modes.FallingMeteoriteOneTime || obstacle.yRoll != 0))
			{
				r.Y = -10000;
				r.Height = 14000;
				if (r.Contains(obstacle.rect) || r.Intersects(obstacle.rect))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static int doEnvironmentalDestructionCollision(Rectangle hereBeThePain, int damageAmount)
	{
		int num = 0;
		foreach (ObstacleObject obstacle in Obstacles)
		{
			if ((obstacle.isActive && obstacle.isDestructible && obstacle.rect.Intersects(hereBeThePain)) || obstacle.rect.Contains(hereBeThePain))
			{
				obstacle.takeDamage(damageAmount, broadcast: true);
				num++;
			}
		}
		return num;
	}

	public static void DrawObstacles()
	{
		bool flag = true;
		bool flag2 = false;
		for (int i = 0; i < Obstacles.Count; i++)
		{
			if (!flag)
			{
				break;
			}
			ObstacleObject obstacleObject = Obstacles[i];
			if (obstacleObject.name == null || !(obstacleObject.name != "") || !obstacleObject.isActive)
			{
				continue;
			}
			if (obstacleObject.rect.Intersects(GraphicsManager.viewableArea) || obstacleObject.AImode == NonFighterAI.modes.FallingMeteoriteOneTime || obstacleObject.AImode == NonFighterAI.modes.FallingMeteorite)
			{
				flag2 = true;
				Rectangle adjustedRectangle = GraphicsManager.getAdjustedRectangle(obstacleObject.rect);
				adjustedRectangle.Y -= obstacleObject.pixelsInTheAir;
				SpriteEffects effects = SpriteEffects.None;
				float rotation = 0f;
				if (obstacleObject.isFlippedVertically && obstacleObject.isFlippedHorizontally)
				{
					effects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
				}
				else if (obstacleObject.isFlippedHorizontally)
				{
					effects = SpriteEffects.FlipHorizontally;
				}
				else if (obstacleObject.isFlippedVertically)
				{
					effects = SpriteEffects.FlipVertically;
				}
				float layerDepth = obstacleObject.getLayerDepth();
				if (obstacleObject.AImode == NonFighterAI.modes.FallingMeteorite || obstacleObject.AImode == NonFighterAI.modes.FallingMeteoriteOneTime)
				{
					int num = 0;
					if (adjustedRectangle.Y > 0)
					{
						num++;
					}
				}
				GraphicsManager.Draw(GraphicsManager.LoadTexture("scenery/obstacles/" + obstacleObject.name), adjustedRectangle, null, Color.White, rotation, Vector2.Zero, effects, Definitions.LayerDepthSecondHighest);
				if (obstacleObject.isBeingCarriedBy == null && obstacleObject.isInPickupableRange && obstacleObject.isPickupable)
				{
					Rectangle destinationRectangle = adjustedRectangle;
					destinationRectangle.X += 75;
					destinationRectangle.Y += 50;
					destinationRectangle.Width = 100;
					destinationRectangle.Height = 100;
					GraphicsManager.Draw(GraphicsManager.imgButtonB, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
					if (obstacleObject.dtLastTimeInPickupableRange.AddSeconds(Definitions.ObstacleSecondsToShowB) < DateTime.Now)
					{
						obstacleObject.isInPickupableRange = false;
					}
				}
			}
			else if (!flag2)
			{
			}
		}
	}

	public static void ProcessFlappyObstacles()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < Obstacles.Count; i++)
		{
			if (flag2)
			{
				break;
			}
			if (Obstacles[i].X < (float)(GraphicsManager.viewableArea.X + 5000) && Obstacles[i].X > (float)(GraphicsManager.viewableArea.X - 5000))
			{
				flag = true;
				if (Obstacles[i].AImode == NonFighterAI.modes.RoadKill && Obstacles[i].isActive && FighterManager.doCollisionCheck(Obstacles[i].rect, humans: true, cupoos: false) != null)
				{
					Obstacles[i].takeDamage(100, broadcast: true);
				}
			}
			else if (flag)
			{
				flag2 = true;
			}
		}
	}

	public static void ProcessObstacles()
	{
		ProcessFlappyObstacles();
	}

	public static ObstacleObject makeObstacleObjectFromPath(string path)
	{
		string text = path.Substring(path.LastIndexOf("Content"));
		text = text.Substring(text.IndexOf("/") + 1);
		text = text.Replace("ContentPhone/", "");
		text = text.Replace("ContentPhone\\", "");
		text = text.Replace("Content/", "");
		text = text.Replace("Content\\", "");
		text = text.Substring(0, text.LastIndexOf("."));
		Texture2D texture2D = GraphicsManager.LoadTexture(text);
		ObstacleObject obstacleObject = new ObstacleObject(default(Rectangle), texture2D);
		obstacleObject.name = text.Substring(text.LastIndexOf("\\") + 1);
		if (texture2D != null)
		{
			obstacleObject.width = texture2D.Width;
			obstacleObject.height = texture2D.Height;
		}
		else
		{
			obstacleObject.width = -1;
			obstacleObject.height = -1;
		}
		obstacleObject.X = 0f;
		obstacleObject.Y = 0f;
		return obstacleObject;
	}

	public static string ExportData()
	{
		string text = "";
		foreach (ObstacleObject obstacle in Obstacles)
		{
			text += string.Format("type=obstacle;x={0};y={1};w={2};h={3};name={4};active={5};uniqueName={6};xRoll={7};yRoll={8};isDestructible={9};hp={10};DPS={11};isPickupable={12};AI={13};AIspeed={14};AIdistance={15};isFlippedVertically={16};isFlippedHorizontally={17};isReallyScenery={18};" + Environment.NewLine, obstacle.X, obstacle.Y, obstacle.width, obstacle.height, obstacle.name, obstacle.isActive, obstacle.uniqueName, obstacle.xRoll, obstacle.yRoll, obstacle.isDestructible, obstacle.hp, obstacle.DPS, obstacle.isPickupable, obstacle.AImode.ToString(), obstacle.AIAmountSpeed, obstacle.AIAmountDistance, obstacle.isFlippedVertically, obstacle.isFlippedHorizontally, obstacle.isReallyScenery);
		}
		return text;
	}

	public static void ImportData(string data)
	{
		ClearData();
		string[] array = data.Split(Environment.NewLine.ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].StartsWith("type=obstacle"))
			{
				Obstacles.Add(convertTextLineToObstacleObject(array[i]));
				Obstacles[Obstacles.Count - 1].ID = Obstacles.Count - 1;
				GraphicsManager.LoadTexture("scenery/obstacles/" + Obstacles[Obstacles.Count - 1].name);
			}
		}
		Sort();
	}

	public static void ClearData()
	{
		Obstacles.Clear();
	}

	public static void AddMeteorite(int x, int y, int width, int height, int fallspeed, int falldistance)
	{
		ObstacleObject obstacleObject = new ObstacleObject();
		obstacleObject.X = x;
		obstacleObject.Y = y;
		obstacleObject.width = width;
		obstacleObject.height = height;
		obstacleObject.name = "BossMeteorite";
		obstacleObject.uniqueName = "Asteroid x" + DateTime.Now.Millisecond;
		obstacleObject.isActive = true;
		obstacleObject.xRoll = 0;
		obstacleObject.yRoll = 0;
		obstacleObject.isDestructible = true;
		obstacleObject.hp = 1000;
		obstacleObject.DPS = 25;
		obstacleObject.isPickupable = true;
		obstacleObject.AImode = NonFighterAI.modes.FallingMeteoriteOneTime;
		obstacleObject.AIAmountSpeed = fallspeed;
		obstacleObject.AIAmountDistance = falldistance;
		obstacleObject.fallSpeedPerFrame = fallspeed / Definitions.UpdatesPerSecond;
		obstacleObject.pixelsInTheAir = falldistance;
		obstacleObject.isFalling = true;
		obstacleObject.circlePivotPoint = new Vector2(obstacleObject.X, obstacleObject.Y);
		obstacleObject.ID = Obstacles.Count;
		Obstacles.Add(obstacleObject);
	}

	public static void AddCrocogator(int x, int y, int width, int height, bool flippedH, bool flippedV)
	{
		ObstacleObject obstacleObject = new ObstacleObject();
		obstacleObject.X = x;
		obstacleObject.Y = y;
		obstacleObject.width = width;
		obstacleObject.height = height;
		obstacleObject.isFlippedHorizontally = flippedH;
		obstacleObject.isFlippedVertically = flippedV;
		obstacleObject.name = "MrGator";
		obstacleObject.uniqueName = "Gator x" + DateTime.Now.Millisecond;
		obstacleObject.isActive = true;
		obstacleObject.xRoll = 0;
		obstacleObject.yRoll = 0;
		obstacleObject.isDestructible = false;
		obstacleObject.hp = 1;
		obstacleObject.DPS = 5000;
		obstacleObject.isPickupable = false;
		obstacleObject.AImode = NonFighterAI.modes.none;
		obstacleObject.isReallyScenery = true;
		obstacleObject.AIAmountSpeed = 0;
		obstacleObject.AIAmountDistance = 0;
		obstacleObject.fallSpeedPerFrame = 0;
		obstacleObject.pixelsInTheAir = 0;
		obstacleObject.isFalling = false;
		obstacleObject.circlePivotPoint = new Vector2(obstacleObject.X, obstacleObject.Y);
		obstacleObject.ID = Obstacles.Count;
		Obstacles.Add(obstacleObject);
	}

	public static void AddSomethingToShoot(int x, int y, int width, int height, int xRoll, int yRoll)
	{
		ObstacleObject obstacleObject = new ObstacleObject();
		obstacleObject.X = x;
		obstacleObject.Y = y;
		obstacleObject.width = width;
		obstacleObject.height = height;
		obstacleObject.name = "MrBird";
		obstacleObject.uniqueName = "Birdy x" + DateTime.Now.Millisecond;
		obstacleObject.isActive = true;
		obstacleObject.xRoll = xRoll;
		obstacleObject.yRoll = yRoll;
		obstacleObject.isDestructible = true;
		obstacleObject.hp = 1;
		obstacleObject.DPS = 0;
		obstacleObject.isPickupable = false;
		obstacleObject.AImode = NonFighterAI.modes.none;
		obstacleObject.isReallyScenery = true;
		obstacleObject.AIAmountSpeed = 0;
		obstacleObject.AIAmountDistance = 0;
		obstacleObject.fallSpeedPerFrame = 0;
		obstacleObject.pixelsInTheAir = 0;
		obstacleObject.isFalling = false;
		obstacleObject.circlePivotPoint = new Vector2(obstacleObject.X, obstacleObject.Y);
		obstacleObject.ID = Obstacles.Count;
		Obstacles.Add(obstacleObject);
	}

	public static void AddCoin(int x, int y, int width, int height, int xRoll, int yRoll)
	{
		ObstacleObject obstacleObject = new ObstacleObject();
		obstacleObject.X = x;
		obstacleObject.Y = y;
		obstacleObject.width = width;
		obstacleObject.height = height;
		obstacleObject.name = "coin";
		obstacleObject.uniqueName = "Coin x" + DateTime.Now.Millisecond;
		obstacleObject.isActive = true;
		obstacleObject.xRoll = xRoll;
		obstacleObject.yRoll = yRoll;
		obstacleObject.isDestructible = true;
		obstacleObject.hp = 100;
		obstacleObject.DPS = 0;
		obstacleObject.isPickupable = false;
		obstacleObject.AImode = NonFighterAI.modes.RoadKill;
		obstacleObject.isReallyScenery = true;
		obstacleObject.AIAmountSpeed = 0;
		obstacleObject.AIAmountDistance = 0;
		obstacleObject.fallSpeedPerFrame = 0;
		obstacleObject.pixelsInTheAir = 0;
		obstacleObject.isFalling = false;
		obstacleObject.circlePivotPoint = new Vector2(obstacleObject.X, obstacleObject.Y);
		obstacleObject.ID = Obstacles.Count;
		Obstacles.Add(obstacleObject);
	}

	public static void AddMeteorInSpace(int x, int y, int width, int height, int xRoll, int yRoll)
	{
		ObstacleObject obstacleObject = new ObstacleObject();
		obstacleObject.X = x;
		obstacleObject.Y = y;
		obstacleObject.width = width;
		obstacleObject.height = height;
		obstacleObject.name = "SpaceMeteor";
		obstacleObject.uniqueName = "Asteroid x" + DateTime.Now.Millisecond;
		obstacleObject.isActive = true;
		obstacleObject.xRoll = xRoll;
		obstacleObject.yRoll = yRoll;
		obstacleObject.isDestructible = true;
		obstacleObject.hp = 1;
		obstacleObject.DPS = 5000;
		obstacleObject.isPickupable = false;
		obstacleObject.AImode = NonFighterAI.modes.none;
		obstacleObject.isReallyScenery = true;
		obstacleObject.AIAmountSpeed = 0;
		obstacleObject.AIAmountDistance = 0;
		obstacleObject.fallSpeedPerFrame = 0;
		obstacleObject.pixelsInTheAir = 0;
		obstacleObject.isFalling = false;
		obstacleObject.circlePivotPoint = new Vector2(obstacleObject.X, obstacleObject.Y);
		obstacleObject.ID = Obstacles.Count;
		Obstacles.Add(obstacleObject);
	}

	private static ObstacleObject convertTextLineToObstacleObject(string s)
	{
		s = s.Trim();
		ObstacleObject obstacleObject = new ObstacleObject();
		string[] array = s.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('=');
			switch (array2[0])
			{
			case "x":
				obstacleObject.X = int.Parse(array2[1]);
				break;
			case "y":
				obstacleObject.Y = int.Parse(array2[1]);
				break;
			case "w":
				obstacleObject.width = int.Parse(array2[1]);
				break;
			case "h":
				obstacleObject.height = int.Parse(array2[1]);
				break;
			case "name":
				obstacleObject.name = array2[1];
				break;
			case "uniqueName":
				obstacleObject.uniqueName = array2[1];
				break;
			case "active":
				if (array2[1] == "True")
				{
					obstacleObject.isActive = true;
				}
				else if (array2[1] == "False")
				{
					obstacleObject.isActive = false;
				}
				break;
			case "xRoll":
				obstacleObject.xRoll = int.Parse(array2[1]);
				break;
			case "yRoll":
				obstacleObject.yRoll = int.Parse(array2[1]);
				break;
			case "isDestructible":
				if (array2[1] == "True")
				{
					obstacleObject.isDestructible = true;
				}
				else if (array2[1] == "False")
				{
					obstacleObject.isDestructible = false;
				}
				break;
			case "hp":
				obstacleObject.hp = int.Parse(array2[1]);
				break;
			case "DPS":
				obstacleObject.DPS = int.Parse(array2[1]);
				break;
			case "isPickupable":
				if (array2[1] == "True")
				{
					obstacleObject.isPickupable = true;
				}
				else if (array2[1] == "False")
				{
					obstacleObject.isPickupable = false;
				}
				break;
			case "AI":
				obstacleObject.AImode = (NonFighterAI.modes)Enum.Parse(typeof(NonFighterAI.modes), array2[1], ignoreCase: true);
				break;
			case "AIspeed":
				obstacleObject.AIAmountSpeed = int.Parse(array2[1]);
				break;
			case "AIdistance":
				obstacleObject.AIAmountDistance = int.Parse(array2[1]);
				break;
			case "isReallyScenery":
				if (array2[1] == "True")
				{
					obstacleObject.isReallyScenery = true;
				}
				else if (array2[1] == "False")
				{
					obstacleObject.isReallyScenery = false;
				}
				break;
			case "isFlippedVertically":
				if (array2[1] == "True")
				{
					obstacleObject.isFlippedVertically = true;
				}
				else if (array2[1] == "False")
				{
					obstacleObject.isFlippedVertically = false;
				}
				break;
			case "isFlippedHorizontally":
				if (array2[1] == "True")
				{
					obstacleObject.isFlippedHorizontally = true;
				}
				else if (array2[1] == "False")
				{
					obstacleObject.isFlippedHorizontally = false;
				}
				break;
			}
		}
		return obstacleObject;
	}

	public static void ActivateNamedObject(string name, bool toggleOnOff)
	{
		for (int i = 0; i < Obstacles.Count; i++)
		{
			if (Obstacles[i].uniqueName == name)
			{
				if (!toggleOnOff || !Obstacles[i].isActive)
				{
					Obstacles[i].isActive = true;
				}
				else
				{
					Obstacles[i].isActive = false;
				}
			}
		}
	}
}
