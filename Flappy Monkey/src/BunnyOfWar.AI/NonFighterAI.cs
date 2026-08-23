using System;
using Microsoft.Xna.Framework;

namespace BunnyOfWar.AI;

public static class NonFighterAI
{
	public enum modes
	{
		none,
		ImActuallyScenery,
		MoveCircular,
		MoveCircularSpeedup,
		MoveUpDown,
		MoveLeftRight,
		InfiniteBounce,
		SinkWhenSatOn,
		RiseWhenSatOn,
		MoveRocketWhenSatOn,
		MoveXWhenSatOn,
		ScrollRatioToPlayer,
		SpaceInvader,
		FallingMeteorite,
		FallingMeteoriteOneTime,
		ImDestructible,
		RoadKill
	}

	public static ObstacleObject moveLikeaFallingMeteorite(ObstacleObject o)
	{
		if (o.circlePivotPoint == Vector2.Zero)
		{
			o.fallSpeedPerFrame = o.AIAmountSpeed / Definitions.UpdatesPerSecond;
			o.pixelsInTheAir = o.AIAmountDistance;
			o.isFalling = true;
			o.circlePivotPoint = new Vector2(o.X, o.Y);
		}
		if (o.AImode == modes.FallingMeteoriteOneTime && o.pixelsInTheAir <= 0)
		{
			o.isActive = false;
		}
		if (o.pixelsInTheAir <= 0)
		{
			if (o.AImode == modes.FallingMeteoriteOneTime)
			{
				o.AImode = modes.none;
			}
			else
			{
				o.pixelsInTheAir = o.AIAmountDistance;
			}
		}
		return o;
	}

	public static ObstacleObject SinkWhenSatOn(ObstacleObject o)
	{
		if (o.circlePivotPoint == Vector2.Zero)
		{
			o.circlePivotPoint = new Vector2(o.X, o.Y);
		}
		Rectangle rect = o.rect;
		rect.Inflate(0, 30);
		if (FighterManager.doCollisionCheckFeet(rect, humans: true, cupoos: true) != null)
		{
			o.Y += (float)o.AIAmountSpeed / (float)Definitions.UpdatesPerSecond;
			if (RandomStaticGlobals.makePositive(o.Y - o.circlePivotPoint.Y) > (float)o.AIAmountDistance)
			{
				o.Y = o.circlePivotPoint.Y + (float)o.AIAmountDistance;
			}
		}
		else if (o.Y != o.circlePivotPoint.Y)
		{
			o.Y -= (float)o.AIAmountSpeed / 2f / (float)Definitions.UpdatesPerSecond;
			if (o.Y < o.circlePivotPoint.Y)
			{
				o.Y = o.circlePivotPoint.Y;
			}
		}
		return o;
	}

	public static ObstacleObject RiseWhenSatOn(ObstacleObject o)
	{
		if (o.circlePivotPoint == Vector2.Zero)
		{
			o.circlePivotPoint = new Vector2(o.X, o.Y);
		}
		Rectangle rect = o.rect;
		rect.Inflate(0, 8);
		if (FighterManager.doCollisionCheckFeet(rect, humans: true, cupoos: true) != null)
		{
			o.Y -= (float)o.AIAmountSpeed / (float)Definitions.UpdatesPerSecond;
			if (RandomStaticGlobals.makePositive(o.Y - o.circlePivotPoint.Y) > (float)o.AIAmountDistance)
			{
				return o;
			}
			FighterManager.doObstaclePush(new Rectangle((int)o.X, (int)o.Y - 50, o.width, o.height + 100), new Vector2(0f, -o.AIAmountSpeed / Definitions.UpdatesPerSecond), hitHumans: true, hitCPUs: true, o);
		}
		else if (o.Y != o.circlePivotPoint.Y)
		{
			o.Y += (float)o.AIAmountSpeed / 2f / (float)Definitions.UpdatesPerSecond;
			if (o.Y > o.circlePivotPoint.Y)
			{
				o.Y = o.circlePivotPoint.Y;
			}
		}
		return o;
	}

