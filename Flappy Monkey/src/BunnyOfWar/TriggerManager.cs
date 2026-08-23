using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar;

public static class TriggerManager
{
	public enum TriggerType
	{
		custom,
		ExitLevel,
		AutoSave,
		LetterboxOn,
		LetterboxOff,
		PlaySound,
		PlayMusic,
		PauseMusic,
		StopMusic,
		LoadEnemy,
		LoadScenery,
		LoadObstacle,
		CheckForAwardment,
		VolumeRaise20p,
		VolumeLower20p,
		GhostEnemies,
		ShowOverlay,
		IfCoOp,
		IfHP80Plus,
		IfHP50Plus,
		HPBoostToMax,
		HPLowerBy50p,
		Death,
		CutScene,
		QuickTimeEvent,
		CutSceneUNSKIPPABLE,
		Checkpoint,
		HighScoreAdd1,
		HighScoreAdd10,
		HighScoreAdd100,
		IncreaseDifficulty
	}

	public static Dictionary<string, string> TriggerEvents = new Dictionary<string, string>();

	public static List<TriggerObject> triggers = new List<TriggerObject>(50);

	public static int timerTriggerCount = 0;

	public static void SetTriggerEvent(string name)
	{
		SetTriggerEvent(name, "done");
	}

	public static void SetTriggerEvent(string name, string value)
	{
		if (TriggerEvents == null)
		{
			TriggerEvents = new Dictionary<string, string>();
		}
		TriggerEvents[name] = value;
		CheckEventTrigger(name);
	}

	public static void addTrigger(TriggerObject trigger)
	{
		triggers.Add(trigger);
		if (trigger.activationTime != DateTime.MaxValue)
		{
			timerTriggerCount++;
		}
	}

