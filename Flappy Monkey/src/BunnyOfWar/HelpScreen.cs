using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public static class HelpScreen
{
	private const byte TransitionAlpha = byte.MaxValue;

	private const string usageText = "A button = Okay\nB button = Cancel";

	private static SpriteFont smallFont = GraphicsManager.font;

	private static event EventHandler<EventArgs> Accepted;

	private static event EventHandler<EventArgs> Cancelled;

	public static void ProcessInput()
	{
		InputFromAnywhere inputFromAnywhere = null;
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

	public static void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput != null && anywhereInput.B_pressed)
		{
			ExitScreen();
		}
	}

	public static void ExitScreen()
	{
		ScreenManager.HideHighScoress();
		RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
	}

	public static void Draw()
	{
		string s = "\r\nX - Quick Attack\r\nY - Slow Attack\r\nA - Jump\r\nB - Pickup and again to throw (enemies and obstacles)\r\n\r\nLeft Trigger - Block (counter when done RIGHT before an attack)\r\nRight Trigger - Shoot (and throw)\r\nRight Joystick - Aim\r\n\r\nCombos:\r\n<-, ->, <-, -> - whirlwind AoE\r\nwhirlwind, Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight, Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight ), \r\n\r\nLeft Trigger + random B,A,X, or Y - counter\r\nLeft Joystick down, right, Y (or down, left, Y) - Hadouken\r\n<-, <- - push left\r\n->, -> - push right\r\n\r\nA, X - When Swingers Can Fly\r\nA, Y - It's a Chopper Baby\r\n\r\n\r\nGreatBallsOfFire, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickRight, Buttons.X, Buttons.X, Buttons.Y) {IsSubMove=false},\r\nGreatBallsOfFire, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.X, Buttons.X, Buttons.Y) {IsSubMove=false},\r\n\r\nHammerOfDoom, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickRight, Buttons.Y) {IsSubMove=false},\r\nHammerOfDoom, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickLeft, Buttons.Y) {IsSubMove=false},\r\n\r\n\r\n\r\nNot Active Yet:\r\n\r\nairborneSwingerSUPER, Buttons.A, Buttons.B, Buttons.X),\r\nairborneChopperSUPER, Buttons.A, Buttons.B, Buttons.Y),\r\n\r\nexplode, Buttons.A, Buttons.RightShoulder, Buttons.A, Buttons.B),\r\ndecapitate, Buttons.X, Buttons.X, Buttons.Y, Buttons.Y) { IsSubMove = false },\r\nhandChop, Buttons.X, Buttons.B) { IsSubMove = true },\r\nhardHit, Buttons.X,Buttons.B, Buttons.A),\r\nimpale, Buttons.X, Buttons.X, Buttons.X),\r\nstun, Buttons.X,Buttons.X,Buttons.Y, Buttons.B),\r\nhamstring, Buttons.Y, Buttons.Y, Buttons.Y),\r\n\r\n\r\n\r\n\r\nsweeper, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickRight, Buttons.X),\r\nsweeper, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.X),\r\n\r\n\r\nMAGICExplodingPushUp, Buttons.B, Buttons.B, Buttons.Y, Buttons.A, Buttons.X, Buttons.RightThumbstickUp),\r\nMAGICExplodingPushDown, Buttons.B, Buttons.B, Buttons.Y, Buttons.A, Buttons.X, Buttons.LeftShoulder, Buttons.RightThumbstickDown),\r\nMAGICExplodingPushLeft, Buttons.B, Buttons.B, Buttons.Y, Buttons.A, Buttons.X, Buttons.LeftShoulder, Buttons.RightThumbstickLeft),\r\nMAGICExplodingPushRight, Buttons.B, Buttons.B, Buttons.Y, Buttons.A, Buttons.X, Buttons.RightThumbstickRight)\r\n\r\n\r\n";
		Vector2 screenFullSize = GraphicsManager.ScreenFullSize;
		Vector2 titleSafeTopLeft = GraphicsManager.TitleSafeTopLeft;
		Vector2 vector = smallFont.MeasureString("A button = Okay\nB button = Cancel");
		Vector2 vector2 = (screenFullSize - vector) / 2f;
		vector2.Y = titleSafeTopLeft.Y + (float)GraphicsManager.font.LineSpacing * 1.1f;
		Color c = new Color(255, 255, 255, 255);
		Rectangle viewportRect = GraphicsManager.viewportRect;
		viewportRect.X -= (int)(0.1f * (float)viewportRect.Width);
		viewportRect.Y -= (int)(0.1f * (float)viewportRect.Height);
		viewportRect.Width += (int)(0.2f * (float)viewportRect.Width);
		viewportRect.Height += (int)(0.2f * (float)viewportRect.Height);
		Rectangle rectangle = new Rectangle(viewportRect.X - 1, viewportRect.Y - 1, viewportRect.Width + 2, viewportRect.Height + 2);
		GraphicsManager.DrawRectangle(rectangle, new Color(128, 128, 128, 192));
		GraphicsManager.DrawRectangle(viewportRect, new Color(0, 0, 0, 232));
		GraphicsManager.DrawString((int)titleSafeTopLeft.X, (int)titleSafeTopLeft.Y, s, c, GraphicsManager.font);
		List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
		titleSafeTopLeft.X += 250f;
		foreach (FighterObject item in humanPlayers)
		{
			string s2 = $"{item.PROPERTIES.GamerTag}\r\n{item.PROPERTIES.HumanProfile.damageDealt}\r\n{item.PROPERTIES.HumanProfile.damageTaken}\r\n{item.PROPERTIES.HumanProfile.kills}\r\n\r\n{item.PROPERTIES.HumanProfile.blocks}\r\n{item.PROPERTIES.HumanProfile.counters}\r\n{item.PROPERTIES.HumanProfile.parries}\r\n\r\n{item.PROPERTIES.HumanProfile.deaths}\r\n{item.PROPERTIES.HumanProfile.revivalsOfTeammate}\r\n\r\n{item.PROPERTIES.HumanProfile.shotsBlocked}\r\n{item.PROPERTIES.HumanProfile.shotsFired}\r\n{item.PROPERTIES.HumanProfile.shotsMade}\r\n\r\n{item.PROPERTIES.HumanProfile.timeSpentPlaying / (double)Definitions.UpdatesPerSecond}\r\n{item.PROPERTIES.HumanProfile.timeSpentBlocking / (double)Definitions.UpdatesPerSecond}\r\n";
			titleSafeTopLeft.X += 150f;
			GraphicsManager.DrawString((int)titleSafeTopLeft.X, (int)titleSafeTopLeft.Y, s2, c, GraphicsManager.font);
		}
	}
}
