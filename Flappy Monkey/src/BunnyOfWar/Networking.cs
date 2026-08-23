using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar;

public static class Networking
{
	public class OperationCompletedEventArgs : EventArgs
	{
		private IAsyncResult asyncResult;

		public IAsyncResult AsyncResult
		{
			get
			{
				return asyncResult;
			}
			set
			{
				asyncResult = value;
			}
		}

		public OperationCompletedEventArgs(IAsyncResult asyncResult)
		{
			this.asyncResult = asyncResult;
		}
	}

	public const int maxGamers = 6;

	public const int maxLocalGamers = 4;

	private const int maximumSessions = 666;

	public static NetworkSession networkSession;

	public static PacketWriter packetWriter;

	public static PacketReader packetReader;

	private static bool gameIsJoinable;

	private static bool updateState;

	private static SignedInGamer invited;

	public static AvailableNetworkSessionCollection availableSessions;

	public static List<string> multiplayerAvailableGamesList;

	private static int selectedEntry;

	public static GamerCollection<NetworkGamer> RemoteGamers
	{
		get
		{
			if (networkSession == null)
			{
				return null;
			}
			return networkSession.RemoteGamers;
		}
		set
		{
		}
	}

	public static int RemoteGamersCount
	{
		get
		{
			if (RemoteGamers != null)
			{
				return ((ReadOnlyCollection<NetworkGamer>)(object)RemoteGamers).Count;
			}
			return 0;
		}
	}

	public static GamerCollection<LocalNetworkGamer> LocalGamers
	{
		get
		{
			if (networkSession == null)
			{
				return null;
			}
			return networkSession.LocalGamers;
		}
		set
		{
		}
	}

	public static GamerCollection<NetworkGamer> AllGamers
	{
		get
		{
			if (networkSession == null)
			{
				return null;
			}
			return networkSession.AllGamers;
		}
		set
		{
		}
	}

