using System;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public class LootsObject
{
	public Vector2 xy = Vector2.One;

	public int height = 10;

	public int width = 10;

	public string imagePath = "";

	public DateTime expires = DateTime.MaxValue;

	public bool isActive = false;

	public int hp = 0;

	public int moneys = 0;

	public int X
	{
		get
		{
			return (int)xy.X;
		}
		set
		{
			xy.X = value;
		}
	}

	public int Y
	{
		get
		{
			return (int)xy.Y;
		}
		set
		{
			xy.Y = value;
		}
	}

	public LootsObject Copy()
	{
		return (LootsObject)MemberwiseClone();
	}
}
