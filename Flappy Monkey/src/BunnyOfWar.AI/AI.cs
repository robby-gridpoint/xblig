using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BunnyOfWar.AI;

public static class AI
{
	public enum modes
	{
		doNothing,
		BaitChaser,
		RoadKill,
		PoliceCar,
		PoliceCarFPS,
		PoliceHelicopterFPS
	}

	public static void doRangedComplexStrategy(FighterObject cpu, List<FighterObject> humans)
	{
		if (cpu.PROPERTIES.carriedByFighter != null)
		{
			return;
		}
		int num = 0;
		if (cpu.PROPERTIES.AIMemory == "")
		{
			bool flag = false;
			foreach (FighterObject human in humans)
			{
				Rectangle personalSpace = cpu.getPersonalSpace();
				personalSpace.Inflate(cpu.PROPERTIES.AIAmountDistance, cpu.PROPERTIES.AIAmountDistance);
				if (personalSpace.Intersects(human.getPersonalSpace()))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			cpu.PROPERTIES.AIMemory = "attacking";
		}
		if (cpu.PROPERTIES.CpuAttackCooldown > DateTime.Now)
		{
			cpu.onIdle();
			return;
		}
		if (cpu.PROPERTIES.CpuAttackCooldown == DateTime.MinValue)
		{
			cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(num);
			return;
		}
		Rectangle rectangle = new Rectangle(cpu.X - cpu.PROPERTIES.AIAmountDistance, 0, cpu.PROPERTIES.AIAmountDistance * 2, 1200);
		if (cpu.PROPERTIES.AIMemory != "animating")
		{
			foreach (FighterObject human2 in humans)
			{
				if (rectangle.Intersects(human2.getWhereBodyIs()))
				{
					cpu.PROPERTIES.AIMemory = "animating";
					FighterAttackCoordinator.shootArrow(cpu, human2);
					if (cpu.X < human2.X)
					{
						cpu.PROPERTIES.isFacing = Definitions.facing.right;
					}
					else
					{
						cpu.PROPERTIES.isFacing = Definitions.facing.left;
					}
					cpu.PROPERTIES.targerFighter = human2;
					break;
				}
			}
			return;
		}
		if (cpu.PROPERTIES.AIMemory == "animating" && !cpu.PROPERTIES.isInTheMiddleOfAnAnimation)
		{
			cpu.PROPERTIES.AIMemory = "attacking";
			cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(cpu.PROPERTIES.AIAmountSpeed);
		}
	}

	private static int getDistance(FighterObject cpu)
	{
		return (int)RandomStaticGlobals.makePositive(cpu.X - (int)cpu.PROPERTIES.CpuJumpDestination.X);
	}

	public static void goAndRunForYourLife(FighterObject cpu, List<FighterObject> humans)
	{
	}

	private static void RotateVector2(ref Vector2 origin, float radians, ref Vector2 Vector)
	{
		Matrix matrix = Matrix.CreateRotationX(radians);
		Vector2 vector = Vector2.Transform(Vector - origin, matrix);
		Vector = vector - origin;
	}

	public static void moveInCircles(FighterObject cpu)
	{
		int num = 5;
		if (cpu.PROPERTIES.circleRadius == 0f)
		{
			cpu.PROPERTIES.circleRadius = 150f;
			cpu.PROPERTIES.circlePivotPoint = cpu.getXYVector2();
			cpu.PROPERTIES.circleVelocity = new Vector2(0f, num);
		}
		if ((float)cpu.Y > cpu.PROPERTIES.circlePivotPoint.Y + cpu.PROPERTIES.circleRadius)
		{
			cpu.PROPERTIES.circleVelocity.Y = -num;
		}
		if ((float)cpu.Y < cpu.PROPERTIES.circlePivotPoint.Y - cpu.PROPERTIES.circleRadius)
		{
			cpu.PROPERTIES.circleVelocity.Y = num;
		}
		if (!cpu.move(cpu.PROPERTIES.circleVelocity.X, cpu.PROPERTIES.circleVelocity.Y))
		{
			cpu.PROPERTIES.circleVelocity.Y *= -1f;
		}
	}

	public static void doBossAttack(FighterObject cpu, List<FighterObject> humans, int initialDelayMS, int delayMS)
	{
	}

	public static bool SeekAndDestroyAndCooldown(FighterObject cpu, List<FighterObject> humans, int initialDelayMS, int delayMS)
	{
		if (cpu.PROPERTIES.AIMemory == "" || cpu.PROPERTIES.AIMemory == "NoDelay")
		{
			bool flag = false;
			foreach (FighterObject human in humans)
			{
				Rectangle personalSpace = cpu.getPersonalSpace();
				personalSpace.Inflate(cpu.PROPERTIES.AIAmountDistance, cpu.PROPERTIES.AIAmountDistance);
				if (RandomStaticGlobals.DoRectsCollide(human.getPersonalSpace(), personalSpace))
				{
					flag = true;
					if (human.PROPERTIES.isFacing == Definitions.facing.left)
					{
						cpu.PROPERTIES.isFacing = Definitions.facing.right;
					}
					else
					{
						cpu.PROPERTIES.isFacing = Definitions.facing.left;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
			if (cpu.PROPERTIES.AIMemory == "NoDelay")
			{
				cpu.PROPERTIES.CpuAttackCooldown = DateTime.MinValue.AddDays(7.0);
			}
			cpu.PROPERTIES.AIMemory = "attacking";
		}
		if (cpu.PROPERTIES.CpuAttackCooldown > DateTime.Now)
		{
			cpu.onIdle();
			return false;
		}
		if (cpu.PROPERTIES.CpuAttackCooldown == DateTime.MinValue)
		{
			cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(initialDelayMS);
			return false;
		}
		return true;
	}

	public static void doDelayedQuickAttack(FighterObject cpu, List<FighterObject> humans, int initialDelayMS, int delayMS)
	{
		if (!SeekAndDestroyAndCooldown(cpu, humans, initialDelayMS, delayMS))
		{
			return;
		}
		FighterObject fighterObject = humans[0];
		if (humans.Count > 1)
		{
			fighterObject = FighterManager.FindTheClosestFighter(cpu, humans);
			if (fighterObject == null)
			{
				fighterObject = humans[0];
			}
		}
		if (cpu.getWhereFistIs().Intersects(fighterObject.getWhereBodyIs()))
		{
			if (cpu.PROPERTIES.getCenter().X < fighterObject.PROPERTIES.getCenter().X)
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.right;
			}
			else
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.left;
			}
			if (!fighterObject.PROPERTIES.isCountering)
			{
				cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
				FighterAttackCoordinator.slowAttack(cpu);
			}
			else
			{
				cpu.stunMe(1.0, broadcast: true);
			}
			cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(delayMS);
		}
		else
		{
			moveCloser(cpu, fighterObject);
		}
	}

	public static FighterObject FindNewHumanTarget(FighterObject cpu, List<FighterObject> humans)
	{
		FighterObject fighterObject = null;
		int num = 10000;
		if (cpu.PROPERTIES.targerFighter != null)
		{
			fighterObject = cpu.PROPERTIES.targerFighter;
		}
		if (cpu.PROPERTIES.targerFighter == null || !cpu.PROPERTIES.targerFighter.PROPERTIES.isAlive)
		{
			foreach (FighterObject human in humans)
			{
				if (human.getPersonalSpace().Intersects(cpu.getPersonalSpace()))
				{
					float num2 = RandomStaticGlobals.makePositive(human.X - cpu.X) + RandomStaticGlobals.makePositive(human.Y + cpu.Y);
					if (num2 < (float)num)
					{
						fighterObject = human;
						cpu.PROPERTIES.targerFighter = human;
					}
				}
			}
		}
		cpu.PROPERTIES.targerFighter = fighterObject;
		return fighterObject;
	}

	public static void doGreekBlocker(FighterObject cpu, List<FighterObject> humans, int initialDelayMS, int delayMS)
	{
		if (!SeekAndDestroyAndCooldown(cpu, humans, initialDelayMS, delayMS))
		{
			return;
		}
		FighterObject fighterObject = FindNewHumanTarget(cpu, humans);
		if (cpu.PROPERTIES.targerFighter == null || !cpu.PROPERTIES.targerFighter.PROPERTIES.isAlive)
		{
			return;
		}
		if (cpu.getWhereFistIs().Intersects(fighterObject.getWhereBodyIs()))
		{
			if (cpu.PROPERTIES.getCenter().X < fighterObject.PROPERTIES.getCenter().X)
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.right;
			}
			else
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.left;
			}
			if (cpu.PROPERTIES.AIMemory == "vulnerable" && (cpu.PROPERTIES.AIattackAfter == DateTime.MaxValue || (cpu.PROPERTIES.AIattackAfter < DateTime.Now && cpu.PROPERTIES.isFinishedPunching)))
			{
				if (!fighterObject.PROPERTIES.isCountering && cpu.PROPERTIES.AIattackAfter < DateTime.Now)
				{
					cpu.PROPERTIES.isFinishedPunching = false;
					cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
					FighterAttackCoordinator.slowAttack(cpu);
				}
				else
				{
					cpu.stunMe(1.0, broadcast: true);
				}
				cpu.PROPERTIES.AIMemory = "attacking";
			}
			else if (cpu.PROPERTIES.AIMemory == "attacking")
			{
				if (cpu.PROPERTIES.currentAttack == Definitions.FighterSpecialMoves.nulll)
				{
					cpu.PROPERTIES.AIMemory = "blocking";
					cpu.onBlock(delayMS / 1000);
					cpu.PROPERTIES.AIattackAfter = DateTime.Now.AddMilliseconds(delayMS);
				}
			}
			else if (cpu.PROPERTIES.AIMemory == "blocking")
			{
				cpu.onBlock();
				if (cpu.PROPERTIES.AIattackAfter < DateTime.Now)
				{
					cpu.onUnBlock();
					cpu.PROPERTIES.AIMemory = "vulnerable";
					cpu.PROPERTIES.AIattackAfter = DateTime.Now.AddMilliseconds(delayMS);
				}
			}
			else
			{
				cpu.onIdle();
				cpu.PROPERTIES.AIMemory = "vulnerable";
			}
		}
		else
		{
			moveCloser(cpu, fighterObject);
		}
	}

