
using System;
using System.IO;

namespace MazeRPG.UI
{
    public class StartScreen
    {
        public enum MenuOption { NewGame = 1, LoadGame = 2, HighScores = 3, Quit = 4 }

        public MenuOption ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("          MAZERPG - HEIGHTS        ");
            Console.WriteLine("===================================");
            Console.WriteLine("[1] New Game");
            Console.WriteLine("[2] Load Game");
            Console.WriteLine("[3] High Scores");
            Console.WriteLine("[4] Quit");
            Console.WriteLine();
            Console.Write("Choice: ");
            var c = Console.ReadLine();
            return c switch { "1" => MenuOption.NewGame, "2" => MenuOption.LoadGame, "3" => MenuOption.HighScores, _ => MenuOption.Quit };
        }

        public void ShowHighScores()
        {
            Console.Clear();
            Console.WriteLine("High Scores");
            var path = Path.Combine(Environment.CurrentDirectory, "highscores.txt");
            if (!File.Exists(path)) { Console.WriteLine("No high scores yet."); Console.WriteLine("Press Enter."); Console.ReadLine(); return; }
            var lines = File.ReadAllLines(path);
            foreach (var l in lines) Console.WriteLine(l);
            Console.WriteLine("Press Enter."); Console.ReadLine();
        }
    }
}
