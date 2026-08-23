using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public static class LootsManager
{
	public static List<LootsObject> Loots = new List<LootsObject>(0);

	public static void Draw()
	{
		if (Loots == null || Loots.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < Loots.Count; i++)
		{
			if (Loots[i].isActive)
			{
				if (DateTime.Now.AddMilliseconds(-500.0) < Loots[i].expires)
				{
					GraphicsManager.Draw(GraphicsManager.GetTextureFromCache(Loots[i].imagePath), new Rectangle(Loots[i].X, Loots[i].Y, Loots[i].width, Loots[i].height), null, GraphicsManager.TheColorTransparentRed, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				}
				else
				{
					GraphicsManager.Draw(GraphicsManager.GetTextureFromCache(Loots[i].imagePath), new Rectangle(Loots[i].X, Loots[i].Y, 100, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				}
			}
		}
	}

	public static void ProcessLoots()
	{
		if (Loots.Count == 0 || Loots == null || Loots.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < Loots.Count; i++)
		{
			if (Loots[i].expires < DateTime.Now)
			{
				Loots.RemoveAt(i);
			}
		}
	}

	public static void AddHealth()
	{
		LootsObject lootsObject = new LootsObject();
		Random random = new Random(DateTime.Now.Millisecond);
		lootsObject.width = 150;
		lootsObject.height = 150;
		lootsObject.X = random.Next(800 - lootsObject.width);
		lootsObject.Y = random.Next(480 - lootsObject.height);
		lootsObject.imagePath = "buttons/button_x";
		lootsObject.hp = 1;
		lootsObject.moneys = 0;
		lootsObject.isActive = true;
		lootsObject.expires = DateTime.Now.AddMilliseconds(2000.0);
		Loots.Add(lootsObject);
	}

	public static void HandleInput(int x, int y, FighterObject toucher)
	{
		if (Loots == null || Loots.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < Loots.Count; i++)
		{
			if (!new Rectangle(Loots[i].X, Loots[i].Y, 100, 100).Contains(x, y))
			{
				continue;
			}
			if (Loots[i].hp != 0)
			{
				toucher.healthChange(Loots[i].hp);
				if (!Definitions.Options.VibrationsOnOff)
				{
				}
			}
			Loots.RemoveAt(i);
		}
	}
}
