using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public static class HighScoreScreen
{
	private const byte TransitionAlpha = byte.MaxValue;

	private const string usageText = "A button = Okay\nB button = Cancel";

	public static DateTime windowsStartedAt = DateTime.MinValue;

	private static Rectangle backButtonRect = Definitions.rectBackButton;

	private static SpriteFont smallFont = GraphicsManager.font;

	private static event EventHandler<EventArgs> Accepted;

	private static event EventHandler<EventArgs> Cancelled;

	public static void ProcessInput()
	{
		if (windowsStartedAt == DateTime.MinValue)
		{
			windowsStartedAt = DateTime.Now;
		}
		if (GamePad.GetState(PlayerIndex.One).IsConnected)
		{
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(PlayerIndex.One, ref InputManager.gamePad1previous, ref InputManager.nullKeyboard);
			FigureOutInput(playerInput, PlayerIndex.One);
		}
		if (GamePad.GetState(PlayerIndex.Two).IsConnected)
		{
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(PlayerIndex.Two, ref InputManager.gamePad2previous, ref InputManager.nullKeyboard);
			FigureOutInput(playerInput, PlayerIndex.Two);
		}
		if (GamePad.GetState(PlayerIndex.Three).IsConnected)
		{
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(PlayerIndex.Three, ref InputManager.gamePad3previous, ref InputManager.nullKeyboard);
			FigureOutInput(playerInput, PlayerIndex.Three);
		}
		if (GamePad.GetState(PlayerIndex.Four).IsConnected)
		{
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(PlayerIndex.Four, ref InputManager.gamePad4previous, ref InputManager.nullKeyboard);
			FigureOutInput(playerInput, PlayerIndex.Four);
		}
	}

	public static void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput == null)
		{
			return;
		}
		if (anywhereInput.B_pressed)
		{
			ScreenManager.ShowMainMenu();
			ExitScreen();
			return;
		}
		if (anywhereInput.A_pressed)
		{
			LevelManager.LoadLevel("EasyFlappy.lvl", isPvP: false);
		}
		if (ScreenManager.isShowingBuyMeScreen && anywhereInput.X_pressed)
		{
			RandomStaticGlobals.BuyMe(pi);
			ExitScreen();
		}
	}

	public static void ExitScreen()
	{
		SoundManager.PlayMenuClick();
		ScreenManager.HideHighScoress();
		windowsStartedAt = DateTime.MinValue;
		ScreenManager.isShowingBuyMeScreen = false;
	}

	public static void Draw()
	{
		if (ScreenManager.isShowingBuyMeScreen)
		{
			GraphicsManager.Draw(GraphicsManager.imgBuyMeScreen, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthTop);
			return;
		}
		string s = "\r\nDamage Dealt:\r\nDamage Taken:\r\nKills:\r\n\r\nBlocks:\r\nCounters:\r\nParries:\r\n\r\nDeaths:\r\n\r\nShots Blocked:\r\nShots Fired:\r\nShots Made:\r\n\r\nTime Spent Playing:\r\nTime Spent Blocking:\r\n\r\n";
		Vector2 screenFullSize = GraphicsManager.ScreenFullSize;
		Vector2 vector = new Vector2(150f, 100f);
		Vector2 vector2 = smallFont.MeasureString("A button = Okay\nB button = Cancel");
		Vector2 vector3 = (screenFullSize - vector2) / 2f;
		vector3.Y = vector.Y + (float)GraphicsManager.font.LineSpacing * 1.1f;
		Color c = new Color(255, 255, 255, 255);
		GraphicsManager.DrawRectangle(new Rectangle(0, 0, 1920, 1080), GraphicsManager.TheColorTransparentGray);
		GraphicsManager.DrawString((int)vector.X, (int)vector.Y, s, c, GraphicsManager.font);
		List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
		vector.X += 100f;
		foreach (FighterObject item in humanPlayers)
		{
			string s2 = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n\r\n{4}\r\n{5}\r\n{6}\r\n\r\n{7}\r\n\r\n{9}\r\n{10}\r\n{11}\r\n\r\n{12}\r\n{13}\r\n", item.PROPERTIES.GamerTag, item.PROPERTIES.HumanProfile.damageDealt, item.PROPERTIES.HumanProfile.damageTaken, item.PROPERTIES.HumanProfile.kills, item.PROPERTIES.HumanProfile.blocks, item.PROPERTIES.HumanProfile.counters, item.PROPERTIES.HumanProfile.parries, item.PROPERTIES.HumanProfile.deaths, item.PROPERTIES.HumanProfile.revivalsOfTeammate, item.PROPERTIES.HumanProfile.shotsBlocked, item.PROPERTIES.HumanProfile.shotsFired, item.PROPERTIES.HumanProfile.shotsMade, RandomStaticGlobals.GetTimeFromSeconds((int)item.PROPERTIES.HumanProfile.stopwatchTimeSpentPlaying.Elapsed.TotalSeconds), RandomStaticGlobals.GetTimeFromSeconds((int)item.PROPERTIES.HumanProfile.stopwatchTimeSpentBlocking.Elapsed.TotalSeconds));
			vector.X += 250f;
			GraphicsManager.DrawString((int)vector.X, (int)vector.Y, s2, c, GraphicsManager.font);
		}
		string text = "okay";
		Vector2 vector4 = GraphicsManager.font.MeasureString(text);
		Rectangle rectangle = new Rectangle(backButtonRect.X, backButtonRect.Y, (int)vector4.X, (int)vector4.Y);
		rectangle.Inflate(50, 10);
		GraphicsManager.DrawRectangle(rectangle, new Color(128, 128, 128, 192));
		GraphicsManager.DrawString(backButtonRect.X, backButtonRect.Y, text, Color.White, GraphicsManager.font);
		GraphicsManager.Draw(GraphicsManager.imgButtonB, new Rectangle(rectangle.X - 70, rectangle.Y - 30, 100, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthTop);
	}
}