	public static void CheckEventTrigger(string triggerEventName)
	{
		if (triggers.Count == 0)
		{
			return;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i].isActive && ((triggers[i].runAfterEventNamed != null && triggers[i].runAfterEventNamed == triggerEventName) || (triggers[i].uniqueName != null && triggers[i].uniqueName == triggerEventName)))
			{
				if (triggers[i].runXSecondssAfterEvent == 0.0)
				{
					triggers[i].onTrigger();
				}
				else
				{
					StartTriggerTimer(triggers[i]);
				}
			}
		}
	}

	public static void checkTriggers(FighterObject fighter)
	{
		if (triggers.Count == 0)
		{
			return;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i].isActive && triggers[i].rectZone.IsNotEmpty() && triggers[i].rectZone.Intersects(fighter.getWhereBodyIs()))
			{
				triggers[i].triggeredBy = fighter;
				if (triggers[i].runXSecondssAfterEvent == 0.0)
				{
					triggers[i].onTrigger();
				}
				else
				{
					StartTriggerTimer(triggers[i]);
				}
			}
		}
	}

	private static void StartTriggerTimer(TriggerObject to)
	{
		if (!(to.activationTime != DateTime.MaxValue))
		{
			to.activationTime = DateTime.Now.AddSeconds(to.runXSecondssAfterEvent);
			timerTriggerCount++;
		}
	}

	public static void checkTimerTriggers()
	{
		if (timerTriggerCount == 0)
		{
			return;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i].isActive && triggers[i].activationTime < DateTime.Now)
			{
				triggers[i].onTrigger();
				timerTriggerCount--;
			}
		}
	}

	public static string ExportData()
	{
		string text = "";
		foreach (TriggerObject trigger in triggers)
		{
			text += string.Format("type=trigger;x={0};y={1};w={2};h={3};typename={4};active={5};uniqueName={6};runXSecondssAfterEvent={7};activateEnemyNamed={8};activateSceneryNamed={9};activateObstacleNamed={10};runAfterEventNamed={11};activateSound={12};activateMusic={13};setEventNamed={14};activateWaveNamed={15};QTEButton={16};cutSceneDuration={17};cutSceneName={18}" + Environment.NewLine, trigger.X, trigger.Y, trigger.width, trigger.height, trigger.type.ToString(), trigger.isActive, trigger.uniqueName, trigger.runXSecondssAfterEvent.ToString(), trigger.activateEnemyNamed, trigger.activateSceneryNamed, trigger.activateObstacleNamed, trigger.runAfterEventNamed, trigger.activateSound, trigger.activateMusic, trigger.setEventNamed, trigger.activateWaveNamed, trigger.QTEButton.ToString(), trigger.cutSceneDurationInMS.ToString(), trigger.cutSceneName);
		}
		return text;
	}

	public static void ImportData(string data)
	{
		ClearData();
		string[] array = data.Split(Environment.NewLine.ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].StartsWith("type=trigger"))
			{
				triggers.Add(convertTextLineToTriggerObject(array[i]));
			}
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			triggers[i].ID = i;
		}
	}

	public static void ClearData()
	{
		triggers.Clear();
		TriggerEvents.Clear();
	}

	private static TriggerObject convertTextLineToTriggerObject(string s)
	{
		s = s.Trim();
		TriggerObject triggerObject = new TriggerObject();
		string[] array = s.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('=');
			if (array2[0] == "x")
			{
				triggerObject.X = int.Parse(array2[1]);
			}
			else if (array2[0] == "y")
			{
				triggerObject.Y = int.Parse(array2[1]);
			}
			else if (array2[0] == "w")
			{
				triggerObject.width = int.Parse(array2[1]);
			}
			else if (array2[0] == "h")
			{
				triggerObject.height = int.Parse(array2[1]);
			}
			else if (array2[0] == "typename")
			{
				triggerObject.type = (TriggerType)Enum.Parse(typeof(TriggerType), array2[1], ignoreCase: true);
			}
			else if (array2[0] == "uniqueName")
			{
				triggerObject.uniqueName = array2[1];
			}
			else if (array2[0] == "active")
			{
				if (array2[1] == "True")
				{
					triggerObject.isActive = true;
				}
				else if (array2[1] == "False")
				{
					triggerObject.isActive = false;
				}
			}
			else if (array2[0] == "runXSecondssAfterEvent")
			{
				triggerObject.runXSecondssAfterEvent = double.Parse(array2[1].ToString().Replace(",", "."));
			}
			else if (array2[0] == "activateEnemyNamed")
			{
				triggerObject.activateEnemyNamed = array2[1];
			}
			else if (array2[0] == "activateSceneryNamed")
			{
				triggerObject.activateSceneryNamed = array2[1];
			}
			else if (array2[0] == "activateObstacleNamed")
			{
				triggerObject.activateObstacleNamed = array2[1];
			}
			else if (array2[0] == "activateWaveNamed")
			{
				triggerObject.activateWaveNamed = array2[1];
			}
			else if (array2[0] == "runAfterEventNamed")
			{
				triggerObject.runAfterEventNamed = array2[1];
			}
			else if (array2[0] == "activateSound")
			{
				triggerObject.activateSound = array2[1];
			}
			else if (array2[0] == "activateMusic")
			{
				triggerObject.activateMusic = array2[1];
			}
			else if (array2[0] == "setEventNamed")
			{
				triggerObject.setEventNamed = array2[1];
			}
			else if (array2[0] == "cutSceneName")
			{
				triggerObject.cutSceneName = array2[1];
			}
			else if (array2[0] == "cutSceneDuration")
			{
				triggerObject.cutSceneDurationInMS = int.Parse(array2[1]);
			}
			else if (array2[0] == "QTEButton")
			{
				triggerObject.QTEButton = (QuickTimeEventsManager.QTEButtons)Enum.Parse(typeof(QuickTimeEventsManager.QTEButtons), array2[1], ignoreCase: true);
			}
			triggerObject.activationTime = DateTime.MaxValue;
		}
		return triggerObject;
	}

	public static void BroadcastTriggerTriggered(int id, int x, int y)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		if (Networking.NullCheckSucceed())
		{
			PacketWriter val = new PacketWriter();
			((BinaryWriter)(object)val).Write((byte)9);
			((BinaryWriter)(object)val).Write((ushort)id);
			((BinaryWriter)(object)val).Write((ushort)x);
			((BinaryWriter)(object)val).Write((ushort)y);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)Networking.networkSession.LocalGamers)[0].SendData(val, (SendDataOptions)1);
		}
	}

	public static void ReadTriggerTriggered(PacketReader packetReader)
	{
		if (triggers != null && triggers.Count != 0)
		{
			int index = ((BinaryReader)(object)packetReader).ReadUInt16();
			int x = ((BinaryReader)(object)packetReader).ReadUInt16();
			int y = ((BinaryReader)(object)packetReader).ReadUInt16();
			if (triggers[index].isActive)
			{
				triggers[index].onTrigger(x, y, isRemotelyTriggered: true);
			}
		}
	}
}
