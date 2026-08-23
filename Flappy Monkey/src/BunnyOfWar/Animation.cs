using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public class Animation
{
	private string texturePath = "";

	public List<string> texturePathList = new List<string>(10);

	public float FrameTime;

	private bool isLooping;

	public float rotationInRadians = 0f;

	public Vector2 Origin = new Vector2(0f, 0f);

	public float scale = 1f;

	public Texture2D Texture => GraphicsManager.GetTextureFromCache(texturePath);

	public bool IsLooping => isLooping;

	public int FrameCount
	{
		get
		{
			if (texturePathList.Count == 0)
			{
				return Texture.Width / FrameWidth;
			}
			return texturePathList.Count;
		}
	}

	public int Width => Texture.Height;

	public int Height => Texture.Height;

	public int FrameWidth => Texture.Height;

	public int FrameHeight
	{
		get
		{
			if (Texture != null)
			{
				return Texture.Height;
			}
			return -1;
		}
	}

	public Animation(string texturePath, float frameTime, bool isLooping, float scale)
	{
		this.texturePath = texturePath;
		FrameTime = frameTime;
		this.isLooping = isLooping;
		this.scale = scale;
		GraphicsManager.LoadTexture(texturePath, cacheResult: true);
	}

	public Animation(string[] animationFileNames, float frameTime, bool isLooping, float scale)
	{
		texturePathList.Clear();
		for (int i = 0; i < animationFileNames.Length; i++)
		{
			texturePathList.Add(animationFileNames[i]);
			GraphicsManager.LoadTexture(animationFileNames[i], cacheResult: true);
		}
		texturePath = texturePathList[0];
		FrameTime = frameTime;
		this.isLooping = isLooping;
		this.scale = scale;
	}
}
