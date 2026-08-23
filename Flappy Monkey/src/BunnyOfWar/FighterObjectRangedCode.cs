using System;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public class FighterObjectRangedCode
{
	public FighterObject parent = null;

	public int rangedDamage = 1;

	private Vector2 rangedPullbackPosition = default(Vector2);

	private Vector2 rangedReleasePosition = default(Vector2);

	public ProjectileManager.ProjectileType rangedProjectileType = ProjectileManager.ProjectileType.arrow;

	public Vector2 rangedOrigin = Vector2.Zero;

	private DateTime rangedLastShotTaken = DateTime.MinValue;

	public FighterObjectRangedCode(FighterObject Parent)
	{
		parent = Parent;
	}

	public void rangedPullbackUpdate(float x, float y)
	{
		float num = RandomStaticGlobals.makePositive(x);
		float num2 = RandomStaticGlobals.makePositive(y);
		float num3 = RandomStaticGlobals.makePositive(rangedPullbackPosition.X);
		float num4 = RandomStaticGlobals.makePositive(rangedPullbackPosition.Y);
		if (num > num3 || num2 > num4)
		{
			rangedPullbackPosition.X = x;
			rangedPullbackPosition.Y = y;
		}
	}

	public void rangedRelease(float x, float y)
	{
		float num = 0f;
		if (parent.PROPERTIES.areWeHuman && parent.PROPERTIES.HumanProfile.AttackLevels.ContainsKey(Definitions.FighterSpecialMoves.rangedArrow))
		{
			num = (float)parent.PROPERTIES.HumanProfile.AttackLevels[Definitions.FighterSpecialMoves.rangedArrow] * 0.2f - 0.2f;
		}
		if (!(rangedLastShotTaken.AddMilliseconds(1000f / (Definitions.HumanRangedMaxShotsPerSecond + num)) > DateTime.Now))
		{
			y *= -1f;
			float speed = 1f;
			switch (rangedProjectileType)
			{
			case ProjectileManager.ProjectileType.arrow:
				parent.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.rangedArrow;
				ProjectileManager.addNewArrow(parent.X + parent.width / 2, parent.Y + parent.height / 2 - parent.JUMP.jumpPixelsOffGround, new Vector2(x, y), speed, parent.PROPERTIES.areWeHuman, parent);
				break;
			case ProjectileManager.ProjectileType.rock:
				parent.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.Hadouken;
				ProjectileManager.addNewRock(parent.X + parent.width / 2, parent.Y + parent.height / 2 - parent.JUMP.jumpPixelsOffGround, new Vector2(x, y), speed, parent.PROPERTIES.areWeHuman, parent);
				break;
			}
			rangedLastShotTaken = DateTime.Now;
		}
	}

	public void rangedTongueAttack(FighterObject enemy)
	{
		if (rangedLastShotTaken.AddMilliseconds(1000f / Definitions.HumanRangedMaxShotsPerSecond) > DateTime.Now)
		{
			return;
		}
		Vector2 vector = new Vector2((float)parent.X + rangedOrigin.X, (float)parent.Y + rangedOrigin.Y);
		Vector2 vector2 = new Vector2(enemy.X, enemy.Y);
		Vector2 direction = new Vector2((float)(enemy.X + enemy.width / 2) - vector.X, (float)(enemy.Y + enemy.height / 2) - vector.Y);
		Vector2 vector3 = new Vector2(RandomStaticGlobals.makePositive(direction.X), RandomStaticGlobals.makePositive(direction.Y));
		if (vector3.X > vector3.Y)
		{
			direction.X /= vector3.X;
			if (direction.Y != 0f)
			{
				direction.Y /= vector3.X;
			}
		}
		else
		{
			direction.Y /= vector3.Y;
			if (direction.X != 0f)
			{
				direction.X /= vector3.Y;
			}
		}
		ProjectileManager.addNewTongue((int)vector.X, (int)vector.Y, direction, 1f, isHumanArrow: false, parent);
		rangedLastShotTaken = DateTime.Now;
	}
}
