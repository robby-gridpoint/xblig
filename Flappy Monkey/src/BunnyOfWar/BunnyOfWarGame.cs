using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public class BunnyOfWarGame : Game
{
	public BunnyOfWarGame()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		try
		{
			GraphicsManager.graphics = new GraphicsDeviceManager(this);
			GraphicsManager.InitGraphicsStuff();
			base.Components.Add((IGameComponent)new GamerServicesComponent((Game)this));
		}
		catch (Exception ex)
		{
			robbyPort.LogException("Game construction", ex);
		}
	}

	protected override void Initialize()
	{
		try
		{
			base.Initialize();
		}
		catch (Exception ex)
		{
			robbyPort.LogException("Game initialization", ex);
		}
	}

	protected override void LoadContent()
	{
		try
		{
			base.Content.RootDirectory = Definitions.ContentRootDirectory;
			RandomStaticGlobals.Content = new ContentManager(base.Services);
			RandomStaticGlobals.Content.RootDirectory = Definitions.ContentRootDirectory;
			RandomStaticGlobals.ContentTemporary = new ContentManager(base.Services);
			RandomStaticGlobals.ContentTemporary.RootDirectory = Definitions.ContentRootDirectory;
			GraphicsManager.spriteBatch = new SpriteBatch(base.GraphicsDevice);
			GraphicsManager.LoadContent(base.Content);
			ScreenManager.ShowMainMenu();
			LevelManager.init(base.Content, GraphicsManager.viewportRect);
			FileManager.Select360StorageDevice();
			FileManager.LoadHighScores();
			LevelManager.LoadPreloadData();
			SoundManager.LoadContent(base.Content);
		}
		catch (Exception ex)
		{
			robbyPort.LogException("Content loading", ex);
		}
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		try
		{
			if (ScreenManager.CurrentScreen == ScreenManager.screens.Blank)
			{
				if (RandomStaticGlobals.UpdateAfterThisTime <= DateTime.Now)
				{
					RandomStaticGlobals.UpdateAfterThisTime = DateTime.Now.AddMilliseconds(1000 / Definitions.UpdatesPerSecond);
					List<FighterObject> computerPlayers = FighterManager.getComputerPlayers(onlyLiving: true, canBeDying: false);
					foreach (FighterObject humanPlayer in FighterManager.humanPlayers)
					{
						if (humanPlayer.PROPERTIES.isLocal)
						{
							if (RandomStaticGlobals.GameMode == Definitions.GameMode.none || RandomStaticGlobals.GameMode == Definitions.GameMode.brawler)
							{
								RandomStaticGlobals.InputManagerInstance.processBrawlerInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.runner)
							{
								RandomStaticGlobals.InputManagerInstance.processRunnerInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.swimmer)
							{
								RandomStaticGlobals.InputManagerInstance.processSwimmerInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.redbaron)
							{
								RandomStaticGlobals.InputManagerInstance.processRedBaronInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.cutsceneORqte)
							{
								RandomStaticGlobals.InputManagerInstance.processCutsceneOrQTEInput(humanPlayer);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.helicopter)
							{
								RandomStaticGlobals.InputManagerInstance.processHelicopterInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.flappy || RandomStaticGlobals.GameMode == Definitions.GameMode.flappychase)
							{
								RandomStaticGlobals.InputManagerInstance.processFlappyInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.shooter)
							{
								RandomStaticGlobals.InputManagerInstance.processShooterInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.space)
							{
								RandomStaticGlobals.InputManagerInstance.processSpaceInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.gunsmoke)
							{
								RandomStaticGlobals.InputManagerInstance.processGunSmokeInput(humanPlayer, computerPlayers);
							}
						}
					}
					TriggerManager.checkTimerTriggers();
					if (RandomStaticGlobals.GameMode != Definitions.GameMode.cutsceneORqte && !QuickTimeEventsManager.hasQTE && (GraphicsManager.messages == null || GraphicsManager.messages.Count == 0))
					{
						ProjectileManager.ProcessProjectiles();
						FighterManager.ProcessFighters();
						ObstacleManager.ProcessObstacles();
					}
					if (RandomStaticGlobals.isPvPEnabled && FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: false).Count <= 1)
					{
						ScreenManager.GameOver();
					}
				}
			}
			else if (RandomStaticGlobals.UpdateAfterThisTime < DateTime.Now)
			{
				RandomStaticGlobals.UpdateAfterThisTime = DateTime.Now.AddMilliseconds(1000 / Definitions.UpdatesPerSecond);
				ScreenManager.UpdateAndProcessInput();
			}
			base.Update(gameTime);
		}
		catch (Exception ex)
		{
			robbyPort.LogException("Game update", ex);
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.cutsceneORqte)
		{
			base.GraphicsDevice.Clear(Color.Transparent);
			GraphicsManager.DrawCutscene(gameTime);
		}
		else
		{
			base.GraphicsDevice.Clear(Color.CornflowerBlue);
			GraphicsManager.Draw(gameTime);
		}
		base.Draw(gameTime);
	}
}
