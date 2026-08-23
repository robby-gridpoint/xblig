using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public static class FighterAttackCoordinator
{
	public static bool areYouFacingTowardsYourEnemy(FighterObject f, FighterObject enemy)
	{
		if (f.PROPERTIES.isFacing == Definitions.facing.left && f.X + f.width / 2 > enemy.X + enemy.width / 2)
		{
			return true;
		}
		if (f.PROPERTIES.isFacing == Definitions.facing.right && f.X + f.width / 2 < enemy.X + enemy.width / 2)
		{
			return true;
		}
		return false;
	}

	public static bool doPunchCollisionDetection(FighterObject f, FighterObject enemy)
	{
		if (enemy == null || f == enemy)
		{
			return false;
		}
		if (RandomStaticGlobals.DoRectsCollide(f.getWhereFistIs(), enemy.getWhereBodyIs()))
		{
			return true;
		}
		return false;
	}

	public static bool doAoECollisionDetection(FighterObject f, FighterObject enemy, bool mustBeFacingEnemy, int inflateAmount)
	{
		if (enemy == null || f == enemy)
		{
			return false;
		}
		Rectangle rectSpriteDisplay = f.rectSpriteDisplay;
		rectSpriteDisplay.Inflate(inflateAmount, inflateAmount);
		if ((!mustBeFacingEnemy || areYouFacingTowardsYourEnemy(f, enemy)) && rectSpriteDisplay.Intersects(enemy.getWhereBodyIs()))
		{
			return true;
		}
		return false;
	}

	public static void shootArrow(FighterObject f, FighterObject target)
	{
		f.PlayAnimation(FighterObjectProperties.AnimationName.QuickPunching, broadcastThis: true);
		f.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.rangedArrow;
		f.PROPERTIES.isInTheMiddleOfAnAnimation = true;
		f.PROPERTIES.isFinishedPunching = false;
	}

	public static void quickAttack(FighterObject f)
	{
		if (f.PROPERTIES.areWeHuman && f.PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.swing)
		{
			SoundManager.playNextQuickWhoosh(f.getPlayHereValue());
		}
		f.PlayAnimation(FighterObjectProperties.AnimationName.QuickPunching, broadcastThis: true);
		f.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.swing;
		f.PROPERTIES.isInTheMiddleOfAnAnimation = true;
		f.PROPERTIES.isFinishedPunching = false;
	}

	public static int quickAttack(FighterObject f, List<FighterObject> enemies)
	{
		f.PROPERTIES.isFinishedPunching = true;
		f.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.nulll;
		f.PROPERTIES.isInTheMiddleOfAnAnimation = false;
		f.onIdle();
		ObstacleManager.doEnvironmentalDestructionCollision(f.getWhereFistIs(), Definitions.quickPunchDamage);
		foreach (FighterObject enemy in enemies)
		{
			if (doPunchCollisionDetection(f, enemy))
			{
				enemy.hitMe(f, isQuickPunch: true);
				if (f.PROPERTIES.areWeHuman && (!enemy.PROPERTIES.isAlive || enemy.PROPERTIES.health < 0f))
				{
					f.PROPERTIES.HumanProfile.VictimsCausesOfDeath.Add(FighterManager.CauseOfDeath.fastAttack);
				}
				return 1;
			}
		}
		return 0;
	}

	public static void slowAttack(FighterObject f)
	{
		if (f.PROPERTIES.areWeHuman && f.PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.chop)
		{
			SoundManager.playNextSlowWhoosh(f.getPlayHereValue());
		}
		f.PlayAnimation(FighterObjectProperties.AnimationName.Punching, broadcastThis: true);
		f.PROPERTIES.isFinishedPunching = false;
		f.PROPERTIES.isInTheMiddleOfAnAnimation = true;
		f.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.chop;
	}

	public static void shootArrowCPU(FighterObject f)
	{
		f.PROPERTIES.isFinishedPunching = true;
		f.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.nulll;
		f.PROPERTIES.isInTheMiddleOfAnAnimation = false;
		f.onIdle();
		if (f.X < f.PROPERTIES.targerFighter.X)
		{
			f.PROPERTIES.isFacing = Definitions.facing.right;
		}
		else
		{
			f.PROPERTIES.isFacing = Definitions.facing.left;
		}
		FighterObject targerFighter = f.PROPERTIES.targerFighter;
		Vector2 vector = new Vector2(targerFighter.X + targerFighter.width / 2 - (f.X + f.getPersonalSpace().Width / 2), targerFighter.Y + targerFighter.height / 2 - (f.Y + f.height / 2));
		Vector2 vector2 = new Vector2(RandomStaticGlobals.makePositive(vector.X), RandomStaticGlobals.makePositive(vector.Y));
		if (vector2.X > vector2.Y)
		{
			vector.X /= vector2.X;
			if (vector.Y != 0f)
			{
				vector.Y /= vector2.X;
			}
		}
		else
		{
			vector.Y /= vector2.Y;
			if (vector.X != 0f)
			{
				vector.X /= vector2.Y;
			}
		}
		vector.Y *= -1f;
		f.RANGED.rangedRelease(vector.X, vector.Y);
	}

	public static int slowAttack(FighterObject f, List<FighterObject> enemies)
	{
		f.PROPERTIES.isFinishedPunching = true;
		f.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.nulll;
		f.PROPERTIES.isInTheMiddleOfAnAnimation = false;
		f.onIdle();
		ObstacleManager.doEnvironmentalDestructionCollision(f.getWhereFistIs(), Definitions.slowPunchDamage);
		foreach (FighterObject enemy in enemies)
		{
			if (doPunchCollisionDetection(f, enemy))
			{
				enemy.hitMe(f, isQuickPunch: false);
				if ((f.PROPERTIES.areWeHuman && !enemy.PROPERTIES.isAlive) || enemy.PROPERTIES.health < 0f)
				{
					f.PROPERTIES.HumanProfile.VictimsCausesOfDeath.Add(FighterManager.CauseOfDeath.heavyAttack);
				}
				return 1;
			}
		}
		return 0;
	}

	public static void ShootBullet(FighterObject f)
	{
		Vector2 zero = Vector2.Zero;
		float num = 1f;
		int damage = (int)f.PROPERTIES.DamageFromQuickAttack;
		zero = ((f.PROPERTIES.isFacing != Definitions.facing.left) ? new Vector2(1f, 0f) : new Vector2(-1f, 0f));
		num += RandomStaticGlobals.RollVelocity.X / (float)Definitions.UpdatesPerSecond;
		ProjectileManager.addNewProjectile(f.X, f.Y, zero, num, f.PROPERTIES.areWeHuman, f, ProjectileManager.ProjectileType.bullet, 100, 50, damage, broadcast: true);
	}

	public static void ShootLaser(FighterObject f)
	{
		Vector2 zero = Vector2.Zero;
		float speed = 10f;
		int damage = (int)f.PROPERTIES.DamageFromQuickAttack;
		ProjectileManager.addNewProjectile(direction: (f.PROPERTIES.isFacing != Definitions.facing.left) ? new Vector2(1f, 0f) : new Vector2(-1f, 0f), x: f.X, y: f.Y, speed: speed, areWeHuman: f.PROPERTIES.areWeHuman, shooter: f, whatsIsIt: ProjectileManager.ProjectileType.laser, width: 750, height: 25, damage: damage, broadcast: true);
		if (f.PROPERTIES.areWeHuman)
		{
			SoundManager.PlaySoundDirectly(SoundManager.laser1);
		}
		else
		{
			SoundManager.PlaySoundDirectly(SoundManager.laser3);
		}
	}

	public static void ShootFlappyProjectile(FighterObject f)
	{
		Vector2 zero = Vector2.Zero;
		float speed = 0.75f;
		int damage = 100;
		ProjectileManager.addNewProjectile(direction: new Vector2(0.3f, 1f), x: f.X, y: f.Y, speed: speed, areWeHuman: f.PROPERTIES.areWeHuman, shooter: f, whatsIsIt: ProjectileManager.ProjectileType.poo, width: 128, height: 128, damage: damage, broadcast: true);
		if (f.PROPERTIES.areWeHuman)
		{
			SoundManager.PlaySoundDirectly(SoundManager.laser1);
		}
		else
		{
			SoundManager.PlaySoundDirectly(SoundManager.laser3);
		}
	}

	public static void DropBomb(FighterObject f)
	{
		int damage = (int)f.PROPERTIES.DamageFromAttack;
		ProjectileManager.addNewProjectile(f.X, f.Y, new Vector2(0f, 1f), 0.3f, f.PROPERTIES.areWeHuman, f, ProjectileManager.ProjectileType.bomb, 200, 100, damage, broadcast: true);
	}

	public static void Hadouken(FighterObject f)
	{
		f.PROPERTIES.currentAttack = Definitions.FighterSpecialMoves.Hadouken;
		float num = (float)f.PROPERTIES.GetLevelOf(Definitions.FighterSpecialMoves.Hadouken) * 0.2f;
		if (f.PROPERTIES.isFacing == Definitions.facing.left)
		{
			ProjectileManager.addNewRock(f.X, f.Y, new Vector2(-1f, 0f), (float)Definitions.SpeedOfHadouken + num, isHumanArrow: true, f);
		}
		else
		{
			ProjectileManager.addNewRock(f.X, f.Y, new Vector2(1f, 0f), (float)Definitions.SpeedOfHadouken + num, isHumanArrow: true, f);
		}
	}
}
