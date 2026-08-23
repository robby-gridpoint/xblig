using System;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public class ObstacleObject
{
	public int ID = -1;

	public Rectangle rect;

	public OnDestructionCallback onDestruction;

	public Texture2D image;

	public string name = "";

	public string uniqueName = "";

	public int hp = 10;

	public bool isActive = true;

	public bool isDestructible = true;

	public bool isOnGround = true;

	public int xRoll = 0;

	public int yRoll = 0;

	public float partialXRoll = 0f;

	public float partialYRoll = 0f;

	public int pixelsInTheAir = 0;

	public bool isFalling = false;

	public int fallSpeedPerFrame = Definitions.ObstaclePixelsToFallPerFrame;

	public int fallDamageAfterLanding = Definitions.ObstacleFallDamageAfterLanding;

	public int DPS = 0;

	public bool isPickupable = true;

	public bool isInPickupableRange = false;

	public bool isReallyScenery = false;

	public bool isFlippedVertically = false;

	public bool isFlippedHorizontally = false;

	public FighterObject isBeingCarriedBy = null;

	public DateTime dtLastTimeInPickupableRange = DateTime.MinValue;

	public Vector2 circlePivotPoint = Vector2.Zero;

	public Vector2 circleVelocity = Vector2.Zero;

	public Vector2 circleProgress = Vector2.Zero;

	public NonFighterAI.modes AImode = NonFighterAI.modes.none;

	public int AIAmountSpeed = 0;

	public int AIAmountDistance = 0;

	public string AIMemory = "";

	private float xPartial = 0f;

	private float yPartial = 0f;

	public float X
	{
		get
		{
			return rect.X;
		}
		set
		{
			rect.X = (int)(value + xPartial);
			xPartial = value - (float)rect.X;
		}
	}

	public float Y
	{
		get
		{
			return rect.Y;
		}
		set
		{
			rect.Y = (int)(value + yPartial);
			yPartial = value - (float)rect.Y;
		}
	}

	public int width
	{
		get
		{
			return rect.Width;
		}
		set
		{
			rect.Width = value;
		}
	}

	public int height
	{
		get
		{
			return rect.Height;
		}
		set
		{
			rect.Height = value;
		}
	}

	public float getLayerDepth()
	{
		float layerDepth = RandomStaticGlobals.getLayerDepth((int)Y, height);
		if (layerDepth > Definitions.LayerDepthFourthHighest)
		{
			return Definitions.LayerDepthFourthHighest;
		}
		return layerDepth;
	}

	public ObstacleObject()
	{
	}

	public ObstacleObject(Rectangle Rect, OnDestructionCallback callback, Texture2D Image, int HP, bool IsActive, bool IsDestructable, bool IsOnGround)
	{
		rect = Rect;
		onDestruction = callback;
		image = Image;
		hp = HP;
		isActive = IsActive;
		isDestructible = IsDestructable;
		isOnGround = IsOnGround;
	}

	public ObstacleObject(Rectangle Rect, Texture2D Image, int HP)
	{
		rect = Rect;
		onDestruction = null;
		image = Image;
		hp = HP;
		isActive = true;
		isDestructible = true;
		isOnGround = true;
	}

	public ObstacleObject(Rectangle Rect, Texture2D Image)
	{
		rect = Rect;
		onDestruction = null;
		image = Image;
		hp = 1;
		isActive = true;
		isDestructible = false;
		isOnGround = true;
	}

	public void takeDamage(int amount, bool broadcast)
	{
		if (isDestructible && isActive)
		{
			hp -= amount;
			if (hp < 0)
			{
				isActive = false;
				TriggerManager.SetTriggerEvent(name + "Destroyed");
				TriggerManager.SetTriggerEvent(uniqueName + "Destroyed");
				AwardmentsManager.CheckForAwardments(name + "Destroyed");
				AwardmentsManager.CheckForAwardments(uniqueName + "Destroyed");
			}
			if (broadcast)
			{
				NetworkGameplayManager.SendObjectDamage(ID, amount);
			}
		}
	}

	public ObstacleObject Copy()
	{
		return (ObstacleObject)MemberwiseClone();
	}
}
