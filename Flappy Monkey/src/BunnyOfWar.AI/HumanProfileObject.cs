using System.Collections.Generic;
using System.Diagnostics;

namespace BunnyOfWar.AI;

public class HumanProfileObject
{
	public List<FighterManager.CauseOfDeath> VictimsCausesOfDeath = new List<FighterManager.CauseOfDeath>(100);

	public Dictionary<Definitions.FighterSpecialMoves, int> AttacksMade = new Dictionary<Definitions.FighterSpecialMoves, int>(10);

	public Dictionary<Definitions.FighterSpecialMoves, int> AttackLevels = new Dictionary<Definitions.FighterSpecialMoves, int>();

	public int damageDealt = 0;

	public int damageTaken = 0;

	public int healthRegenerated = 0;

	public int kills = 0;

	public int deaths = 0;

	public int revivalsOfTeammate = 0;

	public int shotsFired = 0;

	public int shotsMade = 0;

	public int shotsBlocked = 0;

	public int blocks = 0;

	public int parries = 0;

	public int counters = 0;

	public double timeSpentBlocking = 0.0;

	public double timeSpentPlaying = 0.0;

	public Stopwatch stopwatchTimeSpentPlaying = new Stopwatch();

	public Stopwatch stopwatchTimeSpentBlocking = new Stopwatch();

	public int pushes = 0;

	public int hammerAttacks = 0;

	private static int kMaxAttacksHistory = 100;

	public List<Definitions.FighterSpecialMoves> previousMoves = new List<Definitions.FighterSpecialMoves>(kMaxAttacksHistory);

	public void logAttack(Definitions.FighterSpecialMoves attack)
	{
		while (previousMoves.Count >= kMaxAttacksHistory)
		{
			previousMoves.Remove(previousMoves[0]);
		}
		previousMoves.Add(attack);
	}

	public bool isAttackBeingSpammed(Definitions.FighterSpecialMoves attack, int amount)
	{
		if (previousMoves.Count <= amount)
		{
			return false;
		}
		int num = previousMoves.Count - 1;
		while (num > 0 && num > previousMoves.Count - amount)
		{
			if (previousMoves[num] != attack)
			{
				return false;
			}
			num--;
		}
		return true;
	}
}
