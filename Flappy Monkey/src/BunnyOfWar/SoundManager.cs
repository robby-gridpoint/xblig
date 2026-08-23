using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace BunnyOfWar;

public static class SoundManager
{
	private const bool AudioEnabled = true;

	private const bool MusicEnabled = true;

	private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

	private static readonly Dictionary<SoundEffect, string> cuePaths = new Dictionary<SoundEffect, string>();

	private const uint SND_ASYNC = 0x0001;

	private const uint SND_FILENAME = 0x00020000;

	private const uint SND_NOSTOP = 0x0010;

	private const uint SND_LOOP = 0x0008;

	[DllImport("winmm.dll", CharSet = CharSet.Unicode)]
	private static extern bool PlaySoundW(string soundName, nint moduleHandle, uint flags);

	private static bool gameHasControl = false;

	private static bool checkedControl = false;

	private static DateTime lastMusicRequestTime = DateTime.MinValue;

	private static string currentSongNamePlaying = "";

	private static bool musicIsPlaying;

	private static bool currentMusicLoops;

	private static SoundEffect[] punches;

	private static SoundEffect[] clangs;

	private static SoundEffect[] splats;

	private static SoundEffect[] farts;

	public static SoundEffect laser1;

	public static SoundEffect laser2;

	public static SoundEffect laser3;

	private static SoundEffect slowWhoosh;

	private static SoundEffect quickWhoosh;

	private static int punchPosition = 0;

	private static int clangPosition = 0;

	private static int splatPosition = 0;

	public static int fartPosition = 0;

	public static void ClearCache()
	{
		cuePaths.Clear();
		sounds.Clear();
	}

	private static void PlayCue(SoundEffect sound, float volume, float pitch, float pan)
	{
		if (sound == null)
		{
			return;
		}
		volume = Math.Clamp(volume, 0f, 1f);
		pitch = Math.Clamp(pitch, -1f, 1f);
		pan = Math.Clamp(pan, -1f, 1f);
		if (!cuePaths.TryGetValue(sound, out string audioPath))
		{
			return;
		}
		PlaySoundW(audioPath, 0, SND_FILENAME | SND_ASYNC | SND_NOSTOP);
	}

	public static void PlaySoundDirectly(SoundEffect soundin)
	{
		if (!AudioEnabled)
		{
			return;
		}
		if (soundin == null)
		{
			return;
		}
		try
		{
			PlayCue(soundin, Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume, 0f, 0f);
		}
		catch (Exception ex)
		{
			string text = ex.ToString();
		}
	}

