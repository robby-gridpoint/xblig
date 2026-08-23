using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public static class InputManager
{
	public static KeyboardState? previousKeyboardStateMenu = null;

	public static GamePadState? player1 = null;

	public static GamePadState? player2 = null;

	public static GamePadState? player3 = null;

	public static GamePadState? player4 = null;

	public static GamePadState? gamePad1previous = null;

	public static GamePadState? gamePad2previous = null;

	public static GamePadState? gamePad3previous = null;

	public static GamePadState? gamePad4previous = null;

	public static DateTime button1LastPressed = DateTime.MinValue;

	public static DateTime button2LastPressed = DateTime.MinValue;

	public static DateTime button3LastPressed = DateTime.MinValue;

	public static GamePadState? nullGamePad = null;

	public static KeyboardState? nullKeyboard = null;

	public static void ClearPreviousInputs()
	{
		previousKeyboardStateMenu = null;
		player1 = null;
		player2 = null;
		player3 = null;
		player4 = null;
		gamePad1previous = null;
		gamePad2previous = null;
		gamePad3previous = null;
		gamePad4previous = null;
		for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
		{
			FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState = null;
			FighterManager.humanPlayers[i].PROPERTIES.previousKeyboardState = null;
		}
	}

	public static InputFromAnywhere GetPlayerInput(FighterObject player)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			return null;
		}
		return GetPlayerInput(player.PROPERTIES.PlayerIndexControllerNumber, ref player.PROPERTIES.previousGamePadState, ref player.PROPERTIES.previousKeyboardState);
	}

	public static InputFromAnywhere GetPlayerInput(PlayerIndex? xboxControllerNumber, ref GamePadState? previousXboxGamePadState, ref KeyboardState? previousKeyboardState)
	{
		InputFromAnywhere inputFromAnywhere = new InputFromAnywhere();
		GamePadState? gamePadState = null;
		GamePadState? gamePadState2 = null;
		if (xboxControllerNumber.HasValue)
		{
			gamePadState = GamePad.GetState(xboxControllerNumber.Value, GamePadDeadZone.Circular);
			if (gamePadState.HasValue && !gamePadState.Value.IsConnected && !RandomStaticGlobals.isGamePaused)
			{
				RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
				GraphicsManager.Message("Hey your controller was just unplugged!! Do you need new batteries?");
			}
			if (Guide.IsVisible && !RandomStaticGlobals.isGamePaused && ScreenManager.CurrentScreen == ScreenManager.screens.Blank)
			{
				RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
			}
			gamePadState2 = ((!previousXboxGamePadState.HasValue || !previousXboxGamePadState.HasValue) ? gamePadState : previousXboxGamePadState);
			previousXboxGamePadState = gamePadState.Value;
			if (gamePadState.HasValue && gamePadState.Value.Buttons.Start == ButtonState.Released && gamePadState2.Value.Buttons.Start == ButtonState.Pressed)
			{
				inputFromAnywhere.START_pressed = true;
			}
			if (gamePadState.HasValue && gamePadState.Value.Buttons.Start == ButtonState.Pressed)
			{
				inputFromAnywhere.START_held = true;
			}
			if (gamePadState.Value.Buttons.Back == ButtonState.Pressed)
			{
				inputFromAnywhere.SELECT_held = true;
				if (gamePadState.Value.Buttons.Back == ButtonState.Released)
				{
					inputFromAnywhere.SELECT_pressed = true;
				}
			}
			if (gamePadState.Value.DPad.Down == ButtonState.Pressed || gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickDown))
			{
				inputFromAnywhere.DOWN_held = true;
				if ((gamePadState.Value.DPad.Down == ButtonState.Pressed && gamePadState2.Value.DPad.Down == ButtonState.Released) || ((double)gamePadState.Value.ThumbSticks.Left.Y < -0.2 && (double)gamePadState2.Value.ThumbSticks.Left.Y > -0.1))
				{
					inputFromAnywhere.DOWN_pressed = true;
				}
			}
			if (gamePadState.Value.DPad.Up == ButtonState.Pressed || gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickUp))
			{
				inputFromAnywhere.UP_held = true;
				if ((gamePadState.Value.DPad.Up == ButtonState.Pressed && gamePadState2.Value.DPad.Up == ButtonState.Released) || ((double)gamePadState.Value.ThumbSticks.Left.Y > 0.2 && (double)gamePadState2.Value.ThumbSticks.Left.Y < 0.1))
				{
					inputFromAnywhere.UP_pressed = true;
				}
			}
			if (gamePadState.Value.DPad.Left == ButtonState.Pressed || gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickLeft))
			{
				inputFromAnywhere.LEFT_held = true;
				if ((gamePadState.Value.DPad.Left == ButtonState.Pressed && gamePadState2.Value.DPad.Left == ButtonState.Released) || ((double)gamePadState.Value.ThumbSticks.Left.X < -0.2 && (double)gamePadState2.Value.ThumbSticks.Left.X > -0.1))
				{
					inputFromAnywhere.LEFT_pressed = true;
				}
			}
			if (gamePadState.Value.DPad.Right == ButtonState.Pressed || gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickRight))
			{
				inputFromAnywhere.RIGHT_held = true;
				if ((gamePadState.Value.DPad.Right == ButtonState.Pressed && gamePadState2.Value.DPad.Right == ButtonState.Released) || ((double)gamePadState.Value.ThumbSticks.Left.X > 0.2 && (double)gamePadState2.Value.ThumbSticks.Left.X < 0.1))
				{
					inputFromAnywhere.RIGHT_pressed = true;
				}
			}
			if (gamePadState.Value.Buttons.A == ButtonState.Pressed)
			{
				inputFromAnywhere.A_held = true;
				if (gamePadState2.Value.Buttons.A == ButtonState.Released)
				{
					inputFromAnywhere.A_pressed = true;
				}
			}
			if (gamePadState.Value.Buttons.B == ButtonState.Pressed)
			{
				inputFromAnywhere.B_held = true;
				if (gamePadState2.Value.Buttons.B == ButtonState.Released)
				{
					inputFromAnywhere.B_pressed = true;
				}
			}
			if (gamePadState.Value.Buttons.X == ButtonState.Pressed)
			{
				inputFromAnywhere.X_held = true;
				if (gamePadState2.Value.Buttons.X == ButtonState.Released)
				{
					inputFromAnywhere.X_pressed = true;
				}
			}
			if (gamePadState.Value.Buttons.Y == ButtonState.Pressed)
			{
				inputFromAnywhere.Y_held = true;
				if (gamePadState2.Value.Buttons.Y == ButtonState.Released)
				{
					inputFromAnywhere.Y_pressed = true;
				}
			}
			if (gamePadState.Value.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				inputFromAnywhere.LEFT_SHOULDER_held = true;
				if (gamePadState.Value.Buttons.LeftShoulder == ButtonState.Released)
				{
					inputFromAnywhere.LEFT_SHOULDER_pressed = true;
				}
			}
			if (gamePadState.Value.Buttons.LeftStick == ButtonState.Pressed)
			{
				inputFromAnywhere.LEFT_TRIGGER_held = true;
				if (gamePadState.Value.Buttons.LeftStick == ButtonState.Released)
				{
					inputFromAnywhere.LEFT_TRIGGER_pressed = true;
				}
			}
			if (gamePadState.Value.Buttons.RightShoulder == ButtonState.Pressed)
			{
				inputFromAnywhere.RIGHT_SHOULDER_held = true;
				if (gamePadState.Value.Buttons.RightShoulder == ButtonState.Released)
				{
					inputFromAnywhere.RIGHT_SHOULDER_pressed = true;
				}
			}
			if (gamePadState.Value.Buttons.RightStick == ButtonState.Pressed)
			{
				inputFromAnywhere.RIGHT_TRIGGER_held = true;
				if (gamePadState.Value.Buttons.RightStick == ButtonState.Released)
				{
					inputFromAnywhere.RIGHT_TRIGGER_pressed = true;
				}
			}
			if (((double)gamePadState.Value.ThumbSticks.Right.X > 0.2 && (double)gamePadState2.Value.ThumbSticks.Right.X < 0.1) || ((double)gamePadState.Value.ThumbSticks.Right.X < -0.2 && (double)gamePadState2.Value.ThumbSticks.Right.X > -0.1) || ((double)gamePadState.Value.ThumbSticks.Right.Y > 0.2 && (double)gamePadState2.Value.ThumbSticks.Right.Y < 0.1) || ((double)gamePadState.Value.ThumbSticks.Right.Y < -0.2 && (double)gamePadState2.Value.ThumbSticks.Right.Y > -0.1))
			{
				inputFromAnywhere.RIGHT_vector2 = gamePadState.Value.ThumbSticks.Right;
			}
			else
			{
				inputFromAnywhere.RIGHT_vector2 = Vector2.Zero;
			}
			if (((double)gamePadState.Value.ThumbSticks.Left.X > 0.2 && (double)gamePadState2.Value.ThumbSticks.Left.X < 0.1) || ((double)gamePadState.Value.ThumbSticks.Left.X < -0.2 && (double)gamePadState2.Value.ThumbSticks.Left.X > -0.1) || ((double)gamePadState.Value.ThumbSticks.Left.Y > 0.2 && (double)gamePadState2.Value.ThumbSticks.Left.Y < 0.1) || ((double)gamePadState.Value.ThumbSticks.Left.Y < -0.2 && (double)gamePadState2.Value.ThumbSticks.Left.Y > -0.1))
			{
				inputFromAnywhere.LEFT_vector2 = gamePadState.Value.ThumbSticks.Left;
			}
			else
			{
				inputFromAnywhere.LEFT_vector2 = Vector2.Zero;
			}
		}
		return inputFromAnywhere;
	}
}
