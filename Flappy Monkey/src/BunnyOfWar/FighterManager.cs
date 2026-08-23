using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar;

public static class FighterManager
{
	public enum CauseOfDeath
	{
		none,
		fastAttack,
		heavyAttack,
		parry,
		explosion,
		arrow,
		grenade,
		rpg,
		rock,
		tongue,
		bomb,
		bullet,
		nulll,
		other
	}

	public static int localXboxPlayerID = 0;

	public static List<FighterObject> humanPlayers = new List<FighterObject>(14);

	public static List<FighterObject> computerPlayers = new List<FighterObject>(25);

	public static void StopTimers()
	{
		foreach (FighterObject humanPlayer in humanPlayers)
		{
			humanPlayer.PROPERTIES.HumanProfile.stopwatchTimeSpentPlaying.Stop();
			humanPlayer.PROPERTIES.HumanProfile.stopwatchTimeSpentBlocking.Stop();
		}
	}

	public static void StartTimers()
	{
		foreach (FighterObject humanPlayer in humanPlayers)
		{
			if (humanPlayer.PROPERTIES.isAlive)
			{
				humanPlayer.PROPERTIES.HumanProfile.stopwatchTimeSpentPlaying.Start();
			}
		}
	}

	public static FighterObject GetHealthiestPlayer()
	{
		if (humanPlayers.Count == 1)
		{
			return humanPlayers[0];
		}
		FighterObject fighterObject = humanPlayers[0];
		foreach (FighterObject humanPlayer in humanPlayers)
		{
			if (humanPlayer.PROPERTIES.health > fighterObject.PROPERTIES.health)
			{
				fighterObject = humanPlayer;
			}
		}
		return fighterObject;
	}

