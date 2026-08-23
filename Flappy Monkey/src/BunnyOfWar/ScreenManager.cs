using System;
using System.Collections.ObjectModel;
using System.IO;
using BunnyOfWar.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar;

public static class ScreenManager
{
	public enum screens
	{
		Blank,
		MainMenu,
		GameLobby,
		WorldMap,
		Options,
		Credits,
		NetworkBusy,
		PauseMenu,
		NetworkGamesList
	}

	public static screens CurrentScreen = screens.Blank;

	private static MainMenu mainMenu = null;

	private static GameLobby gameLobby = null;

	private static Credits credits = null;

	private static Options options = null;

	private static WorldMap worldMap = null;

	private static NetworkBusyScreen networkBusyScreen = null;

	private static NetworkGamesList networkGamesList = null;

	private static PauseMenu pauseMenu = null;

	private static MessageBoxScreen messageBoxStatic;

	private static bool isNetworkBusy = false;

	public static bool isShowingHighScores = false;

	public static bool isShowingPlayerFailedScreen = false;

	public static bool isShowingBuyMeScreen = false;

	public static bool isShowingLevelInBackground = false;

	public static int loadingState = 0;

	private static Texture2D imgLoading = null;

	private static bool flappyPlayedIntro = false;

	private static DateTime playIntroThemeSongUntil = DateTime.MinValue;

	private static IAsyncResult keyboardResult;

	public static void UpdateLoadingStatus(string msg)
	{
		loadingState += 10;
		if (imgLoading == null)
		{
			imgLoading = GraphicsManager.LoadTexture("screens/Loading", cacheResult: false);
		}
		if (msg != "")
		{
			loadingState += 50;
		}
		if (msg == "done")
		{
			loadingState = 0;
			imgLoading.Dispose();
			imgLoading = null;
			int num = 0;
		}
		else
		{
			GraphicsManager.DrawLoading(GraphicsManager.imgSkull, new Rectangle(100 + loadingState, 400, 50, 50), imgLoading);
		}
	}

	public static void GameOver()
	{
		RandomStaticGlobals.StopAllControllerVibrationRumbles();
		GameOverFlappyMonkey();
	}

	public static void GameOverFlappyMonkey()
	{
		RandomStaticGlobals.CameraRollVelocity = Vector2.Zero;
		FileManager.SaveHighScores();
		InputManagerObject.WipeControllerStates();
		FighterManager.StopTimers();
		SoundManager.StopMusic();
		ShowMainMenu();
	}

	public static void SetWorldMapPosition(int x, int y)
	{
		worldMap.x = x;
		RandomStaticGlobals.currentlySelectedLevel = x;
	}

	public static void showNetworkBusyScreen(string message, IAsyncResult asyncResult, EventHandler<Networking.OperationCompletedEventArgs> OperationCompleted)
	{
		isNetworkBusy = true;
		networkBusyScreen = new NetworkBusyScreen(message, asyncResult);
		networkBusyScreen.OperationCompleted += OperationCompleted;
	}

	public static void hideNetworkBusy()
	{
		isNetworkBusy = false;
	}

	public static void showMessageBox(string message)
	{
		messageBoxStatic = new MessageBoxScreen(message);
	}

	public static void hideMessageBox()
	{
		messageBoxStatic = null;
	}

	public static void playThemeSong()
	{
		if (!flappyPlayedIntro)
		{
			SoundManager.PlayMusic("HappyFeet", IsRepeating: false);
			playIntroThemeSongUntil = DateTime.Now.AddSeconds(43.0);
			flappyPlayedIntro = true;
		}
		else if (playIntroThemeSongUntil < DateTime.Now)
		{
			SoundManager.PlayMusic("FeelingPositive", IsRepeating: true);
		}
	}

	private static void stopThemeSong()
	{
		SoundManager.StopMusic();
	}

	private static void pauseThemeSong()
	{
		SoundManager.PauseMusic();
	}

	public static void ShowBlank()
	{
		Clear();
		CurrentScreen = screens.Blank;
	}

	public static void ShowWorldMap()
	{
		ShowWorldMap(broadcastOverLive: false);
	}

