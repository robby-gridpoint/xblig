using System;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public class FighterObjectProperties
{
	public enum AnimationName
	{
		Idle,
		Blocking,
		Walking,
		Dying,
		Punching,
		QuickPunching,
		RangedSpecialMove,
		Exploding,
		AirborneSlowAttack,
		AirborneQuickAttack,
		BeingCarried,
		CarryingIdle,
		CarryingWalking,
		Whirlwind,
		HammerOfDoom,
		Fling,
		Kicking,
		Crouching,
		PooingStarted,
		PooingFinished,
		Jumping
	}

	private FighterObject parent = null;

	public float scale = 1f;

	public Vector2 startingPosition = Vector2.One;

	public HumanProfileObject HumanProfile = new HumanProfileObject();

	public string GamerTag = "";

	public int gamerTagX = 0;

	public DateTime AIattackAfter = DateTime.MaxValue;

	public AnimationPlayer sprite;

	public AnimationPlayer spriteBlood;

	public int bloodX = 0;

	public int bloodY = 0;

	public bool isBleeding = false;

	public string name = "he who has no name";

	public string uniqueName = "";

	public float health;

	public float healthMax;

	public int moveSpeed;

	public bool areWeHuman = false;

	public bool isLocal = false;

	public bool isAlive = false;

	public bool isNetworkPlayer = false;

	public bool isDying = false;

	public bool isBlocking = false;

	public bool isFlying = false;

	public bool isImmuneToDPS = false;

	public bool isPickupable = true;

	public bool isCrouching = false;

	public bool isKicking = false;

	public DateTime kickExpires = DateTime.MinValue;

	public DateTime crouchExpires = DateTime.MinValue;

	public string CustomAnimationName = "";

	public bool isCountering = false;

	public Buttons counterButton = Buttons.X;

	public DateTime counteringExpires = DateTime.MinValue;

	public FighterObject attackingFighter;

	public FighterObject targerFighter;

	public Vector2 CpuJumpDestination = Vector2.One;

	public Vector2 CpuMoveDestination = Vector2.One;

	public DateTime CpuAttackCooldown = DateTime.MinValue;

	public DateTime CpuBlockDuration = DateTime.MinValue;

	public float DamageFromAttack = 0f;

	public float DamageFromQuickAttack = 0f;

	public ObstacleObject holdingObstacleObject = null;

	public ObstacleObject pushingObstacleObject = null;

	public FighterObject carryingFighter = null;

	public FighterObject carriedByFighter = null;

	public Vector2 circlePivotPoint = Vector2.Zero;

	public Vector2 circleVelocity = Vector2.Zero;

	public float circleRadius = 0f;

	public bool isStunned = false;

	public DateTime stunExpires = DateTime.MinValue;

	public Definitions.FighterSpecialMoves currentAttack = Definitions.FighterSpecialMoves.nulll;

	public bool isFinishedPunching = true;

	public bool isInTheMiddleOfAnAnimation = false;

	public BunnyOfWar.AI.AI.modes AImode = BunnyOfWar.AI.AI.modes.doNothing;

	public int AIAmountSpeed = 0;

	public int AIAmountDistance = 0;

	public string AIMemory = "";

	public string AIMemory2 = "";

	public int AIMemoryInt = 0;

	public int AIMemoryInt2 = 0;

	public string AITrigger = "";

	public Vector2 momentum = Vector2.Zero;

	public Vector2 velocity = Vector2.Zero;

	public PlayerIndex? PlayerIndexControllerNumber;

	public int score;

	public GamePadState? previousGamePadState = null;

	public KeyboardState? previousKeyboardState;

	public Buttons[] recentButtonSequence = new Buttons[100];

	public long recentButtonSequencePosition = 0L;

	public DateTime recentButtonPressTime = DateTime.MinValue;

	public DateTime recentBlockPressTime = DateTime.MinValue;

	public Definitions.facing isFacing = Definitions.facing.right;

	public AnimationName AnimationStateCurrent = AnimationName.Idle;

	public AnimationName AnimationStatePrevious = AnimationName.Idle;

	public DateTime healthBoostTimeLastGiven = DateTime.MinValue;

	public float healthPercentage => health / healthMax;

	public FighterObjectProperties(FighterObject fo)
	{
		parent = fo;
		startingPosition = parent.getXYVector2();
	}

	public Vector2 getCenter()
	{
		return new Vector2(parent.X + parent.width / 2, parent.Y + parent.height / 2);
	}

	public void CountAttack(Definitions.FighterSpecialMoves attack, int hits)
	{
		if (hits <= 0)
		{
			return;
		}
		if (attack == Definitions.FighterSpecialMoves.nulll)
		{
			int num = 0;
			num++;
			return;
		}
		if (!HumanProfile.AttacksMade.ContainsKey(attack))
		{
			HumanProfile.AttacksMade.Add(attack, 0);
			HumanProfile.AttackLevels.Add(attack, 1);
		}
		HumanProfile.AttacksMade[attack] += hits;
		float num2 = 0.05f;
		if (parent.PROPERTIES.areWeHuman)
		{
			if (HumanProfile.AttacksMade[attack] > 1000 && HumanProfile.AttackLevels[attack] < 10)
			{
				HumanProfile.AttackLevels[attack] = 10;
				speedupAnimation(attack, num2 * 3f, 10);
			}
			else if (HumanProfile.AttacksMade[attack] > 750 && HumanProfile.AttackLevels[attack] < 9)
			{
				HumanProfile.AttackLevels[attack] = 9;
				speedupAnimation(attack, num2, 9);
			}
			else if (HumanProfile.AttacksMade[attack] > 500 && HumanProfile.AttackLevels[attack] < 8)
			{
				HumanProfile.AttackLevels[attack] = 8;
				speedupAnimation(attack, num2, 8);
			}
			else if (HumanProfile.AttacksMade[attack] > 250 && HumanProfile.AttackLevels[attack] < 7)
			{
				HumanProfile.AttackLevels[attack] = 7;
				speedupAnimation(attack, num2, 7);
			}
			else if (HumanProfile.AttacksMade[attack] > 175 && HumanProfile.AttackLevels[attack] < 6)
			{
				HumanProfile.AttackLevels[attack] = 6;
				speedupAnimation(attack, num2, 6);
			}
			else if (HumanProfile.AttacksMade[attack] > 100 && HumanProfile.AttackLevels[attack] < 5)
			{
				HumanProfile.AttackLevels[attack] = 5;
				speedupAnimation(attack, num2, 5);
			}
			else if (HumanProfile.AttacksMade[attack] > 75 && HumanProfile.AttackLevels[attack] < 4)
			{
				HumanProfile.AttackLevels[attack] = 4;
				speedupAnimation(attack, num2, 4);
			}
			else if (HumanProfile.AttacksMade[attack] > 50 && HumanProfile.AttackLevels[attack] < 3)
			{
				HumanProfile.AttackLevels[attack] = 3;
				speedupAnimation(attack, num2, 3);
			}
			else if (HumanProfile.AttacksMade[attack] > 25 && HumanProfile.AttackLevels[attack] < 2 && HumanProfile.AttacksMade[attack] >= 75)
			{
				HumanProfile.AttackLevels[attack] = 2;
				speedupAnimation(attack, num2, 2);
			}
		}
	}

	public void speedupAnimation(Definitions.FighterSpecialMoves attack, float increase, int level)
	{
		GraphicsManager.Message("Level up!\r\n\r\n" + attack.ToString() + " level " + level, 6, 0);
		switch (attack)
		{
		case Definitions.FighterSpecialMoves.chop:
			parent.animationPunching.FrameTime = parent.animationPunching.FrameTime * (1f - increase);
			break;
		case Definitions.FighterSpecialMoves.swing:
			parent.animationQuickPunching.FrameTime = parent.animationQuickPunching.FrameTime * (1f - increase);
			break;
		case Definitions.FighterSpecialMoves.rangedArrow:
			break;
		case Definitions.FighterSpecialMoves.Hadouken:
			break;
		}
	}

	public int GetLevelOf(Definitions.FighterSpecialMoves attack)
	{
		if (!HumanProfile.AttacksMade.ContainsKey(attack))
		{
			HumanProfile.AttacksMade.Add(attack, 0);
			HumanProfile.AttackLevels.Add(attack, 1);
		}
		return HumanProfile.AttackLevels[attack];
	}
}
