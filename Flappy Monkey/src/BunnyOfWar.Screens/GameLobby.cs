using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar.Screens;

public class GameLobby
{
	private bool controllerOneReady = false;

	private bool controllerTwoReady = false;

	private bool controllerThreeReady = false;

	private bool controllerFourReady = false;

	private bool remotePlayerOneReady = false;

	private bool remotePlayerTwoReady = false;

	private bool remotePlayerThreeReady = false;

	private bool remotePlayerFourReady = false;

	private bool remotePlayerFiveReady = false;

	private bool remotePlayerSixReady = false;

	private bool remotePlayerSevenReady = false;

	private bool remotePlayerEightReady = false;

	private Texture2D background;

	private Texture2D cursor;

	private Texture2D signedIn;

	private Texture2D bloodCursor;

	private string[] menuChoices = new string[3] { "Play", "Options", "Credits" };

	private int currentSelection = 0;

	public GameLobby()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void Draw()
	{
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		GraphicsManager.spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		if (Networking.IsGameJoinable)
		{
			GraphicsManager.Draw(bloodCursor, new Vector2(950f, 820f), Definitions.LayerDepthSecondHighest);
		}
		else
		{
			GraphicsManager.Draw(bloodCursor, new Vector2(1325f, 820f), Definitions.LayerDepthSecondHighest);
		}
		int num = 500;
		int num2 = 100;
		if (Networking.RemoteGamersCount == 0 || Networking.isHost)
		{
			if (controllerOneReady)
			{
				GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
				num2 += 200;
			}
			if (controllerTwoReady)
			{
				GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
				num2 += 200;
			}
			if (controllerThreeReady)
			{
				GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
				num2 += 200;
			}
			if (controllerFourReady)
			{
				GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
				num2 += 200;
			}
		}
		if (Networking.RemoteGamersCount <= 0)
		{
			return;
		}
		if (!Networking.isHost)
		{
			GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
			num2 += 200;
		}
		if (Networking.RemoteGamersCount > 0)
		{
			GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
			num2 += 200;
		}
		if (Networking.RemoteGamersCount > 1)
		{
			GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
			num2 += 200;
		}
		if (Networking.RemoteGamersCount > 2)
		{
			GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
			num2 += 200;
		}
		if (Networking.RemoteGamersCount > 3)
		{
			GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
			num2 += 200;
		}
		if (Networking.RemoteGamersCount > 4)
		{
			GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
			num2 += 200;
		}
		if (Networking.RemoteGamersCount > 5)
		{
			GraphicsManager.spriteBatch.Draw(signedIn, new Vector2(num2, 500f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
			num2 += 200;
		}
		int num3 = 200;
		int num4 = 0;
		if (Networking.AllGamers == null || Networking.RemoteGamersCount <= 0)
		{
			return;
		}
		GamerCollectionEnumerator<NetworkGamer> enumerator = Networking.AllGamers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NetworkGamer current = enumerator.Current;
				GraphicsManager.DrawString(num3, num + num4, ((Gamer)current).Gamertag, Color.DarkRed, GraphicsManager.fontSmall);
				num4 = ((num4 != 350) ? 350 : 0);
				num3 += 200;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void ProcessInput()
	{
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Invalid comparison between Unknown and I4
		string text = "";
		if (GamePad.GetState(PlayerIndex.One).IsConnected)
		{
			InputManager.player1 = GamePad.GetState(PlayerIndex.One);
			text = "Guest";
			if (Gamer.SignedInGamers[PlayerIndex.One] != null)
			{
				text = ((Gamer)Gamer.SignedInGamers[PlayerIndex.One]).Gamertag;
			}
			FigureOutInput(InputManager.player1, InputManager.gamePad1previous, ref controllerOneReady, PlayerIndex.One, text);
			InputManager.gamePad1previous = InputManager.player1;
		}
		if (GamePad.GetState(PlayerIndex.Two).IsConnected)
		{
			InputManager.player2 = GamePad.GetState(PlayerIndex.Two);
			text = "Guest";
			if (Gamer.SignedInGamers[PlayerIndex.Two] != null)
			{
				text = ((Gamer)Gamer.SignedInGamers[PlayerIndex.Two]).Gamertag;
			}
			FigureOutInput(InputManager.player2, InputManager.gamePad2previous, ref controllerTwoReady, PlayerIndex.Two, text);
			InputManager.gamePad2previous = InputManager.player2;
		}
		if (GamePad.GetState(PlayerIndex.Three).IsConnected)
		{
			InputManager.player3 = GamePad.GetState(PlayerIndex.Three);
			text = "Guest";
			if (Gamer.SignedInGamers[PlayerIndex.Three] != null)
			{
				text = ((Gamer)Gamer.SignedInGamers[PlayerIndex.Three]).Gamertag;
			}
			FigureOutInput(InputManager.player3, InputManager.gamePad3previous, ref controllerThreeReady, PlayerIndex.Three, text);
			InputManager.gamePad3previous = InputManager.player3;
		}
		if (GamePad.GetState(PlayerIndex.Four).IsConnected)
		{
			InputManager.player4 = GamePad.GetState(PlayerIndex.Four);
			text = "Guest";
			if (Gamer.SignedInGamers[PlayerIndex.Four] != null)
			{
				text = ((Gamer)Gamer.SignedInGamers[PlayerIndex.Four]).Gamertag;
			}
			FigureOutInput(InputManager.player4, InputManager.gamePad4previous, ref controllerFourReady, PlayerIndex.Four, text);
			InputManager.gamePad4previous = InputManager.player4;
		}
		if ((int?)Networking.SessionState == 1)
		{
			startGameClient();
		}
	}

	public void startGameClient()
	{
		ScreenManager.ShowWorldMap();
	}

	public void startGame()
	{
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		SoundManager.PlayMenuClick();
		float scale = 1f;
		if (Networking.isHost || Networking.RemoteGamersCount == 0)
		{
			FighterManager.humanPlayers.Clear();
			if (controllerOneReady && GamePad.GetState(PlayerIndex.One).IsConnected)
			{
				FighterObject fighterObject;
				if (Gamer.SignedInGamers[PlayerIndex.One] != null)
				{
					fighterObject = FighterManager.addNewHumanPlayer(PlayerIndex.One, isNetworkPlayer: false, ((Gamer)Gamer.SignedInGamers[PlayerIndex.One]).Gamertag, scale);
					fighterObject.PROPERTIES.GamerTag = ((Gamer)Gamer.SignedInGamers[PlayerIndex.One]).Gamertag;
				}
				else
				{
					fighterObject = FighterManager.addNewHumanPlayer(PlayerIndex.One, isNetworkPlayer: false, "Player 1", scale);
				}
				fighterObject.PROPERTIES.isLocal = true;
			}
			if (controllerTwoReady && GamePad.GetState(PlayerIndex.Two).IsConnected)
			{
				FighterObject fighterObject;
				if (Gamer.SignedInGamers[PlayerIndex.Two] != null)
				{
					fighterObject = FighterManager.addNewHumanPlayer(PlayerIndex.Two, isNetworkPlayer: false, ((Gamer)Gamer.SignedInGamers[PlayerIndex.Two]).Gamertag, scale);
					fighterObject.PROPERTIES.GamerTag = ((Gamer)Gamer.SignedInGamers[PlayerIndex.Two]).Gamertag;
				}
				else
				{
					fighterObject = FighterManager.addNewHumanPlayer(PlayerIndex.Two, isNetworkPlayer: false, "Player 2", scale);
				}
				fighterObject.PROPERTIES.isLocal = true;
			}
			if (controllerThreeReady && GamePad.GetState(PlayerIndex.Three).IsConnected)
			{
				FighterObject fighterObject;
				if (Gamer.SignedInGamers[PlayerIndex.Three] != null)
				{
					fighterObject = FighterManager.addNewHumanPlayer(PlayerIndex.Three, isNetworkPlayer: false, ((Gamer)Gamer.SignedInGamers[PlayerIndex.Three]).Gamertag, scale);
					fighterObject.PROPERTIES.GamerTag = ((Gamer)Gamer.SignedInGamers[PlayerIndex.Three]).Gamertag;
				}
				else
				{
					fighterObject = FighterManager.addNewHumanPlayer(PlayerIndex.Three, isNetworkPlayer: false, "Player 3", scale);
				}
				fighterObject.PROPERTIES.isLocal = true;
			}
			if (controllerFourReady && GamePad.GetState(PlayerIndex.Four).IsConnected)
			{
				FighterObject fighterObject;
				if (Gamer.SignedInGamers[PlayerIndex.Four] != null)
				{
					fighterObject = FighterManager.addNewHumanPlayer(PlayerIndex.Four, isNetworkPlayer: false, ((Gamer)Gamer.SignedInGamers[PlayerIndex.Four]).Gamertag, scale);
					fighterObject.PROPERTIES.GamerTag = ((Gamer)Gamer.SignedInGamers[PlayerIndex.Four]).Gamertag;
				}
				else
				{
					fighterObject = FighterManager.addNewHumanPlayer(PlayerIndex.Four, isNetworkPlayer: false, "Player 4", scale);
				}
				fighterObject.PROPERTIES.isLocal = true;
			}
			Networking.IsGameJoinable = false;
			if (Networking.RemoteGamersCount > 0)
			{
				GamerCollectionEnumerator<NetworkGamer> enumerator = Networking.RemoteGamers.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						NetworkGamer current = enumerator.Current;
						FighterManager.addNewHumanPlayer(null, isNetworkPlayer: true, ((Gamer)current).Gamertag, scale);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
			}
			NetworkGameplayManager.SetAndSendPlayerIDs();
			NetworkGameplayManager.SetAndSendRandomSeed();
			Networking.StartGame();
			ScreenManager.ShowWorldMap(broadcastOverLive: true);
		}
		else
		{
			GraphicsManager.Message("Sorry, only the host cant start the game. Are you the host? NO!");
		}
	}

	private void FigureOutInput(GamePadState? gamePadState, GamePadState? previousGamePadState, ref bool readyVar, PlayerIndex ndx, string gamerTag)
	{
		if (!gamePadState.HasValue || !previousGamePadState.HasValue)
		{
			return;
		}
		if (gamePadState.Value.DPad.Up == ButtonState.Pressed && previousGamePadState.Value.DPad.Up == ButtonState.Released)
		{
			moveUp();
		}
		if (gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickUp) && previousGamePadState.Value.IsButtonUp(Buttons.LeftThumbstickUp))
		{
			moveUp();
		}
		if (gamePadState.Value.DPad.Down == ButtonState.Pressed && previousGamePadState.Value.DPad.Down == ButtonState.Released)
		{
			moveDown();
		}
		if (gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickDown) && previousGamePadState.Value.IsButtonUp(Buttons.LeftThumbstickDown))
		{
			moveDown();
		}
		if (gamePadState.Value.Buttons.A == ButtonState.Released && previousGamePadState.Value.Buttons.A == ButtonState.Pressed)
		{
			readyVar = true;
		}
		if (gamePadState.Value.Buttons.B == ButtonState.Released && previousGamePadState.Value.Buttons.B == ButtonState.Pressed)
		{
			if (!readyVar)
			{
				Networking.StopGame();
				Networking.EndSession();
				ScreenManager.ShowMainMenu();
			}
			readyVar = false;
		}
		if ((readyVar || Networking.RemoteGamersCount > 0) && gamePadState.Value.Buttons.Start == ButtonState.Pressed && previousGamePadState.Value.Buttons.Start == ButtonState.Released)
		{
			readyVar = true;
			startGame();
		}
		if (gamePadState.Value.Buttons.Y == ButtonState.Released && previousGamePadState.Value.Buttons.Y == ButtonState.Pressed)
		{
			NetworkGameplayManager.localPlayerIndex = ndx;
			if (gamerTag != "Guest" && Gamer.SignedInGamers[ndx].Privileges.AllowOnlineSessions)
			{
				Networking.ListPublicGames((NetworkSessionType)2, ndx, gamerTag);
			}
			else
			{
				GraphicsManager.Message("Sorry, Microsoft says you're not allowed to join an online game. Please try again with whatever controller you have signed in with an Xbox Live account.");
			}
		}
		if (gamePadState.Value.Buttons.X == ButtonState.Released && previousGamePadState.Value.Buttons.X == ButtonState.Pressed)
		{
			NetworkGameplayManager.localPlayerIndex = ndx;
			if (gamerTag != "Guest" && Gamer.SignedInGamers[ndx].Privileges.AllowOnlineSessions)
			{
				Networking.InvitePlayer(ndx);
			}
			else
			{
				GraphicsManager.Message("Sorry, Microsoft says you're not allowed to play an online game. Please try again with whatever controller you have signed in with an Xbox Live account.");
			}
		}
		if (gamePadState.Value.Buttons.Back == ButtonState.Released && previousGamePadState.Value.Buttons.Back == ButtonState.Pressed)
		{
			Networking.IsGameJoinable = !Networking.IsGameJoinable;
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

	public void Load(ContentManager Content)
	{
		background = GraphicsManager.LoadTexture("screens/NiceBackground");
		cursor = GraphicsManager.LoadTexture("screens/cursor");
		signedIn = GraphicsManager.LoadTexture("screens/cursor");
		bloodCursor = GraphicsManager.LoadTexture("screens/cursor");
		FighterManager.ClearData();
		Networking.IsGameJoinable = false;
	}

	public void ClearData()
	{
		controllerOneReady = false;
		controllerTwoReady = false;
		controllerThreeReady = false;
		controllerFourReady = false;
	}

	public void Clear()
	{
	}

	private static void SendNetworkPlayerReady(Gamer gamer, bool isReady)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)12);
			((BinaryWriter)(object)Networking.packetWriter).Write(gamer.Gamertag.Length);
			((BinaryWriter)(object)Networking.packetWriter).Write(gamer.Gamertag.ToCharArray());
			((BinaryWriter)(object)Networking.packetWriter).Write(isReady);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)3);
		}
	}

	private static void ReadNetworkPlayerReady(string gamerTag, bool isReady)
	{
	}

	public void SendTestPackets()
	{
	}
}