	public static ObstacleObject moveUpDown(ObstacleObject o)
	{
		if (o.circleVelocity == Vector2.Zero)
		{
			o.circleVelocity = new Vector2(0f, o.AIAmountSpeed / Definitions.UpdatesPerSecond);
			o.circlePivotPoint = new Vector2(o.X, o.Y);
		}
		if (RandomStaticGlobals.makePositive(o.Y - o.circlePivotPoint.Y) > (float)o.AIAmountDistance)
		{
			o.circleVelocity.Y *= -1f;
		}
		o.Y += (int)o.circleVelocity.Y;
		Rectangle rect = o.rect;
		rect.Inflate(0, 10);
		FighterManager.doObstaclePush(new Rectangle((int)o.X, (int)o.Y - 10, o.width, o.height + 20), o.circleVelocity, hitHumans: true, hitCPUs: true, o);
		return o;
	}

	public static SceneryObject moveUpDown(SceneryObject o)
	{
		if (o.circleVelocity == Vector2.Zero)
		{
			o.circleVelocity = new Vector2(0f, o.AIAmountSpeed / Definitions.UpdatesPerSecond);
			o.circlePivotPoint = new Vector2(o.X, o.Y);
		}
		if (RandomStaticGlobals.makePositive(o.Y - o.circlePivotPoint.Y) > (float)o.AIAmountDistance)
		{
			o.circleVelocity.Y *= -1f;
		}
		o.Y += (int)o.circleVelocity.Y;
		Rectangle rect = o.rect;
		rect.Inflate(0, 10);
		return o;
	}

	public static ObstacleObject moveLeftRight(ObstacleObject o)
	{
		if (o.circleVelocity == Vector2.Zero)
		{
			o.circleVelocity = new Vector2(o.AIAmountSpeed / Definitions.UpdatesPerSecond, 0f);
			o.circlePivotPoint = new Vector2(o.X, o.Y);
		}
		if (RandomStaticGlobals.makePositive(o.X - o.circlePivotPoint.X) > (float)o.AIAmountDistance)
		{
			o.circleVelocity.X *= -1f;
		}
		o.X += (int)o.circleVelocity.X;
		if (!o.isReallyScenery)
		{
			Rectangle rect = o.rect;
			rect.Inflate(0, 10);
			FighterManager.doObstaclePush(new Rectangle((int)o.X - 10, (int)o.Y - 10, o.width + 20, o.height + 10), o.circleVelocity, hitHumans: true, hitCPUs: true, o);
		}
		return o;
	}

	public static ObstacleObject moveXWhenSatOn(ObstacleObject o)
	{
		if (o.circlePivotPoint == Vector2.Zero)
		{
			o.circlePivotPoint = new Vector2(o.X, o.Y);
			o.circleVelocity.X = (float)o.AIAmountSpeed / (float)Definitions.UpdatesPerSecond;
			o.circleVelocity.X = (float)Math.Round(o.circleVelocity.X, 3);
		}
		Rectangle rect = o.rect;
		rect.Inflate(0, 30);
		if (FighterManager.doCollisionCheckFeet(rect, humans: true, cupoos: true) != null)
		{
			int num = (int)o.X;
			o.X += o.circleVelocity.X;
			if (RandomStaticGlobals.makePositive(o.Y - o.circlePivotPoint.Y) > (float)o.AIAmountDistance)
			{
				o.X = o.circlePivotPoint.X + (float)o.AIAmountDistance;
			}
			Vector2 velocity = new Vector2((int)o.X - num, 0f);
			if (!o.isReallyScenery)
			{
				rect.Inflate(0, 200);
				FighterManager.doObstaclePush(rect, velocity, hitHumans: true, hitCPUs: true, o);
			}
		}
		else if (o.Y != o.circlePivotPoint.Y)
		{
			o.Y -= (float)o.AIAmountSpeed / 2f / (float)Definitions.UpdatesPerSecond;
			if (o.Y < o.circlePivotPoint.Y)
			{
				o.Y = o.circlePivotPoint.Y;
			}
		}
		return o;
	}

