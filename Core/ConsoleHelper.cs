
using System;
using MazeRPG.Items;
using MazeRPG.Entities;

namespace MazeRPG.Core
{
    public static class ConsoleHelper
    {
        public static void Header(string text)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("=== " + text + " ===");
            Console.ForegroundColor = prev;
        }

        public static void Prompt(string text)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(text);
            Console.ForegroundColor = prev;
        }

        public static void Warning(string text)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[!] " + text);
            Console.ForegroundColor = prev;
        }

        public static void Success(string text)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[✓] " + text);
            Console.ForegroundColor = prev;
        }

        public static void Danger(string text)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[X] " + text);
            Console.ForegroundColor = prev;
        }

        public static void Loot(string text)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[Loot] " + text);
            Console.ForegroundColor = prev;
        }

        public static void Color(string text, ConsoleColor color)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = prev;
        }

        public static void PrintRarity(string name, Items.ItemRarity rarity, int value = 0)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = rarity switch
            {
                ItemRarity.Common => ConsoleColor.Gray,
                ItemRarity.Uncommon => ConsoleColor.Green,
                ItemRarity.Rare => ConsoleColor.Blue,
                ItemRarity.Epic => ConsoleColor.Magenta,
                ItemRarity.Legendary => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };
            if (value > 0) Console.WriteLine($"{name} ({value}g)");
            else Console.WriteLine(name);
            Console.ForegroundColor = prev;
        }

        public static void Status(string text)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(text);
            Console.ForegroundColor = prev;
        }

        public static void RenderMinimapWithHud(char[,] map, bool[,] explored, int px, int py, MazeRPG.Entities.Player p)
        {
            int w = map.GetLength(0), h = map.GetLength(1);
            Console.Write("  +"); for (int i = 0; i < w; i++) Console.Write("-"); Console.WriteLine("+");

            for (int y = 0; y < h; y++)
            {
                Console.Write("  |");
                for (int x = 0; x < w; x++)
                {
                    if (x == px && y == py)
                    {
                        var prev = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write('@');
                        Console.ForegroundColor = prev;
                    }
                    else if (!explored[x, y]) Console.Write('?');
                    else Console.Write(map[x, y]);
                }
                Console.WriteLine("|");
            }

            Console.Write("  +"); for (int i = 0; i < w; i++) Console.Write("-"); Console.WriteLine("+");

            string compass = p.Facing switch { Entities.PlayerFacing.North => "N", Entities.PlayerFacing.South => "S", Entities.PlayerFacing.East => "E", Entities.PlayerFacing.West => "W", _ => "?" };
            Console.WriteLine($"  Compass: {compass}    Legend: @=You .=Floor C=Chest E=Exit");
            var prevc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"HP: {p.HP}/{p.MaxHP}  ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"Mana: {p.Mana}/{p.MaxMana}");
            Console.ForegroundColor = prevc;
            Console.WriteLine();
        }
    }
}
