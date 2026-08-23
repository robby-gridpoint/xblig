using System;
using System.Collections.ObjectModel;
using System.IO;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar;

public static class NetworkGameplayManager
{
	public enum PacketType
	{
		HumanFighterPosition,
		HumanFighterPositionJumping,
		ObjectDamaged,
		AnimationChange,
		HumanHealth,
		ComputerHealthChange,
		RangedAttack,
		AddProjectile,
		Pause,
		TriggerTriggered,
		AssignRandomSeed,
		AssignPlayerID,
		PlayerReadyChange,
		SelectedALevel,
		SelectedAPvPLevel,
		SelectedABonusLevel,
		WorldMapPosition,
		Quit,
		ScreenManagerChange,
		FighterDeath,
		FighterStats,
		FighterStunned
	}

	public static PlayerIndex localPlayerIndex;

	public static string localGamerTag;

	private static PacketWriter packetWriter;

	private static PacketReader packetReader;

	private static int updatesSinceWorldDataSend;

	private static int updatesSinceStatusPacket;

	private static NetworkSession networkSession => Networking.networkSession;

	public static void Load()
	{
	}

	public static void SetAndSendPlayerIDs()
	{
		for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
		{
			FighterManager.humanPlayers[i].ID = i;
			SendNetworkPlayerID(FighterManager.humanPlayers[i].PROPERTIES.GamerTag, (byte)FighterManager.humanPlayers[i].ID);
		}
	}

