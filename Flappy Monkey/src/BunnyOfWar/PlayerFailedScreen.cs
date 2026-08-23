using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public static class PlayerFailedScreen
{
	private const byte TransitionAlpha = byte.MaxValue;

	public static DateTime windowsStartedAt = DateTime.MinValue;

	private static Texture2D background = null;

	private static SpriteFont smallFont = GraphicsManager.font;

	private static Rectangle backButtonRect = Definitions.rectBackButton;

	public static void Load(ContentManager Content)
	{
		if (background == null)
		{
			background = GraphicsManager.LoadTexture("screens/RedFade.png");
		}
	}

	public static void ProcessInput()
	{
		InputFromAnywhere inputFromAnywhere = null;
		if (windowsStartedAt == DateTime.MinValue)
		{
			windowsStartedAt = DateTime.Now;
		}
		if (!(windowsStartedAt.AddSeconds(1.0) > DateTime.Now))
		{
			if (GamePad.GetState(PlayerIndex.One).IsConnected)
			{
				inputFromAnywhere = InputManager.GetPlayerInput(PlayerIndex.One, ref InputManager.gamePad1previous, ref InputManager.nullKeyboard);
				FigureOutInput(inputFromAnywhere, PlayerIndex.One);
			}
			if (GamePad.GetState(PlayerIndex.Two).IsConnected)
			{
				inputFromAnywhere = InputManager.GetPlayerInput(PlayerIndex.Two, ref InputManager.gamePad2previous, ref InputManager.nullKeyboard);
				FigureOutInput(inputFromAnywhere, PlayerIndex.Two);
			}
			if (GamePad.GetState(PlayerIndex.Three).IsConnected)
			{
				inputFromAnywhere = InputManager.GetPlayerInput(PlayerIndex.Three, ref InputManager.gamePad3previous, ref InputManager.nullKeyboard);
				FigureOutInput(inputFromAnywhere, PlayerIndex.Three);
			}
			if (GamePad.GetState(PlayerIndex.Four).IsConnected)
			{
				inputFromAnywhere = InputManager.GetPlayerInput(PlayerIndex.Four, ref InputManager.gamePad4previous, ref InputManager.nullKeyboard);
				FigureOutInput(inputFromAnywhere, PlayerIndex.Four);
			}
		}
	}

	public static void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput != null && anywhereInput.B_pressed)
		{
			ExitScreen();
		}
	}

	public static void ExitScreen()
	{
		SoundManager.PlayMenuClick();
		ScreenManager.HidePlayerFailedScreen();
		windowsStartedAt = DateTime.MinValue;
	}

	public static void Draw()
	{
		if (RandomStaticGlobals.isPvPEnabled)
		{
			try
			{
				List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: false);
				GraphicsManager.DrawStringCentered(950, 500, humanPlayers[0].PROPERTIES.GamerTag + " WINS!!", Color.DarkRed, GraphicsManager.fontBig);
			}
			catch (Exception)
			{
			}
		}
		else if (!Definitions.Options.MercyOnOff)
		{
			GraphicsManager.DrawString(800, 530, "You Fail", Color.DarkRed, GraphicsManager.fontBig);
		}
		GraphicsManager.Draw(background, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthSecondHighest);
		string text = "continue";
		Vector2 vector = GraphicsManager.font.MeasureString(text);
		Rectangle rectangle = new Rectangle(backButtonRect.X, backButtonRect.Y, (int)vector.X, (int)vector.Y);
		rectangle.Inflate(50, 10);
		GraphicsManager.DrawRectangle(rectangle, new Color(128, 128, 128, 192));
		GraphicsManager.DrawString(backButtonRect.X, backButtonRect.Y, text, Color.White, GraphicsManager.font);
		GraphicsManager.Draw(GraphicsManager.imgButtonB, new Rectangle(rectangle.X - 70, rectangle.Y - 30, 100, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthTop);
	}
}