	public static void ShowWorldMap(bool broadcastOverLive)
	{
		Clear();
		CurrentScreen = screens.WorldMap;
		if (worldMap == null)
		{
			worldMap = new WorldMap();
		}
		worldMap.Clear();
		playThemeSong();
		if (broadcastOverLive)
		{
			BroadcastScreenChange(screens.WorldMap);
		}
	}

	public static void ShowGameLobby(bool broadcast)
	{
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count == 0 && !Guide.IsVisible)
		{
			try
			{
				Guide.ShowSignIn(4, false);
				return;
			}
			catch (Exception)
			{
			}
		}
		Clear();
		CurrentScreen = screens.GameLobby;
		if (gameLobby == null)
		{
			gameLobby = new GameLobby();
		}
		gameLobby.ClearData();
		playThemeSong();
		Networking.initialize();
		if (broadcast)
		{
			BroadcastScreenChange(screens.GameLobby);
		}
	}

	public static void ShowCredits()
	{
		Clear();
		CurrentScreen = screens.Credits;
		if (credits == null)
		{
			credits = new Credits();
		}
		playThemeSong();
	}

	public static void ShowNetworkGamesList()
	{
		Clear();
		CurrentScreen = screens.NetworkGamesList;
		if (networkGamesList == null)
		{
			networkGamesList = new NetworkGamesList();
		}
		playThemeSong();
	}

	public static void CloseNetworkGamesList()
	{
		Clear();
	}

	public static void ShowOptions()
	{
		FileManager.Select360StorageDevice();
		Clear();
		CurrentScreen = screens.Options;
		isShowingLevelInBackground = true;
		if (options == null)
		{
			options = new Options();
		}
	}

	public static void ShowOptionsFromMenus()
	{
		FileManager.Select360StorageDevice();
		Clear();
		CurrentScreen = screens.Options;
		isShowingLevelInBackground = false;
		if (options == null)
		{
			options = new Options();
		}
	}

	public static void CloseOptions()
	{
		Clear();
		if (!isShowingLevelInBackground)
		{
			ShowWorldMap();
			return;
		}
		isShowingLevelInBackground = false;
		if (RandomStaticGlobals.isGamePaused)
		{
			CurrentScreen = screens.PauseMenu;
			return;
		}
		SoundManager.ResumeMusic();
		ShowBlank();
	}

	private static void FinishRegisteringWithKeyboardInput(IAsyncResult result)
	{
		if (!keyboardResult.IsCompleted)
		{
			return;
		}
		string text = Guide.EndShowKeyboardInput(keyboardResult);
		if (text != null && !(text == ""))
		{
			Definitions.Options.UpdateRegistrationKey(text.ToUpper());
			if (!Definitions.Options.RegisteredForBonusContent)
			{
				GraphicsManager.Message("Sorry, that didn't work... Did you type it correctly?");
			}
			else
			{
				GraphicsManager.Message("SUCCESS!!! You are now registered to play bonus content.");
			}
		}
	}

	public static void ShowRegisterOrBonus(PlayerIndex pi)
	{
		Definitions.Options.ReValidateKey();
		if (!Definitions.Options.RegisteredForBonusContent)
		{
			if (!Guide.IsVisible)
			{
				keyboardResult = Guide.BeginShowKeyboardInput(pi, "Register for Free!", "Please go to www.minotaur.bz/register360.aspx and follow the instructions there.", "", (AsyncCallback)FinishRegisteringWithKeyboardInput, (object)null);
			}
		}
		else if (RandomStaticGlobals.IsTrial())
		{
			LevelManager.LoadLevel(2);
		}
		else
		{
			LevelManager.LoadLevel("bonus", isPvP: false);
			NetworkGameplayManager.SendPackets(NetworkGameplayManager.PacketType.SelectedABonusLevel, null, null);
		}
	}

	public static void ShowMainMenu()
	{
		Clear();
		RandomStaticGlobals.ResetAllGameDefaults();
		FileManager.LoadHighScores();
		CurrentScreen = screens.MainMenu;
		if (mainMenu == null)
		{
			mainMenu = new MainMenu();
		}
		playThemeSong();
	}

	public static void ShowPauseMenu()
	{
		CurrentScreen = screens.PauseMenu;
		if (pauseMenu == null)
		{
			pauseMenu = new PauseMenu();
		}
		pauseMenu.clearControllerInput();
	}

	public static void ShowBuyMeScreen()
	{
		if (RandomStaticGlobals.IsTrial())
		{
			isShowingBuyMeScreen = true;
			ShowHighScores();
		}
	}

	public static void ShowHighScores()
	{
		RandomStaticGlobals.StopAllControllerVibrationRumbles();
		isShowingHighScores = true;
		isShowingLevelInBackground = true;
		NetworkGameplayManager.SendPlayerStats();
	}

	public static void HideHighScoress()
	{
		isShowingHighScores = false;
		isShowingLevelInBackground = false;
		if (pauseMenu != null)
		{
			pauseMenu.clearControllerInput();
		}
	}

	public static void ShowPlayerFailedScreen()
	{
		PlayerFailedScreen.Load(RandomStaticGlobals.Content);
		isShowingPlayerFailedScreen = true;
	}

	public static void HidePlayerFailedScreen()
	{
		isShowingPlayerFailedScreen = false;
	}

	private static void Clear()
	{
		RandomStaticGlobals.StopAllControllerVibrationRumbles();
		InputManagerObject.WipeControllerStates();
	}

	public static void Draw(GameTime gameTime)
	{
		if (messageBoxStatic != null)
		{
			messageBoxStatic.Draw();
		}
		if (isNetworkBusy)
		{
			networkBusyScreen.Draw(gameTime);
			return;
		}
		if (isShowingPlayerFailedScreen)
		{
			PlayerFailedScreen.Draw();
			return;
		}
		if (isShowingHighScores)
		{
			HighScoreScreen.Draw();
			return;
		}
		if (GraphicsManager.messages != null && GraphicsManager.messages.Count > 0)
		{
			for (int i = 0; i < GraphicsManager.messages.Count; i++)
			{
			}
			return;
		}
		switch (CurrentScreen)
		{
		case screens.Blank:
			GraphicsManager.DrawMessages();
			break;
		case screens.Credits:
			credits.Draw();
			break;
		case screens.GameLobby:
			gameLobby.Draw();
			break;
		case screens.NetworkGamesList:
			networkGamesList.Draw();
			break;
		case screens.MainMenu:
			mainMenu.Draw();
			break;
		case screens.Options:
			options.Draw();
			break;
		case screens.WorldMap:
			worldMap.Draw();
			break;
		case screens.PauseMenu:
			pauseMenu.Draw();
			break;
		case screens.NetworkBusy:
			break;
		}
	}

	public static void UpdateAndProcessInput()
	{
		if (isNetworkBusy)
		{
			networkBusyScreen.Update();
		}
		if (messageBoxStatic != null)
		{
			messageBoxStatic.HandleInput();
		}
		else
		{
			if (InputManagerObject.handleMessagesInput())
			{
				return;
			}
			if (isShowingPlayerFailedScreen)
			{
				PlayerFailedScreen.ProcessInput();
				return;
			}
			if (isShowingHighScores)
			{
				HighScoreScreen.ProcessInput();
				return;
			}
			switch (CurrentScreen)
			{
			case screens.Blank:
				break;
			case screens.Credits:
				credits.ProcessInput();
				break;
			case screens.GameLobby:
				gameLobby.ProcessInput();
				break;
			case screens.NetworkGamesList:
				networkGamesList.ProcessInput();
				networkGamesList.Update();
				break;
			case screens.MainMenu:
				mainMenu.ProcessInput();
				break;
			case screens.Options:
				options.ProcessInput();
				break;
			case screens.WorldMap:
				worldMap.ProcessInput();
				break;
			case screens.PauseMenu:
				pauseMenu.ProcessInput();
				break;
			case screens.NetworkBusy:
				break;
			}
		}
	}

	public static void BroadcastScreenChange(screens screen)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)18);
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)screen);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)0);
		}
	}

	public static void ReadScreenChange(PacketReader pr)
	{
		try
		{
			byte b = ((BinaryReader)(object)pr).ReadByte();
			switch ((screens)Enum.Parse(typeof(screens), b.ToString(), ignoreCase: true))
			{
			case screens.WorldMap:
				ShowWorldMap(broadcastOverLive: false);
				break;
			case screens.GameLobby:
				ShowGameLobby(broadcast: false);
				break;
			}
		}
		catch (Exception)
		{
		}
	}
}
