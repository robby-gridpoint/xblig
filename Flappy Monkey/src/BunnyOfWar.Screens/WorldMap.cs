using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar.Screens;

public class WorldMap
{
	private static bool isShowingPolishedMap = true;

	private List<Texture2D> levelImages;

	private Texture2D background;

	private Texture2D backgroundTrial;

	private Texture2D cursor;

	private string[,] map;

	private string[] mapForBabewatchTRIAL;

	private string[] mapForBabewatch;

	public int x;

	public int y;

	private PacketWriter packetWriter;

	private PacketReader packetReader;

	private static NetworkSession networkSession => Networking.networkSession;

	public WorldMap()
	{
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Expected O, but got Unknown
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Expected O, but got Unknown
		levelImages = new List<Texture2D>();
		map = new string[5, 9]
		{
			{ "Quit", "", "PvP", "", "", "", "", "", "" },
			{ "1", "2", "3", "4", "5", "6", "7", "8", "9" },
			{ "", "", "", "", "", "", "", "", "10" },
			{ "", "", "", "", "", "", "", "", "11" },
			{ "", "19", "18", "17", "16", "15", "14", "13", "12" }
		};
		mapForBabewatchTRIAL = new string[5] { "", "1/1 intro level QTE cutscene.lvl", "trial/11b faster easy flatlands.lvl", "4/3 CPR Italian Jessica.lvl", "bonus/buyme.lvl" };
		mapForBabewatch = new string[9] { "", "testGunSmoke", "testInfinite.lvl", "EasyFlappy.lvl", "testShooter.lvl", "test.lvl", "testFlappy.lvl", "testFlappyShooter.lvl", "testShooter.lvl" };
		x = 0;
		y = 1;
		packetWriter = new PacketWriter();
		packetReader = new PacketReader();
		Load(RandomStaticGlobals.Content);
	}

