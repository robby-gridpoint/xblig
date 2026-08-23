using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BunnyOfWar.AI;

public static class AIFlappy
{
	public static void doSomething(FighterObject cpu, List<FighterObject> humans)
	{
		switch (cpu.PROPERTIES.AImode)
		{
		case AI.modes.BaitChaser:
			if (cpu.getPersonalSpace().Contains((int)cpu.PROPERTIES.CpuMoveDestination.X, (int)cpu.PROPERTIES.CpuMoveDestination.Y))
			{
				FighterAttackCoordinator.quickAttack(cpu);
				cpu.Y = (int)cpu.PROPERTIES.CpuMoveDestination.Y;
				cpu.PROPERTIES.CpuMoveDestination = Vector2.One;
			}
			else if (cpu.PROPERTIES.CpuMoveDestination != Vector2.One && RandomStaticGlobals.makePositive(cpu.PROPERTIES.CpuMoveDestination.X - (float)cpu.X) <= 150f)
			{
				cpu.PROPERTIES.CpuMoveDestination = RandomStaticGlobals.GetCPUBaitVector2();
				cpu.Y = (int)cpu.PROPERTIES.CpuMoveDestination.Y;
			}
			else
			{
				cpu.PROPERTIES.CpuMoveDestination.X = RandomStaticGlobals.GetCPUBaitVector2().X;
				cpu.PROPERTIES.CpuMoveDestination.Y = cpu.Y;
			}
			break;
		case AI.modes.RoadKill:
		{
			for (int i = 0; i < humans.Count; i++)
			{
				if (cpu.getPersonalSpace().Intersects(humans[i].getPersonalSpace()))
				{
					cpu.healthChange(-1000f);
				}
			}
			break;
		}
		case AI.modes.PoliceCar:
			cpu.X = humans[0].X - cpu.PROPERTIES.AIAmountDistance;
			if (cpu.PROPERTIES.CpuAttackCooldown < DateTime.Now)
			{
				FighterAttackCoordinator.ShootBullet(cpu);
				cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddSeconds(3.0);
			}
			if (cpu.PROPERTIES.velocity == Vector2.Zero)
			{
				cpu.PROPERTIES.velocity = new Vector2(0f, 10f);
			}
			if (cpu.Y > 800)
			{
				cpu.PROPERTIES.velocity = new Vector2(0f, -5f);
			}
			if (cpu.Y < 300)
			{
				cpu.PROPERTIES.velocity = new Vector2(0f, 5f);
			}
			cpu.Y += (int)cpu.PROPERTIES.velocity.Y;
			break;
		case AI.modes.PoliceCarFPS:
			if (cpu.PROPERTIES.scale < 1f)
			{
				cpu.PROPERTIES.scale += 0.2f / (float)Definitions.UpdatesPerSecond;
				cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddSeconds(10.0);
				break;
			}
			if (cpu.PROPERTIES.scale > 1f)
			{
				cpu.PROPERTIES.scale = 1f;
			}
			if (cpu.PROPERTIES.CpuAttackCooldown < DateTime.Now)
			{
				humans[0].healthChange(-10f);
				cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddSeconds(10.0);
			}
			if (cpu.PROPERTIES.velocity == Vector2.Zero)
			{
				cpu.PROPERTIES.velocity = new Vector2(0f, 10f);
			}
			if (cpu.Y > 800)
			{
				cpu.PROPERTIES.velocity = new Vector2(0f, -5f);
			}
			if (cpu.Y < 300)
			{
				cpu.PROPERTIES.velocity = new Vector2(0f, 5f);
			}
			cpu.Y += (int)cpu.PROPERTIES.velocity.Y;
			break;
		case AI.modes.PoliceHelicopterFPS:
			if (cpu.PROPERTIES.scale < 0f)
			{
				cpu.PROPERTIES.scale += 3 / Definitions.UpdatesPerSecond;
			}
			if (cpu.PROPERTIES.scale > 0f)
			{
				cpu.PROPERTIES.scale = 1f;
			}
			if (cpu.PROPERTIES.CpuAttackCooldown < DateTime.Now)
			{
				FighterAttackCoordinator.ShootBullet(cpu);
				cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddSeconds(3.0);
			}
			if (cpu.PROPERTIES.velocity == Vector2.Zero)
			{
				cpu.PROPERTIES.velocity = new Vector2(30f, 0f);
			}
			if (cpu.X > 800)
			{
				cpu.PROPERTIES.velocity = new Vector2(-10f, 0f);
			}
			if (cpu.X < 300)
			{
				cpu.PROPERTIES.velocity = new Vector2(10f, 0f);
			}
			cpu.X += (int)cpu.PROPERTIES.velocity.X;
			break;
		}
	}
}