	public static void doGreekBOSSBlocker(FighterObject cpu, List<FighterObject> humans, int initialDelayMS, int delayMS)
	{
		Rectangle rectSpriteDisplay = cpu.rectSpriteDisplay;
		rectSpriteDisplay.Inflate(500, 500);
		if (FighterManager.CountLivingCPUsWithinRect(rectSpriteDisplay) > 1)
		{
			if (cpu.PROPERTIES.targerFighter != null)
			{
				if (cpu.PROPERTIES.getCenter().X < cpu.PROPERTIES.targerFighter.PROPERTIES.getCenter().X)
				{
					cpu.PROPERTIES.isFacing = Definitions.facing.right;
				}
				else
				{
					cpu.PROPERTIES.isFacing = Definitions.facing.left;
				}
			}
			cpu.onBlock();
		}
		else
		{
			if (!SeekAndDestroyAndCooldown(cpu, humans, initialDelayMS, delayMS))
			{
				return;
			}
			FighterObject fighterObject = FindNewHumanTarget(cpu, humans);
			if (cpu.PROPERTIES.targerFighter == null || !cpu.PROPERTIES.targerFighter.PROPERTIES.isAlive)
			{
				return;
			}
			if (cpu.getWhereFistIs().Intersects(fighterObject.getWhereBodyIs()))
			{
				if (cpu.PROPERTIES.getCenter().X < fighterObject.PROPERTIES.getCenter().X)
				{
					cpu.PROPERTIES.isFacing = Definitions.facing.right;
				}
				else
				{
					cpu.PROPERTIES.isFacing = Definitions.facing.left;
				}
				if (cpu.PROPERTIES.AIMemory == "vulnerable" && (cpu.PROPERTIES.AIattackAfter == DateTime.MaxValue || (cpu.PROPERTIES.AIattackAfter < DateTime.Now && cpu.PROPERTIES.isFinishedPunching)))
				{
					if (!fighterObject.PROPERTIES.isCountering && cpu.PROPERTIES.AIattackAfter < DateTime.Now)
					{
						cpu.onUnBlockCPU();
						cpu.PROPERTIES.isFinishedPunching = false;
						cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
						FighterAttackCoordinator.slowAttack(cpu);
					}
					else
					{
						cpu.stunMe(1.0, broadcast: true);
					}
					cpu.PROPERTIES.AIMemory = "attacking";
				}
				else if (cpu.PROPERTIES.AIMemory == "attacking")
				{
					if (cpu.PROPERTIES.currentAttack == Definitions.FighterSpecialMoves.nulll)
					{
						cpu.PROPERTIES.AIMemory = "blocking";
						cpu.onBlock(delayMS / 1000);
						cpu.PROPERTIES.AIattackAfter = DateTime.Now.AddMilliseconds(delayMS);
					}
				}
				else if (cpu.PROPERTIES.AIMemory == "blocking")
				{
					cpu.onBlock();
					if (cpu.PROPERTIES.AIattackAfter < DateTime.Now)
					{
						cpu.onUnBlockCPU();
						cpu.PROPERTIES.AIMemory = "vulnerable";
						cpu.PROPERTIES.AIattackAfter = DateTime.Now.AddMilliseconds(delayMS);
					}
				}
				else
				{
					cpu.onIdle();
					cpu.PROPERTIES.AIMemory = "vulnerable";
				}
			}
			else
			{
				moveCloser(cpu, fighterObject);
			}
		}
	}

