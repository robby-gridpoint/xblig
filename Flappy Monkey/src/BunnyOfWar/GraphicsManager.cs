using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public static class GraphicsManager
{
	public static GraphicsDeviceManager graphics;

	public static Matrix matrix = Matrix.CreateScale(1f);

	public static Texture2D imgNiceBackground;

	public static Texture2D imgCursor;

	public static SpriteBatch spriteBatch;

	public static Rectangle viewableArea;

	public static Rectangle BoundariesDefault = new Rectangle(0, 0, 1100000, 150000);

	public static Texture2D imgBlack;

	public static Rectangle viewportRect;

	public static Texture2D blankTexture;

	public static SpriteFont font;

	public static SpriteFont fontBig;

	public static SpriteFont fontSmall;

	public static SpriteFont fontMedium;

	public static int ScreenWidth = 240;

	public static int ScreenHeight = 240;

	public static Vector2 ScreenFullSize = new Vector2(1920f, 1080f);

	public static Vector2 TitleSafeTopLeft = new Vector2(200f, 125f);

	public static Texture2D imgButtonA;

	public static Texture2D imgButtonB;

	public static Texture2D imgButtonX;

	public static Texture2D imgButtonY;

	public static Texture2D imgCounterAttack;

	public static Texture2D imgButtonRSLeft;

	public static Texture2D imgButtonRSRight;

	public static Texture2D imgButtonRSDown;

	public static Texture2D imgButtonRSUp;

	public static Texture2D imgButtonKBW;

	public static Texture2D imgButtonKBA;

	public static Texture2D imgButtonKBS;

	public static Texture2D imgButtonKBD;

	public static Texture2D imgBuyMeScreen;

	public static Texture2D imgSkull;

	public static Texture2D imgSlingshot;

	public static Texture2D imgSlingshotRopeIdle;

	public static Texture2D imgSlingshotRopeSlanted;

	public static Texture2D imgSlingshotRopeSlanted2;

	public static List<Texture2D> textureCache = new List<Texture2D>(1000);

	public static List<string> textureCachePaths = new List<string>(1000);

	private static List<string> overlays = new List<string>();

	public static List<Animation> animatedBloodSplatterList = new List<Animation>(0);

	public static List<Texture2D> bloodStainList = new List<Texture2D>(0);

	public static List<Animation> animatedBloodSplatterListGREEN = new List<Animation>(0);

	public static List<Texture2D> bloodStainListGREEN = new List<Texture2D>(0);

	public static Color TheColorTransparentGray = new Color(Color.Gray.R, Color.Gray.G, Color.Gray.B, 128);

	public static Color TheColorTransparentRed = new Color(Color.Red.R, Color.Red.G, Color.Red.B, 128);

	public static bool isDrawingEnemiesAsGhosts = false;

	public static List<MessageBoxScreen> messages = new List<MessageBoxScreen>(0);

	private static bool isInLetterbox = false;

	private static DateTime letterBoxExpires = DateTime.MinValue;

	public static bool Is512Mb
	{
		get
		{
			try
			{
				return true;
			}
			catch (ArgumentOutOfRangeException)
			{
				return true;
			}
		}
	}

	public static bool IsInLetterBox
	{
		get
		{
			if (isInLetterbox && DateTime.Now > letterBoxExpires)
			{
				isInLetterbox = false;
			}
			return isInLetterbox;
		}
		set
		{
			isInLetterbox = value;
		}
	}

	public static void InitGraphicsStuff()
	{
		ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
		ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
		matrix = Matrix.CreateScale((float)ScreenWidth / 1920f, (float)ScreenHeight / 1080f, 1f);
		graphics.PreferredBackBufferWidth = ScreenWidth;
		graphics.PreferredBackBufferHeight = ScreenHeight;
		graphics.IsFullScreen = true;
	}

	public static void ToggleOverlay(string name)
	{
		string text = "";
		if (name == null || name == "")
		{
			text = "/buttons/button_x";
			if (overlays.Contains(text))
			{
				overlays.Remove(text);
			}
			else
			{
				overlays.Add(text);
			}
		}
		if (name == null)
		{
			return;
		}
		if (name == "Move")
		{
			text = "/screens/OverlayMove";
			if (overlays.Contains(text))
			{
				overlays.Remove(text);
			}
			else
			{
				overlays.Add(text);
			}
		}
		if (name == "Jab")
		{
			text = "/screens/OverlayJab";
			if (overlays.Contains(text))
			{
				overlays.Remove(text);
			}
			else
			{
				overlays.Add(text);
			}
		}
		if (name == "Chop")
		{
			text = "/screens/OverlayChop";
			if (overlays.Contains(text))
			{
				overlays.Remove(text);
			}
			else
			{
				overlays.Add(text);
			}
		}
		if (name == "Block")
		{
			text = "/screens/OverlayBlock";
			if (overlays.Contains(text))
			{
				overlays.Remove(text);
			}
			else
			{
				overlays.Add(text);
			}
		}
	}

	public static Texture2D GetTextureFromCache(string path)
	{
		if (textureCachePaths.Contains(path))
		{
			int index = textureCachePaths.IndexOf(path);
			if (textureCache[index] == null || textureCache[index].IsDisposed)
			{
				return imgButtonX;
			}
			return textureCache[textureCachePaths.IndexOf(path)];
		}
		return LoadTexture(path, cacheResult: true);
	}

	public static Texture2D LoadTexture(string path)
	{
		return LoadTexture(path, cacheResult: true);
	}

	public static Texture2D LoadTexture(string path, bool cacheResult)
	{
		path = path.Replace(".png", "");
		path = path.Replace(".jpg", "");
		path = path.Replace(".xnb", "");
		path = path.Replace(Definitions.ContentRootDirectory + "/", "");
		path = path.Replace(Definitions.ContentRootDirectory + "\\", "");
		string text = path;
		text = text.Replace(".jpg", "").Replace(".png", "");
		if (textureCachePaths.Contains(text))
		{
			if (textureCache[textureCachePaths.IndexOf(text)] != null && !textureCache[textureCachePaths.IndexOf(text)].IsDisposed)
			{
				return textureCache[textureCachePaths.IndexOf(text)];
			}
			textureCache.RemoveAt(textureCachePaths.IndexOf(text));
			textureCachePaths.Remove(text);
		}
		Texture2D texture2D = null;
		try
		{
			texture2D = ((!Definitions.IsContentPermanent(path)) ? RandomStaticGlobals.ContentTemporary.Load<Texture2D>(path) : RandomStaticGlobals.Content.Load<Texture2D>(path));
		}
		catch (Exception)
		{
			long num = 0L;
			string text2 = path + "sdfsdfs";
		}
		if (cacheResult)
		{
			textureCachePaths.Add(text);
			textureCache.Add(texture2D);
		}
		return texture2D;
	}

	public static SpriteFont LoadFont(string path)
	{
		return RandomStaticGlobals.Content.Load<SpriteFont>(path);
	}

	public static void LoadContent(ContentManager Content)
	{
		viewportRect = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
		viewableArea = viewportRect;
		fontSmall = LoadFont("fonts/SmallFont");
		font = LoadFont("fonts/GameFont");
		fontBig = LoadFont("fonts/PausedFont");
		fontMedium = LoadFont("fonts/MediumFont");
		DrawLoadingScreen();
		imgButtonA = LoadTexture("buttons/button_a.png", cacheResult: false);
		imgButtonB = LoadTexture("buttons/button_b.png", cacheResult: false);
		imgButtonX = LoadTexture("buttons/button_x.png", cacheResult: false);
		imgButtonY = LoadTexture("buttons/button_y.png", cacheResult: false);
		imgButtonKBW = LoadTexture("buttons/W.png", cacheResult: false);
		imgButtonKBA = LoadTexture("buttons/A.png", cacheResult: false);
		imgButtonKBS = LoadTexture("buttons/S.png", cacheResult: false);
		imgButtonKBD = LoadTexture("buttons/D.png", cacheResult: false);
		imgNiceBackground = LoadTexture("screens/NiceBackground.png", cacheResult: false);
		imgCursor = LoadTexture("screens/cursor", cacheResult: false);
		imgBlack = LoadTexture("colors/black.png", cacheResult: false);
	}

	public static void DrawLoadingScreen()
	{
	}

	public static void DrawLoading(Texture2D tex, Rectangle r, Texture2D background)
	{
	}

	public static void DrawCutscene(GameTime gameTime)
	{
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, matrix);
		bool flag = false;
		if (messages != null && messages.Count > 0)
		{
			flag = DrawMessages();
			QuickTimeEventsManager.Draw();
		}
		if (ScreenManager.CurrentScreen != ScreenManager.screens.Blank)
		{
			ScreenManager.Draw(gameTime);
		}
		StopDrawing();
	}

	public static void Draw(GameTime gameTime)
	{
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, matrix);
		if (ScreenManager.CurrentScreen != ScreenManager.screens.Blank)
		{
			ScreenManager.Draw(gameTime);
		}
		bool flag = false;
		if (messages != null && messages.Count > 0)
		{
			DrawMessages();
			QuickTimeEventsManager.Draw();
			flag = true;
		}
		if (ScreenManager.CurrentScreen == ScreenManager.screens.Blank && !RandomStaticGlobals.isGamePaused && !flag)
		{
			DrawHealth();
		}
		if ((!flag && (ScreenManager.CurrentScreen == ScreenManager.screens.Blank || ScreenManager.CurrentScreen == ScreenManager.screens.PauseMenu)) || ScreenManager.isShowingPlayerFailedScreen || ScreenManager.isShowingLevelInBackground)
		{
			if (RandomStaticGlobals.CameraRollVelocity != Vector2.Zero && !RandomStaticGlobals.isGamePaused)
			{
				if (RandomStaticGlobals.CameraRollVelocity.X != 0f)
				{
					viewableArea.X += (int)(RandomStaticGlobals.CameraRollVelocity.X / (float)Definitions.UpdatesPerSecond);
				}
				if (RandomStaticGlobals.CameraRollVelocity.Y != 0f)
				{
					viewableArea.Y += (int)(RandomStaticGlobals.CameraRollVelocity.Y / (float)Definitions.UpdatesPerSecond);
				}
			}
			else
			{
				FighterObject fighterObject = null;
				if (FighterManager.humanPlayers != null && FighterManager.humanPlayers.Count > 0)
				{
					fighterObject = FighterManager.humanPlayers[0];
				}
				if (!RandomStaticGlobals.isGamePaused && fighterObject != null)
				{
					FighterManager.adjustScreenViewableAreaForThisPlayer(fighterObject, ref viewableArea);
				}
			}
			List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: true);
			if (!isDrawingEnemiesAsGhosts)
			{
				List<FighterObject> computerPlayers = FighterManager.getComputerPlayers(onlyLiving: false, canBeDying: true);
				foreach (FighterObject item in computerPlayers)
				{
					item.Draw(gameTime, spriteBatch, viewableArea);
				}
			}
			if (!RandomStaticGlobals.isSkullSlingshotMode)
			{
				foreach (FighterObject item2 in humanPlayers)
				{
					item2.Draw(gameTime, spriteBatch, viewableArea);
				}
			}
			if ((RandomStaticGlobals.isPvPEnabled || FighterManager.humanPlayers.Count > 1) && !RandomStaticGlobals.isGamePaused)
			{
				DrawPlayerNames();
			}
			DrawHighscores(RandomStaticGlobals.ScoreCurrent.ToString(), RandomStaticGlobals.ScoreAllTimeHigh.ToString());
			SceneryManager.DrawScenerySkyAndGround();
			ObstacleManager.DrawObstacles();
			SceneryManager.DrawSceneryFloaters();
			ProjectileManager.DrawProjectiles();
			QuickTimeEventsManager.Draw();
			if (IsInLetterBox)
			{
				Draw(imgBlack, new Rectangle(0, 0, 1920, 200), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				Draw(imgBlack, new Rectangle(0, 880, 1920, 200), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			}
			if (RandomStaticGlobals.isSkullSlingshotMode && viewableArea.X == 0 && ScreenManager.CurrentScreen == ScreenManager.screens.Blank)
			{
				int num = -1;
				foreach (FighterObject item3 in humanPlayers)
				{
					num++;
					Draw(imgSkull, new Rectangle((int)RandomStaticGlobals.SkullSlingshotCurrentPosition[num].X, (int)RandomStaticGlobals.SkullSlingshotCurrentPosition[num].Y, Definitions.SkullSlingshotSize, Definitions.SkullSlingshotSize), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
					Draw(imgSlingshot, new Rectangle(item3.X, item3.Y, 100, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
					if (RandomStaticGlobals.SkullSlingshotCurrentPosition[num] == RandomStaticGlobals.SkullSlingshotOrigin[num])
					{
						Draw(imgSlingshotRopeIdle, new Rectangle(item3.X, item3.Y, 100, 100), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
						continue;
					}
					float rotation = (float)Math.Atan2(RandomStaticGlobals.SkullSlingshotCurrentPosition[num].Y - (float)item3.Y, RandomStaticGlobals.SkullSlingshotCurrentPosition[num].X - (float)item3.X);
					int num2 = (int)RandomStaticGlobals.makePositive(RandomStaticGlobals.SkullSlingshotCurrentPosition[num].X - (float)item3.X);
					int num3 = (int)RandomStaticGlobals.makePositive(RandomStaticGlobals.SkullSlingshotCurrentPosition[num].Y - (float)item3.Y);
					double d = num2 * num2 + num3 * num3;
					d = Math.Sqrt(d);
					Draw(imgSlingshotRopeSlanted, new Rectangle(item3.X + 25, item3.Y + 25, (int)d, 15), null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthFifthHighest);
					num2 = (int)RandomStaticGlobals.makePositive(RandomStaticGlobals.SkullSlingshotCurrentPosition[num].X - (float)item3.X - 80f);
					d = num2 * num2 + num3 * num3;
					d = Math.Sqrt(d);
					rotation = (float)Math.Atan2(RandomStaticGlobals.SkullSlingshotCurrentPosition[num].Y - (float)item3.Y, RandomStaticGlobals.SkullSlingshotCurrentPosition[num].X - (float)item3.X - 80f);
					Draw(imgSlingshotRopeSlanted2, new Rectangle(item3.X + 80, item3.Y + 20, (int)d, 15), null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthFifthHighest);
				}
			}
			if (overlays != null && overlays.Count > 0 && messages.Count == 0 && !RandomStaticGlobals.isGamePaused)
			{
				for (int i = 0; i < overlays.Count; i++)
				{
					Draw(GetTextureFromCache(Definitions.ContentRootDirectory + overlays[i]), new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				}
			}
			StopDrawing();
			if (!isDrawingEnemiesAsGhosts)
			{
				return;
			}
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, null, null, null, null, matrix);
			List<FighterObject> computerPlayers2 = FighterManager.getComputerPlayers(onlyLiving: true, canBeDying: true);
			foreach (FighterObject item4 in computerPlayers2)
			{
				item4.Draw(gameTime, spriteBatch, viewableArea);
			}
			spriteBatch.End();
		}
		else
		{
			StopDrawing();
		}
	}

	public static void ClearOverlays()
	{
		if (overlays != null && overlays.Count > 0)
		{
			overlays.Clear();
		}
	}

	public static void ClearMessages()
	{
		if (messages != null && messages.Count > 0)
		{
			messages.Clear();
		}
		QuickTimeEventsManager.hasQTE = false;
	}

	public static void StopDrawing()
	{
		spriteBatch.End();
	}

	public static void DrawHighscores(string currentScore, string alltimeHighScore)
	{
		spriteBatch.DrawString(fontMedium, "Score: " + currentScore, new Vector2(250f, 115f), Color.SaddleBrown, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		spriteBatch.DrawString(fontMedium, "High Score: " + alltimeHighScore, new Vector2(1100f, 115f), Color.SaddleBrown, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
	}

	public static void DrawHighScoresForMainScreen(string currentScore, string alltimeHighScore)
	{
		spriteBatch.DrawString(fontMedium, "Latest Score: " + currentScore, new Vector2(260f, 715f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		if (!RandomStaticGlobals.IsTrial())
		{
			spriteBatch.DrawString(fontMedium, "High Score: " + alltimeHighScore, new Vector2(1100f, 815f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}
	}

	public static void DrawPlayerNames()
	{
		if (FighterManager.humanPlayers.Count > 0 && FighterManager.humanPlayers[0] != null)
		{
			if (!RandomStaticGlobals.isSkullSlingshotMode)
			{
				if (FighterManager.humanPlayers[0].PROPERTIES.gamerTagX == 0)
				{
					FighterManager.humanPlayers[0].PROPERTIES.gamerTagX = FighterManager.humanPlayers[0].width / 2 - (int)font.MeasureString(FighterManager.humanPlayers[0].PROPERTIES.GamerTag).X / 2;
				}
				spriteBatch.DrawString(font, FighterManager.humanPlayers[0].PROPERTIES.GamerTag, getAdjustedVector2(FighterManager.humanPlayers[0].X + FighterManager.humanPlayers[0].PROPERTIES.gamerTagX, FighterManager.humanPlayers[0].Y + FighterManager.humanPlayers[0].height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			else
			{
				spriteBatch.DrawString(font, FighterManager.humanPlayers[0].PROPERTIES.GamerTag, getAdjustedVector2(FighterManager.humanPlayers[0].X + 140, FighterManager.humanPlayers[0].Y + 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
		}
		if (FighterManager.humanPlayers.Count > 1 && FighterManager.humanPlayers[1] != null)
		{
			if (!RandomStaticGlobals.isSkullSlingshotMode)
			{
				if (FighterManager.humanPlayers[1].PROPERTIES.gamerTagX == 0)
				{
					FighterManager.humanPlayers[1].PROPERTIES.gamerTagX = FighterManager.humanPlayers[1].width / 2 - (int)font.MeasureString(FighterManager.humanPlayers[1].PROPERTIES.GamerTag).X / 2;
				}
				spriteBatch.DrawString(font, FighterManager.humanPlayers[1].PROPERTIES.GamerTag, getAdjustedVector2(FighterManager.humanPlayers[1].X + FighterManager.humanPlayers[1].PROPERTIES.gamerTagX, FighterManager.humanPlayers[1].Y + FighterManager.humanPlayers[1].height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			else
			{
				spriteBatch.DrawString(font, FighterManager.humanPlayers[1].PROPERTIES.GamerTag, getAdjustedVector2(FighterManager.humanPlayers[1].X + 140, FighterManager.humanPlayers[1].Y + 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
		}
		if (FighterManager.humanPlayers.Count > 2 && FighterManager.humanPlayers[2] != null)
		{
			if (!RandomStaticGlobals.isSkullSlingshotMode)
			{
				if (FighterManager.humanPlayers[2].PROPERTIES.gamerTagX == 0)
				{
					FighterManager.humanPlayers[2].PROPERTIES.gamerTagX = FighterManager.humanPlayers[2].width / 2 - (int)font.MeasureString(FighterManager.humanPlayers[2].PROPERTIES.GamerTag).X / 2;
				}
				spriteBatch.DrawString(font, FighterManager.humanPlayers[2].PROPERTIES.GamerTag, getAdjustedVector2(FighterManager.humanPlayers[2].X + FighterManager.humanPlayers[2].PROPERTIES.gamerTagX, FighterManager.humanPlayers[2].Y + FighterManager.humanPlayers[2].height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			else
			{
				spriteBatch.DrawString(font, FighterManager.humanPlayers[2].PROPERTIES.GamerTag, getAdjustedVector2(FighterManager.humanPlayers[2].X + 140, FighterManager.humanPlayers[2].Y + 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
		}
		if (FighterManager.humanPlayers.Count <= 3 || FighterManager.humanPlayers[3] == null)
		{
			return;
		}
		if (!RandomStaticGlobals.isSkullSlingshotMode)
		{
			if (FighterManager.humanPlayers[3].PROPERTIES.gamerTagX == 0)
			{
				FighterManager.humanPlayers[3].PROPERTIES.gamerTagX = FighterManager.humanPlayers[3].width / 2 - (int)font.MeasureString(FighterManager.humanPlayers[3].PROPERTIES.GamerTag).X / 2;
			}
			spriteBatch.DrawString(font, FighterManager.humanPlayers[3].PROPERTIES.GamerTag, getAdjustedVector2(FighterManager.humanPlayers[3].X + FighterManager.humanPlayers[3].PROPERTIES.gamerTagX, FighterManager.humanPlayers[3].Y + FighterManager.humanPlayers[3].height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}
		else
		{
			spriteBatch.DrawString(font, FighterManager.humanPlayers[3].PROPERTIES.GamerTag, getAdjustedVector2(FighterManager.humanPlayers[3].X + 140, FighterManager.humanPlayers[3].Y + 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}
	}

	public static Rectangle getAdjustedRectangleForSlowScroller(Rectangle rect, int ratio)
	{
		if (ratio == 0)
		{
			return rect;
		}
		rect.X -= viewableArea.X / ratio;
		return rect;
	}

	public static Rectangle getAdjustedRectangleForSlowScrollerLENGTH(Rectangle rect, int scrollLength)
	{
		if (scrollLength == 0)
		{
			return rect;
		}
		double num = viewableArea.X - rect.X;
		num /= (double)scrollLength;
		if (num > 0.0 && num <= 1.0)
		{
			rect.X += (int)((double)scrollLength * num);
		}
		return new Rectangle(rect.X - viewableArea.X, rect.Y - viewableArea.Y, rect.Width, rect.Height);
	}

	public static Rectangle getAdjustedRectangle(Rectangle rect)
	{
		return new Rectangle(rect.X - viewableArea.X, rect.Y - viewableArea.Y, rect.Width, rect.Height);
	}

	public static Vector2 getAdjustedVector2(Rectangle rect)
	{
		return new Vector2(rect.X - viewableArea.X, rect.Y - viewableArea.Y);
	}

	public static Vector2 getAdjustedVector2(Vector2 rect)
	{
		return new Vector2(rect.X - (float)viewableArea.X, rect.Y - (float)viewableArea.Y);
	}

	public static Vector2 getAdjustedVector2(float x, float y)
	{
		return new Vector2(x - (float)viewableArea.X, y - (float)viewableArea.Y);
	}

	public static void Message(string msg)
	{
		messages.Add(new MessageBoxScreen(msg));
	}

	public static void Message(string msg, int duration_in_seconds, int iconID)
	{
		messages.Add(new MessageBoxScreen(msg, duration_in_seconds));
	}

	public static void Message(Texture2D tex)
	{
		messages.Add(new MessageBoxScreen(tex));
	}

	public static void Message(Texture2D tex, float duration_in_seconds)
	{
		messages.Add(new MessageBoxScreen(tex, duration_in_seconds));
	}

	public static void Message(Texture2D tex, float duration_in_seconds, bool isQTE, bool isCutScene, string sceneName)
	{
		messages.Add(new MessageBoxScreen(tex, duration_in_seconds, isQTE, isCutScene, sceneName));
	}

	public static void Message(string msg, Texture2D tex)
	{
		messages.Add(new MessageBoxScreen(msg, tex));
	}

	public static void ShowCutSceneBtoContinue(string sceneName, int durationMS, bool isSkippable)
	{
		RandomStaticGlobals.isShowingCutScene = true;
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen(LoadTexture("CutScenes/" + sceneName), (float)durationMS / 1000f, isQTEa: false, isCutScenea: true, sceneName);
		messageBoxScreen.isSkippable = isSkippable;
		messageBoxScreen.isBisYourOnlyEscape = true;
		messages.Add(messageBoxScreen);
		messages.RemoveRange(0, messages.Count - 1);
	}

	public static void ShowCutScene(string sceneName, int durationMS, bool isSkippable)
	{
		RandomStaticGlobals.isShowingCutScene = true;
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen(LoadTexture("CutScenes/" + sceneName), (float)durationMS / 1000f, isQTEa: false, isCutScenea: true, sceneName);
		messageBoxScreen.isSkippable = isSkippable;
		messages.Add(messageBoxScreen);
		messages.RemoveRange(0, messages.Count - 1);
	}

	public static void ShowQuickTimeEvent(string sceneName, int durationMS)
	{
		RandomStaticGlobals.isShowingCutScene = true;
		Message(LoadTexture("CutScenes/" + sceneName), (float)durationMS / 1000f, isQTE: true, isCutScene: true, sceneName);
		messages.RemoveRange(0, messages.Count - 1);
	}

	public static bool DrawMessages()
	{
		bool flag = false;
		for (int i = 0; i < messages.Count; i++)
		{
			if (messages[i].isActive && !flag)
			{
				messages[i].Draw();
				if (messages[i].isQTE)
				{
				}
				flag = true;
			}
			if (!messages[i].isQueuedUp && !messages[i].isActive)
			{
				messages.Remove(messages[i]);
			}
		}
		if (!flag && messages.Count > 0)
		{
			for (int i = 0; i < messages.Count; i++)
			{
				if (!messages[i].isActive)
				{
					messages[i].isActive = true;
					messages[i].isQueuedUp = false;
					messages[i].startTimer();
					messages[i].Draw();
					return flag;
				}
			}
		}
		return flag;
	}

	public static void DrawRectangle(Rectangle rectangle, Color color)
	{
		DrawRectangle(rectangle, color, Definitions.LayerDepthFourthHighest);
	}

	public static void DrawRectangle(Rectangle rectangle, Color color, float layerDepth)
	{
		if (blankTexture == null)
		{
			blankTexture = LoadTexture("colors/gradient.png", cacheResult: false);
		}
		Draw(blankTexture, rectangle, null, color, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
	}

	public static void DrawString(int x, int y, string s)
	{
		try
		{
			DrawString(x, y, s, Color.Blue, fontBig);
		}
		catch (Exception)
		{
		}
	}

	public static void DrawStringCentered(int x, int y, string s, Color c, SpriteFont f)
	{
		Vector2 vector = f.MeasureString(s);
		x -= (int)vector.X / 2;
		y -= (int)vector.Y / 2;
		DrawString(x, y, s, c, f);
	}

	public static void DrawString(int x, int y, string s, Color c, SpriteFont f)
	{
		DrawString(x, y, s, c, f, noWrap: false);
	}

	public static void DrawString(int x, int y, string s, Color c, SpriteFont f, bool noWrap)
	{
		int num = 150;
		int num2 = 1500;
		if (f == null)
		{
			if (font == null)
			{
				return;
			}
			f = font;
		}
		if (f.MeasureString(s).X > (float)num2)
		{
			x = num;
			string[] array = s.Split(' ');
			string text = "";
			int num3 = (int)f.MeasureString(s).Y;
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + " ";
				if (f.MeasureString(text).X > (float)(num2 - num))
				{
					spriteBatch.DrawString(f, text, new Vector2(x, y), c, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
					text = "";
					y += num3;
				}
			}
			spriteBatch.DrawString(f, text, new Vector2(x, y), c, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}
		else
		{
			spriteBatch.DrawString(f, s, new Vector2(x, y), c, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}
	}

	public static void DrawHealth()
	{
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.flappy || RandomStaticGlobals.GameMode == Definitions.GameMode.runner)
		{
			return;
		}
		Vector2 titleSafeTopLeft = TitleSafeTopLeft;
		titleSafeTopLeft.X += 30f;
		titleSafeTopLeft.Y += 30f;
		if (FighterManager.humanPlayers.Count > 0 && FighterManager.humanPlayers[0] != null)
		{
			if (FighterManager.humanPlayers.Count > 1)
			{
				spriteBatch.DrawString(font, FighterManager.humanPlayers[0].PROPERTIES.GamerTag, new Vector2(titleSafeTopLeft.X + 20f, titleSafeTopLeft.Y + 5f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			DrawRectangle(new Rectangle((int)titleSafeTopLeft.X + 5, (int)titleSafeTopLeft.Y + 5, (int)FighterManager.humanPlayers[0].PROPERTIES.health * 5, 30), Color.DarkRed, Definitions.LayerDepthThirdHighest);
			DrawRectangle(new Rectangle((int)titleSafeTopLeft.X, (int)titleSafeTopLeft.Y, (int)FighterManager.humanPlayers[0].PROPERTIES.healthMax * 5 + 10, 40), TheColorTransparentGray, Definitions.LayerDepthFourthHighest);
			titleSafeTopLeft.Y += 50f;
		}
		if (FighterManager.humanPlayers.Count > 1 && FighterManager.humanPlayers[1] != null)
		{
			if (FighterManager.humanPlayers.Count > 1)
			{
				spriteBatch.DrawString(font, FighterManager.humanPlayers[1].PROPERTIES.GamerTag, new Vector2(titleSafeTopLeft.X + 20f, titleSafeTopLeft.Y + 5f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			DrawRectangle(new Rectangle((int)titleSafeTopLeft.X + 5, (int)titleSafeTopLeft.Y + 5, (int)FighterManager.humanPlayers[1].PROPERTIES.health * 5, 30), Color.DarkRed, Definitions.LayerDepthThirdHighest);
			DrawRectangle(new Rectangle((int)titleSafeTopLeft.X, (int)titleSafeTopLeft.Y, (int)FighterManager.humanPlayers[1].PROPERTIES.healthMax * 5 + 10, 40), TheColorTransparentGray, Definitions.LayerDepthFourthHighest);
			titleSafeTopLeft.Y += 50f;
		}
		if (FighterManager.humanPlayers.Count > 2 && FighterManager.humanPlayers[2] != null)
		{
			if (FighterManager.humanPlayers.Count > 1)
			{
				spriteBatch.DrawString(font, FighterManager.humanPlayers[2].PROPERTIES.GamerTag, new Vector2(titleSafeTopLeft.X + 20f, titleSafeTopLeft.Y + 5f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			DrawRectangle(new Rectangle((int)titleSafeTopLeft.X + 5, (int)titleSafeTopLeft.Y + 5, (int)FighterManager.humanPlayers[2].PROPERTIES.health * 5, 30), Color.DarkRed, Definitions.LayerDepthThirdHighest);
			DrawRectangle(new Rectangle((int)titleSafeTopLeft.X, (int)titleSafeTopLeft.Y, (int)FighterManager.humanPlayers[2].PROPERTIES.healthMax * 5 + 10, 40), TheColorTransparentGray, Definitions.LayerDepthFourthHighest);
			titleSafeTopLeft.Y += 50f;
		}
		if (FighterManager.humanPlayers.Count > 3 && FighterManager.humanPlayers[3] != null)
		{
			if (FighterManager.humanPlayers.Count > 1)
			{
				spriteBatch.DrawString(font, FighterManager.humanPlayers[3].PROPERTIES.GamerTag, new Vector2(titleSafeTopLeft.X + 20f, titleSafeTopLeft.Y + 5f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			DrawRectangle(new Rectangle((int)titleSafeTopLeft.X + 5, (int)titleSafeTopLeft.Y + 5, (int)FighterManager.humanPlayers[3].PROPERTIES.health * 5, 30), Color.DarkRed, Definitions.LayerDepthThirdHighest);
			DrawRectangle(new Rectangle((int)titleSafeTopLeft.X, (int)titleSafeTopLeft.Y, (int)FighterManager.humanPlayers[3].PROPERTIES.healthMax * 5 + 10, 40), TheColorTransparentGray, Definitions.LayerDepthFourthHighest);
			titleSafeTopLeft.Y += 50f;
		}
	}

	public static void letterBox(DateTime expires)
	{
		letterBoxExpires = expires;
		isInLetterbox = true;
	}

	public static void letterBox()
	{
		isInLetterbox = true;
	}

	public static void ClearTextureCache()
	{
	}

	public static void ClearTextureCacheCutscenes()
	{
		for (int num = textureCachePaths.Count - 1; num > 0; num--)
		{
			if (textureCachePaths[num].Contains("CutScenes/"))
			{
				textureCache.RemoveAt(num);
				textureCachePaths.RemoveAt(num);
			}
		}
	}

	public static void ClearTextureCacheUNFINISHED(string level)
	{
		for (int i = 0; i < textureCache.Count; i++)
		{
			if (!textureCachePaths.Contains(""))
			{
				textureCache[i].Dispose();
			}
		}
		textureCachePaths.Clear();
		textureCache.Clear();
	}

	public static Rectangle GetRectangleFromTexture(Texture2D tex)
	{
		if (tex == null)
		{
			return Rectangle.Empty;
		}
		return new Rectangle(0, 0, tex.Width, tex.Height);
	}

	public static void DrawTexture(Texture2D tex, Rectangle r, Color c)
	{
		spriteBatch.Draw(tex, r, c);
	}

	public static void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
	{
		spriteBatch.Draw(texture, destinationRectangle, color);
	}

	public static void Draw(Texture2D texture, Vector2 position, Color color)
	{
		spriteBatch.Draw(texture, position, color);
	}

	public static void Draw(Texture2D texture, Vector2 position, float layerDepth)
	{
		spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, layerDepth);
	}

	public static void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
	{
		spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
	}

	public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
	{
		spriteBatch.Draw(texture, position, sourceRectangle, color);
	}

	public static void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
	{
		if (texture == null)
		{
			string text = "who is sending this???";
		}
		else
		{
			spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
		}
	}

	public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
	{
		spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	}

	public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
	{
		spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	}
}