	public void Draw()
	{
		if (isShowingPolishedMap)
		{
			Texture2D texture2D = (RandomStaticGlobals.IsTrial() ? GraphicsManager.LoadTexture("screens/UnPermanent/TRIAL" + RandomStaticGlobals.currentlySelectedLevel, cacheResult: true) : GraphicsManager.LoadTexture("screens/UnPermanent/" + RandomStaticGlobals.currentlySelectedLevel, cacheResult: true));
			if (texture2D == null)
			{
				texture2D = GraphicsManager.imgButtonX;
			}
			if (!RandomStaticGlobals.IsTrial())
			{
				GraphicsManager.spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			}
			else
			{
				GraphicsManager.spriteBatch.Draw(backgroundTrial, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			}
			GraphicsManager.spriteBatch.Draw(texture2D, new Rectangle(300, 150, 1333, 750), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthSecondHighest);
			GraphicsManager.DrawString(750, 50, "Level " + RandomStaticGlobals.currentlySelectedLevel, Color.WhiteSmoke, GraphicsManager.fontBig);
			return;
		}
		GraphicsManager.Draw(background, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		int num = 100;
		int num2 = 100;
		if (y > 0)
		{
			num2 = y * 200 + 100;
		}
		if (x > 0)
		{
			num = x * 200 + 100;
		}
		GraphicsManager.Draw(cursor, new Vector2(num, num2), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
	}

	public void ProcessInput()
	{
		if (RandomStaticGlobals.currentlySelectedLevel < 0)
		{
			RandomStaticGlobals.currentlySelectedLevel = 0;
		}
		if (RandomStaticGlobals.currentlySelectedLevel >= mapForBabewatch.Length)
		{
			RandomStaticGlobals.currentlySelectedLevel = mapForBabewatch.Length - 1;
		}
		List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
		if (humanPlayers.Count == 0)
		{
			ScreenManager.ShowMainMenu();
		}
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.HasValue && FighterManager.humanPlayers[i].PROPERTIES.isLocal)
			{
				InputFromAnywhere playerInput = InputManager.GetPlayerInput(FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.Value, ref FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState, ref InputManager.previousKeyboardStateMenu);
				FigureOutInput(playerInput, FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.Value);
			}
		}
	}

	private void FigureOutPhoneInput(int x, int y)
	{
	}

	private void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex? pi)
	{
		if (!pi.HasValue || Guide.IsVisible || !isShowingPolishedMap)
		{
			return;
		}
		if (anywhereInput.LEFT_pressed)
		{
			int num = RandomStaticGlobals.currentlySelectedLevel - 1;
			if (isThisMoveAllowed(RandomStaticGlobals.currentlySelectedLevel.ToString(), num.ToString()))
			{
				RandomStaticGlobals.currentlySelectedLevel--;
				UpdateNetworkGamers();
			}
		}
		if (anywhereInput.RIGHT_pressed)
		{
			int num = RandomStaticGlobals.currentlySelectedLevel + 1;
			if (isThisMoveAllowed(RandomStaticGlobals.currentlySelectedLevel.ToString(), num.ToString()))
			{
				RandomStaticGlobals.currentlySelectedLevel++;
				UpdateNetworkGamers();
			}
			else
			{
				RandomStaticGlobals.BuyMe(pi.Value);
			}
		}
		if (RandomStaticGlobals.currentlySelectedLevel <= 0)
		{
			RandomStaticGlobals.currentlySelectedLevel = 1;
		}
		if (anywhereInput.X_pressed)
		{
			RandomStaticGlobals.BuyMe(pi.Value);
		}
		if (anywhereInput.B_held && anywhereInput.X_held)
		{
			LevelManager.currentLevel = 0;
			RandomStaticGlobals.currentlySelectedLevel = 0;
			LevelManager.LoadLevel("test.lvl", isPvP: false);
		}
		if (anywhereInput.A_pressed || anywhereInput.START_pressed)
		{
			SoundManager.PlayMenuClick();
			SendPackets(NetworkGameplayManager.PacketType.SelectedALevel, RandomStaticGlobals.currentlySelectedLevel, null);
			if (!RandomStaticGlobals.IsTrial())
			{
				LevelManager.currentLevel = RandomStaticGlobals.currentlySelectedLevel;
				LevelManager.LoadLevel(mapForBabewatch[RandomStaticGlobals.currentlySelectedLevel], isPvP: false);
			}
			else
			{
				LevelManager.currentLevel = RandomStaticGlobals.currentlySelectedLevel;
				LevelManager.LoadLevel(mapForBabewatchTRIAL[RandomStaticGlobals.currentlySelectedLevel], isPvP: false);
			}
		}
		if (anywhereInput.SELECT_pressed)
		{
			ScreenManager.ShowMainMenu();
		}
		if (anywhereInput.Y_pressed)
		{
			ScreenManager.ShowCredits();
		}
	}

	private bool isThisMoveAllowed(string currentSpot, string futureSpot)
	{
		if (RandomStaticGlobals.IsTrial())
		{
			switch (futureSpot)
			{
			default:
				if (!(futureSpot == "4"))
				{
					if (futureSpot == "-1")
					{
						return false;
					}
					return false;
				}
				goto case "0";
			case "0":
			case "1":
			case "2":
			case "3":
				return true;
			}
		}
		if (futureSpot == "0")
		{
			return false;
		}
		if (int.Parse(futureSpot) >= mapForBabewatch.Length)
		{
			return false;
		}
		try
		{
			if (RandomStaticGlobals.currentlySelectedLevel > int.Parse(futureSpot))
			{
				return true;
			}
			if (int.Parse(currentSpot) > int.Parse(futureSpot))
			{
				return true;
			}
		}
		catch (Exception)
		{
		}
		if (RandomStaticGlobals.GameProgress.ContainsKey(currentSpot))
		{
			return true;
		}
		if (RandomStaticGlobals.GameProgress.ContainsKey(futureSpot))
		{
			return true;
		}
		switch (futureSpot)
		{
		default:
			if (!(futureSpot == "PvP"))
			{
				return false;
			}
			goto case "Quit";
		case "Quit":
		case "Store":
		case "Home":
		case "x":
		case "1":
			return true;
		}
	}

