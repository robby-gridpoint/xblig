using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar.Screens;

public class NetworkGamesList
{
	private const byte TransitionAlpha = byte.MaxValue;

	private bool pauseMenu = false;

	private string message = "";

	private SpriteFont smallFont;

	private string usageText = "why is this here exactly?";

	private static Rectangle backButtonRect = Definitions.rectBackButton;

	private GamePadState? player1 = null;

	private GamePadState? player2 = null;

	private GamePadState? player3 = null;

	private GamePadState? player4 = null;

	private GamePadState? player1previous = null;

	private GamePadState? player2previous = null;

	private GamePadState? player3previous = null;

	private GamePadState? player4previous = null;

	private Texture2D background;

	private Texture2D cursor;

	private List<Rectangle> menuRectsPhone = new List<Rectangle>();

	private Vector2 PHONE_TEXT_POSITION = new Vector2(100f, 30f);

	private Vector2 XBOX_TEXT_POSITION = new Vector2(450f, 200f);

	private int currentSelection = 0;

	private int currentPage = 0;

	private int showXGamesPerPage = 10;

	public NetworkGamesList()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void Update()
	{
	}

	public void ProcessInput()
	{
		if (GamePad.GetState(PlayerIndex.One).IsConnected && NetworkGameplayManager.localPlayerIndex == PlayerIndex.One)
		{
			player1 = GamePad.GetState(PlayerIndex.One);
			FigureOutInput(player1, player1previous);
			player1previous = player1;
		}
		if (GamePad.GetState(PlayerIndex.Two).IsConnected && NetworkGameplayManager.localPlayerIndex == PlayerIndex.Two)
		{
			player2 = GamePad.GetState(PlayerIndex.Two);
			FigureOutInput(player2, player2previous);
			player2previous = player2;
		}
		if (GamePad.GetState(PlayerIndex.Three).IsConnected && NetworkGameplayManager.localPlayerIndex == PlayerIndex.Three)
		{
			player3 = GamePad.GetState(PlayerIndex.Three);
			FigureOutInput(player3, player3previous);
			player3previous = player3;
		}
		if (GamePad.GetState(PlayerIndex.Four).IsConnected && NetworkGameplayManager.localPlayerIndex == PlayerIndex.Four)
		{
			player4 = GamePad.GetState(PlayerIndex.Four);
			FigureOutInput(player4, player4previous);
			player4previous = player4;
		}
	}

	public void ExitScreen()
	{
		currentSelection = 0;
		ScreenManager.CloseNetworkGamesList();
		ScreenManager.ShowGameLobby(broadcast: false);
	}

