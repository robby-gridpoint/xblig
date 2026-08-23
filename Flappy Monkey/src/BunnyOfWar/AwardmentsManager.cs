using System;
using System.Collections.Generic;

namespace BunnyOfWar;

public static class AwardmentsManager
{
	public static Dictionary<string, string> AwardmentsReceived = new Dictionary<string, string>();

	public static void SaveAwardment(string name)
	{
		string text = DateTime.Now.ToLongDateString();
		AwardmentsReceived[name] = text;
		FileManager.SaveAwardments(name + "=" + text + "\n");
	}

	public static void CheckForAwardments()
	{
		CheckForAwardments("");
	}

	public static void CheckForAwardments(string eventName)
	{
	}

	public static void ShowAwardment(string name, string message)
	{
		SoundManager.PlaySound("itunesComplete");
		GraphicsManager.Message(name + "\r\n\r\n" + message, 6, 0);
	}
}
