using System;

namespace BunnyOfWar;

internal static class Program
{
	public static BunnyOfWarGame game;

	private static void Main(string[] args)
	{
		robbyPort.InstallCrashManager();
		try
		{
			using (game = new BunnyOfWarGame())
			{
				game.Run();
			}
		}
		catch (Exception ex)
		{
			robbyPort.ReportCrash("Unhandled game exception", ex);
			Environment.ExitCode = 1;
		}
	}
}
