using System;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public class FighterObjectJumpCode
{
	public int jumpPixelsOffGround = 0;

	public bool jumperIsOnWayUp = false;

	public int jumpMaxAmount = 500;

	public int jumpMaxAmountSecondTime = 1000;

	public int jumpUpSpeed = 1500;

	public int jumpFallSpeed = 1500;

	public DateTime jumpLastUpdate = DateTime.MinValue;

	public DateTime jumpCooldown = DateTime.MinValue;

	public bool wasTrulyAirborne = false;

	private int becameAirborneAtYPixelValue = 0;

	public bool isJumping = false;

	private bool isDoubleJumping = false;

	private FighterObject parent = null;

	public DateTime gravityImmunity = DateTime.MinValue;

	private static int updateEveryXMiliSeconds = 0;

	public FighterObjectJumpCode(FighterObject fo)
	{
		parent = fo;
	}

	public void jumpStopped()
	{
		jumperIsOnWayUp = false;
	}

	public void FlingUp()
	{
		jumpMaxAmount = 500;
		jumpUpSpeed = 3000;
		if (!isDoubleJumping && isJumping)
		{
			isDoubleJumping = true;
			jumperIsOnWayUp = true;
		}
		Rectangle rectSpriteDisplay = parent.rectSpriteDisplay;
		rectSpriteDisplay.Height += 20;
		if (jumpPixelsOffGround <= 0 && !isJumping && SceneryManager.AmIOnSolidGround(rectSpriteDisplay, parent.PROPERTIES.holdingObstacleObject))
		{
			isJumping = true;
			jumperIsOnWayUp = true;
		}
		NetworkGameplayManager.SendJumping(parent.ID, parent.X, parent.Y, parent.JUMP.jumpPixelsOffGround, parent.PROPERTIES.areWeHuman);
	}

	public void JumpUp()
	{
		if (DateTime.Now < gravityImmunity)
		{
			return;
		}
		if (CustomsManager.GetIsUnderWater())
		{
			isJumping = true;
			jumperIsOnWayUp = true;
			jumpPixelsOffGround += jumpMaxAmount;
			if (parent.willThisMoveCollide().HasValue)
			{
				jumpPixelsOffGround -= jumpMaxAmount;
			}
			return;
		}
		if (Definitions.isDoubleJumpEnabled && !isDoubleJumping && isJumping)
		{
			isDoubleJumping = true;
			jumperIsOnWayUp = true;
		}
		Rectangle rectSpriteDisplay = parent.rectSpriteDisplay;
		rectSpriteDisplay.Height += 20;
		if (jumpPixelsOffGround <= 0 && !isJumping && SceneryManager.AmIOnSolidGround(rectSpriteDisplay, parent.PROPERTIES.holdingObstacleObject))
		{
			isJumping = true;
			jumperIsOnWayUp = true;
		}
	}

	public void ProcessJumpStuff()
	{
		if (!isJumping)
		{
			return;
		}
		if (updateEveryXMiliSeconds == 0)
		{
			updateEveryXMiliSeconds = 1000 / Definitions.UpdatesPerSecond;
		}
		bool flag = false;
		if (isJumping && jumperIsOnWayUp)
		{
			int num = jumpUpSpeed / Definitions.UpdatesPerSecond;
			jumpPixelsOffGround += num;
			if (parent.willThisMoveCollide().HasValue)
			{
				jumperIsOnWayUp = false;
				jumpPixelsOffGround -= num;
				return;
			}
			if (!isDoubleJumping)
			{
				if (jumpPixelsOffGround >= jumpMaxAmount)
				{
					jumperIsOnWayUp = false;
				}
			}
			else if (jumpPixelsOffGround >= jumpMaxAmountSecondTime)
			{
				jumperIsOnWayUp = false;
			}
			if (!wasTrulyAirborne && !SceneryManager.AmIOnSolidGround(parent.getWhereFeetAre(), parent.PROPERTIES.holdingObstacleObject))
			{
				wasTrulyAirborne = true;
				becameAirborneAtYPixelValue = parent.Y - jumpPixelsOffGround;
			}
		}
		else
		{
			FallBackDown();
		}
		jumpLastUpdate = DateTime.Now;
	}

	public void FallBackDown()
	{
		if (DateTime.Now < gravityImmunity)
		{
			return;
		}
		if (jumpPixelsOffGround > 0 && !jumperIsOnWayUp)
		{
			bool flag = false;
			if (wasTrulyAirborne && SceneryManager.AmIOnSolidGround(parent.getWhereFeetAre(), parent.PROPERTIES.holdingObstacleObject) && parent.Y - jumpPixelsOffGround < becameAirborneAtYPixelValue)
			{
				StopTheJump();
			}
			else
			{
				int num = jumpFallSpeed / Definitions.UpdatesPerSecond;
				if (num < jumpPixelsOffGround)
				{
					jumpPixelsOffGround -= num;
				}
				else
				{
					jumpPixelsOffGround = 0;
				}
				if (parent.willThisMoveCollide().HasValue)
				{
					jumpPixelsOffGround += num;
					StopTheJump();
				}
				else if (jumpPixelsOffGround <= 0)
				{
					StopTheJump();
				}
			}
			jumperIsOnWayUp = false;
		}
		else
		{
			StopTheJump();
		}
	}

	public void StopTheJump()
	{
		isJumping = false;
		isDoubleJumping = false;
		parent.Y -= jumpPixelsOffGround;
		jumpPixelsOffGround = 0;
		jumperIsOnWayUp = false;
		jumpLastUpdate = DateTime.MinValue;
		wasTrulyAirborne = false;
		becameAirborneAtYPixelValue = 0;
		NetworkGameplayManager.SendJumping(parent.ID, parent.X, parent.Y, parent.JUMP.jumpPixelsOffGround, parent.PROPERTIES.areWeHuman);
	}

	public void jumpMoreORIGINAL(bool isNewJump)
	{
	}

	public void jumpMoreORIGINALOLD()
	{
	}
}
