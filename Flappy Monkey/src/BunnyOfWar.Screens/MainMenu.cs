using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar.Screens;

public class MainMenu
{
	private bool showMoreFromAwesomeEnterprises = false;

	private DateTime showLogoSplashUntil = DateTime.MinValue;

	private DateTime lastMonkeyBlink = DateTime.MinValue;

	private string[] menuChoices = new string[4] { "Play", "Credits", "Exit", "Buy Me!!" };

	private int currentSelection = 0;

	private static DateTime musicLastChecked = DateTime.MinValue;

	public Texture2D background => GraphicsManager.LoadTexture("screens/mainscreen.png", cacheResult: true);

	public Texture2D cursor => GraphicsManager.LoadTexture("screens/cursor", cacheResult: true);

	public Texture2D buyMe => GraphicsManager.LoadTexture("screens/BuyMeText", cacheResult: true);

	public MainMenu()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void Draw()
	{
		if (showLogoSplashUntil == DateTime.MinValue)
		{
			GraphicsManager.LoadTexture("screens/AwesomeEnterprises", cacheResult: true);
			showLogoSplashUntil = DateTime.Now.AddSeconds(3.0);
		}
		if (showLogoSplashUntil > DateTime.Now)
		{
			GraphicsManager.Draw(GraphicsManager.LoadTexture("screens/AwesomeEnterprises", cacheResult: true), new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthTop);
			return;
		}
		if (lastMonkeyBlink.AddMilliseconds(5000.0) < DateTime.Now)
		{
			GraphicsManager.Draw(GraphicsManager.LoadTexture("screens/mainscreenblink", cacheResult: true), new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthSecondHighest);
			if (lastMonkeyBlink.AddMilliseconds(5100.0) < DateTime.Now)
			{
				lastMonkeyBlink = DateTime.Now;
			}
		}
		if (RandomStaticGlobals.ScoreCurrent > 0)
		{
			GraphicsManager.DrawHighScoresForMainScreen(RandomStaticGlobals.ScoreCurrent.ToString(), RandomStaticGlobals.ScoreAllTimeHigh.ToString());
		}
		if (RandomStaticGlobals.IsTrial())
		{
			GraphicsManager.Draw(GraphicsManager.LoadTexture("screens/mainscreenbuy", cacheResult: true), new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthTop);
		}
		GraphicsManager.Draw(background, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthForSky);
	}

	public void ProcessInput()
	{
		if (musicLastChecked < DateTime.Now.AddSeconds(1.0))
		{
			ScreenManager.playThemeSong();
			musicLastChecked = DateTime.Now;
		}
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

	private void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput == null)
		{
			return;
		}
		if (showMoreFromAwesomeEnterprises && anywhereInput.X_pressed)
		{
			GraphicsManager.Message(GraphicsManager.LoadTexture("screens/BuyMe360", cacheResult: false));
		}
		if (GraphicsManager.messages != null && GraphicsManager.messages.Count > 0)
		{
			if (anywhereInput.B_pressed)
			{
				GraphicsManager.messages.Clear();
			}
			return;
		}
		if (anywhereInput.A_pressed)
		{
			startFlappyGame(pi);
		}
		if (anywhereInput.Y_pressed)
		{
			ScreenManager.ShowCredits();
		}
		if (anywhereInput.X_pressed)
		{
			RandomStaticGlobals.BuyMe(pi);
		}
		if ((anywhereInput.LEFT_TRIGGER_held && anywhereInput.RIGHT_TRIGGER_held) || (anywhereInput.B_held && anywhereInput.X_held))
		{
			string text = "unsigned in gamer";
			if (!RandomStaticGlobals.IsTrial() && Gamer.SignedInGamers[pi] != null)
			{
				text = ((Gamer)Gamer.SignedInGamers[pi]).Gamertag;
				GraphicsManager.Message("Your bonus code for " + text + " is: " + Definitions.Options.GenerateXbox360Key(text));
			}
			else
			{
				GraphicsManager.Message("You have to buy the game first, and sign in to an Xbox Live profile, and then you'll get a bonus code.");
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
		if (!RandomStaticGlobals.IsTrial() && menuChoices[currentSelection] == "Buy Me!!")
		{
			currentSelection--;
		}
	}

	private void moveDown()
	{
		currentSelection++;
		if (currentSelection >= menuChoices.Length)
		{
			currentSelection = 0;
		}
		if (!RandomStaticGlobals.IsTrial() && menuChoices[currentSelection] == "Buy Me!!")
		{
			currentSelection = 0;
		}
	}

	private void startFlappyGame(PlayerIndex pi)
	{
		InputManager.ClearPreviousInputs();
		FighterManager.humanPlayers.Clear();
		FighterManager.addNewHumanPlayer(pi, isNetworkPlayer: false, "XBOX360", 1f);
		if (!RandomStaticGlobals.IsTrial())
		{
			LevelManager.LoadLevel("EasyFlappy.lvl", isPvP: false);
		}
		else
		{
			LevelManager.LoadLevel("TrialFlappy.lvl", isPvP: false);
		}
	}

	private void selectedSomething(PlayerIndex pi)
	{
		SoundManager.PlayMenuClick();
		switch (menuChoices[currentSelection])
		{
		case "Play":
			InputManager.ClearPreviousInputs();
			FighterManager.humanPlayers.Clear();
			if (Gamer.SignedInGamers[pi] == null)
			{
				GraphicsManager.Message("Sorry. You can play the game with this controller, but you have to log into a profile with it first.");
				break;
			}
			if (Gamer.SignedInGamers[pi].IsSignedInToLive)
			{
				FighterManager.addNewHumanPlayer(pi, isNetworkPlayer: false, "XBOX360", 1f);
			}
			else
			{
				GraphicsManager.Message("Sorry, Microsoft says you need an active Xbox Live Gold subscription. Please try again with whatever controller you have signed in with an Xbox Live account.");
			}
			if (!RandomStaticGlobals.IsTrial())
			{
				LevelManager.LoadLevel("EasyFlappy.lvl", isPvP: false);
			}
			else
			{
				LevelManager.LoadLevel("TrialFlappy.lvl", isPvP: false);
			}
			break;
		case "Credits":
			ScreenManager.ShowCredits();
			break;
		case "Settings":
			ScreenManager.ShowOptionsFromMenus();
			break;
		case "Register":
			break;
		case "Exit":
			Program.game.Exit();
			break;
		case "Tutorial":
			break;
		case "Buy Me!!":
			currentSelection = 0;
			RandomStaticGlobals.BuyMe(pi);
			break;
		}
	}

	private void guideButton()
	{
		Guide.BeginShowMessageBox(PlayerIndex.One, "Byaaaaa Title", "Texty texty long time!!", (IEnumerable<string>)new string[1] { "Ok" }, 0, (MessageBoxIcon)3, AsyncCallback(0), (object)object.Equals(0, 0));
	}

	private AsyncCallback AsyncCallback(object p)
	{
		throw new Exception("The method or operation is not implemented.");
	}

	public void Load(ContentManager Content)
	{
	}

	public void Clear()
	{
	}
}
