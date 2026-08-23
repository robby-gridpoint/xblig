using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public struct AnimationPlayer
{
	private Animation animation;

	public Rectangle? rectangleResizeToThis;

	private int frameIndex;

	private float time;

	public Animation Animation => animation;

	public int FrameIndex
	{
		get
		{
			return frameIndex;
		}
		set
		{
			frameIndex = value;
		}
	}

	public void PlayAnimation(Animation animation)
	{
		PlayAnimation(animation, forceRestart: false);
	}

	public void PlayAnimation(Animation animation, bool forceRestart)
	{
		if (Animation != animation || forceRestart)
		{
			this.animation = animation;
			frameIndex = 0;
			time = 0f;
		}
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 positionXY, SpriteEffects spriteEffects, float layerDepth, Rectangle originalRect, float scale2)
	{
		if (RandomStaticGlobals.isGamePaused)
		{
			return;
		}
		if (Animation == null)
		{
			throw new NotSupportedException("No animation is currently playing.");
		}
		time += (float)gameTime.ElapsedGameTime.TotalSeconds;
		while (time > Animation.FrameTime)
		{
			time -= Animation.FrameTime;
			if (Animation.IsLooping)
			{
				frameIndex = (frameIndex + 1) % Animation.FrameCount;
			}
			else
			{
				frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
			}
		}
		Vector2 position = positionXY;
		if (Animation.texturePathList.Count == 0)
		{
			Rectangle value = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);
			position.Y = position.Y - (float)value.Height + (float)originalRect.Height;
			position.X = position.X - (float)(value.Width / 2) + (float)(originalRect.Width / 2);
			Vector2 origin = new Vector2(value.Width / 2, value.Height);
			position.X += origin.X;
			position.Y += origin.Y;
			spriteBatch.Draw(Animation.Texture, position, value, Color.White, Animation.rotationInRadians, origin, scale2, spriteEffects, layerDepth);
		}
		else
		{
			Rectangle value = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);
			string text = Animation.texturePathList[frameIndex].ToString();
			value = GraphicsManager.GetRectangleFromTexture(GraphicsManager.GetTextureFromCache(Animation.texturePathList[frameIndex]));
			position.Y = position.Y - (float)value.Height + (float)originalRect.Height;
			position.X = position.X - (float)(value.Width / 2) + (float)(originalRect.Width / 2);
			Vector2 origin = new Vector2(value.Width / 2, value.Height);
			position.X += origin.X;
			position.Y += origin.Y;
			spriteBatch.Draw(GraphicsManager.GetTextureFromCache(Animation.texturePathList[frameIndex]), position, value, Color.White, Animation.rotationInRadians, origin, scale2, spriteEffects, layerDepth);
		}
	}

	public void DrawOLDworking(GameTime gameTime, SpriteBatch spriteBatch, Vector2 positionXY, SpriteEffects spriteEffects, float layerDepth, Rectangle originalRect, float scale2)
	{
		if (RandomStaticGlobals.isGamePaused)
		{
			return;
		}
		if (Animation == null)
		{
			throw new NotSupportedException("No animation is currently playing.");
		}
		time += (float)gameTime.ElapsedGameTime.TotalSeconds;
		while (time > Animation.FrameTime)
		{
			time -= Animation.FrameTime;
			if (Animation.IsLooping)
			{
				frameIndex = (frameIndex + 1) % Animation.FrameCount;
			}
			else
			{
				frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
			}
		}
		Rectangle value = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);
		Vector2 position = positionXY;
		if (Animation.texturePathList.Count == 0)
		{
			position.Y = position.Y - (float)value.Height + (float)originalRect.Height;
			position.X = position.X - (float)(value.Width / 2) + (float)(originalRect.Width / 2);
			if (!rectangleResizeToThis.HasValue)
			{
				Vector2 origin = new Vector2(value.Width / 2, value.Height);
				position.X += origin.X;
				position.Y += origin.Y;
				spriteBatch.Draw(Animation.Texture, position, value, Color.White, Animation.rotationInRadians, origin, scale2, spriteEffects, layerDepth);
			}
			else
			{
				rectangleResizeToThis = new Rectangle((int)position.X, (int)position.Y, rectangleResizeToThis.Value.Width, rectangleResizeToThis.Value.Height);
				Vector2 origin = new Vector2(rectangleResizeToThis.Value.Width / 2, rectangleResizeToThis.Value.Height);
				rectangleResizeToThis = new Rectangle(rectangleResizeToThis.Value.X + (int)origin.X, rectangleResizeToThis.Value.Y + (int)origin.Y, rectangleResizeToThis.Value.Width, rectangleResizeToThis.Value.Height);
				spriteBatch.Draw(Animation.Texture, rectangleResizeToThis.Value, value, Color.White, Animation.rotationInRadians, origin, spriteEffects, layerDepth);
			}
			return;
		}
		string text = Animation.texturePathList[frameIndex].ToString();
		value = GraphicsManager.GetRectangleFromTexture(GraphicsManager.GetTextureFromCache(Animation.texturePathList[frameIndex]));
		position.Y = position.Y - (float)value.Height + (float)originalRect.Height;
		position.X = position.X - (float)(value.Width / 2) + (float)(originalRect.Width / 2);
		if (!rectangleResizeToThis.HasValue)
		{
			Vector2 origin = new Vector2(value.Width / 2, value.Height);
			position.X += origin.X;
			position.Y += origin.Y;
			spriteBatch.Draw(GraphicsManager.GetTextureFromCache(Animation.texturePathList[frameIndex]), position, value, Color.White, Animation.rotationInRadians, origin, scale2, spriteEffects, layerDepth);
		}
		else
		{
			rectangleResizeToThis = new Rectangle((int)positionXY.X, (int)positionXY.Y, rectangleResizeToThis.Value.Width, rectangleResizeToThis.Value.Height);
			Vector2 origin = new Vector2(rectangleResizeToThis.Value.Width / 2, rectangleResizeToThis.Value.Height);
			rectangleResizeToThis = new Rectangle(rectangleResizeToThis.Value.X + (int)origin.X, rectangleResizeToThis.Value.Y + (int)origin.Y, rectangleResizeToThis.Value.Width, rectangleResizeToThis.Value.Height);
			spriteBatch.Draw(GraphicsManager.GetTextureFromCache(Animation.texturePathList[frameIndex]), rectangleResizeToThis.Value, value, Color.White, Animation.rotationInRadians, origin, spriteEffects, layerDepth);
		}
	}
}
