using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public static class ProjectileManager
{
	public enum ProjectileType
	{
		none,
		arrow,
		grenade,
		rpg,
		rock,
		tongue,
		skull,
		bullet,
		bomb,
		laser,
		poo,
		pooSmall
	}

	public static Dictionary<string, Texture2D> htImages = new Dictionary<string, Texture2D>();

	public static ContentManager Content;

	public static List<ProjectileObject> projectiles = new List<ProjectileObject>();

	public static void DrawProjectiles()
	{
		for (int i = 0; i < projectiles.Count; i++)
		{
			if (projectiles[i].isVisible && projectiles[i].rect.Intersects(GraphicsManager.viewableArea))
			{
				if (projectiles[i].type == ProjectileType.arrow)
				{
					GraphicsManager.Draw(getImage("projectiles/" + projectiles[i].type), GraphicsManager.getAdjustedVector2(projectiles[i].rect), null, Color.White, projectiles[i].Rotation, new Vector2(projectiles[i].width / 2, projectiles[i].height / 2), 1f, SpriteEffects.None, projectiles[i].getLayerDepth());
				}
				else if (projectiles[i].type == ProjectileType.tongue)
				{
					GraphicsManager.Draw(getImage("projectiles/" + projectiles[i].type), GraphicsManager.getAdjustedRectangle(projectiles[i].getRectStretched()), null, Color.White, projectiles[i].Rotation, new Vector2(projectiles[i].width, projectiles[i].height), SpriteEffects.None, projectiles[i].getLayerDepth());
				}
				else if (projectiles[i].type == ProjectileType.rock)
				{
					GraphicsManager.Draw(getImage("projectiles/" + projectiles[i].type), GraphicsManager.getAdjustedVector2(projectiles[i].rect), null, Color.White, projectiles[i].Rotation, new Vector2(projectiles[i].width / 2, projectiles[i].height / 2), 1f, SpriteEffects.None, projectiles[i].getLayerDepth());
				}
				else if (projectiles[i].type == ProjectileType.skull)
				{
					GraphicsManager.Draw(getImage("projectiles/" + projectiles[i].type), GraphicsManager.getAdjustedVector2(projectiles[i].rect), null, Color.White, projectiles[i].Rotation, new Vector2(projectiles[i].width, projectiles[i].height), 1f, SpriteEffects.None, projectiles[i].getLayerDepth());
				}
				else if (projectiles[i].type == ProjectileType.laser)
				{
					GraphicsManager.Draw(getImage("projectiles/" + projectiles[i].type), GraphicsManager.getAdjustedRectangle(projectiles[i].rect), null, Color.White, projectiles[i].Rotation, new Vector2(projectiles[i].width / 2, projectiles[i].height / 2), SpriteEffects.None, projectiles[i].getLayerDepth());
				}
				else
				{
					GraphicsManager.Draw(getImage("projectiles/" + projectiles[i].type), GraphicsManager.getAdjustedVector2(projectiles[i].rect), null, Color.White, projectiles[i].Rotation, new Vector2(projectiles[i].width / 2, projectiles[i].height / 2), 1f, SpriteEffects.None, projectiles[i].getLayerDepth());
				}
			}
		}
	}

	public static void ProcessProjectiles()
	{
		try
		{
			if (RandomStaticGlobals.isSkullSlingshotMode && projectiles.Count == 0)
			{
				GraphicsManager.viewableArea.X = 0;
			}
			for (int i = 0; i < projectiles.Count; i++)
			{
				if (projectiles[i].isActive)
				{
					if (RandomStaticGlobals.isSkullSlingshotMode && !projectiles[i].itDamagesHumans && GraphicsManager.viewableArea.X < projectiles[i].X && projectiles[i].shooter.PROPERTIES.isLocal)
					{
						GraphicsManager.viewableArea.X = projectiles[i].X - projectiles[i].width * 2;
					}
					int num = 0;
					if (projectiles[i].itDamagesComputers)
					{
						num = FighterManager.doProjectileDamage(projectiles[i].rect, projectiles[i].shooter, FighterManager.getComputerPlayers(onlyLiving: true, canBeDying: false), justOneVictim: true, projectiles[i]);
						if (num > 0)
						{
							projectiles[i].shooter.PROPERTIES.CountAttack(projectiles[i].currentAttack, num);
							projectiles[i].isActive = false;
						}
					}
					if (num == 0 && i < projectiles.Count && projectiles[i].itDamagesHumans)
					{
						num = FighterManager.doProjectileDamage(projectiles[i].rect, projectiles[i].shooter, FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: true), justOneVictim: true, projectiles[i]);
						if (num > 0 || !projectiles[i].rect.Intersects(GraphicsManager.viewableArea))
						{
							projectiles[i].shooter.PROPERTIES.CountAttack(projectiles[i].currentAttack, num);
							projectiles.Remove(projectiles[i]);
						}
					}
					if (num == 0 && i < projectiles.Count)
					{
						num = ObstacleManager.doEnvironmentalDestructionCollision(projectiles[i].rect, projectiles[i].damage);
						if (num > 0 || !projectiles[i].rect.Intersects(GraphicsManager.viewableArea))
						{
							projectiles[i].shooter.PROPERTIES.CountAttack(projectiles[i].currentAttack, num);
							projectiles.Remove(projectiles[i]);
						}
					}
				}
				else
				{
					projectiles.Remove(projectiles[i]);
				}
			}
			for (int i = 0; i < projectiles.Count; i++)
			{
				if (projectiles[i].isActive)
				{
					projectiles[i].move();
				}
			}
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static void Clear()
	{
	}

	public static void addNewProjectile(int x, int y, Vector2 direction, float speed, bool areWeHuman, FighterObject shooter, ProjectileType whatsIsIt, int width, int height, int damage, bool broadcast)
	{
		ProjectileObject projectileObject = new ProjectileObject();
		projectileObject.type = whatsIsIt;
		projectileObject.X = x + (int)(shooter.RANGED.rangedOrigin.X * shooter.PROPERTIES.scale);
		projectileObject.Y = y + (int)(shooter.RANGED.rangedOrigin.Y * shooter.PROPERTIES.scale);
		projectileObject.origin = Vector2.Zero;
		projectileObject.currentAttack = shooter.PROPERTIES.currentAttack;
		projectileObject.direction = direction;
		projectileObject.speedFallingInPixelsPerSecond = 500;
		projectileObject.speedInPixelsPerSecond = (int)(1000f * speed);
		projectileObject.damage = damage;
		projectileObject.Rotation = (float)Math.Atan2(direction.Y, direction.X);
		projectileObject.itDamagesHumans = !areWeHuman;
		projectileObject.itDamagesComputers = areWeHuman;
		projectileObject.shooter = shooter;
		projectileObject.width = width;
		projectileObject.height = height;
		projectiles.Add(projectileObject);
		shooter.PROPERTIES.HumanProfile.shotsFired++;
		if (broadcast)
		{
			FighterManager.BroadcastAddProjectile(shooter.ID, x, y, direction, speed, areWeHuman, (int)whatsIsIt, width, height, damage);
		}
	}

	public static void addNewArrow(int x, int y, Vector2 direction, float speed, bool isHumanArrow, FighterObject shooter)
	{
		ProjectileObject projectileObject = new ProjectileObject();
		projectileObject.type = ProjectileType.arrow;
		projectileObject.X = x + (int)shooter.RANGED.rangedOrigin.X;
		projectileObject.Y = y + (int)shooter.RANGED.rangedOrigin.Y;
		projectileObject.origin = Vector2.Zero;
		projectileObject.currentAttack = shooter.PROPERTIES.currentAttack;
		projectileObject.direction = direction;
		projectileObject.speedFallingInPixelsPerSecond = 500;
		projectileObject.speedInPixelsPerSecond = (int)(1000f * speed);
		projectileObject.damage = 10;
		projectileObject.Rotation = (float)Math.Atan2(direction.Y, direction.X);
		projectileObject.itDamagesHumans = !isHumanArrow;
		projectileObject.itDamagesComputers = isHumanArrow;
		projectileObject.shooter = shooter;
		projectileObject.width = 100;
		projectileObject.height = 15;
		projectiles.Add(projectileObject);
		shooter.PROPERTIES.HumanProfile.shotsFired++;
	}

	public static void addNewRock(int x, int y, Vector2 direction, float speed, bool isHumanArrow, FighterObject shooter)
	{
		ProjectileObject projectileObject = new ProjectileObject();
		projectileObject.type = ProjectileType.rock;
		projectileObject.X = x;
		projectileObject.Y = y;
		projectileObject.origin = new Vector2(x, y);
		projectileObject.currentAttack = shooter.PROPERTIES.currentAttack;
		projectileObject.direction = direction;
		projectileObject.speedFallingInPixelsPerSecond = 2000;
		projectileObject.speedInPixelsPerSecond = (int)(1000f * speed);
		projectileObject.damage = 40;
		projectileObject.Rotation = (float)Math.Atan2(direction.Y, direction.X);
		projectileObject.itDamagesHumans = !isHumanArrow;
		projectileObject.itDamagesComputers = isHumanArrow;
		projectileObject.shooter = shooter;
		projectileObject.width = 300;
		projectileObject.height = 261;
		projectiles.Add(projectileObject);
		shooter.PROPERTIES.HumanProfile.shotsFired++;
	}

	public static void addNewTongue(int x, int y, Vector2 direction, float speed, bool isHumanArrow, FighterObject shooter)
	{
		ProjectileObject projectileObject = new ProjectileObject();
		projectileObject.type = ProjectileType.tongue;
		projectileObject.X = x;
		projectileObject.Y = y;
		projectileObject.origin = new Vector2(x, y);
		projectileObject.direction = direction;
		projectileObject.speedFallingInPixelsPerSecond = 1;
		projectileObject.speedInPixelsPerSecond = (int)(1000f * speed);
		projectileObject.damage = 10;
		projectileObject.Rotation = (float)Math.Atan2((double)direction.Y * -1.0, (double)direction.X * -1.0);
		projectileObject.itDamagesHumans = !isHumanArrow;
		projectileObject.itDamagesComputers = isHumanArrow;
		projectileObject.isCounterable = true;
		projectileObject.shooter = shooter;
		projectileObject.width = 100;
		projectileObject.height = 15;
		projectileObject.maxRange = 500;
		projectileObject.maxRangeLoiterDurationMS = 500;
		projectiles.Add(projectileObject);
		shooter.PROPERTIES.HumanProfile.shotsFired++;
	}

	public static Texture2D getImage(string path)
	{
		if (htImages == null)
		{
			htImages = new Dictionary<string, Texture2D>();
		}
		if (!htImages.ContainsKey(path))
		{
			if (LevelManager.Content == null)
			{
				return null;
			}
			htImages[path] = GraphicsManager.LoadTexture(path, cacheResult: false);
		}
		return htImages[path];
	}

	public static void loadImage(string path)
	{
		if (htImages == null)
		{
			htImages = new Dictionary<string, Texture2D>();
		}
		htImages[path] = GraphicsManager.LoadTexture(path);
	}
}
