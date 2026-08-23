using System;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public class ProjectileObject
{
	public Rectangle rect;

	public ProjectileManager.ProjectileType type = ProjectileManager.ProjectileType.none;

	public FighterObject shooter = null;

	public string name = "";

	public string uniqueName = "";

	public int speedInPixelsPerSecond = 10;

	public int speedFallingInPixelsPerSecond = 10;

	public int damage = 1;

	public int maxRange = 9000;

	public int maxRangeLoiterDurationMS = 100;

	private DateTime? clearFromScreenAfter = null;

	public Vector2 direction = new Vector2(-1000f, 500f);

	public bool isActive = true;

	public bool isVisible = true;

	public bool isCounterable = false;

	public bool itDamagesHumans = false;

	public bool itDamagesComputers = false;

	public Definitions.FighterSpecialMoves currentAttack = Definitions.FighterSpecialMoves.nulll;

	public float Rotation = 0f;

	public Vector2 origin = Vector2.Zero;

	public int X
	{
		get
		{
			return rect.X;
		}
		set
		{
			rect.X = value;
		}
	}

	public int Y
	{
		get
		{
			return rect.Y;
		}
		set
		{
			rect.Y = value;
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
		return 0.99999f;
	}

	public Rectangle getRectStretched()
	{
		return new Rectangle((int)origin.X, (int)origin.Y, (int)RandomStaticGlobals.makePositive(shooter.X - X) + width, height);
	}

	public void blocked()
	{
		direction.X = 0f - direction.X;
		direction.Y = 1f;
		speedInPixelsPerSecond = (int)Math.Ceiling((float)speedInPixelsPerSecond * 0.1f) + 1;
		itDamagesComputers = false;
		itDamagesHumans = false;
		SoundManager.PlaySound("clank");
	}

	public void move()
	{
		if (clearFromScreenAfter.HasValue && clearFromScreenAfter < DateTime.Now)
		{
			X = -10000;
			Y = -10000;
			isActive = false;
			isVisible = false;
			return;
		}
		if ((float)maxRange < RandomStaticGlobals.makePositive(X - shooter.X) + RandomStaticGlobals.makePositive(Y - shooter.Y) || Y > GraphicsManager.BoundariesDefault.Height + 333)
		{
			if (!clearFromScreenAfter.HasValue)
			{
				clearFromScreenAfter = DateTime.Now.AddMilliseconds(maxRangeLoiterDurationMS);
			}
			return;
		}
		X += (int)(direction.X * (float)speedInPixelsPerSecond / (float)Definitions.UpdatesPerSecond);
		Y += (int)(direction.Y * (float)speedInPixelsPerSecond / (float)Definitions.UpdatesPerSecond);
		if (type == ProjectileManager.ProjectileType.tongue || type == ProjectileManager.ProjectileType.laser)
		{
			return;
		}
		if (type != ProjectileManager.ProjectileType.bullet)
		{
			direction.Y += 0.1f / (float)Definitions.UpdatesPerSecond;
			if (direction.Y > 1f)
			{
				direction.Y = 1f;
			}
			if (direction.Y < 0f)
			{
				speedInPixelsPerSecond -= 150 / Definitions.UpdatesPerSecond;
			}
			else
			{
				speedInPixelsPerSecond += 150 / Definitions.UpdatesPerSecond;
			}
		}
		Rotation = (float)Math.Atan2(direction.Y, direction.X);
	}
}
