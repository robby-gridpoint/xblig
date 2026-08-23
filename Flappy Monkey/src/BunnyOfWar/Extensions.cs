using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public static class Extensions
{
	public static bool IsNotEmpty(this Rectangle r)
	{
		if (r.X != Definitions.EmptyRectangle.X || r.Y != Definitions.EmptyRectangle.Y || r.Width != Definitions.EmptyRectangle.Width || r.Height != Definitions.EmptyRectangle.Height)
		{
			return true;
		}
		return false;
	}
}
