using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public static class QuickTimeEventsManager
{
	public enum QTEButtons
	{
		A,
		B,
		X,
		Y,
		ABXY,
		AB,
		XY,
		up,
		down,
		left,
		right,
		updown,
		leftright,
		nulll
	}

	public static QuickTimeEventsObject QTE = new QuickTimeEventsObject();

	public static bool hasQTE = false;

	public static void AddQTE(string QTEName, string sceneName, int durationInMS, QTEButtons buttons)
	{
		if (hasQTE)
		{
			FAIL(FighterManager.humanPlayers[0]);
		}
		hasQTE = true;
		QTE = new QuickTimeEventsObject(buttons, QTEName, durationInMS);
		GraphicsManager.ShowQuickTimeEvent(sceneName, durationInMS);
	}

	public static void ProcessQTE(FighterObject fo)
	{
		if (hasQTE && QTE.expires < DateTime.Now)
		{
			FAIL(fo);
		}
	}

	public static int ButtonWasPressed(Buttons b, FighterObject fo)
	{
		if (QTE.theHardestButtonToButton.Value == b)
		{
			SUCCESS();
			return 0;
		}
		FAIL(fo);
		return 1;
	}

	public static void SUCCESS()
	{
		hasQTE = false;
		TriggerManager.SetTriggerEvent(QTE.name + "SUCCESS");
	}

	public static void FAIL(FighterObject player)
	{
		hasQTE = false;
		TriggerManager.SetTriggerEvent(QTE.name + "FAILED");
		TriggerManager.SetTriggerEvent("QTEFAIL");
		if (Definitions.Options.Difficulty <= 1)
		{
			player.healthChange(0f - player.PROPERTIES.healthMax / 5f);
		}
		if (Definitions.Options.Difficulty == 2)
		{
			player.healthChange(0f - player.PROPERTIES.healthMax / 2f);
		}
		if (Definitions.Options.Difficulty == 3)
		{
			player.healthChange(0f - player.PROPERTIES.healthMax * 2f);
		}
	}

	public static void Draw()
	{
		if (!hasQTE)
		{
			return;
		}
		Rectangle destinationRectangle = new Rectangle(GraphicsManager.ScreenWidth / 2, GraphicsManager.ScreenHeight / 2, 100, 100);
		int num = 200;
		bool flag = true;
		for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
		{
			if (FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState.HasValue)
			{
				flag = false;
			}
		}
		switch (QTE.theHardestButtonToButton)
		{
		case Buttons.A:
			if (flag)
			{
				destinationRectangle.X -= num * 2;
				GraphicsManager.Draw(GraphicsManager.imgButtonKBA, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			else
			{
				destinationRectangle.Y += num;
				GraphicsManager.Draw(GraphicsManager.imgButtonA, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			break;
		case Buttons.B:
			if (flag)
			{
				destinationRectangle.X += num * 2;
				GraphicsManager.Draw(GraphicsManager.imgButtonKBD, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			else
			{
				destinationRectangle.X += num;
				GraphicsManager.Draw(GraphicsManager.imgButtonB, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			break;
		case Buttons.X:
			if (flag)
			{
				destinationRectangle.Y += (int)((double)num * 1.25);
				GraphicsManager.Draw(GraphicsManager.imgButtonKBS, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			else
			{
				destinationRectangle.X -= num;
				GraphicsManager.Draw(GraphicsManager.imgButtonX, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			break;
		case Buttons.Y:
			if (flag)
			{
				destinationRectangle.Y -= num * 2;
				GraphicsManager.Draw(GraphicsManager.imgButtonKBW, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			else
			{
				destinationRectangle.Y -= num;
				GraphicsManager.Draw(GraphicsManager.imgButtonY, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			break;
		case Buttons.RightThumbstickLeft:
			destinationRectangle.X -= num;
			GraphicsManager.Draw(GraphicsManager.imgButtonRSLeft, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			break;
		case Buttons.RightThumbstickRight:
			destinationRectangle.X += num;
			GraphicsManager.Draw(GraphicsManager.imgButtonRSRight, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			break;
		case Buttons.RightThumbstickUp:
			destinationRectangle.Y -= num;
			GraphicsManager.Draw(GraphicsManager.imgButtonRSUp, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			break;
		case Buttons.RightThumbstickDown:
			destinationRectangle.Y += num;
			GraphicsManager.Draw(GraphicsManager.imgButtonRSDown, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			break;
		}
		int num2 = (int)QTE.expires.Subtract(DateTime.Now).TotalMilliseconds;
		int x = (GraphicsManager.ScreenWidth - num2) / 2;
		GraphicsManager.DrawRectangle(new Rectangle(x, 980, num2, 100), Color.Red, Definitions.LayerDepthTop);
	}
}
