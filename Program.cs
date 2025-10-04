
using System;
using MazeRPG.UI;
using MazeRPG.Core;

namespace MazeRPG
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "MazeRPG - Console";
            var start = new StartScreen();
            while (true)
            {
                var choice = start.ShowMenu();
                if (choice == StartScreen.MenuOption.NewGame)
                {
                    var gm = new GameManager();
                    gm.StartNewGame();
                }
                else if (choice == StartScreen.MenuOption.LoadGame)
                {
                    var gm = new GameManager();
                    gm.TryLoadGame();
                }
                else if (choice == StartScreen.MenuOption.HighScores)
                {
                    start.ShowHighScores();
                }
                else break;
            }
        }
    }
}
