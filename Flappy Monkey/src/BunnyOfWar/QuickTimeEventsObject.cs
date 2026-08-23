using System;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public class QuickTimeEventsObject
{
	public string name = "";

	public QuickTimeEventsManager.QTEButtons buttons = QuickTimeEventsManager.QTEButtons.nulll;

	public Buttons? theHardestButtonToButton = null;

	public DateTime expires = DateTime.MinValue;

	public int durationInMS = 1;

	public QuickTimeEventsObject()
	{
	}

	public QuickTimeEventsObject(QuickTimeEventsManager.QTEButtons buttons, string QTEName, int durationMS)
	{
		name = QTEName;
		expires = DateTime.Now.AddMilliseconds(durationMS);
		durationInMS = durationMS;
		Random random = new Random(DateTime.Now.Millisecond);
		switch (buttons)
		{
		case QuickTimeEventsManager.QTEButtons.A:
			theHardestButtonToButton = Buttons.A;
			break;
		case QuickTimeEventsManager.QTEButtons.B:
			theHardestButtonToButton = Buttons.B;
			break;
		case QuickTimeEventsManager.QTEButtons.X:
			theHardestButtonToButton = Buttons.X;
			break;
		case QuickTimeEventsManager.QTEButtons.Y:
			theHardestButtonToButton = Buttons.Y;
			break;
		case QuickTimeEventsManager.QTEButtons.AB:
			if (random.Next(100) > 50)
			{
				theHardestButtonToButton = Buttons.A;
			}
			else
			{
				theHardestButtonToButton = Buttons.B;
			}
			break;
		case QuickTimeEventsManager.QTEButtons.XY:
			if (random.Next(100) > 50)
			{
				theHardestButtonToButton = Buttons.X;
			}
			else
			{
				theHardestButtonToButton = Buttons.Y;
			}
			break;
		case QuickTimeEventsManager.QTEButtons.ABXY:
			switch (random.Next(3))
			{
			case 3:
				theHardestButtonToButton = Buttons.A;
				break;
			case 2:
				theHardestButtonToButton = Buttons.B;
				break;
			case 1:
				theHardestButtonToButton = Buttons.X;
				break;
			case 0:
				theHardestButtonToButton = Buttons.Y;
				break;
			}
			break;
		case QuickTimeEventsManager.QTEButtons.down:
			theHardestButtonToButton = Buttons.RightThumbstickDown;
			break;
		case QuickTimeEventsManager.QTEButtons.up:
			theHardestButtonToButton = Buttons.RightThumbstickUp;
			break;
		case QuickTimeEventsManager.QTEButtons.left:
			theHardestButtonToButton = Buttons.RightThumbstickLeft;
			break;
		case QuickTimeEventsManager.QTEButtons.right:
			theHardestButtonToButton = Buttons.RightThumbstickRight;
			break;
		case QuickTimeEventsManager.QTEButtons.leftright:
			if (random.Next(1) == 1)
			{
				theHardestButtonToButton = Buttons.RightThumbstickLeft;
			}
			else
			{
				theHardestButtonToButton = Buttons.RightThumbstickRight;
			}
			break;
		case QuickTimeEventsManager.QTEButtons.updown:
			if (random.Next(1) == 1)
			{
				theHardestButtonToButton = Buttons.RightThumbstickUp;
			}
			else
			{
				theHardestButtonToButton = Buttons.RightThumbstickDown;
			}
			break;
		}
	}
}