	public static void PlaySound(string soundName)
	{
		if (!AudioEnabled)
		{
			return;
		}
		try
		{
			if (sounds.ContainsKey(soundName))
			{
				PlayCue(sounds[soundName], Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume, 0f, 0f);
			}
			else if (!sounds.ContainsKey(soundName))
			{
				try
				{
					sounds[soundName] = LoadSoundEffect("sounds/" + soundName);
					PlaySound(soundName);
					return;
				}
				catch (Exception)
				{
					sounds[soundName] = null;
					return;
				}
			}
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static bool isMusicPlaying()
	{
		return MusicEnabled && musicIsPlaying;
	}

	public static bool DoesGameHaveControl()
	{
		return MusicEnabled;
	}

	public static void PlayMusic(string musicName, bool IsRepeating)
	{
		if (!MusicEnabled)
		{
			return;
		}
		if (lastMusicRequestTime.AddSeconds(1.0) > DateTime.Now)
		{
			return;
		}
		lastMusicRequestTime = DateTime.Now;
		if (isMusicPlaying() && musicName == currentSongNamePlaying)
		{
			return;
		}
		currentSongNamePlaying = musicName;
		if (!DoesGameHaveControl())
		{
			return;
		}
		try
		{
			PlaySoundW(null, 0, 0);
			string audioPath = Path.Combine(AppContext.BaseDirectory, Definitions.ContentRootDirectory, "music", musicName + ".wav");
			musicIsPlaying = PlaySoundW(audioPath, 0, SND_FILENAME | SND_ASYNC | (IsRepeating ? SND_LOOP : 0));
			currentMusicLoops = IsRepeating;
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static void UpdateVolumes()
	{
	}

	public static void StopMusic()
	{
		if (!MusicEnabled)
		{
			return;
		}
		PlaySoundW(null, 0, 0);
		musicIsPlaying = false;
	}

	public static void PauseMusic()
	{
		if (!MusicEnabled)
		{
			return;
		}
		PlaySoundW(null, 0, 0);
		musicIsPlaying = false;
	}

	public static void ResumeMusic()
	{
		if (!MusicEnabled)
		{
			return;
		}
		if (currentSongNamePlaying != "")
		{
			PlayMusic(currentSongNamePlaying, currentMusicLoops);
		}
	}

	public static SoundEffect LoadSoundEffect(string path)
	{
		if (!AudioEnabled)
		{
			return null;
		}
		string audioPath = Path.Combine(AppContext.BaseDirectory, Definitions.ContentRootDirectory, path.Replace('/', Path.DirectorySeparatorChar) + ".wav");
		using FileStream stream = File.OpenRead(audioPath);
		SoundEffect sound = SoundEffect.FromStream(stream);
		cuePaths[sound] = audioPath;
		return sound;
	}

	public static void LoadContent(ContentManager Content)
	{
		if (!AudioEnabled)
		{
			return;
		}
		punches = new SoundEffect[11];
		clangs = new SoundEffect[12];
		splats = new SoundEffect[2];
		farts = new SoundEffect[7];
		farts[0] = LoadSoundEffect("sounds/farts/fart1");
		farts[1] = LoadSoundEffect("sounds/farts/fart2");
		farts[2] = LoadSoundEffect("sounds/farts/fart3");
		farts[3] = LoadSoundEffect("sounds/farts/fart4");
		farts[4] = LoadSoundEffect("sounds/farts/fart6");
		farts[5] = LoadSoundEffect("sounds/farts/fart7");
		farts[6] = LoadSoundEffect("sounds/farts/fart9");
	}

	public static void PlayMenuClick()
	{
	}

	public static void playNextQuickWhoosh(float pan)
	{
		if (!AudioEnabled)
		{
			return;
		}
		if (quickWhoosh != null)
		{
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			float num = Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume * 0.8f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			PlayCue(quickWhoosh, num, 0f, pan);
		}
	}

	public static void playNextSlowWhoosh(float pan)
	{
		if (!AudioEnabled)
		{
			return;
		}
		if (slowWhoosh != null)
		{
			if (punchPosition >= punches.Length)
			{
				punchPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			float num = Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume * 1.2f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			PlayCue(slowWhoosh, num, 0f, pan);
		}
	}

	public static void playNextGoreyHitStereo(float pan)
	{
		if (!AudioEnabled)
		{
			return;
		}
		if (punches != null && punches.Length != 0)
		{
			if (punchPosition >= punches.Length)
			{
				punchPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			if (punches != null && punches[punchPosition] != null)
			{
				PlayCue(punches[punchPosition], Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume, 0f, pan);
			}
			punchPosition++;
		}
	}

	public static void playNextClangStereo(float pan)
	{
		if (!AudioEnabled)
		{
			return;
		}
		try
		{
			if (clangPosition >= clangs.Length)
			{
				clangPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			PlayCue(clangs[clangPosition], Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume, -0.5f, pan);
			clangPosition++;
		}
		catch (Exception)
		{
		}
	}

	public static void playNextSplatStereo(float pan)
	{
		if (!AudioEnabled)
		{
			return;
		}
		try
		{
			if (splatPosition >= splats.Length)
			{
				splatPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			PlayCue(splats[splatPosition], Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume * 0.7f, -0.5f, pan);
			splatPosition++;
		}
		catch (Exception)
		{
		}
	}

	public static void playNextFartStereo(float pan)
	{
		if (!AudioEnabled)
		{
			return;
		}
		try
		{
			if (fartPosition >= farts.Length)
			{
				fartPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			PlayCue(farts[fartPosition], Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume * 0.7f, -0.5f, pan);
			fartPosition++;
		}
		catch (Exception)
		{
		}
	}
}
