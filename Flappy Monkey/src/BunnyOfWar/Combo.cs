using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public class Combo
{
	public string Name;

	public Definitions.FighterSpecialMoves SpecialMove;

	public Buttons[] Sequence;

	public bool IsSubMove;

	public bool enabled = false;

	public Combo(Definitions.FighterSpecialMoves specialMove, params Buttons[] sequence)
	{
		SpecialMove = specialMove;
		Sequence = sequence;
	}
}
