using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar.Screens;

public class NetworkBusyScreen
{
	private const float busyTextureScale = 0.8f;

	private string message;

	private IAsyncResult asyncResult;

	private Texture2D busyTexture;

	public byte TransitionAlpha => 180;

	public event EventHandler<Networking.OperationCompletedEventArgs> OperationCompleted;

	public NetworkBusyScreen(string message, IAsyncResult asyncResult)
	{
		this.message = message;
		this.asyncResult = asyncResult;
		LoadContent();
	}

	public void LoadContent()
	{
		busyTexture = RandomStaticGlobals.Content.Load<Texture2D>("colors/busy");
	}

	public void Update()
	{
		if (asyncResult != null && asyncResult.IsCompleted)
		{
			if (OperationCompleted != null)
			{
				OperationCompleted(this, new Networking.OperationCompletedEventArgs(asyncResult));
			}
			asyncResult = null;
		}
	}

	public void Draw(GameTime gameTime)
	{
		SpriteFont font = GraphicsManager.font;
		Vector2 screenFullSize = GraphicsManager.ScreenFullSize;
		Vector2 vector = font.MeasureString(message);
		Vector2 vector2 = new Vector2((float)busyTexture.Width * 0.8f);
		Vector2 origin = new Vector2(busyTexture.Width / 2, busyTexture.Height / 2);
		vector.X = Math.Max(vector.X, vector2.X);
		vector.Y += vector2.Y + 16f;
		Vector2 vector3 = (screenFullSize - vector) / 2f;
		Rectangle rectangle = new Rectangle((int)vector3.X - 32, (int)vector3.Y - 16, (int)vector.X + 64, (int)vector.Y + 32);
		Color color = new Color(255, 255, 255, 187);
		Rectangle rectangle2 = new Rectangle(rectangle.X - 1, rectangle.Y - 1, rectangle.Width + 2, rectangle.Height + 2);
		GraphicsManager.DrawRectangle(rectangle2, new Color(128, 128, 128, (byte)(192f * (float)(int)TransitionAlpha / 255f)));
		GraphicsManager.DrawRectangle(rectangle, new Color(0, 0, 0, (byte)(232f * (float)(int)TransitionAlpha / 255f)));
		GraphicsManager.DrawString((int)vector3.X, (int)vector3.Y, message, color, GraphicsManager.font);
		float rotation = (float)gameTime.TotalGameTime.TotalSeconds * 3f;
		Vector2 position = new Vector2(vector3.X + vector.X / 2f, vector3.Y + vector.Y - vector2.Y / 2f);
		GraphicsManager.spriteBatch.Draw(busyTexture, position, null, color, rotation, origin, 0.8f, SpriteEffects.None, 0f);
		if (!RandomStaticGlobals.isGamePaused)
		{
			GraphicsManager.Draw(GraphicsManager.imgNiceBackground, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}
	}
}
