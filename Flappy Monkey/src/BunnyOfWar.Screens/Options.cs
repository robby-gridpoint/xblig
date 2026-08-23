using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar.Screens;

public class Options
{
	private const byte TransitionAlpha = byte.MaxValue;

	private const string usageText = "A button = Okay\nB button = Cancel";

	private bool pauseMenu = false;

	private string message = "";

	private SpriteFont smallFont;

	private static bool changesWereMade = false;

	private static string[] menuChoicesBaseValues = new string[8] { "Blood", "Vibration", "Master Volume", "Music Volume", "Sound Effects Volume", "Difficulty", "Screen Edge", "Back" };

	private static string[] menuChoices = new string[8] { "Blood", "Vibration", "Master Volume", "Music Volume", "Sound Effects Volume", "Difficulty", "Screen Edge", "Back" };

	private List<Rectangle> menuRectsPhone = new List<Rectangle>();

	private Vector2 PHONE_TEXT_POSITION = new Vector2(100f, 30f);

	private Vector2 XBOX_TEXT_POSITION = new Vector2(450f, 200f);

	private Vector2 PSVITA_TEXT_POSITION = new Vector2(200f, 150f);

	private int currentSelection = 0;

	public Texture2D background => GraphicsManager.imgNiceBackground;

	public Options()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void ProcessInput()
	{
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

	public void ExitScreen()
	{
		if (changesWereMade)
		{
			Definitions.Options.Save();
		}
		changesWereMade = false;
		ScreenManager.CloseOptions();
	}

	private void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput != null)
		{
			if (anywhereInput.UP_pressed)
			{
				moveUp();
			}
			if (anywhereInput.DOWN_pressed)
			{
				moveDown();
			}
			if (anywhereInput.A_pressed)
			{
				selectedSomething();
			}
			if (anywhereInput.B_pressed)
			{
				ExitScreen();
			}
		}
	}

	private void moveUp()
	{
		currentSelection--;
		if (currentSelection < 0)
		{
			currentSelection = menuChoices.Length - 1;
		}
	}

	private void moveDown()
	{
		currentSelection++;
		if (currentSelection >= menuChoices.Length)
		{
			currentSelection = 0;
		}
	}

	private void selectedSomething()
	{
		SoundManager.PlayMenuClick();
		switch (menuChoicesBaseValues[currentSelection])
		{
		case "Blood":
			menuChoices[currentSelection] = menuChoicesBaseValues[currentSelection] + " - " + Definitions.Options.toggleBlood();
			changesWereMade = true;
			break;
		case "Vibration":
			menuChoices[currentSelection] = menuChoicesBaseValues[currentSelection] + " - " + Definitions.Options.toggleVibrations();
			changesWereMade = true;
			break;
		case "Master Volume":
			menuChoices[currentSelection] = menuChoicesBaseValues[currentSelection] + " - " + Definitions.Options.toggleVolume();
			changesWereMade = true;
			break;
		case "Music Volume":
			menuChoices[currentSelection] = menuChoicesBaseValues[currentSelection] + " - " + Definitions.Options.toggleMusic();
			changesWereMade = true;
			break;
		case "Sound Effects Volume":
			menuChoices[currentSelection] = menuChoicesBaseValues[currentSelection] + " - " + Definitions.Options.toggleSounds();
			changesWereMade = true;
			break;
		case "Easy Mode":
			menuChoices[currentSelection] = menuChoicesBaseValues[currentSelection] + " - " + Definitions.Options.toggleMercy();
			changesWereMade = true;
			break;
		case "Difficulty":
			menuChoices[currentSelection] = menuChoicesBaseValues[currentSelection] + " - " + Definitions.Options.toggleMercy();
			changesWereMade = true;
			break;
		case "Screen Edge":
			menuChoices[currentSelection] = menuChoicesBaseValues[currentSelection] + " - " + Definitions.Options.toggleTitleSafe();
			changesWereMade = true;
			break;
		case "Back":
			ExitScreen();
			break;
		}
	}

	public void Load(ContentManager Content)
	{
		smallFont = GraphicsManager.font;
		int num = 0;
		menuChoices[num] = menuChoicesBaseValues[num] + " - " + Definitions.Options.getWording(Definitions.Options.BloodOnOff);
		num++;
		menuChoices[num] = menuChoicesBaseValues[num] + " - " + Definitions.Options.getWording(Definitions.Options.VibrationsOnOff);
		num++;
		menuChoices[num] = menuChoicesBaseValues[num] + " - " + Definitions.Options.getWording(Definitions.Options.masterVolume);
		num++;
		menuChoices[num] = menuChoicesBaseValues[num] + " - " + Definitions.Options.getWording(Definitions.Options.MusicVolume);
		num++;
		menuChoices[num] = menuChoicesBaseValues[num] + " - " + Definitions.Options.getWording(Definitions.Options.SoundsVolume);
		num++;
		menuChoices[num] = menuChoicesBaseValues[num] + " - " + Definitions.Options.getDifficultyWording(Definitions.Options.Difficulty);
		num++;
		menuChoices[num] = menuChoicesBaseValues[num] + " - " + Definitions.Options.TitleSafePercent + "%";
		num++;
		menuRectsPhone.Clear();
		for (int i = 0; i < menuChoices.Length; i++)
		{
			Vector2 vector = GraphicsManager.font.MeasureString(menuChoices[i]);
			Rectangle item = new Rectangle((int)PHONE_TEXT_POSITION.X, (int)PHONE_TEXT_POSITION.Y + i * 60, (int)vector.X, (int)vector.Y);
			item.Inflate(50, 10);
			menuRectsPhone.Add(item);
		}
		changesWereMade = false;
	}

	public void Clear()
	{
		changesWereMade = false;
	}

	public void Draw()
	{
		if (RandomStaticGlobals.isGamePaused)
		{
			GraphicsManager.DrawRectangle(new Rectangle(0, 0, (int)GraphicsManager.ScreenFullSize.X, (int)GraphicsManager.ScreenFullSize.Y), GraphicsManager.TheColorTransparentGray);
		}
		else
		{
			GraphicsManager.Draw(background, new Rectangle(0, 0, (int)GraphicsManager.ScreenFullSize.X, (int)GraphicsManager.ScreenFullSize.Y), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}
		Vector2 screenFullSize = GraphicsManager.ScreenFullSize;
		Vector2 vector = GraphicsManager.font.MeasureString(message);
		Vector2 xBOX_TEXT_POSITION = XBOX_TEXT_POSITION;
		Vector2 vector2 = smallFont.MeasureString("A button = Okay\nB button = Cancel");
		Vector2 vector3 = (screenFullSize - vector2) / 2f;
		vector3.Y = xBOX_TEXT_POSITION.Y + (float)GraphicsManager.font.LineSpacing * 1.1f;
		Color c = new Color(255, 255, 255, 255);
		Rectangle rectangle = new Rectangle((int)Math.Min(vector3.X, xBOX_TEXT_POSITION.X), (int)xBOX_TEXT_POSITION.Y, (int)Math.Max(vector2.X, vector.X), (int)((float)GraphicsManager.font.LineSpacing * 1.1f + vector2.Y));
		for (int i = 0; i < menuChoices.Length; i++)
		{
			Vector2 vector4 = GraphicsManager.font.MeasureString(menuChoices[i]);
			Rectangle rectangle2 = new Rectangle((int)xBOX_TEXT_POSITION.X, (int)xBOX_TEXT_POSITION.Y + i * 60, (int)vector4.X, (int)vector4.Y);
			rectangle2.Inflate(50, 10);
			GraphicsManager.DrawRectangle(rectangle2, GraphicsManager.TheColorTransparentGray);
			GraphicsManager.DrawString((int)xBOX_TEXT_POSITION.X, (int)xBOX_TEXT_POSITION.Y + i * 60, menuChoices[i], c, GraphicsManager.font);
		}
		GraphicsManager.spriteBatch.Draw(GraphicsManager.imgButtonB, new Rectangle((int)xBOX_TEXT_POSITION.X - 120, (int)xBOX_TEXT_POSITION.Y + menuChoices.Length * 60 - 90, 100, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthTop);
		GraphicsManager.Draw(GraphicsManager.imgCursor, new Vector2((int)xBOX_TEXT_POSITION.X - 250, xBOX_TEXT_POSITION.Y + (float)(currentSelection * 60)), null, Color.White, 0f, Vector2.Zero, new Vector2(0.5f, 0.5f), SpriteEffects.None, 1f);
	}

	public void clearControllerInput()
	{
		currentSelection = 0;
	}
}