	public static void SetAndSendRandomSeed()
	{
		int millisecond = DateTime.Now.Millisecond;
		RandomStaticGlobals.RandomAI = new Random(millisecond);
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)10);
			((BinaryWriter)(object)Networking.packetWriter).Write(millisecond);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)3);
		}
	}

	public static void SendNetworkPlayerID(string gamerTag, byte ID)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)Networking.packetWriter).Write((byte)11);
			((BinaryWriter)(object)Networking.packetWriter).Write(ID);
			((BinaryWriter)(object)Networking.packetWriter).Write(gamerTag.Length);
			((BinaryWriter)(object)Networking.packetWriter).Write(gamerTag.ToCharArray());
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(Networking.packetWriter, (SendDataOptions)3);
		}
	}

	private static void ReadNetworkPlayerID(string gamerTag, byte ID)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		while (ID >= FighterManager.humanPlayers.Count)
		{
			FighterManager.addNewHumanPlayer(null, isNetworkPlayer: false, "", 1f);
		}
		FighterManager.humanPlayers[ID].PROPERTIES.GamerTag = gamerTag;
		FighterManager.humanPlayers[ID].ID = ID;
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				if (((Gamer)current).Gamertag.ToLower().Trim() == gamerTag.ToLower().Trim())
				{
					FighterManager.humanPlayers[ID].PROPERTIES.PlayerIndexControllerNumber = current.PlayerIndex;
					FighterManager.localXboxPlayerID = ID;
					FighterManager.humanPlayers[ID].PROPERTIES.isLocal = true;
				}
				else if (!Networking.isHost)
				{
					FighterManager.humanPlayers[ID].PROPERTIES.isLocal = false;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public static void SendPlayerStats()
	{
		if (!Networking.NullCheckSucceed())
		{
			return;
		}
		foreach (FighterObject humanPlayer in FighterManager.humanPlayers)
		{
			if (humanPlayer.PROPERTIES.isLocal && humanPlayer.ID >= 0)
			{
				byte value = (byte)humanPlayer.ID;
				HumanProfileObject humanProfile = humanPlayer.PROPERTIES.HumanProfile;
				((BinaryWriter)(object)packetWriter).Write((byte)20);
				((BinaryWriter)(object)packetWriter).Write(value);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.blocks);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.counters);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.damageDealt);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.damageTaken);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.deaths);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.kills);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.parries);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.shotsFired);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.shotsMade);
				((BinaryWriter)(object)packetWriter).Write((ushort)humanProfile.shotsBlocked);
				((BinaryWriter)(object)packetWriter).Write((ulong)humanProfile.timeSpentBlocking);
				((BinaryWriter)(object)packetWriter).Write((ulong)humanProfile.timeSpentPlaying);
				((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
			}
		}
	}

	public static void ReadPlayerStats(PacketReader pr)
	{
		try
		{
			byte index = ((BinaryReader)(object)pr).ReadByte();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.blocks = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.counters = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.damageDealt = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.damageTaken = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.deaths = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.kills = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.parries = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.shotsFired = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.shotsMade = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.shotsBlocked = ((BinaryReader)(object)pr).ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.timeSpentBlocking = ((BinaryReader)(object)pr).ReadUInt64();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.timeSpentPlaying = ((BinaryReader)(object)pr).ReadUInt64();
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static void SendFighterPosition(int id, int x, int y)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)packetWriter).Write((byte)0);
			((BinaryWriter)(object)packetWriter).Write((byte)id);
			((BinaryWriter)(object)packetWriter).Write((ushort)x);
			((BinaryWriter)(object)packetWriter).Write((ushort)y);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)0);
		}
	}

	public static void ReadFighterPosition(PacketReader pr)
	{
		try
		{
			int index = ((BinaryReader)(object)packetReader).ReadByte();
			int x = ((BinaryReader)(object)packetReader).ReadUInt16();
			int y = ((BinaryReader)(object)packetReader).ReadUInt16();
			FighterManager.humanPlayers[index].moveRemotely(x, y, null);
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static void SendPackets(PacketType pt, int? a, int? b)
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

	public static void SendPauseState()
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)packetWriter).Write((byte)8);
			((BinaryWriter)(object)packetWriter).Write(RandomStaticGlobals.isGamePaused);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
		}
	}

	private static void ReadPauseState(PacketReader pr)
	{
		bool flag = ((BinaryReader)(object)pr).ReadBoolean();
		if (flag != RandomStaticGlobals.isGamePaused)
		{
			if (!flag)
			{
				ScreenManager.ShowBlank();
			}
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: false);
		}
	}

	public static void SendJumping(int playerID, int x, int y, int jumpHeight, bool areWeHuman)
	{
		if (Networking.NullCheckSucceed())
		{
			if (jumpHeight > 2000 || jumpHeight < 0)
			{
				jumpHeight = 0;
			}
			((BinaryWriter)(object)packetWriter).Write((byte)1);
			((BinaryWriter)(object)packetWriter).Write((ushort)playerID);
			((BinaryWriter)(object)packetWriter).Write((ushort)x);
			((BinaryWriter)(object)packetWriter).Write((ushort)y);
			((BinaryWriter)(object)packetWriter).Write((ushort)jumpHeight);
			((BinaryWriter)(object)packetWriter).Write(areWeHuman);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)0);
		}
	}

	public static void ReadJumping(PacketReader pr)
	{
		int index = ((BinaryReader)(object)pr).ReadUInt16();
		int x = ((BinaryReader)(object)pr).ReadUInt16();
		int y = ((BinaryReader)(object)pr).ReadUInt16();
		int value = ((BinaryReader)(object)pr).ReadUInt16();
		if (((BinaryReader)(object)pr).ReadBoolean())
		{
			FighterManager.humanPlayers[index].moveRemotely(x, y, value);
		}
		else
		{
			FighterManager.computerPlayers[index].moveRemotely(x, y, value);
		}
	}

	public static void SendObjectDamage(int objectID, int damageAmount)
	{
		if (Networking.NullCheckSucceed())
		{
			((BinaryWriter)(object)packetWriter).Write((byte)2);
			((BinaryWriter)(object)packetWriter).Write((ushort)objectID);
			((BinaryWriter)(object)packetWriter).Write((ushort)damageAmount);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)networkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)1);
		}
	}

	public static void ReadPackets()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!Networking.NullCheckSucceed())
		{
			return;
		}
		GamerCollectionEnumerator<LocalNetworkGamer> enumerator = Networking.LocalGamers.GetEnumerator();
		try
		{
			NetworkGamer val = default(NetworkGamer);
			while (enumerator.MoveNext())
			{
				LocalNetworkGamer current = enumerator.Current;
				while (current.IsDataAvailable)
				{
					current.ReceiveData(packetReader, ref val);
					if (val.IsLocal)
					{
						continue;
					}
					switch ((PacketType)((BinaryReader)(object)packetReader).ReadByte())
					{
					case PacketType.Pause:
						ReadPauseState(packetReader);
						break;
					case PacketType.HumanFighterPosition:
						ReadFighterPosition(packetReader);
						break;
					case PacketType.HumanFighterPositionJumping:
						ReadJumping(packetReader);
						break;
					case PacketType.AnimationChange:
						FighterManager.ReadAnimationChange(packetReader);
						break;
					case PacketType.HumanHealth:
						FighterManager.ReadHumanHealth(packetReader);
						break;
					case PacketType.ComputerHealthChange:
						FighterManager.ReadComputerDamage(packetReader);
						break;
					case PacketType.RangedAttack:
						FighterManager.ReadRangedAttack(packetReader);
						break;
					case PacketType.AssignRandomSeed:
						RandomStaticGlobals.RandomAI = new Random(((BinaryReader)(object)packetReader).ReadInt32());
						break;
					case PacketType.AssignPlayerID:
					{
						byte iD = ((BinaryReader)(object)packetReader).ReadByte();
						int count = ((BinaryReader)(object)packetReader).ReadInt32();
						string gamerTag = new string(((BinaryReader)(object)packetReader).ReadChars(count));
						ReadNetworkPlayerID(gamerTag, iD);
						break;
					}
					case PacketType.WorldMapPosition:
						ScreenManager.SetWorldMapPosition(((BinaryReader)(object)packetReader).ReadInt32(), ((BinaryReader)(object)packetReader).ReadInt32());
						break;
					case PacketType.SelectedALevel:
						LevelManager.LoadLevel(((BinaryReader)(object)packetReader).ReadInt32());
						break;
					case PacketType.SelectedAPvPLevel:
						LevelManager.LoadPvPLevel(((BinaryReader)(object)packetReader).ReadInt32());
						break;
					case PacketType.SelectedABonusLevel:
						LevelManager.LoadLevel("bonus", isPvP: false);
						break;
					case PacketType.Quit:
						ScreenManager.ShowMainMenu();
						break;
					case PacketType.ScreenManagerChange:
						ScreenManager.ReadScreenChange(packetReader);
						break;
					case PacketType.TriggerTriggered:
						TriggerManager.ReadTriggerTriggered(packetReader);
						break;
					case PacketType.ObjectDamaged:
						try
						{
							int index = ((BinaryReader)(object)packetReader).ReadUInt16();
							int amount = ((BinaryReader)(object)packetReader).ReadUInt16();
							ObstacleManager.Obstacles[index].takeDamage(amount, broadcast: false);
						}
						catch (Exception)
						{
						}
						break;
					case PacketType.FighterDeath:
						FighterManager.ReadFighterDeath(packetReader);
						break;
					case PacketType.FighterStats:
						ReadPlayerStats(packetReader);
						break;
					case PacketType.FighterStunned:
						FighterManager.ReadFighterStunned(packetReader);
						break;
					case PacketType.AddProjectile:
						FighterManager.ReadAddProjectile(packetReader);
						break;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	static NetworkGameplayManager()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		localPlayerIndex = PlayerIndex.Four;
		localGamerTag = "";
		packetWriter = new PacketWriter();
		packetReader = new PacketReader();
		updatesSinceWorldDataSend = 0;
		updatesSinceStatusPacket = 0;
	}
}
