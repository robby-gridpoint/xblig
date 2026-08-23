using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public class SceneryObject
{
	public float Z = -1f;

	public int ID = -1;

	public string name = "";

	public string uniqueName = "";

	public bool isVisible = true;

	public int xRoll = 0;

	public int yRoll = 0;

	public int xRollFighters = 0;

	public int yRollFighters = 0;

	public float partialXRoll = 0f;

	public float partialYRoll = 0f;

	public int DPS = 0;

	public bool isDPSFeetOnly = true;

	public Rectangle rect;

	public NonFighterAI.modes AImode = NonFighterAI.modes.none;

	public int AIAmountSpeed = 0;

	public int AIAmountDistance = 0;

	public string AIMemory = "";

	public int AICounter = 0;

	public Vector2 circlePivotPoint = Vector2.Zero;

	public Vector2 circleVelocity = Vector2.Zero;

	public Vector2 circleProgress = Vector2.Zero;

	private float xPartial = 0f;

	private float yPartial = 0f;

	public bool isFlippedHorizontally = false;

	public bool isFlippedVertically = false;

	public float X
	{
		get
		{
			return rect.X;
		}
		set
		{
			rect.X = (int)(value + xPartial);
			xPartial = value - (float)rect.X;
		}
	}

	public float Y
	{
		get
		{
			return rect.Y;
		}
		set
		{
			rect.Y = (int)(value + yPartial);
			yPartial = value - (float)rect.Y;
		}
	}

	public int width
	{
		get
		{
			return rect.Width;
		}
		set
		{
			rect.Width = value;
		}
	}

	public int height
	{
		get
		{
			return rect.Height;
		}
		set
		{
			rect.Height = value;
		}
	}

	public float getLayerDepth()
	{
		if (Z != -1f)
		{
			return Z;
		}
		return RandomStaticGlobals.getLayerDepth((int)Y, height) + (float)ID * 1E-05f;
	}

	public SceneryObject(int ID)
	{
		ID = ID;
	}

	public SceneryObject(int ID, string Name, int x, int y, int width, int height, bool visible)
	{
		ID = ID;
		name = Name;
		rect = new Rectangle(x, y, width, height);
		isVisible = visible;
	}

	public SceneryObject(int ID, string folder, string Name, int x, int y)
	{
		ID = ID;
		name = Name;
		Texture2D texture2D = GraphicsManager.LoadTexture(folder + Name, cacheResult: true);
		rect = new Rectangle(x, y, texture2D.Width, texture2D.Height);
		isVisible = true;
	}

	public SceneryObject Copy()
	{
		return (SceneryObject)MemberwiseClone();
	}
}
