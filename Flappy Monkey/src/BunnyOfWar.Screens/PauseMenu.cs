using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar.Screens;

public class PauseMenu
{
	private const byte TransitionAlpha = byte.MaxValue;

	private const string usageText = "A button = Okay\nB button = Cancel";

	private bool pauseMenu = false;

	private string message = "";

	private SpriteFont smallFont;

	private static string[] menuChoices = new string[3] { "UnPause", " ", "Exit Level" };

	private List<Rectangle> menuRectsPhone = new List<Rectangle>();

	private Vector2 PHONE_TEXT_POSITION = new Vector2(100f, 80f);

	private Vector2 XBOX_TEXT_POSITION = new Vector2(450f, 200f);

	private Vector2 PSVITA_TEXT_POSITION = new Vector2(200f, 100f);

	private int currentSelection = 0;

	public PauseMenu()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void ProcessInput()
	{
		List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
		if (humanPlayers.Count == 0)
		{
			ScreenManager.ShowMainMenu();
		}
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.HasValue && FighterManager.humanPlayers[i].PROPERTIES.isLocal)
			{
				InputFromAnywhere playerInput = InputManager.GetPlayerInput(FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.Value, ref FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState, ref FighterManager.humanPlayers[i].PROPERTIES.previousKeyboardState);
				FigureOutInput(playerInput, FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.Value);
			}
		}
	}

	private void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput != null && anywhereInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
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
		switch (menuChoices[currentSelection])
		{
		case "UnPause":
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
			break;
		case "Exit Level":
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
			SoundManager.StopMusic();
			ScreenManager.ShowMainMenu();
			break;
		case "Game Stats":
			ScreenManager.ShowHighScores();
			break;
		case "Settings":
			ScreenManager.ShowOptions();
			break;
		case "Help":
			GraphicsManager.Message(RandomStaticGlobals.HelpTextForLevel, 0, -1);
			break;
		}
	}

	public void Load(ContentManager Content)
	{
		smallFont = GraphicsManager.font;
		menuRectsPhone.Clear();
		for (int i = 0; i < menuChoices.Length; i++)
		{
			Vector2 vector = GraphicsManager.font.MeasureString(menuChoices[i]);
			Rectangle item = new Rectangle((int)PHONE_TEXT_POSITION.X, (int)PHONE_TEXT_POSITION.Y + i * 60, (int)vector.X, (int)vector.Y);
			item.Inflate(50, 10);
			menuRectsPhone.Add(item);
		}
	}

	public void Clear()
	{
	}

	public void Draw()
	{
		GraphicsManager.Draw(GraphicsManager.LoadTexture("screens/paused", cacheResult: true), new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthSecondHighest);
	}

	public static string GetMemoryUsage()
	{
		return "";
	}

	public void clearControllerInput()
	{
		currentSelection = 0;
	}
}