	public static void BoostAllHumansHealth()
	{
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (!humanPlayers[i].PROPERTIES.isAlive || humanPlayers[i].PROPERTIES.isDying)
			{
				continue;
			}
			if (Definitions.Options.Difficulty <= 2)
			{
				humanPlayers[i].healthChange(humanPlayers[i].PROPERTIES.healthMax);
				continue;
			}
			float num = humanPlayers[i].PROPERTIES.healthMax * 0.5f;
			if (humanPlayers[i].PROPERTIES.health < num)
			{
				humanPlayers[i].healthChange(num);
			}
		}
	}

	public static void AdjustAllHumanHealth(float percentageChange)
	{
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (humanPlayers[i].PROPERTIES.isAlive && !humanPlayers[i].PROPERTIES.isDying)
			{
				humanPlayers[i].healthChange(humanPlayers[i].PROPERTIES.healthMax * percentageChange);
			}
		}
	}

	public static int GetTotalSignedInLocalGamers()
	{
		int num = 0;
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (humanPlayers[i].PROPERTIES.GamerTag != null && humanPlayers[i].PROPERTIES.GamerTag != "")
			{
				num++;
			}
		}
		return num;
	}

	public static void Sort()
	{
		computerPlayers.Sort(delegate(FighterObject so, FighterObject so2)
		{
			int num = so.X.CompareTo(so2.X);
			return (num != 0) ? num : so.Y.CompareTo(so2.Y);
		});
	}

	public static int CountLivingCPUsWithinRect(Rectangle r)
	{
		int num = 0;
		foreach (FighterObject computerPlayer in computerPlayers)
		{
			if (computerPlayer.PROPERTIES.isAlive && r.Contains(computerPlayer.rectSpriteDisplay))
			{
				num++;
			}
		}
		return num;
	}

	public static void ProcessFighters()
	{
		int num = 0;
		foreach (FighterObject humanPlayer in humanPlayers)
		{
			if (humanPlayer.PROPERTIES.isAlive && humanPlayer.PROPERTIES.areWeHuman)
			{
				num++;
				ObstacleManager.doCollisionCheck(humanPlayer.getWhereBodyIs(), humanPlayer.getWhereFeetAre(), humanPlayer.getPersonalSpace(), null, null);
				humanPlayer.JUMP.ProcessJumpStuff();
				if (humanPlayer.PROPERTIES.isBlocking)
				{
					humanPlayer.PROPERTIES.HumanProfile.stopwatchTimeSpentBlocking.Start();
				}
				else
				{
					humanPlayer.PROPERTIES.HumanProfile.stopwatchTimeSpentBlocking.Stop();
				}
				if (Definitions.Options.VibrationsOnOff && humanPlayer.PROPERTIES.PlayerIndexControllerNumber.HasValue)
				{
					if (humanPlayer.PROPERTIES.isStunned)
					{
						if (humanPlayer.PROPERTIES.stunExpires < DateTime.Now)
						{
							humanPlayer.PROPERTIES.isStunned = false;
						}
						else if (ScreenManager.CurrentScreen == ScreenManager.screens.Blank)
						{
							GamePad.SetVibration(humanPlayer.PROPERTIES.PlayerIndexControllerNumber.Value, 0.5f, 1f);
						}
					}
					else
					{
						GamePad.SetVibration(humanPlayer.PROPERTIES.PlayerIndexControllerNumber.Value, 0f, 0f);
					}
				}
			}
			if (humanPlayer.PROPERTIES.isAlive && humanPlayer.JUMP.jumpPixelsOffGround <= 0 && !humanPlayer.PROPERTIES.isFlying)
			{
				if (!SceneryManager.AmIOnSolidGround(humanPlayer.getWhereFeetAre(), humanPlayer.PROPERTIES.holdingObstacleObject))
				{
					humanPlayer.fallDownBecauseYouFellOffACliff();
				}
				else
				{
					humanPlayer.JUMP.jumpPixelsOffGround = 0;
				}
			}
			if (humanPlayer.PROPERTIES.isAlive && !humanPlayer.PROPERTIES.isImmuneToDPS)
			{
				int num2 = SceneryManager.AmITakingSceneryDPS(humanPlayer.getWhereBodyIs(), humanPlayer.getWhereFeetAre(), humanPlayer.PROPERTIES.isCrouching, humanPlayer.PROPERTIES.isKicking);
				if (num2 != 0)
				{
					humanPlayer.healthChange((float)(-num2) / (float)Definitions.UpdatesPerSecond);
				}
			}
			if (humanPlayer.PROPERTIES.momentum != Vector2.Zero)
			{
				humanPlayer.move(humanPlayer.PROPERTIES.momentum.X, humanPlayer.PROPERTIES.momentum.Y);
				humanPlayer.PROPERTIES.momentum.X = humanPlayer.PROPERTIES.momentum.X / 2f;
				humanPlayer.PROPERTIES.momentum.Y = humanPlayer.PROPERTIES.momentum.Y / 2f;
				if (humanPlayer.PROPERTIES.momentum.X < 10f && humanPlayer.PROPERTIES.momentum.Y < 10f)
				{
					humanPlayer.PROPERTIES.momentum = Vector2.Zero;
				}
			}
			if (humanPlayer.PROPERTIES.velocity != Vector2.Zero)
			{
				humanPlayer.move(humanPlayer.PROPERTIES.velocity.X / (float)Definitions.UpdatesPerSecond, humanPlayer.PROPERTIES.velocity.Y / (float)Definitions.UpdatesPerSecond);
				if (RandomStaticGlobals.GameMode == Definitions.GameMode.runner && !humanPlayer.PROPERTIES.isCrouching && !humanPlayer.PROPERTIES.isKicking)
				{
					humanPlayer.PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: false);
				}
				if (humanPlayer.PROPERTIES.velocity.X >= 0f)
				{
					humanPlayer.PROPERTIES.isFacing = Definitions.facing.right;
				}
				else
				{
					humanPlayer.PROPERTIES.isFacing = Definitions.facing.left;
				}
			}
		}
		if (num == 0)
		{
			ScreenManager.GameOver();
		}
	}

	public static void PickupOrThrowObject(FighterObject f, float throwX, float throwY)
	{
		if (f.PROPERTIES.holdingObstacleObject != null)
		{
			int iD = f.PROPERTIES.holdingObstacleObject.ID;
			ObstacleManager.Obstacles[iD].Y += (int)(throwY * 1f * (float)ObstacleManager.Obstacles[iD].height);
			ObstacleManager.Obstacles[iD].pixelsInTheAir = f.height + ObstacleManager.Obstacles[iD].height;
			ObstacleManager.Obstacles[iD].isFalling = true;
			ObstacleManager.Obstacles[iD].X += (int)((double)(throwX * (float)ObstacleManager.Obstacles[iD].width) * 0.7);
			if (f.PROPERTIES.isFacing == Definitions.facing.right)
			{
				ObstacleManager.Obstacles[iD].X += f.width + ObstacleManager.Obstacles[iD].width;
			}
			else if (f.PROPERTIES.isFacing == Definitions.facing.left)
			{
				ObstacleManager.Obstacles[iD].X -= f.width + ObstacleManager.Obstacles[iD].width;
			}
			ObstacleManager.Obstacles[iD].isBeingCarriedBy = null;
			f.PROPERTIES.holdingObstacleObject = null;
		}
		else
		{
			if (f.PROPERTIES.holdingObstacleObject != null)
			{
				return;
			}
			float num = 1000000f;
			List<ObstacleObject> pickupableObjects = ObstacleManager.getPickupableObjects(f.getPersonalSpace());
			int num2 = -1;
			if (pickupableObjects != null && pickupableObjects.Count > 0)
			{
				if (pickupableObjects.Count == 1)
				{
					num2 = pickupableObjects[0].ID;
				}
				else
				{
					foreach (ObstacleObject item in pickupableObjects)
					{
						float num3 = RandomStaticGlobals.makePositive((float)f.X - item.X) + RandomStaticGlobals.makePositive((float)f.Y + item.Y);
						if (num3 < num)
						{
							num = RandomStaticGlobals.makePositive((float)f.X - item.X) + RandomStaticGlobals.makePositive((float)f.Y + item.Y);
							num2 = item.ID;
						}
					}
				}
			}
			if (num2 != -1)
			{
				ObstacleManager.Obstacles[num2].isBeingCarriedBy = f;
				f.PROPERTIES.holdingObstacleObject = ObstacleManager.Obstacles[num2];
			}
		}
	}

	public static FighterObject getHumanPlayerWhoseMostRight(bool onlyLiving, bool canBeDying)
	{
		if (humanPlayers.Count == 1)
		{
			return null;
		}
		List<FighterObject> list = getHumanPlayers(onlyLiving, canBeDying);
		if (list.Count == 0)
		{
			return null;
		}
		FighterObject fighterObject = list[0];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].X > fighterObject.X)
			{
				fighterObject = list[i];
			}
		}
		return fighterObject;
	}

	public static FighterObject getHumanPlayerWhoseMostRightForNetworking()
	{
		FighterObject fighterObject = null;
		if (humanPlayers.Count == 1)
		{
			return null;
		}
		List<FighterObject> list = getHumanPlayers(onlyLiving: true, canBeDying: false);
		if (list.Count == 0)
		{
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].PROPERTIES.isLocal)
			{
				if (fighterObject == null)
				{
					fighterObject = list[i];
				}
				else if (list[i].X > fighterObject.X)
				{
					fighterObject = list[i];
				}
			}
		}
		if (fighterObject != null)
		{
			return fighterObject;
		}
		if (fighterObject == null)
		{
			fighterObject = list[0];
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].X > fighterObject.X)
			{
				fighterObject = list[i];
			}
		}
		return fighterObject;
	}

	public static List<FighterObject> getPvPEnemies(bool onlyLiving, bool canBeDying, int myNameIs)
	{
		List<FighterObject> list = new List<FighterObject>(humanPlayers.Count + computerPlayers.Count);
		foreach (FighterObject humanPlayer in humanPlayers)
		{
			if ((humanPlayer.PROPERTIES.isAlive || (canBeDying && humanPlayer.PROPERTIES.isDying)) && humanPlayer.ID != myNameIs)
			{
				list.Add(humanPlayer);
			}
		}
		list.AddRange(getComputerPlayers(onlyLiving, canBeDying));
		return list;
	}

	public static List<FighterObject> getHumanPlayersLocal(bool onlyLiving)
	{
		List<FighterObject> list = new List<FighterObject>(humanPlayers.Count);
		foreach (FighterObject humanPlayer in humanPlayers)
		{
			if (humanPlayer.PROPERTIES.isAlive == onlyLiving && humanPlayer.PROPERTIES.isLocal)
			{
				list.Add(humanPlayer);
			}
		}
		return list;
	}

	public static void ClearHighScores()
	{
		foreach (FighterObject humanPlayer in humanPlayers)
		{
			Dictionary<Definitions.FighterSpecialMoves, int> attacksMade = humanPlayer.PROPERTIES.HumanProfile.AttacksMade;
			Dictionary<Definitions.FighterSpecialMoves, int> attackLevels = humanPlayer.PROPERTIES.HumanProfile.AttackLevels;
			humanPlayer.PROPERTIES.HumanProfile = new HumanProfileObject();
			humanPlayer.PROPERTIES.HumanProfile.AttacksMade = attacksMade;
			humanPlayer.PROPERTIES.HumanProfile.AttackLevels = attackLevels;
		}
	}

	public static FighterObject FindTheClosestFighter(FighterObject you, List<FighterObject> fighters)
	{
		int num = 10000;
		FighterObject result = null;
		foreach (FighterObject fighter in fighters)
		{
			if (fighter.getPersonalSpace().Intersects(you.getPersonalSpace()))
			{
				float num2 = RandomStaticGlobals.makePositive(fighter.X - you.X) + RandomStaticGlobals.makePositive(fighter.Y + you.Y);
				if (num2 < (float)num)
				{
					result = fighter;
				}
			}
		}
		return result;
	}

	public static List<FighterObject> getHumanPlayers(bool onlyLiving, bool canBeDying)
	{
		if (!onlyLiving)
		{
			return humanPlayers;
		}
		List<FighterObject> list = new List<FighterObject>(humanPlayers.Count);
		foreach (FighterObject humanPlayer in humanPlayers)
		{
			if ((!onlyLiving || humanPlayer.PROPERTIES.isAlive) && (canBeDying || !humanPlayer.PROPERTIES.isDying))
			{
				list.Add(humanPlayer);
			}
		}
		return list;
	}

	public static void AddComputerPlayer(FighterObject fo, int AIspeed, int AIdistance, string AIMemory)
	{
		fo.PROPERTIES.AIAmountDistance = AIdistance;
		fo.PROPERTIES.AIAmountSpeed = AIspeed;
		fo.PROPERTIES.AIMemory = AIMemory;
		fo.ID = computerPlayers.Count;
		fo.PROPERTIES.isAlive = true;
		AdjustComputerXYIfCollision(ref fo);
		computerPlayers.Add(fo);
	}

	public static void AdjustComputerXYIfCollision(ref FighterObject fo)
	{
		int x = fo.X;
		int y = fo.Y;
		for (int i = 1; i < 100; i++)
		{
			if (!doCollisionCheckOnCPUs(fo))
			{
				break;
			}
			fo.X = x - fo.width * i - 10;
			fo.Y = y;
			if (!doCollisionCheckOnCPUs(fo))
			{
				break;
			}
			fo.Y = y + (int)((double)fo.height * 0.5) + 10;
			if (!doCollisionCheckOnCPUs(fo))
			{
				break;
			}
			fo.Y = y - (int)((double)fo.height * 0.5) - 10;
			if (!doCollisionCheckOnCPUs(fo))
			{
				break;
			}
			fo.X = x + fo.width * i + 10;
			fo.Y = y;
			if (!doCollisionCheckOnCPUs(fo))
			{
				break;
			}
			fo.Y = y + (int)((double)fo.height * 0.5) + 10;
			if (!doCollisionCheckOnCPUs(fo))
			{
				break;
			}
			fo.Y = y - (int)((double)fo.height * 0.5) - 10;
			if (!doCollisionCheckOnCPUs(fo))
			{
				break;
			}
		}
		if (x != fo.X || y != fo.Y)
		{
			string text = "breakitdown";
		}
	}

	public static void AddComputerPlayer(FighterObject fo)
	{
		fo.ID = computerPlayers.Count;
		fo.PROPERTIES.isDying = false;
		AdjustComputerXYIfCollision(ref fo);
		computerPlayers.Add(fo);
	}

	public static List<FighterObject> getComputerPlayers(bool onlyLiving, bool canBeDying)
	{
		if (!onlyLiving && canBeDying)
		{
			return computerPlayers;
		}
		List<FighterObject> list = new List<FighterObject>(computerPlayers.Count);
		foreach (FighterObject computerPlayer in computerPlayers)
		{
			if (computerPlayer.PROPERTIES.isAlive || (canBeDying && computerPlayer.PROPERTIES.isDying))
			{
				list.Add(computerPlayer);
			}
		}
		return list;
	}

	public static List<FighterObject> getComputerPlayersForFlappy()
	{
		bool flag = false;
		bool flag2 = false;
		List<FighterObject> list = new List<FighterObject>(15);
		for (int i = 0; i < computerPlayers.Count; i++)
		{
			if (flag2)
			{
				break;
			}
			if (computerPlayers[i].X < GraphicsManager.viewableArea.X + 5000 && computerPlayers[i].X > GraphicsManager.viewableArea.X - 5000)
			{
				flag = true;
				list.Add(computerPlayers[i]);
			}
			else if (flag)
			{
				flag2 = true;
			}
		}
		return list;
	}

	public static int doProjectileDamage(Rectangle rect, FighterObject shooter, List<FighterObject> fighters, bool justOneVictim, ProjectileObject po)
	{
		int num = 0;
		if (po.type != ProjectileManager.ProjectileType.skull)
		{
			rect.Width = (int)((float)rect.Width * 0.9f);
			rect.Width++;
			rect.Height = (int)((float)rect.Height * 0.9f);
			rect.Height++;
		}
		foreach (FighterObject fighter in fighters)
		{
			if (fighter.getWhereBodyIs().Intersects(rect))
			{
				po.shooter.PROPERTIES.HumanProfile.shotsMade++;
				shooter.PROPERTIES.CountAttack(po.currentAttack, 1);
				if (fighter.hitMeRanged(shooter, po))
				{
					return 0;
				}
				num++;
				if (justOneVictim)
				{
					SoundManager.playNextSplatStereo(0.5f);
					return 1;
				}
			}
		}
		if (num > 0)
		{
			SoundManager.playNextSplatStereo(0.5f);
		}
		return num;
	}

	public static int doObstaclePush(Rectangle r, Vector2 velocity, bool hitHumans, bool hitCPUs, ObstacleObject o)
	{
		int num = 0;
		if (hitHumans)
		{
			foreach (FighterObject humanPlayer in humanPlayers)
			{
				if (!humanPlayer.PROPERTIES.isAlive || !r.Intersects(humanPlayer.getWhereBodyIs()))
				{
					continue;
				}
				num++;
				if (o != null)
				{
					if (velocity.Y < 0f && (float)humanPlayer.Y < o.Y)
					{
						humanPlayer.Y = (int)o.Y - humanPlayer.height - 2;
					}
					if (velocity.Y > 0f && (float)humanPlayer.Y > o.Y)
					{
						humanPlayer.Y = (int)o.Y + o.height + 2;
					}
					if (velocity.X < 0f && (float)humanPlayer.X < o.X)
					{
						humanPlayer.X = (int)o.X - humanPlayer.width - 2;
					}
					if (velocity.X > 0f && (float)humanPlayer.X > o.X + (float)o.width)
					{
						humanPlayer.X = (int)o.X + o.width + 2;
					}
				}
				humanPlayer.PROPERTIES.pushingObstacleObject = o;
				if (humanPlayer.willThisMoveCollide().HasValue)
				{
					humanPlayer.onDeath();
				}
				humanPlayer.PROPERTIES.pushingObstacleObject = null;
				humanPlayer.move(velocity.X, velocity.Y);
			}
		}
		if (hitCPUs)
		{
			foreach (FighterObject computerPlayer in computerPlayers)
			{
				if (!computerPlayer.PROPERTIES.isAlive || !r.Intersects(computerPlayer.getWhereBodyIs()))
				{
					continue;
				}
				num++;
				if (o != null)
				{
					if (velocity.Y < 0f && (float)computerPlayer.Y < o.Y)
					{
						computerPlayer.Y = (int)o.Y - computerPlayer.height - 2;
					}
					if (velocity.Y > 0f && (float)computerPlayer.Y > o.Y)
					{
						computerPlayer.Y = (int)o.Y + o.height + 2;
					}
					if (velocity.X < 0f && (float)computerPlayer.X < o.X)
					{
						computerPlayer.X = (int)o.X - computerPlayer.width - 2;
					}
					if (velocity.X > 0f && (float)computerPlayer.X > o.X + (float)o.width)
					{
						computerPlayer.X = (int)o.X + o.width + 2;
					}
				}
				computerPlayer.PROPERTIES.pushingObstacleObject = o;
				if (computerPlayer.willThisMoveCollide().HasValue)
				{
					computerPlayer.onDeath();
				}
				computerPlayer.PROPERTIES.pushingObstacleObject = null;
				computerPlayer.X += (int)velocity.X;
				computerPlayer.Y += (int)velocity.Y;
			}
		}
		return num;
	}

	public static int doObstacleRollingDamage(ObstacleObject o)
	{
		int num = 0;
		Rectangle rect = o.rect;
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (humanPlayers[i].PROPERTIES.isAlive && rect.Intersects(humanPlayers[i].getWhereBodyIs()))
			{
				num++;
				humanPlayers[i].hitMeWithObstacle(o);
			}
		}
		for (int i = 0; i < computerPlayers.Count; i++)
		{
			if (computerPlayers[i].PROPERTIES.isAlive && rect.Intersects(computerPlayers[i].getWhereBodyIs()))
			{
				num++;
				computerPlayers[i].hitMeWithObstacle(o);
			}
		}
		return num;
	}

	public static void doStunAOE(FighterObject fighter, List<FighterObject> enemies)
	{
		foreach (FighterObject enemy in enemies)
		{
			if (fighter.getWhereFistIs().Intersects(enemy.getWhereBodyIs()))
			{
				enemy.stunMe();
			}
		}
	}

	public static void RemakeHumanPlayersForCustoms()
	{
		string customPlayerAnimation = CustomsManager.GetCustomPlayerAnimation();
		FighterObject fighterObject = new FighterObject();
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			humanPlayers[i] = remakeHumanPlayer(humanPlayers[i]);
		}
	}

	public static FighterObject addNewHumanPlayer(PlayerIndex? playerIndex, bool isNetworkPlayer, string gamerTag, float scale)
	{
		FighterObject fighterObject = makeNewHumanPlayer(playerIndex, isNetworkPlayer, gamerTag, scale);
		fighterObject.PROPERTIES.isLocal = !isNetworkPlayer;
		humanPlayers.Add(fighterObject);
		return fighterObject;
	}

	public static FighterObject remakeHumanPlayer(FighterObject foo)
	{
		HumanProfileObject humanProfile = foo.PROPERTIES.HumanProfile;
		bool isLocal = foo.PROPERTIES.isLocal;
		int x = foo.X;
		int y = foo.Y;
		bool isAlive = foo.PROPERTIES.isAlive;
		foo = makeNewHumanPlayer(foo.PROPERTIES.PlayerIndexControllerNumber, foo.PROPERTIES.isNetworkPlayer, foo.PROPERTIES.GamerTag, foo.PROPERTIES.scale);
		foo.PROPERTIES.HumanProfile = humanProfile;
		foo.PROPERTIES.isLocal = isLocal;
		foo.X = x;
		foo.Y = y;
		foo.PROPERTIES.isAlive = isAlive;
		return foo;
	}

	public static FighterObject makeNewHumanPlayer(PlayerIndex? playerIndex, bool isNetworkPlayer, string gamerTag, float scale)
	{
		FighterObject fighterObject = new FighterObject();
		string customPlayerAnimation = CustomsManager.GetCustomPlayerAnimation();
		if (customPlayerAnimation == "")
		{
			fighterObject = createNewFlappyMonkey(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 100, 800, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, scale);
		}
		else if (customPlayerAnimation == "ImAFuckingSHARK" || customPlayerAnimation == "SharkyShark")
		{
			fighterObject = createNewSharkyShark(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 100, 800, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, scale);
		}
		else
		{
			switch (customPlayerAnimation)
			{
			case "UFO":
				fighterObject = createNewUFO(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 100, 800, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, scale);
				break;
			case "BoothBabe":
				fighterObject = createNewBoothBabe(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 100, 800, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, scale);
				break;
			case "FlappyMonkey":
				fighterObject = createNewFlappyMonkey(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 100, 800, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, scale);
				break;
			case "Target":
				fighterObject = createNewTarget(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 100, 800, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, scale);
				break;
			case "UFOsmall":
				fighterObject = createNewUFO(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, 100, 800, isAlive: true, BunnyOfWar.AI.AI.modes.doNothing, 0.3f);
				break;
			}
		}
		fighterObject.PROPERTIES.CustomAnimationName = customPlayerAnimation;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = playerIndex;
		fighterObject.PROPERTIES.areWeHuman = true;
		fighterObject.PROPERTIES.HumanProfile = new HumanProfileObject();
		fighterObject.PROPERTIES.isNetworkPlayer = isNetworkPlayer;
		fighterObject.PROPERTIES.GamerTag = gamerTag;
		return fighterObject;
	}

	public static FighterObject createNewPooTarget(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale, string optionalCustomFighter, string uniqueName)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.PROPERTIES.uniqueName = uniqueName;
		fighterObject.setCollisionRects(boundaries, new Rectangle(100, 100, 300, 300), new Rectangle(100, 100, 300, 300), new Rectangle(100, 100, 300, 300));
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.bullet;
		fighterObject.RANGED.rangedDamage = 5;
		fighterObject.RANGED.rangedOrigin = new Vector2(150f, 90f);
		fighterObject.PROPERTIES.DamageFromAttack = Definitions.damageFromShark;
		fighterObject.PROPERTIES.DamageFromQuickAttack = Definitions.damageFromShark;
		string text = Definitions.ContentRootDirectory + "/fighters/Vegan";
		if (optionalCustomFighter != "")
		{
			text = Definitions.ContentRootDirectory + "/fighters/" + optionalCustomFighter;
		}
		fighterObject.animationIdle = new Animation(new string[1] { text + "/idle.png" }, 0.8f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[2]
		{
			text + "/idle.png",
			text + "/idle.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[2]
		{
			text + "/idle.png",
			text + "/idle.png"
		}, 0.3f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(new string[1] { text + "/idle.png" }, 0.225f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/dead.png", 0.1f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = 50f;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedSharkySHark;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static FighterObject createNewSomethingRed(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.setCollisionRects(boundaries, new Rectangle(0, 0, 122, 71), new Rectangle(0, 0, 122, 71), new Rectangle(0, 0, 122, 71));
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.bullet;
		fighterObject.RANGED.rangedDamage = 5;
		fighterObject.RANGED.rangedOrigin = new Vector2(150f, 90f);
		fighterObject.PROPERTIES.DamageFromAttack = Definitions.damageFromShark;
		fighterObject.PROPERTIES.DamageFromQuickAttack = Definitions.damageFromShark;
		string text = Definitions.ContentRootDirectory + "/fighters/SomethingRed";
		fighterObject.animationIdle = new Animation(new string[1] { text + "/idle.png" }, 0.8f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[2]
		{
			text + "/idle.png",
			text + "/idle.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[2]
		{
			text + "/idle.png",
			text + "/idle.png"
		}, 0.3f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(new string[1] { text + "/idle.png" }, 0.225f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/dead.png", 0.1f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = 1f;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedSharkySHark;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static FighterObject createNewPoliceCar(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.setCollisionRects(boundaries, new Rectangle(50, 0, 200, 120), new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0));
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.bullet;
		fighterObject.RANGED.rangedDamage = 5;
		fighterObject.RANGED.rangedOrigin = new Vector2(150f, 90f);
		fighterObject.PROPERTIES.DamageFromAttack = Definitions.damageFromShark;
		fighterObject.PROPERTIES.DamageFromQuickAttack = Definitions.damageFromShark;
		string text = Definitions.ContentRootDirectory + "/fighters/FlappyPolice";
		fighterObject.animationIdle = new Animation(new string[1] { text + "/idle.png" }, 0.8f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[2]
		{
			text + "/idle.png",
			text + "/idle.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[2]
		{
			text + "/idle.png",
			text + "/idle.png"
		}, 0.3f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(new string[1] { text + "/idle.png" }, 0.225f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/idle.png", 0.1f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = Definitions.HPSharkyShark;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedSharkySHark;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static FighterObject createNewSharkyShark(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.setCollisionRects(boundaries, new Rectangle(50, 0, 200, 120), new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0));
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.bullet;
		fighterObject.RANGED.rangedDamage = 5;
		fighterObject.RANGED.rangedOrigin = new Vector2(150f, 90f);
		fighterObject.PROPERTIES.DamageFromAttack = Definitions.damageFromShark;
		fighterObject.PROPERTIES.DamageFromQuickAttack = Definitions.damageFromShark;
		string text = Definitions.ContentRootDirectory + "/fighters/SharkyShark";
		fighterObject.animationIdle = new Animation(new string[1] { text + "/idle.png" }, 0.8f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[2]
		{
			text + "/attack.png",
			text + "/attack.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[2]
		{
			text + "/attack.png",
			text + "/attack.png"
		}, 0.3f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(new string[4]
		{
			text + "/walk1.png",
			text + "/walk2.png",
			text + "/walk3.png",
			text + "/walk2.png"
		}, 0.225f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/death.png", 0.1f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = Definitions.HPSharkyShark;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedSharkySHark;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static FighterObject createNewAlligator(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.setCollisionRects(boundaries, new Rectangle(50, 250, 950, 200), new Rectangle(50, 50, 200, 350), new Rectangle(200, 300, 300, 50));
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.skull;
		fighterObject.RANGED.rangedDamage = 5;
		fighterObject.RANGED.rangedOrigin = Vector2.Zero;
		fighterObject.PROPERTIES.DamageFromAttack = Definitions.damageFromAlligator;
		fighterObject.PROPERTIES.DamageFromQuickAttack = Definitions.damageFromAlligator;
		string text = Definitions.ContentRootDirectory + "/fighters/Alligator";
		fighterObject.animationIdle = new Animation(new string[1] { text + "/idle.png" }, 0.8f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[5]
		{
			text + "/attack1.png",
			text + "/attack2.png",
			text + "/attack3.png",
			text + "/attack4.png",
			text + "/attack5.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[1] { text + "/attack1.png" }, 0.3f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(new string[4]
		{
			text + "/walk1.png",
			text + "/walk2.png",
			text + "/walk3.png",
			text + "/walk4.png"
		}, 0.225f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/idle.png", 0.1f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = Definitions.HPAlligator;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedAlligator;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static FighterObject createNewUFO(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.setCollisionRects(boundaries, new Rectangle(200, 200, 100, 100), new Rectangle(200, 200, 100, 100), new Rectangle(200, 200, 100, 100));
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedUFO;
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.skull;
		fighterObject.RANGED.rangedDamage = 5;
		fighterObject.RANGED.rangedOrigin = new Vector2(250f, 300f);
		fighterObject.PROPERTIES.DamageFromAttack = Definitions.BombDamageUFO;
		fighterObject.PROPERTIES.DamageFromQuickAttack = Definitions.BulletDamageUFO;
		string text = Definitions.ContentRootDirectory + "/fighters/UFO";
		fighterObject.animationIdle = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.8f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.225f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(text + "/idle.png", 0.1f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/idle.png", 0.1f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = Definitions.HPUFO;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.isAlive = false;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static FighterObject createNewTarget(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.setCollisionRects(boundaries, new Rectangle(100, 100, 156, 156), new Rectangle(100, 100, 156, 156), new Rectangle(100, 100, 156, 156));
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedUFO;
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.skull;
		fighterObject.RANGED.rangedDamage = 5;
		fighterObject.RANGED.rangedOrigin = new Vector2(0f, 60f);
		fighterObject.PROPERTIES.DamageFromAttack = Definitions.BombDamageUFO;
		fighterObject.PROPERTIES.DamageFromQuickAttack = Definitions.BulletDamageUFO;
		string text = Definitions.ContentRootDirectory + "/fighters/Target";
		fighterObject.animationIdle = new Animation(new string[1] { text + "/idle.png" }, 1f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.225f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(text + "/idle.png", 1f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/idle.png", 1f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = Definitions.HPUFO;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.isAlive = false;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static FighterObject createNewFlappyMonkey(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.setCollisionRects(boundaries, new Rectangle(40, 40, 176, 176), new Rectangle(40, 40, 176, 176), new Rectangle(40, 40, 176, 176));
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedUFO;
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.skull;
		fighterObject.RANGED.rangedDamage = 50;
		fighterObject.RANGED.rangedOrigin = new Vector2(120f, 240f);
		fighterObject.PROPERTIES.DamageFromAttack = 100f;
		fighterObject.PROPERTIES.DamageFromQuickAttack = 50f;
		string text = Definitions.ContentRootDirectory + "/fighters/FlappyMonkey";
		fighterObject.animationIdle = new Animation(new string[1] { text + "/idle.png" }, 0.1f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.225f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(text + "/pooing1.png", 0.1f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/idle.png", 1f, isLooping: true, scale);
		fighterObject.animationPooingStart = new Animation(text + "/pooing1.png", 0.5f, isLooping: true, scale);
		fighterObject.animationPooingFinished = new Animation(text + "/pooing2.png", 1.5f, isLooping: true, scale);
		fighterObject.animationJumping = new Animation(new string[2]
		{
			text + "/jumping.png",
			text + "/idle.png"
		}, 0.25f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = Definitions.HPUFO;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.isAlive = false;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static FighterObject createNewBoothBabe(ContentManager Content, Rectangle boundaries, int X, int Y, bool isAlive, BunnyOfWar.AI.AI.modes aiMode, float scale)
	{
		FighterObject fighterObject = new FighterObject(Content);
		fighterObject.setCollisionRects(boundaries, new Rectangle(0, 400, 512, 125), new Rectangle(0, 400, 512, 125), new Rectangle(0, 400, 512, 125));
		fighterObject.PROPERTIES.moveSpeed = Definitions.MoveSpeedUFO;
		fighterObject.PROPERTIES.isAlive = isAlive;
		fighterObject.PROPERTIES.AImode = aiMode;
		fighterObject.RANGED.rangedProjectileType = ProjectileManager.ProjectileType.skull;
		fighterObject.RANGED.rangedDamage = 5;
		fighterObject.RANGED.rangedOrigin = new Vector2(420f, 460f);
		fighterObject.PROPERTIES.DamageFromAttack = Definitions.BombDamageUFO;
		fighterObject.PROPERTIES.DamageFromQuickAttack = Definitions.BulletDamageUFO;
		string text = Definitions.ContentRootDirectory + "/fighters/BoothBabe";
		fighterObject.animationIdle = new Animation(new string[1] { text + "/idle.png" }, 1f, isLooping: true, scale);
		fighterObject.animationQuickPunching = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.15f, isLooping: false, scale);
		fighterObject.animationPunching = new Animation(new string[3]
		{
			text + "/idle.png",
			text + "/idle.png",
			text + "/idle.png"
		}, 0.225f, isLooping: false, scale);
		fighterObject.animationWalking = new Animation(text + "/idle.png", 1f, isLooping: true, scale);
		fighterObject.animationDying = new Animation(text + "/idle.png", 1f, isLooping: true, scale);
		fighterObject.PROPERTIES.health = Definitions.HPUFO;
		fighterObject.PROPERTIES.healthMax = fighterObject.PROPERTIES.health;
		fighterObject.PROPERTIES.isAlive = false;
		fighterObject.PROPERTIES.PlayerIndexControllerNumber = null;
		fighterObject.X = X;
		fighterObject.Y = Y;
		fighterObject.rectSpriteDisplay.Width = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.rectSpriteDisplay.Height = (int)((float)fighterObject.animationIdle.FrameHeight * scale);
		fighterObject.PROPERTIES.scale = scale;
		fighterObject.PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		return fighterObject;
	}

	public static string ExportData()
	{
		string text = "";
		foreach (FighterObject computerPlayer in computerPlayers)
		{
			if (computerPlayer.PROPERTIES.name != "he who has no name")
			{
				text += string.Format("type=BadGuy;x={0};y={1};name={2};isAlive={3};AI={4};uniqueName={5};scale={6};AIspeed={7};AIdistance={8}" + Environment.NewLine, computerPlayer.X, computerPlayer.Y, computerPlayer.PROPERTIES.name, computerPlayer.PROPERTIES.isAlive, computerPlayer.PROPERTIES.AImode.ToString(), computerPlayer.PROPERTIES.uniqueName, computerPlayer.PROPERTIES.scale.ToString(), computerPlayer.PROPERTIES.AIAmountSpeed.ToString(), computerPlayer.PROPERTIES.AIAmountDistance.ToString());
			}
		}
		return text;
	}

	public static void ImportData(string data)
	{
		computerPlayers.Clear();
		InitComputerPlayers();
		string[] array = data.Split(Environment.NewLine.ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].StartsWith("type=BadGuy"))
			{
				FighterObject fighterObject = convertTextLineToFighterObject(array[i]);
				if (fighterObject.PROPERTIES.name != "he who has no name")
				{
					computerPlayers.Add(fighterObject);
				}
			}
		}
		for (int i = 0; i < computerPlayers.Count; i++)
		{
			computerPlayers[i].ID = i;
		}
	}

	private static FighterObject convertTextLineToFighterObject(string s)
	{
		s = s.Trim();
		FighterObject fighterObject = new FighterObject();
		string[] array = s.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('=');
			switch (array2[0])
			{
			case "x":
				fighterObject.X = int.Parse(array2[1]);
				break;
			case "y":
				fighterObject.Y = int.Parse(array2[1]);
				break;
			case "name":
				fighterObject.PROPERTIES.name = array2[1];
				break;
			case "isAlive":
				if (array2[1] == "True")
				{
					fighterObject.PROPERTIES.isAlive = true;
				}
				else if (array2[1] == "False")
				{
					fighterObject.PROPERTIES.isAlive = false;
				}
				break;
			case "AI":
				fighterObject.PROPERTIES.AImode = (BunnyOfWar.AI.AI.modes)Enum.Parse(typeof(BunnyOfWar.AI.AI.modes), array2[1], ignoreCase: true);
				break;
			case "AIspeed":
				fighterObject.PROPERTIES.AIAmountSpeed = int.Parse(array2[1]);
				break;
			case "AIdistance":
				fighterObject.PROPERTIES.AIAmountDistance = int.Parse(array2[1]);
				break;
			case "uniqueName":
				fighterObject.PROPERTIES.uniqueName = array2[1];
				break;
			case "scale":
				fighterObject.PROPERTIES.scale = float.Parse(array2[1].Replace(",", "."));
				break;
			}
		}
		FighterObject fighterObject2 = MakeNewFighterObjectFromName(fighterObject.PROPERTIES.name, fighterObject);
		fighterObject2.PROPERTIES.name = fighterObject.PROPERTIES.name;
		fighterObject2.PROPERTIES.uniqueName = fighterObject.PROPERTIES.uniqueName;
		return fighterObject2;
	}

	public static FighterObject MakeNewFighterObjectFromName(string name, FighterObject fo)
	{
		FighterObject fighterObject = new FighterObject();
		switch (name)
		{
		case "SharkyShark":
			fighterObject = createNewSharkyShark(LevelManager.Content, GraphicsManager.BoundariesDefault, fo.X, fo.Y, fo.PROPERTIES.isAlive, fo.PROPERTIES.AImode, fo.PROPERTIES.scale);
			break;
		case "Alligator":
			fighterObject = createNewAlligator(LevelManager.Content, GraphicsManager.BoundariesDefault, fo.X, fo.Y, fo.PROPERTIES.isAlive, fo.PROPERTIES.AImode, fo.PROPERTIES.scale);
			break;
		case "PoliceCar":
			fighterObject = createNewPoliceCar(LevelManager.Content, GraphicsManager.BoundariesDefault, fo.X, fo.Y, fo.PROPERTIES.isAlive, fo.PROPERTIES.AImode, fo.PROPERTIES.scale);
			break;
		case "FPSPoliceHelicopter":
			fighterObject = createNewSomethingRed(LevelManager.Content, GraphicsManager.BoundariesDefault, fo.X, fo.Y, fo.PROPERTIES.isAlive, fo.PROPERTIES.AImode, 0.1f);
			break;
		case "FPSPoliceCar":
			fighterObject = createNewSomethingRed(LevelManager.Content, GraphicsManager.BoundariesDefault, fo.X, fo.Y, fo.PROPERTIES.isAlive, fo.PROPERTIES.AImode, 0.1f);
			break;
		}
		fighterObject.PROPERTIES.AIAmountDistance = fo.PROPERTIES.AIAmountDistance;
		fighterObject.PROPERTIES.AIAmountSpeed = fo.PROPERTIES.AIAmountSpeed;
		fighterObject.PROPERTIES.name = name;
		return fighterObject;
	}

	public static void ClearNonPlayerData()
	{
		computerPlayers.Clear();
	}

	public static void ClearData()
	{
		ClearNonPlayerData();
		humanPlayers.Clear();
	}

	public static void adjustScreenViewableAreaForPlayerOne()
	{
		adjustScreenViewableAreaForThisPlayer(humanPlayers[0], ref GraphicsManager.viewableArea);
	}

	public static void adjustScreenViewableAreaForThisPlayer(FighterObject fighter, ref Rectangle viewableArea)
	{
		if (RandomStaticGlobals.isSkullSlingshotMode)
		{
			return;
		}
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.runner || RandomStaticGlobals.GameMode == Definitions.GameMode.driver || RandomStaticGlobals.GameMode == Definitions.GameMode.swimmer || RandomStaticGlobals.GameMode == Definitions.GameMode.redbaron || RandomStaticGlobals.GameMode == Definitions.GameMode.helicopter)
		{
			viewableArea.X = fighter.X - 100;
			return;
		}
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.flappy)
		{
			viewableArea.X = fighter.X - 500;
			return;
		}
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.flappychase)
		{
			viewableArea.X = fighter.X - 1200;
			return;
		}
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.shooter || RandomStaticGlobals.GameMode == Definitions.GameMode.space)
		{
			viewableArea.X = fighter.X - (GraphicsManager.ScreenWidth - fighter.width) / 2;
			viewableArea.Y = fighter.Y - (GraphicsManager.ScreenHeight - fighter.height) / 2;
			return;
		}
		int num = 400;
		if (fighter.X + fighter.width + num > viewableArea.X + viewableArea.Width)
		{
			viewableArea.X = fighter.X + fighter.width + num - viewableArea.Width;
		}
		if (fighter.X - num < viewableArea.X)
		{
			viewableArea.X = fighter.X - num;
		}
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.zelda || RandomStaticGlobals.GameMode == Definitions.GameMode.shooter || RandomStaticGlobals.GameMode == Definitions.GameMode.space || RandomStaticGlobals.GameMode == Definitions.GameMode.gunsmoke)
		{
			if (fighter.Y + fighter.height + num > viewableArea.Y + viewableArea.Height)
			{
				viewableArea.Y = fighter.Y + fighter.height + num - viewableArea.Height;
			}
			if (fighter.Y - num < viewableArea.Y)
			{
				viewableArea.Y = fighter.Y - num;
			}
		}
	}

	public static void InitComputerPlayers()
	{
		for (int i = 0; i < computerPlayers.Capacity; i++)
		{
		}
	}

	public static void SetZeroGravityForHumans(bool on)
	{
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (on)
			{
				humanPlayers[i].JUMP.gravityImmunity = DateTime.MaxValue;
				humanPlayers[i].PROPERTIES.isFlying = true;
			}
			else
			{
				humanPlayers[i].JUMP.gravityImmunity = DateTime.MinValue;
				humanPlayers[i].PROPERTIES.isFlying = false;
			}
		}
	}

	public static void SetHumanRandomThings(double? forwardRollSpeed, int? moveSpeed)
	{
		SetHumanRandomThings(forwardRollSpeed, null, moveSpeed);
	}

	public static void SetHumanRandomThings(double? forwardRollSpeed, double? upwardRollSpeed, int? moveSpeed)
	{
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (forwardRollSpeed.HasValue)
			{
				humanPlayers[i].PROPERTIES.velocity.X = (float)forwardRollSpeed.Value;
			}
			if (moveSpeed.HasValue)
			{
				humanPlayers[i].PROPERTIES.moveSpeed = moveSpeed.Value;
			}
			if (upwardRollSpeed.HasValue)
			{
				humanPlayers[i].PROPERTIES.velocity.Y = (float)upwardRollSpeed.Value;
			}
		}
		if (forwardRollSpeed.HasValue)
		{
			RandomStaticGlobals.RollVelocity = new Vector2((float)forwardRollSpeed.Value, 0f);
		}
	}

	public static void AdjustEveryonesRollSpeed(double? forwardRollSpeed, double? upwardRollSpeed, int? moveSpeed)
	{
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (forwardRollSpeed.HasValue)
			{
				humanPlayers[i].PROPERTIES.velocity.X += (float)forwardRollSpeed.Value;
			}
			if (moveSpeed.HasValue)
			{
				humanPlayers[i].PROPERTIES.moveSpeed += moveSpeed.Value;
			}
			if (upwardRollSpeed.HasValue)
			{
				humanPlayers[i].PROPERTIES.velocity.Y += (float)upwardRollSpeed.Value;
			}
		}
		if (forwardRollSpeed.HasValue)
		{
			RandomStaticGlobals.RollVelocity = new Vector2((float)forwardRollSpeed.Value, 0f);
		}
	}

	public static void SetJumpSpeeds(Definitions.GameMode gameMode)
	{
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			switch (gameMode)
			{
			case Definitions.GameMode.none:
				humanPlayers[i].JUMP.jumpMaxAmount = Definitions.DefaultJumpMaxAmount;
				humanPlayers[i].JUMP.jumpMaxAmountSecondTime = Definitions.DefaultJumpMaxAmountSecondTime;
				humanPlayers[i].JUMP.jumpUpSpeed = Definitions.DefaultJumpUpSpeed;
				humanPlayers[i].JUMP.jumpFallSpeed = Definitions.DefaultJumpFallSpeed;
				break;
			case Definitions.GameMode.swimmer:
				humanPlayers[i].JUMP.jumpMaxAmount = Definitions.DefaultJumpUnderwaterMaxAmount;
				humanPlayers[i].JUMP.jumpMaxAmountSecondTime = Definitions.DefaultJumpUnderwaterMaxAmountSecondTime;
				humanPlayers[i].JUMP.jumpUpSpeed = Definitions.DefaultJumpUnderwaterUpSpeed;
				humanPlayers[i].JUMP.jumpFallSpeed = Definitions.DefaultJumpUnderwaterFallSpeed;
				break;
			case Definitions.GameMode.runner:
				humanPlayers[i].JUMP.jumpMaxAmount = 400;
				humanPlayers[i].JUMP.jumpMaxAmountSecondTime = 600;
				humanPlayers[i].JUMP.jumpUpSpeed = Definitions.DefaultJumpUpSpeed;
				humanPlayers[i].JUMP.jumpFallSpeed = (int)humanPlayers[i].PROPERTIES.velocity.X;
				if (humanPlayers[i].JUMP.jumpFallSpeed < 1500)
				{
					humanPlayers[i].JUMP.jumpFallSpeed = 1500;
				}
				break;
			}
		}
	}

	public static void ResetHumanPlayers()
	{
		SetZeroGravityForHumans(on: false);
		SetHumanRandomThings(0.0, Definitions.MoveSpeedHuman);
		SetJumpSpeeds(Definitions.GameMode.none);
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			humanPlayers[i].PROPERTIES.health = humanPlayers[i].PROPERTIES.healthMax;
			humanPlayers[i].PROPERTIES.HumanProfile.stopwatchTimeSpentBlocking.Reset();
			humanPlayers[i].PROPERTIES.HumanProfile.stopwatchTimeSpentPlaying.Reset();
			if (!CustomsManager.LevelCustomizations.ContainsKey(CustomsManager.Customizations.Gigantor))
			{
				humanPlayers[i].PROPERTIES.scale = 1f;
			}
			if (Definitions.Options.Difficulty <= 1)
			{
				humanPlayers[i].PROPERTIES.healthMax = Definitions.HumanHealthEasy;
				humanPlayers[i].PROPERTIES.health = Definitions.HumanHealthEasy;
			}
			else
			{
				humanPlayers[i].PROPERTIES.healthMax = Definitions.HumanHealth;
				humanPlayers[i].PROPERTIES.health = Definitions.HumanHealth;
			}
			humanPlayers[i].PROPERTIES.isAlive = true;
			humanPlayers[i].PROPERTIES.isDying = false;
			humanPlayers[i].PROPERTIES.isStunned = false;
			humanPlayers[i].PROPERTIES.stunExpires = DateTime.MinValue;
			humanPlayers[i].PROPERTIES.carryingFighter = null;
		}
	}

	public static void ResetHumanPlayersXY()
	{
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			humanPlayers[i].X = 400 + humanPlayers[i].ID * 100;
			humanPlayers[i].Y = 100;
			humanPlayers[i].JUMP.jumpPixelsOffGround = 0;
			if (RandomStaticGlobals.isSkullSlingshotMode)
			{
				humanPlayers[i].X = (int)RandomStaticGlobals.SkullSlingshotOrigin[i].X - 50;
				humanPlayers[i].Y = (int)RandomStaticGlobals.SkullSlingshotOrigin[i].Y - 20;
			}
			else if (RandomStaticGlobals.GameMode == Definitions.GameMode.gunsmoke)
			{
				humanPlayers[i].X = 1000 - i * 50;
				humanPlayers[i].Y = 100000 - i * 20;
				GraphicsManager.viewableArea.Y = humanPlayers[i].Y - 500;
				GraphicsManager.viewableArea.X = 0;
			}
		}
	}

	public static void HumanDied(FighterObject fo)
	{
		List<FighterObject> list = getHumanPlayers(onlyLiving: true, canBeDying: false);
		if (list.Count == 0)
		{
			ScreenManager.GameOver();
		}
		if (RandomStaticGlobals.isPvPEnabled && list.Count <= 1)
		{
			ScreenManager.GameOver();
		}
	}

	public static void ActivateNamedObject(string name)
	{
		for (int i = 0; i < computerPlayers.Count; i++)
		{
			if (computerPlayers[i].PROPERTIES.uniqueName == name)
			{
				computerPlayers[i].PROPERTIES.isAlive = true;
			}
		}
	}

	public static bool doCollisionCheckOnCPUs(FighterObject f)
	{
		Rectangle whereFeetAre = f.getWhereFeetAre();
		foreach (FighterObject computerPlayer in computerPlayers)
		{
			Rectangle whereFeetAre2 = computerPlayer.getWhereFeetAre();
			if (computerPlayer != f && computerPlayer.PROPERTIES.isAlive && (whereFeetAre2.Intersects(whereFeetAre) || whereFeetAre2.Contains(whereFeetAre)))
			{
				return true;
			}
		}
		return false;
	}

	public static bool doCollisionCheckAndDamage(FighterObject f)
	{
		if (!CustomsManager.GetIsCollidableWithCPUs())
		{
			return false;
		}
		Rectangle whereBodyIs = f.getWhereBodyIs();
		foreach (FighterObject computerPlayer in computerPlayers)
		{
			Rectangle whereBodyIs2 = computerPlayer.getWhereBodyIs();
			if (computerPlayer != f && computerPlayer.PROPERTIES.isAlive && (whereBodyIs2.Intersects(whereBodyIs) || whereBodyIs2.Contains(whereBodyIs)))
			{
				computerPlayer.healthChange(Definitions.DamageToCPUonCollision);
				f.healthChange(Definitions.DamageToHumanOnCollision);
				return true;
			}
		}
		return false;
	}

	public static void MoveFighters(Rectangle r, float x, float y)
	{
		r.Inflate(0, 10);
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (humanPlayers[i].rectSpriteDisplay.Intersects(r))
			{
				humanPlayers[i].move(x, y);
			}
		}
	}

	public static void BroadcastAnimationChange(int ID, FighterObjectProperties.AnimationName animation, Definitions.facing facing)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)3);
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)ID);
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)animation);
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)facing);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)0);
		}
	}

	public static void ReadAnimationChange(PacketReader pr)
	{
		int index = ((BinaryReader)(object)pr).ReadByte();
		byte animation = ((BinaryReader)(object)pr).ReadByte();
		byte isFacing = ((BinaryReader)(object)pr).ReadByte();
		try
		{
			humanPlayers[index].animateRemotely((FighterObjectProperties.AnimationName)animation, (Definitions.facing)isFacing);
		}
		catch (Exception)
		{
		}
	}

	public static void BroadcastHumanHealth(int playerID, int health)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)4);
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)playerID);
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)health);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)0);
		}
	}

	public static void ReadHumanHealth(PacketReader pr)
	{
		int index = ((BinaryReader)(object)pr).ReadByte();
		byte b = ((BinaryReader)(object)pr).ReadByte();
		try
		{
			humanPlayers[index].PROPERTIES.health = (int)b;
		}
		catch (Exception)
		{
		}
	}

	public static void BroadcastFighterStunned(int playerID, bool areWeHuman, double durationInSeconds)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)21);
			((BinaryWriter)(object)Networking.packetWriter).Write((ushort)playerID);
			((BinaryWriter)(object)Networking.packetWriter).Write(areWeHuman);
			((BinaryWriter)(object)Networking.packetWriter).Write(durationInSeconds);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)0);
		}
	}

	public static void ReadFighterStunned(PacketReader pr)
	{
		int index = ((BinaryReader)(object)pr).ReadUInt16();
		bool flag = ((BinaryReader)(object)pr).ReadBoolean();
		double seconds = ((BinaryReader)(object)pr).ReadDouble();
		try
		{
			if (flag)
			{
				humanPlayers[index].stunMe(seconds, broadcast: false);
			}
			else
			{
				computerPlayers[index].stunMe(seconds, broadcast: false);
			}
		}
		catch (Exception)
		{
		}
	}

	public static void BroadcastAddProjectile(int playerID, int x, int y, Vector2 direction, float speed, bool areWeHuman, int ProjectileTypeWhatsIsIt, int width, int height, int damage)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)7);
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)playerID);
			((BinaryWriter)(object)Networking.packetWriter).Write((uint)x);
			((BinaryWriter)(object)Networking.packetWriter).Write((uint)y);
			Networking.packetWriter.Write(direction);
			((BinaryWriter)(object)Networking.packetWriter).Write((double)speed);
			((BinaryWriter)(object)Networking.packetWriter).Write(areWeHuman);
			((BinaryWriter)(object)Networking.packetWriter).Write((ushort)ProjectileTypeWhatsIsIt);
			((BinaryWriter)(object)Networking.packetWriter).Write((uint)width);
			((BinaryWriter)(object)Networking.packetWriter).Write((uint)height);
			((BinaryWriter)(object)Networking.packetWriter).Write((uint)damage);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)0);
		}
	}

	public static void ReadAddProjectile(PacketReader pr)
	{
		try
		{
			int index = ((BinaryReader)(object)pr).ReadByte();
			int num = (int)((BinaryReader)(object)pr).ReadUInt32();
			int num2 = (int)((BinaryReader)(object)pr).ReadUInt32();
			Vector2 direction = pr.ReadVector2();
			float speed = (float)((BinaryReader)(object)pr).ReadDouble();
			bool flag = ((BinaryReader)(object)pr).ReadBoolean();
			int whatsIsIt = ((BinaryReader)(object)pr).ReadUInt16();
			int width = (int)((BinaryReader)(object)pr).ReadUInt32();
			int height = (int)((BinaryReader)(object)pr).ReadUInt32();
			int damage = (int)((BinaryReader)(object)pr).ReadUInt32();
			if (flag)
			{
				ProjectileManager.addNewProjectile(num, num2, direction, speed, flag, humanPlayers[index], (ProjectileManager.ProjectileType)whatsIsIt, width, height, damage, broadcast: false);
			}
			else
			{
				ProjectileManager.addNewProjectile(num, num2, direction, speed, flag, computerPlayers[index], (ProjectileManager.ProjectileType)whatsIsIt, width, height, damage, broadcast: false);
			}
			humanPlayers[index].RANGED.rangedRelease(num, num2);
		}
		catch (Exception)
		{
		}
	}

	public static void BroadcastRangedAttack(int playerID, float x, float y)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)6);
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)playerID);
			((BinaryWriter)(object)Networking.packetWriter).Write((double)x);
			((BinaryWriter)(object)Networking.packetWriter).Write((double)y);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)0);
		}
	}

	public static void ReadRangedAttack(PacketReader pr)
	{
		try
		{
			int index = ((BinaryReader)(object)pr).ReadByte();
			float x = (float)((BinaryReader)(object)pr).ReadDouble();
			float y = (float)((BinaryReader)(object)pr).ReadDouble();
			humanPlayers[index].RANGED.rangedRelease(x, y);
		}
		catch (Exception)
		{
		}
	}

	public static void BroadcastFighterDeath(int ID, bool areWeHuman)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)19);
			((BinaryWriter)(object)Networking.packetWriter).Write((ushort)ID);
			((BinaryWriter)(object)Networking.packetWriter).Write(areWeHuman);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)1);
		}
	}

	public static void ReadFighterDeath(PacketReader pr)
	{
		int iD = ((BinaryReader)(object)pr).ReadUInt16();
		bool human = ((BinaryReader)(object)pr).ReadBoolean();
		try
		{
			KillFighter(iD, human);
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static void KillFighter(int ID, bool human)
	{
		if (!human)
		{
			for (int i = 0; i < computerPlayers.Count; i++)
			{
				if (computerPlayers[i].ID == ID)
				{
					computerPlayers[i].onDeath(broadcast: false);
				}
			}
			return;
		}
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (humanPlayers[i].ID == ID)
			{
				humanPlayers[i].PROPERTIES.health = 0f;
				humanPlayers[i].PROPERTIES.isAlive = false;
				humanPlayers[i].PROPERTIES.isDying = false;
			}
		}
	}

	public static void BroadcastComputerHealthChange(int ID, int healthChangeAmount)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)5);
			((BinaryWriter)(object)Networking.packetWriter).Write((ushort)ID);
			((BinaryWriter)(object)Networking.packetWriter).Write((short)healthChangeAmount);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)1);
		}
	}

	public static void ReadComputerDamage(PacketReader pr)
	{
		int num = ((BinaryReader)(object)pr).ReadUInt16();
		int num2 = ((BinaryReader)(object)pr).ReadInt16();
		try
		{
			for (int i = 0; i < computerPlayers.Count; i++)
			{
				if (computerPlayers[i].ID == num)
				{
					computerPlayers[i].PROPERTIES.health += num2;
					computerPlayers[i].BleedForMe(2, 1f);
					if (computerPlayers[i].PROPERTIES.health <= 0f)
					{
						computerPlayers[i].onDeath(broadcast: false);
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static FighterObject doCollisionCheck(Rectangle r, bool humans, bool cupoos)
	{
		if (humans)
		{
			foreach (FighterObject humanPlayer in humanPlayers)
			{
				if (humanPlayer.PROPERTIES.isAlive && humanPlayer.getWhereBodyIs().Intersects(r))
				{
					return humanPlayer;
				}
			}
		}
		if (cupoos)
		{
			foreach (FighterObject computerPlayer in computerPlayers)
			{
				if (computerPlayer.PROPERTIES.isAlive && computerPlayer.getWhereBodyIs().Intersects(r))
				{
					return computerPlayer;
				}
			}
		}
		return null;
	}

	public static FighterObject doCollisionCheckFeet(Rectangle r, bool humans, bool cupoos)
	{
		if (humans)
		{
			foreach (FighterObject humanPlayer in humanPlayers)
			{
				Rectangle whereFeetAre = humanPlayer.getWhereFeetAre();
				if (humanPlayer.PROPERTIES.isAlive && humanPlayer.getWhereFeetAre().Intersects(r))
				{
					return humanPlayer;
				}
			}
		}
		if (cupoos)
		{
			foreach (FighterObject computerPlayer in computerPlayers)
			{
				if (computerPlayer.PROPERTIES.isAlive && computerPlayer.getWhereFeetAre().Intersects(r))
				{
					return computerPlayer;
				}
			}
		}
		return null;
	}
}
