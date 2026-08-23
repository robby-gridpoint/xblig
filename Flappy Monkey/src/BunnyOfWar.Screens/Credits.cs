using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar.Screens;

public class Credits
{
	public Texture2D background => GraphicsManager.LoadTexture("screens/credits.png", cacheResult: true);

	public Credits()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void Draw()
	{
		GraphicsManager.DrawTexture(background, new Rectangle(0, 0, 1920, 1080), Color.White);
	}

	public void ProcessUnsignedInput()
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

	public void ProcessInput()
	{
		ProcessUnsignedInput();
	}

	private void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput.B_pressed || anywhereInput.A_pressed)
		{
			exit();
		}
	}

	private void exit()
	{
		SoundManager.PlayMenuClick();
		ScreenManager.ShowMainMenu();
	}

	public void Load(ContentManager Content)
	{
		Texture2D texture2D = background;
	}

	public void Clear()
	{
		background.Dispose();
	}
}