	public static NetworkSessionState? SessionState
	{
		get
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (networkSession != null && !networkSession.IsDisposed)
			{
				return networkSession.SessionState;
			}
			return null;
		}
	}

	public static bool IsGameJoinable
	{
		get
		{
			return gameIsJoinable;
		}
		set
		{
			gameIsJoinable = value;
			UpdatePrivateGamerSlots();
		}
	}

	public static bool isHost
	{
		get
		{
			if (networkSession == null || networkSession.IsDisposed)
			{
				return false;
			}
			return networkSession.IsHost;
		}
	}

	public static void initialize()
	{
		try
		{
			if (networkSession == null || networkSession.IsDisposed)
			{
				networkSession = NetworkSession.Create((NetworkSessionType)2, 4, 6, 0, (NetworkSessionProperties)null);
				InitNewNetworkSession();
			}
		}
		catch (Exception)
		{
			int num = 0;
		}
	}

	public static void SessionsFound(object sender, OperationCompletedEventArgs e)
	{
		//IL_001c: Expected O, but got Unknown
		//IL_002c: Expected O, but got Unknown
		ScreenManager.hideNetworkBusy();
		try
		{
			availableSessions = NetworkSession.EndFind(e.AsyncResult);
		}
		catch (NetworkException ex)
		{
			NetworkException ex2 = ex;
			ScreenManager.showMessageBox("Failed searching for the session.");
		}
		catch (GamerPrivilegeException ex3)
		{
			GamerPrivilegeException ex4 = ex3;
			ScreenManager.showMessageBox("You do not have permission to search for a session.");
		}
		multiplayerAvailableGamesList.Clear();
		if (availableSessions != null)
		{
			foreach (AvailableNetworkSession item in (ReadOnlyCollection<AvailableNetworkSession>)(object)availableSessions)
			{
				if (item.CurrentGamerCount < 6)
				{
					multiplayerAvailableGamesList.Add(item.HostGamertag + " (" + item.CurrentGamerCount + "/" + 6 + ")");
				}
				if (multiplayerAvailableGamesList.Count >= 666)
				{
					break;
				}
			}
		}
		if (availableSessions == null || ((ReadOnlyCollection<AvailableNetworkSession>)(object)availableSessions).Count == 0)
		{
			ScreenManager.ShowGameLobby(broadcast: false);
			GraphicsManager.Message("Sorry, no games were found. Why don't you host one? Or put your Xbox 360 on your router's DMZ?", GraphicsManager.imgNiceBackground);
		}
		else
		{
			ScreenManager.ShowNetworkGamesList();
		}
	}

	public static void JoinInvitedGame()
	{
		try
		{
			IAsyncResult asyncResult = NetworkSession.BeginJoinInvited(1, (AsyncCallback)null, (object)null);
			ScreenManager.showNetworkBusyScreen("Joining the session...", asyncResult, InvitedSessionJoined);
		}
		catch
		{
		}
		invited = null;
		updateState = true;
	}

	public static void CreateSession(NetworkSessionType sessionType)
	{
		//IL_0032: Expected O, but got Unknown
		//IL_0042: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null && !networkSession.IsDisposed)
		{
			return;
		}
		try
		{
			networkSession = NetworkSession.Create(sessionType, 4, 6);
			InitNewNetworkSession();
		}
		catch (NetworkException ex)
		{
			NetworkException ex2 = ex;
			ScreenManager.showMessageBox("Failed creating the session.");
		}
		catch (GamerPrivilegeException ex3)
		{
			GamerPrivilegeException ex4 = ex3;
			ScreenManager.showMessageBox("You do not have permission to create a session.");
		}
	}

	public static void ListPublicGames(NetworkSessionType sessionType, PlayerIndex ndx, string gamerTag)
	{
		//IL_005c: Expected O, but got Unknown
		//IL_006c: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (networkSession != null || (networkSession != null && !networkSession.IsDisposed))
		{
			networkSession.Dispose();
		}
		ScreenManager.ShowNetworkGamesList();
		try
		{
			IAsyncResult asyncResult = NetworkSession.BeginFind(sessionType, 1, (NetworkSessionProperties)null, (AsyncCallback)null, (object)null);
			ScreenManager.showNetworkBusyScreen("Searching for a session...", asyncResult, SessionsFound);
		}
		catch (NetworkException ex)
		{
			NetworkException ex2 = ex;
			ScreenManager.showMessageBox("Failed searching for the session.");
		}
		catch (GamerPrivilegeException ex3)
		{
			GamerPrivilegeException ex4 = ex3;
			ScreenManager.showMessageBox("You do not have permission to search for a session.");
		}
	}

	public static bool JoinGame(int entryIndex)
	{
		//IL_0057: Expected O, but got Unknown
		//IL_0068: Expected O, but got Unknown
		if (availableSessions != null && entryIndex >= 0 && entryIndex < ((ReadOnlyCollection<AvailableNetworkSession>)(object)availableSessions).Count)
		{
			try
			{
				IAsyncResult asyncResult = NetworkSession.BeginJoin(((ReadOnlyCollection<AvailableNetworkSession>)(object)availableSessions)[entryIndex], (AsyncCallback)null, (object)null);
				ScreenManager.showNetworkBusyScreen("Joining the session...", asyncResult, LoadLobbyScreen);
				return true;
			}
			catch (NetworkException ex)
			{
				NetworkException ex2 = ex;
				ScreenManager.showMessageBox("Failed joining the session.");
				return false;
			}
			catch (GamerPrivilegeException ex3)
			{
				GamerPrivilegeException ex4 = ex3;
				ScreenManager.showMessageBox("You do not have permission to join a session.");
				return false;
			}
		}
		return false;
	}

	private static void InvitedSessionJoined(object sender, OperationCompletedEventArgs e)
	{
		//IL_001c: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			networkSession = NetworkSession.EndJoinInvited(e.AsyncResult);
			InitNewNetworkSession();
		}
		catch (NetworkSessionJoinException ex)
		{
			NetworkSessionJoinException ex2 = ex;
			ScreenManager.showMessageBox("Failed joining the session (" + ((object)ex2.JoinError).ToString() + ").");
		}
		catch (Exception ex3)
		{
			ScreenManager.showMessageBox("Failed joining the session (" + ex3.Message + ").");
		}
		if (networkSession != null)
		{
			ScreenManager.ShowGameLobby(broadcast: false);
		}
	}

	public static void SessionCreated(object sender, OperationCompletedEventArgs e)
	{
		//IL_0022: Expected O, but got Unknown
		//IL_0032: Expected O, but got Unknown
		ScreenManager.hideNetworkBusy();
		try
		{
			networkSession = NetworkSession.EndCreate(e.AsyncResult);
			InitNewNetworkSession();
		}
		catch (NetworkException ex)
		{
			NetworkException ex2 = ex;
			ScreenManager.showMessageBox("Failed creating the session.");
		}
		catch (GamerPrivilegeException ex3)
		{
			GamerPrivilegeException ex4 = ex3;
			ScreenManager.showMessageBox("You do not have permission to create a session.");
		}
		if (networkSession != null)
		{
			networkSession.AllowHostMigration = true;
			networkSession.AllowJoinInProgress = false;
			ScreenManager.showMessageBox("Success! Your game lobby is now public for anyone to join.");
		}
	}

	private static void LoadLobbyScreen(object sender, OperationCompletedEventArgs e)
	{
		//IL_0022: Expected O, but got Unknown
		//IL_0032: Expected O, but got Unknown
		ScreenManager.hideNetworkBusy();
		try
		{
			networkSession = NetworkSession.EndJoin(e.AsyncResult);
			InitNewNetworkSession();
		}
		catch (NetworkException ex)
		{
			NetworkException ex2 = ex;
			ScreenManager.showMessageBox("Failed joining session.");
		}
		catch (GamerPrivilegeException ex3)
		{
			GamerPrivilegeException ex4 = ex3;
			ScreenManager.showMessageBox("You do not have permission to join a session.");
		}
		if (networkSession != null)
		{
			ScreenManager.ShowGameLobby(broadcast: false);
		}
	}

	public static void NetworkSession_InviteAccepted(object sender, InviteAcceptedEventArgs e)
	{
		//IL_0037: Expected O, but got Unknown
		//IL_0047: Expected O, but got Unknown
		if (Guide.IsTrialMode)
		{
			GraphicsManager.Message("Sorry, you have to buy the game before you can accept this invite.");
			return;
		}
		EndSession();
		try
		{
			networkSession = NetworkSession.JoinInvited(1);
			InitNewNetworkSession();
		}
		catch (NetworkException ex)
		{
			NetworkException ex2 = ex;
			ScreenManager.showMessageBox("Failed joining the session.");
		}
		catch (GamerPrivilegeException ex3)
		{
			GamerPrivilegeException ex4 = ex3;
			ScreenManager.showMessageBox("You do not have permission to join a session.");
		}
		if (networkSession != null)
		{
			LoadLobbyScreen(null, null);
		}
	}

	public static void EndSession()
	{
		if (networkSession != null)
		{
			networkSession.Dispose();
		}
	}

	public static void UpdateNetworkSession()
	{
		if (networkSession != null && !networkSession.IsDisposed)
		{
			networkSession.Update();
		}
	}

	public static void networkSession_GameStarted(object sender, GameStartedEventArgs e)
	{
	}

	public static void networkSession_GameEnded(object sender, GameEndedEventArgs e)
	{
		ScreenManager.ShowGameLobby(broadcast: true);
	}

	public static void networkSession_GamerLeft(object sender, GamerLeftEventArgs e)
	{
		for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
		{
			if (FighterManager.humanPlayers[i].PROPERTIES.GamerTag.ToLower() == ((Gamer)e.Gamer).Gamertag.ToString().ToLower())
			{
				FighterManager.humanPlayers[i].onDeath();
				FighterManager.humanPlayers.RemoveAt(i);
				GraphicsManager.Message(((Gamer)e.Gamer).Gamertag + " has left the game.");
			}
		}
	}

	public static void networkSession_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (isHost)
		{
			networkSession.EndGame();
		}
		GraphicsManager.Message("The game has ended because " + e.EndReason);
		ScreenManager.ShowGameLobby(broadcast: true);
		ScreenManager.ShowMainMenu();
	}

	public static bool NullCheckSucceed()
	{
		if (networkSession == null || networkSession.IsDisposed || packetWriter == null)
		{
			return false;
		}
		if (RemoteGamersCount < 1)
		{
			return false;
		}
		if (AllGamers == null || ((ReadOnlyCollection<NetworkGamer>)(object)AllGamers).Count == 0 || LocalGamers == null || ((ReadOnlyCollection<LocalNetworkGamer>)(object)LocalGamers).Count == 0)
		{
			ScreenManager.showMessageBox("Something horrible happened, and all of the gamers disappeared. :O");
			return false;
		}
		return true;
	}

	public static void StartGame()
	{
		if (isHost && networkSession != null && !networkSession.IsDisposed)
		{
			networkSession.StartGame();
		}
	}

	public static void StopGame()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		if (networkSession != null && !networkSession.IsDisposed && (int)networkSession.SessionState == 1)
		{
			networkSession.EndGame();
			networkSession.Dispose();
		}
	}

	public static void InviteParty()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		try
		{
			GamerCollectionEnumerator<NetworkGamer> enumerator = networkSession.AllGamers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					LocalNetworkGamer val = (LocalNetworkGamer)enumerator.Current;
					val.SendPartyInvites();
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
		}
		catch (Exception)
		{
		}
	}

	public static void InvitePlayer(PlayerIndex yourPlayerIndex)
	{
		try
		{
			Guide.ShowGameInvite(yourPlayerIndex, (IEnumerable<Gamer>)null);
		}
		catch (Exception)
		{
		}
	}

	public static void InitNewNetworkSession()
	{
		try
		{
			if (networkSession != null && !networkSession.IsDisposed)
			{
				NetworkSession.InviteAccepted += NetworkSession_InviteAccepted;
				networkSession.GamerLeft += networkSession_GamerLeft;
				networkSession.SessionEnded += networkSession_SessionEnded;
				networkSession.GameEnded += networkSession_GameEnded;
				networkSession.GameStarted += networkSession_GameStarted;
			}
			UpdatePrivateGamerSlots();
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	private static void UpdatePrivateGamerSlots()
	{
		if (!isHost || networkSession == null || networkSession.IsDisposed)
		{
			return;
		}
		if (!gameIsJoinable)
		{
			try
			{
				int privateGamerSlots = networkSession.MaxGamers - ((ReadOnlyCollection<NetworkGamer>)(object)networkSession.RemoteGamers).Count - ((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers).Count;
				networkSession.PrivateGamerSlots = privateGamerSlots;
				return;
			}
			catch (Exception)
			{
				networkSession.PrivateGamerSlots = 0;
				return;
			}
		}
		networkSession.PrivateGamerSlots = 0;
	}

	static Networking()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		packetWriter = new PacketWriter();
		packetReader = new PacketReader();
		gameIsJoinable = false;
		updateState = false;
		availableSessions = null;
		multiplayerAvailableGamesList = new List<string>();
		selectedEntry = 0;
	}
}