	private void moveUp()
	{
		string currentSpot = map[y, x];
		y--;
		if (y < 0 || map[y, x] == "")
		{
			y++;
			return;
		}
		string futureSpot = map[y, x];
		if (!isThisMoveAllowed(currentSpot, futureSpot))
		{
			y++;
		}
		else
		{
			UpdateNetworkGamers();
		}
	}

	private void moveDown()
	{
		string currentSpot = map[y, x];
		y++;
		if (y > 10 || map[y, x] == "")
		{
			y--;
			return;
		}
		string futureSpot = map[y, x];
		if (!isThisMoveAllowed(currentSpot, futureSpot))
		{
			y--;
		}
		else
		{
			UpdateNetworkGamers();
		}
	}

	private void moveRight()
	{
		string currentSpot = map[y, x];
		x++;
		if (x > 20 || map[y, x] == "")
		{
			x--;
			return;
		}
		string futureSpot = map[y, x];
		if (!isThisMoveAllowed(currentSpot, futureSpot))
		{
			x--;
		}
		else
		{
			UpdateNetworkGamers();
		}
	}

	private void moveLeft()
	{
		string currentSpot = map[y, x];
		x--;
		if (x < 0 || map[y, x] == "")
		{
			x++;
			return;
		}
		string futureSpot = map[y, x];
		if (!isThisMoveAllowed(currentSpot, futureSpot))
		{
			x++;
		}
		else
		{
			UpdateNetworkGamers();
		}
	}

	private void selectedSomething()
	{
		if (map[y, x] == "")
		{
			return;
		}
		try
		{
			int levelNumber = int.Parse(map[y, x]);
			SendPackets(NetworkGameplayManager.PacketType.SelectedALevel, RandomStaticGlobals.currentlySelectedLevel, null);
			SoundManager.PlayMenuClick();
			LevelManager.LoadLevel(levelNumber);
		}
		catch (Exception)
		{
		}
	}

	public void Load(ContentManager Content)
	{
		if (RandomStaticGlobals.IsTrial())
		{
			backgroundTrial = GraphicsManager.LoadTexture("screens/WorldMapTRIAL");
		}
		background = GraphicsManager.LoadTexture("screens/WorldMap");
		cursor = GraphicsManager.LoadTexture("screens/cursor");
		Clear();
	}

	public void Clear()
	{
		if (RandomStaticGlobals.IsTrial())
		{
			RandomStaticGlobals.currentlySelectedLevel = 1;
		}
		if (RandomStaticGlobals.currentlySelectedLevel != -1)
		{
			return;
		}
		RandomStaticGlobals.currentlySelectedLevel = 1;
		foreach (string key in RandomStaticGlobals.GameProgress.Keys)
		{
			try
			{
				int num = int.Parse(RandomStaticGlobals.GameProgress[key].ToString());
				if (num > RandomStaticGlobals.currentlySelectedLevel)
				{
					RandomStaticGlobals.currentlySelectedLevel = num;
				}
			}
			catch (Exception)
			{
			}
		}
		if (isThisMoveAllowed(RandomStaticGlobals.currentlySelectedLevel.ToString(), (RandomStaticGlobals.currentlySelectedLevel + 1).ToString()))
		{
			RandomStaticGlobals.currentlySelectedLevel++;
		}
	}

	private void UpdateNetworkGamers()
	{
		SendPackets(NetworkGameplayManager.PacketType.WorldMapPosition, RandomStaticGlobals.currentlySelectedLevel, 0);
	}

	public void SendPackets(NetworkGameplayManager.PacketType pt, int? a, int? b)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)packetWriter).Write((byte)pt);
			if (a.HasValue)
			{
				((BinaryWriter)(object)packetWriter).Write(a.Value);
			}
			if (b.HasValue)
			{
				((BinaryWriter)(object)packetWriter).Write(b.Value);
			}
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
		}
	}
}
