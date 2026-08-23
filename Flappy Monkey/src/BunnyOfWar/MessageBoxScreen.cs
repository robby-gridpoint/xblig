using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public class MessageBoxScreen
{
	private const byte TransitionAlpha = byte.MaxValue;

	private const string usageText = "A button = Okay\nB button = Cancel";

	private bool pauseMenu = false;

	private string message;

	private Texture2D texture = null;

	private SpriteFont smallFont;

	private bool isPopup = false;

	private Stopwatch stopWatch = new Stopwatch();

	public float durationInSeconds = 0f;

	public bool isActive = false;

	public bool isQueuedUp = false;

	public bool isSkippable = true;

	public bool isCutScene = false;

	public bool isQTE = false;

	public bool isBisYourOnlyEscape = false;

	private string sceneName = "";

	private static Rectangle backButtonRect = Definitions.rectBackButton;

	private static Rectangle rectCutsceneSkipRect = new Rectangle(1600, 900, 50, 20);

	private static Rectangle rectCutsceneNextRect = new Rectangle(1400, 900, 50, 20);

	public event EventHandler<EventArgs> Accepted;

	public event EventHandler<EventArgs> Cancelled;

	public MessageBoxScreen(string message)
	{
		LoadContent();
		this.message = message;
		isActive = true;
	}

	public MessageBoxScreen(string message, int durationSeconds)
	{
		LoadContent();
		this.message = message;
		stopWatch.Start();
		durationInSeconds = durationSeconds;
		isActive = false;
		isQueuedUp = true;
	}

	public MessageBoxScreen(Texture2D tex)
	{
		LoadContent();
		message = "";
		isActive = true;
		texture = tex;
	}

	public MessageBoxScreen(string msg, Texture2D tex)
	{
		LoadContent();
		message = msg;
		isActive = true;
		texture = tex;
	}

	public MessageBoxScreen(Texture2D tex, float durationSeconds)
	{
		LoadContent();
		message = "";
		stopWatch.Start();
		durationInSeconds = durationSeconds;
		isActive = false;
		isQueuedUp = true;
		texture = tex;
	}

	public MessageBoxScreen(Texture2D tex, float durationSeconds, bool isQTEa, bool isCutScenea, string sceneNamea)
	{
		LoadContent();
		isCutScene = isCutScenea;
		isQTE = isQTEa;
		message = "";
		stopWatch.Start();
		durationInSeconds = durationSeconds;
		isActive = false;
		isQueuedUp = true;
		sceneName = sceneNamea;
		texture = tex;
	}

	public void startTimer()
	{
		stopWatch.Start();
	}

	public MessageBoxScreen(string message, bool pauseMenu)
		: this(message)
	{
		this.pauseMenu = pauseMenu;
	}

	public void LoadContent()
	{
		smallFont = GraphicsManager.font;
	}

	public void HandleInput()
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

	public void HandleTouches()
	{
	}

	public void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput == null || isQTE || (durationInSeconds > 0f && RandomStaticGlobals.GameMode == Definitions.GameMode.cutsceneORqte) || (durationInSeconds > 0f && !isSkippable))
		{
			return;
		}
		if (isCutScene)
		{
			if (anywhereInput.B_pressed && (isSkippable || isBisYourOnlyEscape))
			{
				if (!isBisYourOnlyEscape)
				{
					if (sceneName != null && sceneName != "")
					{
						TriggerManager.SetTriggerEvent(sceneName + "SCENECANCELLED");
					}
					TriggerManager.SetTriggerEvent("SCENECANCELLED");
				}
				ExitScreen(cancelled: true);
			}
			if (anywhereInput.A_pressed && !isBisYourOnlyEscape)
			{
				if (sceneName != null && sceneName != "")
				{
					TriggerManager.SetTriggerEvent(sceneName + "SCENEOVER");
				}
				ExitScreen();
			}
		}
		else if (anywhereInput.B_pressed && isSkippable)
		{
			if (Cancelled != null)
			{
				Cancelled(this, EventArgs.Empty);
			}
			ExitScreen(cancelled: true);
		}
	}

	public void ExitScreen()
	{
		ExitScreen(cancelled: false);
	}

	public void ExitScreen(bool cancelled)
	{
		if (!cancelled && sceneName != null && sceneName != "")
		{
			TriggerManager.SetTriggerEvent(sceneName + "SCENEOVER");
		}
		RandomStaticGlobals.isShowingCutScene = false;
		SoundManager.PlayMenuClick();
		isActive = false;
		ScreenManager.hideMessageBox();
		if (GraphicsManager.messages.Count == 1 && RandomStaticGlobals.isGamePaused)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
	}

	public void DrawForFlappy()
	{
		if (durationInSeconds > 0f && (float)stopWatch.ElapsedMilliseconds > durationInSeconds * 1000f)
		{
			ExitScreen();
		}
		if (message != "" && message != null)
		{
			Vector2 screenFullSize = GraphicsManager.ScreenFullSize;
			Vector2 vector = GraphicsManager.font.MeasureString(message);
			Vector2 vector2 = (screenFullSize - vector) / 2f;
			Rectangle rectangle = new Rectangle((int)vector2.X, (int)vector2.Y, (int)vector.X, (int)((float)GraphicsManager.font.LineSpacing * 1.1f + vector.Y));
			rectangle.X -= (int)(0.1f * (float)rectangle.Width);
			rectangle.Y -= (int)(0.1f * (float)rectangle.Height);
			rectangle.Width += (int)(0.2f * (float)rectangle.Width);
			rectangle.Height += (int)(0.2f * (float)rectangle.Height);
			GraphicsManager.DrawString((int)vector2.X + 150, (int)vector2.Y, message, Color.White, GraphicsManager.font);
		}
		if (texture != null)
		{
			GraphicsManager.Draw(texture, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthFifthHighest);
		}
		GraphicsManager.Draw(GraphicsManager.imgNiceBackground, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
	}

	public void Draw()
	{
		DrawForFlappy();
	}
}