	private void FigureOutInput(GamePadState? gamePadState, GamePadState? previousGamePadState)
	{
		if (gamePadState.HasValue && previousGamePadState.HasValue)
		{
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
				selectedSomething();
			}
			if (gamePadState.Value.Buttons.B == ButtonState.Released && previousGamePadState.Value.Buttons.B == ButtonState.Pressed)
			{
				ExitScreen();
			}
		}
	}

	private void moveUp()
	{
		currentSelection--;
		if (currentSelection < 0 && currentPage > 0)
		{
			currentSelection = 0;
			currentPage--;
		}
		if (currentSelection < 0 && currentPage <= 0)
		{
			currentSelection = 0;
		}
	}

	private void moveDown()
	{
		currentSelection++;
		if (currentSelection + showXGamesPerPage * currentPage >= Networking.multiplayerAvailableGamesList.Count)
		{
			currentSelection--;
		}
		if (currentSelection >= showXGamesPerPage)
		{
			currentPage++;
			currentSelection = 0;
		}
	}

	private void selectedSomething()
	{
		SoundManager.PlayMenuClick();
		int num = currentSelection;
		if (currentPage > 0)
		{
			num += currentPage * showXGamesPerPage;
		}
		Networking.JoinGame(num);
		currentSelection = 0;
	}

	public void Load(ContentManager Content)
	{
		cursor = GraphicsManager.LoadTexture("screens/cursor");
		smallFont = GraphicsManager.font;
		int num = 0;
	}

	public void Clear()
	{
		currentSelection = 0;
	}

	public void Draw()
	{
		if (!RandomStaticGlobals.isGamePaused)
		{
			GraphicsManager.Draw(GraphicsManager.imgNiceBackground, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}
		else
		{
			GraphicsManager.DrawRectangle(new Rectangle(0, 0, 1920, 1080), GraphicsManager.TheColorTransparentGray);
		}
		Vector2 screenFullSize = GraphicsManager.ScreenFullSize;
		Vector2 vector = GraphicsManager.font.MeasureString(message);
		Vector2 xBOX_TEXT_POSITION = XBOX_TEXT_POSITION;
		Vector2 vector2 = smallFont.MeasureString(usageText);
		Vector2 vector3 = (screenFullSize - vector2) / 2f;
		vector3.Y = xBOX_TEXT_POSITION.Y + (float)GraphicsManager.font.LineSpacing * 1.1f;
		Color c = new Color(255, 255, 255, 255);
		Rectangle rectangle = new Rectangle((int)Math.Min(vector3.X, xBOX_TEXT_POSITION.X), (int)xBOX_TEXT_POSITION.Y, (int)Math.Max(vector2.X, vector.X), (int)((float)GraphicsManager.font.LineSpacing * 1.1f + vector2.Y));
		int num = 0;
		if (currentPage > 0)
		{
			num = showXGamesPerPage * currentPage;
		}
		for (int i = 0; i < showXGamesPerPage && i + num < Networking.multiplayerAvailableGamesList.Count; i++)
		{
			Vector2 vector4 = GraphicsManager.font.MeasureString(Networking.multiplayerAvailableGamesList[i]);
			Rectangle rectangle2 = new Rectangle((int)xBOX_TEXT_POSITION.X, (int)xBOX_TEXT_POSITION.Y + i * 60, (int)vector4.X, (int)vector4.Y);
			rectangle2.Inflate(50, 10);
			GraphicsManager.DrawRectangle(rectangle2, GraphicsManager.TheColorTransparentGray);
			GraphicsManager.DrawString((int)xBOX_TEXT_POSITION.X, (int)xBOX_TEXT_POSITION.Y + i * 60, Networking.multiplayerAvailableGamesList[i + num], c, GraphicsManager.font);
		}
		GraphicsManager.Draw(cursor, new Vector2((int)xBOX_TEXT_POSITION.X - 250, xBOX_TEXT_POSITION.Y + (float)(currentSelection * 60)), null, Color.White, 0f, Vector2.Zero, new Vector2(0.5f, 0.5f), SpriteEffects.None, 1f);
		string text = "cancel";
		Vector2 vector5 = GraphicsManager.font.MeasureString(text);
		Rectangle rectangle3 = new Rectangle(backButtonRect.X, backButtonRect.Y, (int)vector5.X, (int)vector5.Y);
		rectangle3.Inflate(50, 10);
		GraphicsManager.DrawRectangle(rectangle3, GraphicsManager.TheColorTransparentGray, Definitions.LayerDepthSecondHighest);
		GraphicsManager.DrawRectangle(rectangle3, GraphicsManager.TheColorTransparentGray, Definitions.LayerDepthSecondHighest);
		GraphicsManager.DrawString(backButtonRect.X, backButtonRect.Y, text, Color.White, GraphicsManager.font);
		GraphicsManager.Draw(GraphicsManager.imgButtonB, new Rectangle(rectangle3.X - 70, rectangle3.Y - 30, 100, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthTop);
	}

	public void clearControllerInput()
	{
		player1 = null;
		player2 = null;
		player3 = null;
		player4 = null;
		player1previous = null;
		player2previous = null;
		player3previous = null;
		player4previous = null;
		currentSelection = 0;
	}
}
