
using System;
using MazeRPG.Exploration;
using MazeRPG.Entities;

namespace MazeRPG.Core
{
    public class GameManager
    {
        private Player? _player;
        private RunStats _stats;

        public GameManager()
        {
            _stats = new RunStats();
        }

        public void StartNewGame()
        {
            Console.Clear();
            ConsoleHelper.Header("New Game - Character Creation");

            ConsoleHelper.Prompt("Enter your name:");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name)) name = "Hero";

            ConsoleHelper.Prompt("Choose class: (1) Warrior  (2) Rogue  (3) Hunter  (4) Mage");
            var c = (Console.ReadLine() ?? "").Trim();
            PlayerClass cls = c switch
            {
                "2" => PlayerClass.Rogue,
                "3" => PlayerClass.Hunter,
                "4" => PlayerClass.Mage,
                _ => PlayerClass.Warrior
            };

            _player = new Player(name, cls);
            ConsoleHelper.Success($"Welcome {_player.Name} the {_player.Class}!");

            if (_player.IsMage && _player.KnownSpells.Count > 0)
            {
                var s = _player.KnownSpells[0];
                ConsoleHelper.Prompt($"You begin with the spell: {s.Name} (Cost: {s.ManaCost})");
            }

            var rng = new Random();
            var dungeon = new Dungeon(width: 9, height: 7, rng: rng);
            Console.Clear();
            dungeon.EnterLoop(_player, _stats);
        }

        public bool TryLoadGame()
        {
            ConsoleHelper.Prompt("Enter save slot name to load (or blank to cancel):");
            var slot = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(slot)) return false;

            if (SaveSystem.Load(slot, out Player loaded))
            {
                _player = loaded;
                ConsoleHelper.Success("Loaded save.");
                var rng = new Random();
                var dungeon = new Dungeon(9, 7, rng);
                dungeon.EnterLoop(_player, _stats);
                return true;
            }

            ConsoleHelper.Warning("Load failed or slot not found.");
            return false;
        }
    }
}