	public static ObstacleObject MoveRocketWhenSatOn(ObstacleObject o)
	{
		Rectangle rect = o.rect;
		rect.Inflate(0, 10);
		if (FighterManager.doCollisionCheckFeet(rect, humans: true, cupoos: true) != null)
		{
			if (o.circlePivotPoint == Vector2.Zero)
			{
				o.circleVelocity = new Vector2(o.AIAmountSpeed / 10 / Definitions.UpdatesPerSecond, 0f);
				o.circlePivotPoint = new Vector2(o.X, o.Y);
				if (o.circleVelocity.X == 0f)
				{
					if (o.AIAmountSpeed > 0)
					{
						o.circleVelocity.X = 1f;
					}
					else
					{
						o.circleVelocity.X = -1f;
					}
				}
			}
			if (o.circleVelocity.X < (float)(o.AIAmountSpeed / Definitions.UpdatesPerSecond))
			{
				o.circleVelocity.X = 2f * o.circleVelocity.X;
			}
			if (RandomStaticGlobals.makePositive(o.X - o.circlePivotPoint.X) > (float)o.AIAmountDistance)
			{
				o.circleVelocity.X = 0f;
			}
			o.X += (int)o.circleVelocity.X;
			if (!o.isReallyScenery)
			{
				FighterManager.doObstaclePush(new Rectangle((int)o.X - 10, (int)o.Y - 10, o.width + 20, o.height + 10), o.circleVelocity, hitHumans: true, hitCPUs: true, o);
			}
		}
		return o;
	}

	public static ObstacleObject moveCircular(ObstacleObject o)
	{
		Vector2 vector = new Vector2(o.X, o.Y);
		if (o.circlePivotPoint == Vector2.Zero)
		{
			o.circlePivotPoint = new Vector2(o.X, o.Y);
			o.circleProgress = new Vector2(o.AIAmountDistance, o.AIAmountDistance);
			o.AIMemory = "0";
		}
		o.circleProgress = Vector2.TransformNormal(o.circleProgress, Matrix.CreateRotationZ(MathHelper.ToRadians((float)o.AIAmountSpeed / (float)Definitions.UpdatesPerSecond)));
		o.X = (int)o.circleProgress.X + (int)o.circlePivotPoint.X;
		o.Y = (int)o.circleProgress.Y + (int)o.circlePivotPoint.Y;
		if (o.AImode == modes.MoveCircularSpeedup && o.AIAmountSpeed < 540)
		{
			o.AIMemory = (int.Parse(o.AIMemory) + o.AIAmountSpeed).ToString();
			if (int.Parse(o.AIMemory) > 720)
			{
				o.AIMemory = "0";
				o.AIAmountSpeed += 5;
			}
		}
		if (!o.isReallyScenery)
		{
			vector -= new Vector2(o.X, o.Y);
			vector.Y = 0f;
			FighterManager.doObstaclePush(new Rectangle((int)o.X, (int)o.Y, o.width, o.height), vector * -4f, hitHumans: true, hitCPUs: true, o);
		}
		return o;
	}

	public static SceneryObject moveCircular(ref SceneryObject o)
	{
		Vector2 vector = new Vector2(o.X, o.Y);
		if (o.circlePivotPoint == Vector2.Zero)
		{
			o.circlePivotPoint = new Vector2(o.X, o.Y);
			o.circleProgress = new Vector2(o.AIAmountDistance, o.AIAmountDistance);
			o.AIMemory = "0";
		}
		o.circleProgress = Vector2.TransformNormal(o.circleProgress, Matrix.CreateRotationZ(MathHelper.ToRadians((float)o.AIAmountSpeed / (float)Definitions.UpdatesPerSecond)));
		o.X = (int)o.circleProgress.X + (int)o.circlePivotPoint.X;
		o.Y = (int)o.circleProgress.Y + (int)o.circlePivotPoint.Y;
		if (o.AImode == modes.MoveCircularSpeedup && o.AIAmountSpeed < 540)
		{
			o.AIMemory = (int.Parse(o.AIMemory) + o.AIAmountSpeed).ToString();
			if (int.Parse(o.AIMemory) > 720)
			{
				o.AIMemory = "0";
				o.AIAmountSpeed += 5;
			}
		}
		return o;
	}

	private static void moveCircularDOH(ObstacleObject o)
	{
		float num = 25f;
		if (o.X >= o.circlePivotPoint.X + (float)o.AIAmountDistance)
		{
			o.circleVelocity.X = 0f - num;
			o.circleVelocity.Y = -1f;
		}
		if (o.X <= o.circlePivotPoint.X - (float)o.AIAmountDistance)
		{
			o.circleVelocity.X = num;
			o.circleVelocity.Y = 1f;
		}
		o.X += (int)o.circleVelocity.X;
		o.Y += (int)o.circleVelocity.X;
	}
}
