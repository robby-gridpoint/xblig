using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public class FighterObject
{
	public FighterObjectProperties PROPERTIES = null;

	public FighterObjectRangedCode RANGED = null;

	public FighterObjectJumpCode JUMP = null;

	public int ID = -1;

	private float xPartial = 0f;

	private float yPartial = 0f;

	protected Rectangle rectBoundaries;

	private Rectangle rrectSpriteDisplay;

	public Rectangle rectSpriteDisplay;

	protected Rectangle rectCollisionBody;

	protected Rectangle rectCollisionFist;

	protected Rectangle rectCollisionFoot;

	protected Texture2D spriteBody;

	protected Texture2D spriteFist;

	protected Texture2D spriteFoot;

	protected Texture2D spriteTestLine;

	public Animation animationIdle;

	public Animation animationBlocking;

	public Animation animationWalking;

	public Animation animationDying;

	public Animation animationBeingCarried;

	public Animation animationCarryingIdle;

	public Animation animationCarryingWalking;

	public Animation animationPunching;

	public Animation animationExploding;

	public Animation animationQuickPunching;

	public Animation animationRangedSpecialMove;

	public Animation animationWhirlwind;

	public Animation animationAirborneSwinger;

	public Animation animationAirborneChopper;

	public Animation animationHammerOfDoom;

	public Animation animationFling;

	public Animation animationKicking;

	public Animation animationCrouching;

	public Animation animationJumping;

	public Animation animationPooingStart;

	public Animation animationPooingFinished;

	public int X
	{
		get
		{
			return rectSpriteDisplay.X;
		}
		set
		{
			rectSpriteDisplay.X = value;
		}
	}

	public int Y
	{
		get
		{
			return rectSpriteDisplay.Y;
		}
		set
		{
			rectSpriteDisplay.Y = value;
		}
	}

	public int width
	{
		get
		{
			if (PROPERTIES.scale == 1f)
			{
				return rectSpriteDisplay.Width;
			}
			return (int)((float)rectSpriteDisplay.Width * PROPERTIES.scale);
		}
		set
		{
			rectSpriteDisplay.Width = value;
		}
	}

	public int height
	{
		get
		{
			if (PROPERTIES.scale == 1f)
			{
				return rectSpriteDisplay.Height;
			}
			return (int)((float)rectSpriteDisplay.Height * PROPERTIES.scale);
		}
		set
		{
			rectSpriteDisplay.Height = value;
		}
	}

	public Vector2 getXYVector2()
	{
		return new Vector2(X, Y);
	}

	public float getLayerDepth()
	{
		return RandomStaticGlobals.getLayerDepth(Y, height);
	}

	public FighterObject()
	{
		RANGED = new FighterObjectRangedCode(this);
		PROPERTIES = new FighterObjectProperties(this);
		JUMP = new FighterObjectJumpCode(this);
	}

	public FighterObject(ContentManager Content)
	{
		RANGED = new FighterObjectRangedCode(this);
		PROPERTIES = new FighterObjectProperties(this);
		JUMP = new FighterObjectJumpCode(this);
		spriteBody = GraphicsManager.LoadTexture("colors/green");
		spriteFist = GraphicsManager.LoadTexture("colors/red");
		spriteFoot = GraphicsManager.LoadTexture("colors/yellow");
		spriteTestLine = GraphicsManager.LoadTexture("colors/line");
	}

	public void setCollisionRects(Rectangle boundaries, Rectangle body, Rectangle fist, Rectangle foot)
	{
		rectBoundaries = boundaries;
		rectCollisionBody = body;
		rectCollisionFist = fist;
		rectCollisionFoot = foot;
	}

	public Rectangle? willThisMoveCollide()
	{
		return willThisMoveCollide(cpuCheck: true);
	}

	public Rectangle? willThisMoveCollide(bool cpuCheck)
	{
		Rectangle? rectangle = null;
		Rectangle whereBodyIs = getWhereBodyIs();
		Rectangle whereFeetAre = getWhereFeetAre();
		rectangle = ObstacleManager.doCollisionCheck(whereBodyIs, whereFeetAre, null, PROPERTIES.holdingObstacleObject, PROPERTIES.pushingObstacleObject);
		if (rectangle.HasValue)
		{
			return rectangle;
		}
		if (!PROPERTIES.areWeHuman && cpuCheck)
		{
			if (!FighterManager.doCollisionCheckOnCPUs(this))
			{
				return null;
			}
			return default(Rectangle);
		}
		if (!GraphicsManager.BoundariesDefault.Contains(whereFeetAre))
		{
			return default(Rectangle);
		}
		return null;
	}

	public bool moveLeft()
	{
		return moveLeft(PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond, run: false);
	}

	public bool moveLeft(bool run)
	{
		return moveLeft(PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond, run);
	}

	public bool moveLeft(int distance, bool run)
	{
		onUnBlock();
		int x = X;
		PROPERTIES.isFacing = Definitions.facing.left;
		PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: true);
		X -= distance;
		if (PROPERTIES.areWeHuman && RandomStaticGlobals.CameraRollVelocity != Vector2.Zero && !GraphicsManager.viewableArea.Intersects(getWhereFeetAre()))
		{
			X += distance;
			return false;
		}
		if (run || JUMP.isJumping)
		{
			X -= distance;
		}
		if (X < 0)
		{
			X = 0;
		}
		if (willThisMoveCollide(cpuCheck: true).HasValue)
		{
			X += distance;
			if (run || JUMP.isJumping)
			{
				X += distance;
			}
		}
		if (PROPERTIES.areWeHuman)
		{
			TriggerManager.checkTriggers(this);
		}
		if (x != X)
		{
			return true;
		}
		return false;
	}

	public bool moveRight()
	{
		return moveRight(PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond, run: false);
	}

	public bool moveRight(bool run)
	{
		return moveRight(PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond, run);
	}

	public bool moveRight(int distance, bool run)
	{
		onUnBlock();
		int x = X;
		PROPERTIES.isFacing = Definitions.facing.right;
		PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: true);
		X += distance;
		if (PROPERTIES.areWeHuman && RandomStaticGlobals.CameraRollVelocity != Vector2.Zero && !GraphicsManager.viewableArea.Intersects(getWhereFeetAre()))
		{
			X -= distance;
			return false;
		}
		if (run || JUMP.isJumping)
		{
			X += distance;
		}
		if (X + rectSpriteDisplay.Width > rectBoundaries.Width)
		{
			X = rectBoundaries.Width - rectSpriteDisplay.Width;
		}
		if (willThisMoveCollide(cpuCheck: true).HasValue)
		{
			X -= distance;
			if (run || JUMP.isJumping)
			{
				X -= distance;
			}
		}
		if (PROPERTIES.areWeHuman)
		{
			TriggerManager.checkTriggers(this);
		}
		if (x != X)
		{
			return true;
		}
		return false;
	}

	public bool moveUp()
	{
		return moveUp(PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond);
	}

	public bool moveUp(int distance)
	{
		if (JUMP.jumpPixelsOffGround > 0 && !PROPERTIES.isFlying)
		{
			return false;
		}
		onUnBlock();
		PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: true);
		Y -= distance;
		if (PROPERTIES.areWeHuman && RandomStaticGlobals.CameraRollVelocity != Vector2.Zero && !GraphicsManager.viewableArea.Intersects(getWhereFeetAre()))
		{
			Y += distance;
			return false;
		}
		if (willThisMoveCollide(cpuCheck: true).HasValue || (!PROPERTIES.isFlying && !SceneryManager.AmIOnSolidGround(getWhereFeetAre(), PROPERTIES.holdingObstacleObject)))
		{
			Y += distance;
		}
		if (PROPERTIES.areWeHuman)
		{
			TriggerManager.checkTriggers(this);
		}
		return true;
	}

	public bool moveDown()
	{
		return moveDown(PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond);
	}

	public bool moveDownV2()
	{
		return moveDownV2(PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond);
	}

	public bool moveDownV2(int distance)
	{
		if (JUMP.jumpPixelsOffGround > 0)
		{
			return false;
		}
		onUnBlock();
		PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: true);
		Y += distance;
		if (PROPERTIES.areWeHuman && RandomStaticGlobals.CameraRollVelocity != Vector2.Zero && !GraphicsManager.viewableArea.Intersects(getWhereFeetAre()))
		{
			Y -= distance;
			return false;
		}
		if (willThisMoveCollide(cpuCheck: true).HasValue || (!PROPERTIES.isFlying && !SceneryManager.AmIOnSolidGround(getWhereFeetAre(), null)))
		{
			Y -= distance;
		}
		if (PROPERTIES.areWeHuman)
		{
			TriggerManager.checkTriggers(this);
		}
		return true;
	}

	public bool moveDown(int distance)
	{
		if (JUMP.jumpPixelsOffGround > 0)
		{
			return false;
		}
		onUnBlock();
		PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: true);
		if (Y + rectSpriteDisplay.Height > rectBoundaries.Height)
		{
			return false;
		}
		Y += distance;
		if (Y + rectSpriteDisplay.Height > rectBoundaries.Height)
		{
			Y = rectBoundaries.Height - rectSpriteDisplay.Height;
		}
		if (willThisMoveCollide(cpuCheck: true).HasValue || (!PROPERTIES.isFlying && !SceneryManager.AmIOnSolidGround(getWhereFeetAre(), null)))
		{
			Y -= distance;
		}
		if (PROPERTIES.areWeHuman)
		{
			TriggerManager.checkTriggers(this);
		}
		return true;
	}

	public bool move(float x, float y)
	{
		yPartial += y;
		xPartial += x;
		int y2 = Y;
		if (yPartial >= 1f || yPartial <= -1f)
		{
			Y += (int)yPartial;
			yPartial -= Y - y2;
		}
		if (Y + rectSpriteDisplay.Height > rectBoundaries.Height)
		{
			Y = rectBoundaries.Height - rectSpriteDisplay.Height;
		}
		if (willThisMoveCollide().HasValue)
		{
			Y = y2;
			yPartial = 0f;
		}
		int x2 = X;
		if (xPartial >= 1f || xPartial <= -1f)
		{
			X += (int)xPartial;
			xPartial -= X - x2;
		}
		if (X + rectSpriteDisplay.Width > rectBoundaries.Width)
		{
			X = rectBoundaries.Width - rectSpriteDisplay.Width;
		}
		if (willThisMoveCollide().HasValue)
		{
			X = x2;
			xPartial = 0f;
			return false;
		}
		if (PROPERTIES.areWeHuman)
		{
			TriggerManager.checkTriggers(this);
		}
		return true;
	}

	public void fallDownBecauseYouFellOffACliff()
	{
		if (PROPERTIES.isFlying || JUMP.jumpPixelsOffGround > 0 || Y > GraphicsManager.ScreenHeight)
		{
			return;
		}
		if (!PROPERTIES.isDying)
		{
			PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: true);
		}
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.runner)
		{
			Y += JUMP.jumpFallSpeed / Definitions.UpdatesPerSecond;
		}
		else
		{
			Y += Definitions.GravityFallSpeed / Definitions.UpdatesPerSecond;
		}
		if (Y > Definitions.ScreenMaxRect.Height)
		{
			Y = Definitions.ScreenMaxRect.Height - rectSpriteDisplay.Height;
			onDeath();
			return;
		}
		Rectangle? rectangle = willThisMoveCollide();
		if (rectangle.HasValue)
		{
			Y = rectangle.Value.Y - height;
		}
		if (PROPERTIES.areWeHuman)
		{
			TriggerManager.checkTriggers(this);
		}
	}

	public void moveRemotely(int x, int y, int? jumpHeight)
	{
		X = x;
		Y = y;
		if (jumpHeight.HasValue)
		{
			JUMP.jumpPixelsOffGround = jumpHeight.Value;
			JUMP.jumperIsOnWayUp = true;
		}
		TriggerManager.checkTriggers(this);
	}

	public void animateRemotely(FighterObjectProperties.AnimationName animation, Definitions.facing isFacing)
	{
		PlayAnimation(animation, broadcastThis: false);
		PROPERTIES.isFacing = isFacing;
		if (animation == FighterObjectProperties.AnimationName.Blocking)
		{
			PROPERTIES.isBlocking = true;
		}
		else
		{
			PROPERTIES.isBlocking = false;
		}
	}

	private bool doExplosionCollisionDetection(FighterObject enemy)
	{
		if (enemy == null)
		{
			return false;
		}
		if (getPersonalSpace().Intersects(enemy.getPersonalSpace()) || getPersonalSpace().Contains(enemy.getPersonalSpace()) || enemy.getPersonalSpace().Contains(getPersonalSpace()))
		{
			return true;
		}
		return false;
	}

	private bool doAOECollisionDetection(Rectangle r, FighterObject enemy)
	{
		if (enemy == null)
		{
			return false;
		}
		if (r.Intersects(enemy.getPersonalSpace()) || r.Contains(enemy.getPersonalSpace()) || enemy.getPersonalSpace().Contains(r))
		{
			return true;
		}
		return false;
	}

	public Rectangle getWhereFistIs()
	{
		if (PROPERTIES.isFacing == Definitions.facing.left)
		{
			return new Rectangle(X + (int)((float)rectCollisionFist.X * PROPERTIES.scale), Y + (int)((float)rectCollisionFist.Y * PROPERTIES.scale) - JUMP.jumpPixelsOffGround, (int)((float)rectCollisionFist.Width * PROPERTIES.scale), (int)((float)rectCollisionFist.Height * PROPERTIES.scale));
		}
		return new Rectangle(X + (int)((float)rectCollisionFist.X * PROPERTIES.scale) + width / 2, Y + (int)((float)rectCollisionFist.Y * PROPERTIES.scale) - JUMP.jumpPixelsOffGround, (int)((float)rectCollisionFist.Width * PROPERTIES.scale), (int)((float)rectCollisionFist.Height * PROPERTIES.scale));
	}

	public Rectangle getWhereFeetAre()
	{
		return new Rectangle(X + (int)((float)rectCollisionFoot.X * PROPERTIES.scale), Y + (int)((float)rectCollisionFoot.Y * PROPERTIES.scale) - JUMP.jumpPixelsOffGround, (int)((float)rectCollisionFoot.Width * PROPERTIES.scale), (int)((float)rectCollisionFoot.Height * PROPERTIES.scale));
	}

	public Rectangle getWhereBodyIs()
	{
		return new Rectangle(X + (int)((float)rectCollisionBody.X * PROPERTIES.scale), Y + (int)((float)rectCollisionBody.Y * PROPERTIES.scale) - JUMP.jumpPixelsOffGround, (int)((float)rectCollisionBody.Width * PROPERTIES.scale), (int)((float)rectCollisionBody.Height * PROPERTIES.scale));
	}

	public Rectangle getPersonalSpace()
	{
		return getPersonalSpace(75);
	}

	public Rectangle getPersonalSpace(int spaceSize)
	{
		return new Rectangle(X - spaceSize, Y - spaceSize - JUMP.jumpPixelsOffGround, (int)((float)rectCollisionBody.Width + (float)(spaceSize * 2) * PROPERTIES.scale), (int)((float)rectCollisionBody.Height + (float)(spaceSize * 2) * PROPERTIES.scale));
	}

	private void beginCountering()
	{
		if (!RandomStaticGlobals.isCounteringEnabled)
		{
			return;
		}
		if (!PROPERTIES.isCountering)
		{
			PROPERTIES.isCountering = true;
			Random random = new Random(DateTime.Now.Millisecond);
			int num = random.Next(4);
			if (Definitions.Options.Difficulty <= 1)
			{
				num = random.Next(2);
			}
			switch (num)
			{
			case 0:
				PROPERTIES.counterButton = Buttons.A;
				break;
			case 1:
				PROPERTIES.counterButton = Buttons.B;
				break;
			case 2:
				PROPERTIES.counterButton = Buttons.Y;
				break;
			default:
				PROPERTIES.counterButton = Buttons.X;
				break;
			}
		}
		PROPERTIES.counteringExpires = DateTime.Now.AddMilliseconds(Definitions.HumanCounterDurationMS);
		PROPERTIES.HumanProfile.parries++;
	}

	public void onCounter()
	{
		int num = PROPERTIES.attackingFighter.healthChange(-Definitions.counterMoveDamage);
		PROPERTIES.HumanProfile.counters++;
		PROPERTIES.HumanProfile.damageDealt += -num;
		if (PROPERTIES.attackingFighter.PROPERTIES.health <= 0f)
		{
			PROPERTIES.HumanProfile.kills++;
		}
		PROPERTIES.attackingFighter.hitMe(this, isQuickPunch: true);
		PROPERTIES.attackingFighter.BleedForMe(3, 1f);
		StopAnimation();
		PlayAnimation(FighterObjectProperties.AnimationName.QuickPunching, broadcastThis: true);
		PROPERTIES.isCountering = false;
	}

	public bool hitMeRanged(FighterObject enemy, ProjectileObject po)
	{
		if (enemy.PROPERTIES.isDying || enemy.PROPERTIES.health <= 0f || !enemy.PROPERTIES.isAlive)
		{
			return false;
		}
		int num = 0;
		if (PROPERTIES.isBlocking)
		{
			if (PROPERTIES.recentBlockPressTime.AddMilliseconds(Definitions.HumanCounterResponseWindowMS) > DateTime.Now && po.isCounterable)
			{
				PROPERTIES.attackingFighter = po.shooter;
				beginCountering();
				PROPERTIES.HumanProfile.shotsBlocked++;
				return true;
			}
			po.blocked();
			PROPERTIES.HumanProfile.shotsBlocked++;
			if (!PROPERTIES.areWeHuman || !enemy.PROPERTIES.areWeHuman)
			{
				return true;
			}
			num = healthChange((float)(-po.damage) * Definitions.HumanOnHumanBlockDamageLeakage);
		}
		else
		{
			num = healthChange(-po.damage);
		}
		BleedForMe(1, 1f);
		enemy.PROPERTIES.HumanProfile.damageDealt += -num;
		PROPERTIES.HumanProfile.damageTaken += -num;
		if (PROPERTIES.health <= 0f)
		{
			onDeath();
			try
			{
				enemy.PROPERTIES.HumanProfile.kills++;
				if (enemy.PROPERTIES.areWeHuman)
				{
					PROPERTIES.HumanProfile.VictimsCausesOfDeath.Add((FighterManager.CauseOfDeath)Enum.Parse(typeof(FighterManager.CauseOfDeath), po.type.ToString(), ignoreCase: true));
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
			}
		}
		return false;
	}

	public void explode(List<FighterObject> enemies, int damage, int selfDamage)
	{
		if (animationExploding != null)
		{
			PlayAnimation(FighterObjectProperties.AnimationName.Exploding, broadcastThis: true);
			PROPERTIES.isInTheMiddleOfAnAnimation = true;
		}
		foreach (FighterObject enemy in enemies)
		{
			if (doExplosionCollisionDetection(enemy))
			{
				enemy.explodeOnMe(this, damage);
				if ((PROPERTIES.areWeHuman && !enemy.PROPERTIES.isAlive) || enemy.PROPERTIES.health < 0f)
				{
					PROPERTIES.HumanProfile.VictimsCausesOfDeath.Add(FighterManager.CauseOfDeath.explosion);
				}
			}
		}
		ObstacleManager.doEnvironmentalDestructionCollision(getWhereFistIs(), damage);
		int num = healthChange(-selfDamage);
		PROPERTIES.HumanProfile.damageTaken += -num;
		onIdle();
		PROPERTIES.isFinishedPunching = true;
	}

	public void explodeOnMe(FighterObject enemy, int damage)
	{
		PROPERTIES.attackingFighter = enemy;
		float num = getAdjustedXY(GraphicsManager.viewableArea).X + (float)(rectSpriteDisplay.Width / 2);
		float num2 = -1f + num / (float)GraphicsManager.viewportRect.Width * 2f;
		if (PROPERTIES.isBlocking && FighterAttackCoordinator.areYouFacingTowardsYourEnemy(this, enemy))
		{
			enemy.PROPERTIES.HumanProfile.damageDealt += -healthChange(-damage / 2);
		}
		else
		{
			enemy.PROPERTIES.HumanProfile.damageDealt += -healthChange(-damage);
		}
		if (PROPERTIES.health <= 0f)
		{
			onDeath();
		}
	}

	public void AttackAOE(List<FighterObject> enemies, int aoeRange, int damage)
	{
		PROPERTIES.isInTheMiddleOfAnAnimation = true;
		foreach (FighterObject enemy in enemies)
		{
			if (doAOECollisionDetection(getPersonalSpace(aoeRange), enemy))
			{
				enemy.AOEOnMe(this, damage);
				if ((PROPERTIES.areWeHuman && !enemy.PROPERTIES.isAlive) || enemy.PROPERTIES.health < 0f)
				{
					PROPERTIES.HumanProfile.VictimsCausesOfDeath.Add(FighterManager.CauseOfDeath.explosion);
				}
			}
		}
		ObstacleManager.doEnvironmentalDestructionCollision(getWhereFistIs(), damage);
		onIdle();
		PROPERTIES.isFinishedPunching = true;
	}

	public void AOEOnMe(FighterObject enemy, int damage)
	{
		PROPERTIES.attackingFighter = enemy;
		float num = getAdjustedXY(GraphicsManager.viewableArea).X + (float)(rectSpriteDisplay.Width / 2);
		float num2 = -1f + num / (float)GraphicsManager.viewportRect.Width * 2f;
		if (PROPERTIES.isBlocking && PROPERTIES.recentBlockPressTime.AddMilliseconds(Definitions.HumanCounterResponseWindowMS) > DateTime.Now)
		{
			beginCountering();
			return;
		}
		if (PROPERTIES.isBlocking && FighterAttackCoordinator.areYouFacingTowardsYourEnemy(this, enemy))
		{
			enemy.PROPERTIES.HumanProfile.damageDealt += -healthChange(-damage / 10);
		}
		else
		{
			enemy.PROPERTIES.HumanProfile.damageDealt += -healthChange(-damage);
		}
		if (PROPERTIES.health <= 0f)
		{
			onDeath();
		}
		BleedForMe(1, 1f);
	}

	public void hitMeWithObstacle(ObstacleObject o)
	{
		healthChange(-o.fallDamageAfterLanding);
		Random random = new Random(DateTime.Now.Millisecond);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
		SceneryManager.AddBloodStain(X - width + random.Next(width * 2), Y - height + random.Next(height * 2), width, height, o);
	}

	public float getPlayHereValue()
	{
		float num = getAdjustedXY(GraphicsManager.viewableArea).X + (float)(rectSpriteDisplay.Width / 2);
		return -1f + num / (float)GraphicsManager.viewportRect.Width * 2f;
	}

	public void hitMe(FighterObject enemy, bool isQuickPunch)
	{
		if (JUMP.jumpPixelsOffGround > 10 || enemy.PROPERTIES.isDying || enemy.PROPERTIES.health <= 0f || !enemy.PROPERTIES.isAlive)
		{
			return;
		}
		PROPERTIES.attackingFighter = enemy;
		float playHereValue = getPlayHereValue();
		if (PROPERTIES.isBlocking)
		{
			PROPERTIES.HumanProfile.blocks++;
		}
		if (PROPERTIES.isBlocking && RandomStaticGlobals.isCounteringEnabled && PROPERTIES.recentBlockPressTime.AddMilliseconds(Definitions.HumanCounterResponseWindowMS) > DateTime.Now)
		{
			beginCountering();
			return;
		}
		int num = 0;
		if (!PROPERTIES.isBlocking || !FighterAttackCoordinator.areYouFacingTowardsYourEnemy(this, enemy))
		{
			num = ((!isQuickPunch) ? healthChange(0f - enemy.PROPERTIES.DamageFromAttack) : healthChange(0f - enemy.PROPERTIES.DamageFromQuickAttack));
		}
		else
		{
			SoundManager.playNextClangStereo(playHereValue);
			if (!PROPERTIES.areWeHuman || !enemy.PROPERTIES.areWeHuman)
			{
				return;
			}
			num = ((!isQuickPunch) ? healthChange((0f - enemy.PROPERTIES.DamageFromAttack) * Definitions.HumanOnHumanBlockDamageLeakage) : healthChange((0f - enemy.PROPERTIES.DamageFromQuickAttack) * Definitions.HumanOnHumanBlockDamageLeakage));
		}
		enemy.PROPERTIES.HumanProfile.damageDealt += -num;
		PROPERTIES.HumanProfile.damageTaken += -num;
		if (!PROPERTIES.areWeHuman && !RandomStaticGlobals.isPvPEnabled)
		{
			SoundManager.playNextGoreyHitStereo(playHereValue);
		}
		if (!PROPERTIES.areWeHuman || RandomStaticGlobals.isPvPEnabled)
		{
			if (isQuickPunch)
			{
				BleedForMe(1, Definitions.BloodSplatterSize);
			}
			else
			{
				BleedForMe(2, Definitions.BloodSplatterSize * 1.5f);
			}
		}
		if (PROPERTIES.health <= 0f)
		{
			onDeath();
		}
	}

	public void onDeath()
	{
		onDeath(broadcast: true);
	}

	public void onDeath(bool broadcast)
	{
		PROPERTIES.HumanProfile.stopwatchTimeSpentPlaying.Stop();
		PROPERTIES.HumanProfile.stopwatchTimeSpentBlocking.Stop();
		if ((!PROPERTIES.areWeHuman || PROPERTIES.isLocal || RandomStaticGlobals.isPvPEnabled) && !PROPERTIES.isDying && PROPERTIES.isAlive)
		{
			if (PROPERTIES.attackingFighter != null && PROPERTIES.isAlive)
			{
				PROPERTIES.attackingFighter.PROPERTIES.HumanProfile.kills++;
			}
			PlayAnimation(FighterObjectProperties.AnimationName.Dying, broadcastThis: true);
			PROPERTIES.isAlive = false;
			PROPERTIES.isDying = true;
			SceneryManager.AddBloodStain(X, Y, width, height, null);
			if (PROPERTIES.uniqueName != null && PROPERTIES.uniqueName != "")
			{
				TriggerManager.SetTriggerEvent(PROPERTIES.uniqueName + "IsDead");
				AwardmentsManager.CheckForAwardments(PROPERTIES.uniqueName + "IsDead");
			}
			else
			{
				AwardmentsManager.CheckForAwardments();
			}
			if (broadcast)
			{
				FighterManager.BroadcastFighterDeath(ID, PROPERTIES.areWeHuman);
			}
		}
	}

	public void stunMe()
	{
		stunMe(2.0, broadcast: true);
	}

	public void stunMe(double seconds, bool broadcast)
	{
		onIdle();
		PROPERTIES.isStunned = true;
		PROPERTIES.stunExpires = DateTime.Now.AddSeconds(seconds);
		if (broadcast)
		{
			FighterManager.BroadcastFighterStunned(ID, PROPERTIES.areWeHuman, seconds);
		}
	}

	public void hamstringMe()
	{
		if (PROPERTIES.moveSpeed > 0)
		{
			PROPERTIES.moveSpeed--;
		}
		healthChange(-1f);
	}

	public void onKick()
	{
		PROPERTIES.isKicking = true;
		PROPERTIES.kickExpires = DateTime.Now.AddMilliseconds(Definitions.HumanKickDurationMS);
		PlayAnimation(FighterObjectProperties.AnimationName.Kicking, broadcastThis: true);
	}

	public void onUnKick()
	{
		PROPERTIES.isKicking = false;
	}

	public void onCrouch()
	{
		PROPERTIES.isCrouching = true;
		PROPERTIES.crouchExpires = DateTime.Now.AddMilliseconds(Definitions.HumanCrouchDurationMS);
		PlayAnimation(FighterObjectProperties.AnimationName.Crouching, broadcastThis: true);
	}

	public void onUnCrouch()
	{
		PROPERTIES.isCrouching = false;
	}

	public bool onBlock()
	{
		if (PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.nulll)
		{
			return false;
		}
		PlayAnimation(FighterObjectProperties.AnimationName.Blocking, broadcastThis: true);
		PROPERTIES.isBlocking = true;
		PROPERTIES.HumanProfile.timeSpentBlocking++;
		return true;
	}

	public bool onBlock(float blockSecondsCPU)
	{
		if (PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.nulll)
		{
			return false;
		}
		PlayAnimation(FighterObjectProperties.AnimationName.Blocking, broadcastThis: true);
		PROPERTIES.isBlocking = true;
		PROPERTIES.CpuBlockDuration = DateTime.Now.AddSeconds(blockSecondsCPU);
		PROPERTIES.HumanProfile.timeSpentBlocking++;
		return true;
	}

	public void onUnBlockCPU()
	{
		if (PROPERTIES.isBlocking && !(PROPERTIES.CpuBlockDuration > DateTime.Now))
		{
			onUnBlock();
		}
	}

	public void onUnBlock()
	{
		if (PROPERTIES.isBlocking)
		{
			PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
			PROPERTIES.isBlocking = false;
		}
	}

	public void onIdle()
	{
		onIdle(force: false);
	}

	public void onIdle(bool force)
	{
		if (!force)
		{
			if (PROPERTIES.isDying || PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.nulll)
			{
				return;
			}
		}
		else
		{
			StopAnimation();
			PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.nulll;
		}
		if (PROPERTIES.velocity.X == 0f)
		{
			PlayAnimation(FighterObjectProperties.AnimationName.Idle, broadcastThis: true);
		}
		if (!PROPERTIES.areWeHuman)
		{
			onUnBlockCPU();
		}
	}

	public void healthBoostForResting()
	{
		if (PROPERTIES.areWeHuman && !(PROPERTIES.health >= PROPERTIES.healthMax) && PROPERTIES.healthBoostTimeLastGiven.AddSeconds(1.0) < DateTime.Now)
		{
			int num = healthChange(10f);
			PROPERTIES.healthBoostTimeLastGiven = DateTime.Now;
			PROPERTIES.HumanProfile.healthRegenerated += num;
		}
	}

	public void healthBoostTimerReset()
	{
		PROPERTIES.healthBoostTimeLastGiven = DateTime.Now.AddSeconds(3.0);
	}

	public Vector2 getAdjustedXY(Rectangle viewableArea)
	{
		return new Vector2(X - viewableArea.X, Y - viewableArea.Y);
	}

	public int healthChange(float amount)
	{
		if (PROPERTIES.areWeHuman && !PROPERTIES.isLocal && !RandomStaticGlobals.isPvPEnabled)
		{
			return 0;
		}
		if (amount == 0f)
		{
			return 0;
		}
		if (PROPERTIES.areWeHuman && Definitions.Options.Difficulty == 3)
		{
			amount *= 1.3f;
		}
		if (amount < 0f)
		{
			healthBoostTimerReset();
		}
		float health = PROPERTIES.health;
		PROPERTIES.health += amount;
		if (PROPERTIES.uniqueName != null && PROPERTIES.uniqueName != "" && amount < 0f)
		{
			float num = PROPERTIES.health / PROPERTIES.healthMax;
			if (num < 0.75f)
			{
				TriggerManager.SetTriggerEvent(PROPERTIES.uniqueName + "At75HP");
				PROPERTIES.AITrigger = "At75HP";
			}
			if (num < 0.5f)
			{
				TriggerManager.SetTriggerEvent(PROPERTIES.uniqueName + "At50HP");
				PROPERTIES.AITrigger = "At50HP";
			}
			if (num < 0.25f)
			{
				TriggerManager.SetTriggerEvent(PROPERTIES.uniqueName + "At25HP");
				PROPERTIES.AITrigger = "At25HP";
			}
			if (num < 0.1f)
			{
				TriggerManager.SetTriggerEvent(PROPERTIES.uniqueName + "At10HP");
				PROPERTIES.AITrigger = "At10HP";
			}
		}
		if (PROPERTIES.health > PROPERTIES.healthMax)
		{
			PROPERTIES.health = PROPERTIES.healthMax;
		}
		if (PROPERTIES.areWeHuman && (PROPERTIES.isLocal || RandomStaticGlobals.isPvPEnabled))
		{
			FighterManager.BroadcastHumanHealth(ID, (int)PROPERTIES.health);
		}
		if (!PROPERTIES.areWeHuman)
		{
			FighterManager.BroadcastComputerHealthChange(ID, (int)amount);
		}
		if (PROPERTIES.health <= 0f)
		{
			PROPERTIES.health = 0f;
			onDeath();
		}
		if (PROPERTIES.areWeHuman && Definitions.Options.Difficulty == 3)
		{
			float num2 = PROPERTIES.healthMax * 0.5f;
			if (health < PROPERTIES.health && health < num2 && PROPERTIES.health > num2)
			{
				PROPERTIES.health = num2;
			}
		}
		return (int)(PROPERTIES.health - health);
	}

	public void StopAnimation()
	{
		PROPERTIES.isInTheMiddleOfAnAnimation = false;
	}

	public void PlayAnimation(FighterObjectProperties.AnimationName animation, bool broadcastThis)
	{
		if ((RandomStaticGlobals.GameMode == Definitions.GameMode.flappy && animation == FighterObjectProperties.AnimationName.Walking) || (PROPERTIES.isInTheMiddleOfAnAnimation && !PROPERTIES.isDying) || (animation == FighterObjectProperties.AnimationName.Walking && PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.nulll && PROPERTIES.isInTheMiddleOfAnAnimation))
		{
			return;
		}
		PROPERTIES.AnimationStatePrevious = PROPERTIES.AnimationStateCurrent;
		PROPERTIES.AnimationStateCurrent = animation;
		if (PROPERTIES.isDying)
		{
			PROPERTIES.sprite.PlayAnimation(animationDying);
		}
		else
		{
			switch (animation)
			{
			case FighterObjectProperties.AnimationName.Blocking:
				PROPERTIES.sprite.PlayAnimation(animationBlocking);
				break;
			case FighterObjectProperties.AnimationName.Exploding:
				PROPERTIES.sprite.PlayAnimation(animationExploding);
				break;
			case FighterObjectProperties.AnimationName.Dying:
				PROPERTIES.sprite.PlayAnimation(animationDying);
				break;
			case FighterObjectProperties.AnimationName.Idle:
				PROPERTIES.sprite.PlayAnimation(animationIdle);
				break;
			case FighterObjectProperties.AnimationName.Punching:
				PROPERTIES.sprite.PlayAnimation(animationPunching);
				break;
			case FighterObjectProperties.AnimationName.QuickPunching:
				PROPERTIES.sprite.PlayAnimation(animationQuickPunching);
				break;
			case FighterObjectProperties.AnimationName.RangedSpecialMove:
				PROPERTIES.sprite.PlayAnimation(animationRangedSpecialMove);
				break;
			case FighterObjectProperties.AnimationName.Walking:
				PROPERTIES.sprite.PlayAnimation(animationWalking);
				break;
			case FighterObjectProperties.AnimationName.AirborneQuickAttack:
				PROPERTIES.sprite.PlayAnimation(animationAirborneSwinger);
				break;
			case FighterObjectProperties.AnimationName.AirborneSlowAttack:
				PROPERTIES.sprite.PlayAnimation(animationAirborneChopper);
				break;
			case FighterObjectProperties.AnimationName.BeingCarried:
				if (animationBeingCarried != null)
				{
					PROPERTIES.sprite.PlayAnimation(animationBeingCarried);
				}
				else
				{
					PROPERTIES.sprite.PlayAnimation(animationIdle);
				}
				break;
			case FighterObjectProperties.AnimationName.Whirlwind:
				if (animationWhirlwind != null)
				{
					PROPERTIES.sprite.PlayAnimation(animationWhirlwind);
				}
				else
				{
					PROPERTIES.sprite.PlayAnimation(animationExploding);
				}
				break;
			case FighterObjectProperties.AnimationName.HammerOfDoom:
				if (animationHammerOfDoom != null)
				{
					PROPERTIES.sprite.PlayAnimation(animationHammerOfDoom);
				}
				else
				{
					PROPERTIES.sprite.PlayAnimation(animationExploding);
				}
				break;
			case FighterObjectProperties.AnimationName.Fling:
				PROPERTIES.sprite.PlayAnimation(animationFling);
				break;
			case FighterObjectProperties.AnimationName.Kicking:
				PROPERTIES.sprite.PlayAnimation(animationKicking);
				break;
			case FighterObjectProperties.AnimationName.Crouching:
				PROPERTIES.sprite.PlayAnimation(animationCrouching);
				break;
			case FighterObjectProperties.AnimationName.PooingStarted:
				PROPERTIES.sprite.PlayAnimation(animationPooingStart);
				break;
			case FighterObjectProperties.AnimationName.PooingFinished:
				PROPERTIES.sprite.PlayAnimation(animationPooingFinished);
				break;
			case FighterObjectProperties.AnimationName.Jumping:
				PROPERTIES.sprite.PlayAnimation(animationJumping);
				break;
			}
		}
		if (broadcastThis && PROPERTIES.areWeHuman && PROPERTIES.AnimationStatePrevious != PROPERTIES.AnimationStateCurrent)
		{
			FighterManager.BroadcastAnimationChange(ID, PROPERTIES.AnimationStateCurrent, PROPERTIES.isFacing);
		}
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle viewableArea)
	{
		if (PROPERTIES.isDying)
		{
			PlayAnimation(FighterObjectProperties.AnimationName.Dying, broadcastThis: true);
			if (PROPERTIES.sprite.FrameIndex >= animationDying.FrameCount - 1)
			{
				PROPERTIES.isDying = false;
				PROPERTIES.isAlive = false;
				if (PROPERTIES.areWeHuman)
				{
					FighterManager.HumanDied(this);
				}
			}
		}
		if (PROPERTIES.carriedByFighter != null)
		{
			PlayAnimation(FighterObjectProperties.AnimationName.BeingCarried, broadcastThis: false);
		}
		if (!viewableArea.Intersects(rectSpriteDisplay))
		{
			return;
		}
		Vector2 adjustedXY = getAdjustedXY(viewableArea);
		adjustedXY.Y -= JUMP.jumpPixelsOffGround;
		if (PROPERTIES.isFacing == Definitions.facing.right)
		{
			PROPERTIES.sprite.Draw(gameTime, spriteBatch, adjustedXY, SpriteEffects.FlipHorizontally, getLayerDepth(), rectSpriteDisplay, PROPERTIES.scale);
		}
		else
		{
			PROPERTIES.sprite.Draw(gameTime, spriteBatch, adjustedXY, SpriteEffects.None, getLayerDepth(), rectSpriteDisplay, PROPERTIES.scale);
		}
		if (PROPERTIES.isBleeding)
		{
			if (PROPERTIES.spriteBlood.FrameIndex >= PROPERTIES.spriteBlood.Animation.FrameCount - 1)
			{
				PROPERTIES.isBleeding = false;
			}
			Vector2 positionXY = new Vector2(PROPERTIES.bloodX - viewableArea.X, PROPERTIES.bloodY - viewableArea.Y);
			if (PROPERTIES.isFacing == Definitions.facing.left)
			{
				positionXY.X += width / 3;
				positionXY.Y -= height / 2;
				PROPERTIES.spriteBlood.Draw(gameTime, spriteBatch, positionXY, SpriteEffects.None, getLayerDepth(), rectSpriteDisplay, PROPERTIES.scale);
			}
			else
			{
				positionXY.X -= width / 3;
				positionXY.Y -= height / 2;
				PROPERTIES.spriteBlood.Draw(gameTime, spriteBatch, positionXY, SpriteEffects.FlipHorizontally, getLayerDepth(), rectSpriteDisplay, PROPERTIES.scale);
			}
		}
		if (PROPERTIES.isInTheMiddleOfAnAnimation && PROPERTIES.sprite.FrameIndex >= PROPERTIES.sprite.Animation.FrameCount - 1)
		{
			if (!PROPERTIES.areWeHuman)
			{
				PROPERTIES.isInTheMiddleOfAnAnimation = false;
			}
			if (PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.nulll && FinishYourAttack(PROPERTIES.currentAttack))
			{
				PROPERTIES.isInTheMiddleOfAnAnimation = false;
			}
		}
		if (!PROPERTIES.isCountering)
		{
			return;
		}
		if (PROPERTIES.counteringExpires < DateTime.Now)
		{
			PROPERTIES.isCountering = false;
			return;
		}
		Rectangle destinationRectangle = new Rectangle((int)adjustedXY.X + 75, (int)adjustedXY.Y + 50, 100, 100);
		if (PROPERTIES.counterButton == Buttons.A)
		{
			spriteBatch.Draw(GraphicsManager.imgButtonA, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
		}
		else if (PROPERTIES.counterButton == Buttons.B)
		{
			spriteBatch.Draw(GraphicsManager.imgButtonB, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
		}
		else if (PROPERTIES.counterButton == Buttons.X)
		{
			spriteBatch.Draw(GraphicsManager.imgButtonX, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
		}
		else if (PROPERTIES.counterButton == Buttons.Y)
		{
			spriteBatch.Draw(GraphicsManager.imgButtonY, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
		}
	}

	private bool FinishYourAttack(Definitions.FighterSpecialMoves attack)
	{
		List<FighterObject> list = new List<FighterObject>(0);
		if (PROPERTIES.areWeHuman)
		{
			list.AddRange(FighterManager.getComputerPlayers(onlyLiving: true, canBeDying: false));
		}
		if (!PROPERTIES.areWeHuman || RandomStaticGlobals.isPvPEnabled)
		{
			list.AddRange(FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: false));
		}
		PROPERTIES.HumanProfile.logAttack(attack);
		int hits = 0;
		if (attack == Definitions.FighterSpecialMoves.rangedArrow)
		{
			FighterAttackCoordinator.shootArrowCPU(this);
		}
		PROPERTIES.CountAttack(attack, hits);
		PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.nulll;
		onIdle();
		return true;
	}

	public void BleedForMe(int bloodStains, float bloodSizePercentage)
	{
		if (!CustomsManager.IsBloodEnabled())
		{
			return;
		}
		bloodSizePercentage = 1f;
		if (bloodSizePercentage == 1f)
		{
			Rectangle whereBodyIs = getWhereBodyIs();
			if (PROPERTIES.isFacing == Definitions.facing.left)
			{
				PROPERTIES.bloodX = X + width / 2;
			}
			else
			{
				PROPERTIES.bloodX = X - width / 2;
			}
			PROPERTIES.bloodY = Y + height / 4;
			PROPERTIES.spriteBlood.rectangleResizeToThis = new Rectangle(PROPERTIES.bloodX, PROPERTIES.bloodY, width, height);
		}
		else
		{
			int num = (int)((float)rectSpriteDisplay.Height * bloodSizePercentage);
			int num2 = (int)((float)rectSpriteDisplay.Width * bloodSizePercentage);
			PROPERTIES.bloodX = rectSpriteDisplay.X;
			PROPERTIES.bloodY = rectSpriteDisplay.Y;
			PROPERTIES.spriteBlood.rectangleResizeToThis = new Rectangle(rectSpriteDisplay.X, rectSpriteDisplay.Y, num2, num);
		}
		int num3 = 0;
		Random random = new Random(DateTime.Now.Millisecond);
		for (int i = 0; i < bloodStains; i++)
		{
			num3 = random.Next(GraphicsManager.animatedBloodSplatterList.Count - 1);
			if (Definitions.Options.BloodOnOff)
			{
				PROPERTIES.spriteBlood.PlayAnimation(GraphicsManager.animatedBloodSplatterList[num3], forceRestart: true);
			}
			else
			{
				PROPERTIES.spriteBlood.PlayAnimation(GraphicsManager.animatedBloodSplatterListGREEN[num3], forceRestart: true);
			}
			PROPERTIES.isBleeding = true;
			if (PROPERTIES.isFacing == Definitions.facing.left)
			{
				SceneryManager.AddBloodStain(X + random.Next(width), Y + random.Next(height / 2), width, height, null);
			}
			else
			{
				SceneryManager.AddBloodStain(X - random.Next(width), Y + random.Next(height / 2), width, height, null);
			}
		}
	}

	public FighterObject Copy()
	{
		return (FighterObject)MemberwiseClone();
	}
}