	public static void doGreekBOSSRockThrower(FighterObject cpu, List<FighterObject> humans, int initialDelayMS, int delayMS)
	{
		if (!SeekAndDestroyAndCooldown(cpu, humans, initialDelayMS, delayMS))
		{
			return;
		}
		FighterObject fighterObject = FindNewHumanTarget(cpu, humans);
		if (cpu.PROPERTIES.targerFighter == null || !cpu.PROPERTIES.targerFighter.PROPERTIES.isAlive)
		{
			return;
		}
		if (cpu.getWhereFistIs().Intersects(fighterObject.getWhereBodyIs()))
		{
			if (cpu.PROPERTIES.getCenter().X < fighterObject.PROPERTIES.getCenter().X)
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.right;
			}
			else
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.left;
			}
			if (cpu.PROPERTIES.AIMemory == "vulnerable" && (cpu.PROPERTIES.AIattackAfter == DateTime.MaxValue || (cpu.PROPERTIES.AIattackAfter < DateTime.Now && cpu.PROPERTIES.isFinishedPunching)))
			{
				if (!fighterObject.PROPERTIES.isCountering && cpu.PROPERTIES.AIattackAfter < DateTime.Now)
				{
					cpu.PROPERTIES.isFinishedPunching = false;
					cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
					FighterAttackCoordinator.slowAttack(cpu);
				}
				else
				{
					cpu.stunMe(1.0, broadcast: true);
				}
				cpu.PROPERTIES.AIMemory = "attacking";
			}
			else if (cpu.PROPERTIES.AIMemory == "attacking")
			{
				if (cpu.PROPERTIES.currentAttack == Definitions.FighterSpecialMoves.nulll)
				{
					cpu.PROPERTIES.AIMemory = "throwing";
					int x = cpu.PROPERTIES.targerFighter.X;
					int y = cpu.PROPERTIES.targerFighter.Y;
					int num = 300;
					int num2 = 900;
					ObstacleManager.AddMeteorite(x, y, 225, 150, num, num2);
					cpu.PROPERTIES.AIattackAfter = DateTime.Now.AddMilliseconds(num2 / num * 1100);
				}
			}
			else if (cpu.PROPERTIES.AIMemory == "throwing")
			{
				cpu.onIdle();
				if (cpu.PROPERTIES.AIattackAfter < DateTime.Now)
				{
					cpu.PROPERTIES.AIMemory = "vulnerable";
					cpu.PROPERTIES.AIattackAfter = DateTime.Now.AddMilliseconds(delayMS);
				}
			}
			else
			{
				cpu.onIdle();
				cpu.PROPERTIES.AIMemory = "vulnerable";
			}
		}
		else
		{
			moveCloser(cpu, fighterObject);
		}
	}

	public static void untangle(FighterObject cpu, FighterObject human)
	{
		if (!cpu.moveUp() && !cpu.moveDown() && !cpu.moveLeft())
		{
			cpu.moveRight();
		}
	}

	public static void moveCloser(FighterObject cpu, FighterObject human)
	{
		bool flag = false;
		if (!(RandomStaticGlobals.makePositive(human.X - cpu.X) < 600f) || !RandomStaticGlobals.isSkullSlingshotMode)
		{
		}
		Rectangle rectSpriteDisplay = cpu.rectSpriteDisplay;
		if (human.X < cpu.X)
		{
			rectSpriteDisplay.X -= cpu.PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond;
			if (!ObstacleManager.IsThisMoveDangerous(rectSpriteDisplay))
			{
				cpu.moveLeft();
			}
			else
			{
				cpu.moveRight();
			}
		}
		if (human.X > cpu.X)
		{
			rectSpriteDisplay.X += cpu.PROPERTIES.moveSpeed / Definitions.UpdatesPerSecond;
			if (!ObstacleManager.IsThisMoveDangerous(rectSpriteDisplay))
			{
				cpu.moveRight();
			}
			else
			{
				cpu.moveLeft();
			}
		}
		if (cpu.willThisMoveCollide(cpuCheck: true).HasValue)
		{
			untangle(cpu, human);
		}
	}

	public static void doXFastHardBlockIdle(FighterObject cpu, List<FighterObject> humans, int initialDelayMS, int delayMS, int xFast, int xHard, int blockMS, int vulnerableMS)
	{
		if (!SeekAndDestroyAndCooldown(cpu, humans, initialDelayMS, delayMS) || cpu.PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.nulll)
		{
			return;
		}
		FighterObject fighterObject = humans[0];
		if (humans.Count > 1)
		{
			fighterObject = FighterManager.FindTheClosestFighter(cpu, humans);
			if (fighterObject == null)
			{
				fighterObject = humans[0];
			}
		}
		if (cpu.getWhereFistIs().Intersects(fighterObject.getWhereBodyIs()))
		{
			if (cpu.PROPERTIES.getCenter().X < fighterObject.PROPERTIES.getCenter().X)
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.right;
			}
			else
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.left;
			}
			if (fighterObject.PROPERTIES.isCountering)
			{
				cpu.PROPERTIES.AIMemory = "";
				cpu.PROPERTIES.AIMemory2 = "";
				cpu.PROPERTIES.AIMemoryInt = 0;
				cpu.PROPERTIES.AIMemoryInt2 = 0;
				cpu.stunMe(1.0, broadcast: true);
				cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(vulnerableMS);
				return;
			}
			if (cpu.PROPERTIES.AIMemory2 == "")
			{
				cpu.PROPERTIES.AIMemory2 = "fast";
			}
			cpu.PROPERTIES.AIMemoryInt++;
			switch (cpu.PROPERTIES.AIMemory2)
			{
			case "fast":
				if (cpu.PROPERTIES.AIMemoryInt > xFast)
				{
					cpu.PROPERTIES.AIMemory2 = "hard";
					cpu.PROPERTIES.AIMemoryInt = 0;
				}
				else
				{
					cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
					FighterAttackCoordinator.quickAttack(cpu);
					cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(delayMS);
				}
				break;
			case "hard":
				if (cpu.PROPERTIES.AIMemoryInt > xHard)
				{
					cpu.PROPERTIES.AIMemory2 = "block";
					cpu.PROPERTIES.AIMemoryInt = 0;
				}
				else
				{
					cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
					FighterAttackCoordinator.slowAttack(cpu);
					cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(delayMS);
				}
				break;
			case "block":
				cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
				if (cpu.PROPERTIES.AIMemory != "IMBLOCKING")
				{
					cpu.PROPERTIES.AIMemory = "IMBLOCKING";
					cpu.onBlock(blockMS / 1000);
				}
				else if (cpu.PROPERTIES.CpuBlockDuration < DateTime.Now)
				{
					cpu.onUnBlock();
					cpu.PROPERTIES.AIMemory = "";
					cpu.PROPERTIES.AIMemory2 = "idle";
					cpu.PROPERTIES.AIMemoryInt = 0;
				}
				else
				{
					cpu.PlayAnimation(FighterObjectProperties.AnimationName.Blocking, broadcastThis: false);
				}
				break;
			case "idle":
				cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
				cpu.onIdle();
				cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(vulnerableMS);
				cpu.PROPERTIES.AIMemory2 = "fast";
				cpu.PROPERTIES.AIMemoryInt = 0;
				break;
			}
		}
		else
		{
			moveCloser(cpu, fighterObject);
		}
	}

	public static void doXHitsThenIdle(FighterObject cpu, List<FighterObject> humans, int initialDelayMS, int delayMS, int vulnerableMS, int hits, int hits2)
	{
		if (!SeekAndDestroyAndCooldown(cpu, humans, initialDelayMS, delayMS) || cpu.PROPERTIES.currentAttack != Definitions.FighterSpecialMoves.nulll)
		{
			return;
		}
		FighterObject fighterObject = humans[0];
		if (humans.Count > 1)
		{
			fighterObject = FighterManager.FindTheClosestFighter(cpu, humans);
			if (fighterObject == null)
			{
				fighterObject = humans[0];
			}
		}
		if (cpu.getWhereFistIs().Intersects(fighterObject.getWhereBodyIs()))
		{
			if (cpu.PROPERTIES.getCenter().X < fighterObject.PROPERTIES.getCenter().X)
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.right;
			}
			else
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.left;
			}
			if (fighterObject.PROPERTIES.isCountering)
			{
				cpu.PROPERTIES.AIMemory = "";
				cpu.PROPERTIES.AIMemory2 = "";
				cpu.PROPERTIES.AIMemoryInt = 0;
				cpu.PROPERTIES.AIMemoryInt2 = 0;
				cpu.stunMe(1.0, broadcast: true);
				cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(vulnerableMS);
				return;
			}
			if (hits < cpu.PROPERTIES.AIMemoryInt && cpu.PROPERTIES.AIMemory2 == "resting")
			{
				if (hits2 == 0)
				{
					cpu.PROPERTIES.AIMemoryInt = 0;
					cpu.PROPERTIES.AIMemory = "";
					cpu.PROPERTIES.AIMemory2 = "";
				}
				else if (hits2 > 0 && hits2 < cpu.PROPERTIES.AIMemoryInt2)
				{
					cpu.PROPERTIES.AIMemory = "";
					cpu.PROPERTIES.AIMemory2 = "";
					cpu.PROPERTIES.AIMemoryInt = 0;
					cpu.PROPERTIES.AIMemoryInt2 = 0;
				}
			}
			if (hits >= cpu.PROPERTIES.AIMemoryInt)
			{
				cpu.PROPERTIES.AIMemoryInt++;
				if (hits < cpu.PROPERTIES.AIMemoryInt)
				{
					cpu.PROPERTIES.AIMemory2 = "resting";
					cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(vulnerableMS);
					return;
				}
			}
			else if (hits2 >= cpu.PROPERTIES.AIMemoryInt2 && hits2 != 0)
			{
				cpu.PROPERTIES.AIMemoryInt2++;
				if (hits2 < cpu.PROPERTIES.AIMemoryInt2)
				{
					cpu.PROPERTIES.AIMemory2 = "resting";
					cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(vulnerableMS);
					return;
				}
			}
			cpu.PROPERTIES.recentButtonPressTime = DateTime.Now;
			FighterAttackCoordinator.quickAttack(cpu);
			cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(delayMS);
		}
		else
		{
			moveCloser(cpu, fighterObject);
		}
	}

	public static void RunToTheHills(FighterObject cpu)
	{
		if (cpu.PROPERTIES.circlePivotPoint == Vector2.Zero)
		{
			cpu.PROPERTIES.circlePivotPoint = new Vector2(cpu.X, cpu.Y);
		}
		if (cpu.PROPERTIES.AIAmountSpeed > 0)
		{
			cpu.moveRight(run: true);
		}
		else
		{
			cpu.moveLeft(run: true);
		}
		if (RandomStaticGlobals.makePositive((float)cpu.X - cpu.PROPERTIES.circlePivotPoint.X) > (float)cpu.PROPERTIES.AIAmountDistance)
		{
			cpu.PROPERTIES.isAlive = false;
		}
	}

	public static void doSomething(FighterObject cpu, List<FighterObject> humans)
	{
		if (humans == null || humans.Count == 0 || RandomStaticGlobals.isShowingCutScene)
		{
			return;
		}
		if (cpu.PROPERTIES.CpuMoveDestination != Vector2.One)
		{
			if ((float)cpu.Y > cpu.PROPERTIES.CpuMoveDestination.Y)
			{
				cpu.moveUp();
				if ((float)cpu.Y <= cpu.PROPERTIES.CpuMoveDestination.Y)
				{
					cpu.Y = (int)cpu.PROPERTIES.CpuMoveDestination.Y;
				}
			}
			else if ((float)cpu.Y < cpu.PROPERTIES.CpuMoveDestination.Y)
			{
				cpu.moveDown();
				if ((float)cpu.Y >= cpu.PROPERTIES.CpuMoveDestination.Y)
				{
					cpu.Y = (int)cpu.PROPERTIES.CpuMoveDestination.Y;
				}
			}
			if ((float)cpu.X < cpu.PROPERTIES.CpuMoveDestination.X)
			{
				cpu.moveRight();
				if ((float)cpu.X >= cpu.PROPERTIES.CpuMoveDestination.X)
				{
					cpu.PROPERTIES.CpuMoveDestination = Vector2.One;
				}
			}
			else if ((float)cpu.X > cpu.PROPERTIES.CpuMoveDestination.X)
			{
				cpu.moveLeft();
				if ((float)cpu.X <= cpu.PROPERTIES.CpuMoveDestination.X)
				{
					cpu.PROPERTIES.CpuMoveDestination = Vector2.One;
				}
			}
		}
		AIFlappy.doSomething(cpu, humans);
	}
}
