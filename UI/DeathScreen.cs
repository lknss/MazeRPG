
using System;
using MazeRPG.Core;
using MazeRPG.Entities;

namespace MazeRPG.UI
{
    public class DeathScreen
    {
        private Player _p;
        private RunStats _s;
        public DeathScreen(Player p, RunStats s) { _p = p; _s = s; }

        public void Show()
        {
            Console.Clear();
            ConsoleHelper.Danger("You have died.");
            Console.WriteLine($"Rooms explored: {_s.RoomsExplored}");
            Console.WriteLine($"Mobs killed: {_s.MobKills}");
            Console.WriteLine($"Items found: {_s.ItemsFound}");
            Console.WriteLine($"Gold earned: {_s.GoldEarned}  Gold spent: {_s.GoldSpent}");
            Console.WriteLine($"Crits landed: {_s.CritsLanded}  Crits taken: {_s.CritsTaken}");
            ConsoleHelper.Prompt("Press Enter to exit."); Console.ReadLine(); Environment.Exit(0);
        }
    }
}
