using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace BunnyOfWar;

public static class FileManager
{
	public delegate void delegateLoadFileCallBack(string data);

	private static StorageDevice storageDevice = null;

	private static bool isContainerLocked = false;

	private static StorageContainer container = null;

	private static bool storageSelectionAttempted = false;

	private static string levelData = "";

	private static string pathToAwardments = "awardments.txt";

	private static string newDataAwardments = "";

	public static void SaveProgress(string level)
	{
	}

	public static void BeSavedAppend(string fileName, string fileData)
	{
		BeSaved2(fileName, fileData, FileMode.Append);
	}

	public static void BeSavedWrite(string fileName, string fileData)
	{
		BeSaved2(fileName, fileData, FileMode.CreateNew);
	}

	public static void BeSaved2(string fileName, string fileData, FileMode writeOrAppend)
	{
		fileData += '\n';
		Select360StorageDevice();
		try
		{
			if (storageDevice == null || !storageDevice.IsConnected)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.AddSeconds(Definitions.MaximumWaitSaveSeconds);
			while (isContainerLocked)
			{
				if (DateTime.Now > dateTime)
				{
					return;
				}
			}
			isContainerLocked = true;
			if (container != null)
			{
				while (!container.IsDisposed)
				{
					if (DateTime.Now > dateTime)
					{
						return;
					}
				}
			}
			while (Guide.IsVisible)
			{
				if (DateTime.Now > dateTime)
				{
					return;
				}
			}
			if (storageDevice == null || !storageDevice.IsConnected)
			{
				return;
			}
			IAsyncResult asyncResult = storageDevice.BeginOpenContainer("Minotaur", null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			container = storageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			if (container.FileExists(fileName) && writeOrAppend == FileMode.Append)
			{
				using StreamWriter streamWriter = new StreamWriter(container.OpenFile(fileName, writeOrAppend));
				streamWriter.Write(fileData);
			}
			else
			{
				using StreamWriter streamWriter = new StreamWriter(container.CreateFile(fileName));
				streamWriter.Write(fileData);
			}
			container.Dispose();
			isContainerLocked = false;
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static void Select360StorageDevice()
	{
		try
		{
			if (storageDevice != null && storageDevice.IsConnected)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.AddSeconds(Definitions.MaximumWaitSaveSeconds);
			if (container != null)
			{
				while (!container.IsDisposed)
				{
					if (DateTime.Now > dateTime)
					{
						return;
					}
				}
			}
			while (Guide.IsVisible)
			{
				if (DateTime.Now > dateTime)
				{
					return;
				}
			}
			if (storageDevice != null && !storageDevice.IsConnected)
			{
				return;
			}
			StorageDevice.BeginShowSelector(delegate(IAsyncResult asyncResult)
			{
				storageDevice = StorageDevice.EndShowSelector(asyncResult);
				if (!storageSelectionAttempted)
				{
					storageSelectionAttempted = true;
					if (storageDevice != null)
					{
						Definitions.Options.Load();
						LoadProgress();
						LoadHighScores();
					}
				}
			}, null);
			while (storageDevice == null && !(DateTime.Now > dateTime))
			{
			}
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static void ReadToMe(string fileName, delegateLoadFileCallBack delegated)
	{
		Select360StorageDevice();
		try
		{
			if (storageDevice == null || !storageDevice.IsConnected)
			{
				return;
			}
			string text = "";
			IAsyncResult asyncResult = storageDevice.BeginOpenContainer("Minotaur", null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			container = storageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			if (container.FileExists(fileName))
			{
				using (StreamReader streamReader = new StreamReader(container.OpenFile(fileName, FileMode.Open)))
				{
					while (!streamReader.EndOfStream)
					{
						text = text + streamReader.ReadLine() + '\n';
					}
				}
				delegated(text);
			}
			else
			{
				delegated("");
			}
			container.Dispose();
			isContainerLocked = false;
		}
		catch (Exception ex)
		{
			string message = ex.Message;
		}
	}

	public static string ReadToMeWindows(string filePath)
	{
		if (!File.Exists(filePath))
		{
			Console.WriteLine("{0} does not exist.", filePath);
			return "";
		}
		string text = "";
		StreamReader streamReader = File.OpenText(filePath);
		string text2;
		while ((text2 = streamReader.ReadLine()) != null)
		{
			text = text + text2 + "\n";
		}
		text = text.Trim();
		Console.WriteLine("The end of the stream has been reached.");
		streamReader.Close();
		return text;
	}

	public static void SavePhoneFile(string filename, string data)
	{
	}

	public static string ReadToMePhone(string filepath)
	{
		return "";
	}

	public static void LoadProgress()
	{
	}

	public static void LoadProgressCallback(string data)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = data.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			dictionary[array[i]] = array[i];
		}
		RandomStaticGlobals.GameProgress = dictionary;
	}

	public static void SaveHighScores()
	{
		if (!RandomStaticGlobals.IsTrial() && RandomStaticGlobals.ScoreCurrent >= RandomStaticGlobals.ScoreAllTimeHigh)
		{
			RandomStaticGlobals.ScoreAllTimeHigh = RandomStaticGlobals.ScoreCurrent;
			BeSavedAppend("highscores.txt", RandomStaticGlobals.ScoreCurrent.ToString());
		}
	}

	public static void LoadHighScores()
	{
		if (!RandomStaticGlobals.IsTrial())
		{
			delegateLoadFileCallBack delegated = LoadHighScoresCallback;
			ReadToMe("highscores.txt", delegated);
		}
	}

	private static void LoadHighScoresCallback(string data)
	{
		if (data == null || data == "")
		{
			return;
		}
		string[] array = data.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			try
			{
				int num = int.Parse(array[i].Trim());
				if (num > RandomStaticGlobals.ScoreAllTimeHigh)
				{
					RandomStaticGlobals.ScoreAllTimeHigh = num;
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public static void SaveAwardments(string awardmentData)
	{
		newDataAwardments += awardmentData;
	}

	private static void SaveAwardmentsCallback(IAsyncResult result)
	{
	}

	public static void LoadAwardments()
	{
		if (storageDevice != null && storageDevice.IsConnected)
		{
			LoadAwardmentsCallback(null);
		}
		else
		{
			StorageDevice.BeginShowSelector(LoadAwardmentsCallback, null);
		}
	}

	private static void LoadAwardmentsCallback(IAsyncResult result)
	{
	}

	public static void Screenshot()
	{
		Texture2D texture2D = new Texture2D(GraphicsManager.graphics.GraphicsDevice, GraphicsManager.graphics.GraphicsDevice.Viewport.Width, GraphicsManager.graphics.GraphicsDevice.Viewport.Height, mipMap: false, GraphicsManager.graphics.GraphicsDevice.DisplayMode.Format);
		Color[] data = new Color[GraphicsManager.graphics.GraphicsDevice.Viewport.Width * GraphicsManager.graphics.GraphicsDevice.Viewport.Height];
		GraphicsManager.graphics.GraphicsDevice.GetBackBufferData(data);
		texture2D.SetData(data);
		using (Stream stream = File.OpenWrite("C:\\Users\\MacManzo\\Desktop\\awesome\\screenshot.png"))
		{
			texture2D.SaveAsPng(stream, 1920, 1080);
		}
		using Stream jpegStream = File.OpenWrite("C:\\Users\\MacManzo\\Desktop\\awesome\\screenshot.jpg");
		texture2D.SaveAsJpeg(jpegStream, 1920, 1080);
	}
}
