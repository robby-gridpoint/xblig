using System;
using System.Collections.Generic;

namespace BunnyOfWar;

public static class WaveManager
{
	public class WaveData
	{
		public WaveName waveName = WaveName.Z_End;

		public int X = 0;

		public int Y = 0;

		public WaveData(WaveName name, int x, int y)
		{
			waveName = name;
			X = x;
			Y = y;
		}
	}

	public enum WaveName
	{
		Z_End
	}

	public static List<WaveData> WaveQueue = new List<WaveData>(0);

	public static int WaveQueueCooldownInSeconds = 10;

	private static DateTime WaveQueueSpawnAfter = DateTime.Now;

	public static WaveName? CurrentWave = null;

	public static bool IsWaveModeActive = false;

	public static int waveCoolDownInSeconds = 3;

	private static DateTime dtSpawnAfter = DateTime.Now;

	public static void Update()
	{
		if (WaveQueue != null && WaveQueue.Count > 0 && WaveQueueSpawnAfter < DateTime.Now)
		{
			LoadWave(WaveQueue[0].waveName, WaveQueue[0].X, WaveQueue[0].Y);
			WaveQueue.RemoveAt(0);
			WaveQueueSpawnAfter = DateTime.Now.AddSeconds(WaveQueueCooldownInSeconds);
		}
		if (!IsWaveModeActive)
		{
			return;
		}
		WaveName? currentWave;
		if (FighterManager.getComputerPlayers(onlyLiving: true, canBeDying: true).Count == 0)
		{
			if (dtSpawnAfter == DateTime.MinValue)
			{
				dtSpawnAfter = DateTime.Now.AddSeconds(waveCoolDownInSeconds);
			}
			else if (dtSpawnAfter < DateTime.Now)
			{
				currentWave = CurrentWave;
				if (currentWave.GetValueOrDefault() != WaveName.Z_End || !currentWave.HasValue)
				{
					CurrentWave++;
				}
				LoadWave(CurrentWave, GraphicsManager.viewableArea.X, GraphicsManager.viewableArea.Y);
				dtSpawnAfter = DateTime.MinValue;
			}
		}
		currentWave = CurrentWave;
		if (currentWave.GetValueOrDefault() == WaveName.Z_End && currentWave.HasValue)
		{
			IsWaveModeActive = false;
		}
	}

	public static void StartWaves()
	{
		int x = GraphicsManager.viewableArea.X;
		int y = GraphicsManager.viewableArea.Y;
		Update();
	}

	public static void LoadWaves(string name, int x, int y)
	{
		if (name == "")
		{
			return;
		}
		try
		{
			WaveName? wave = (WaveName?)Enum.Parse(typeof(WaveName), name, ignoreCase: false);
			LoadWave(wave, x, y);
			for (int i = 1; i < FighterManager.humanPlayers.Count; i++)
			{
				WaveQueue.Add(new WaveData(wave.Value, x + i * 10, y + i * 75));
			}
		}
		catch (Exception)
		{
		}
	}

	public static void LoadWave(string name, int x, int y)
	{
		if (name == "")
		{
			return;
		}
		try
		{
			WaveName? wave = (WaveName?)Enum.Parse(typeof(WaveName), name, ignoreCase: false);
			LoadWave(wave, x, y);
		}
		catch (Exception)
		{
		}
	}

	public static void LoadWave(WaveName? wave, int x, int y)
	{
		if (wave.HasValue)
		{
			int num = 500;
			int num2 = 2500;
			int num3 = 2000;
		}
	}
}
