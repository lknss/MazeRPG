
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MazeRPG.Core;
using MazeRPG.Entities;
using MazeRPG.Items;
using MazeRPG.UI;

namespace MazeRPG.Exploration
{
    public class Dungeon
    {
        private int _width;
        private int _height;
        private Room[,] _rooms;
        private bool[,] _explored;
        private char[,] _minimap;
        private Random _rng;
        private int _depth = 1;
        private int _roomIndex = 1;
        private string _theme = "Crypt";
        private bool _bossAlive = false;

        public Dungeon(int width, int height, Random rng)
        {
            _width = width; _height = height; _rng = rng;
            _rooms = new Room[_width, _height];
            _explored = new bool[_width, _height];
            _minimap = new char[_width, _height];
            Generate();
        }

        private void Generate()
        {
            var themes = RoomDescriptions.ThemeKeys;
            _theme = themes[(_depth - 1) % themes.Length];

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    var r = new Room();
                    r.Description = RoomDescriptions.PickForTheme(_theme, x + y + _depth);
                    r.HasChest = false;
                    r.HasExit = false;
                    r.IsSafe = _rng.NextDouble() < 0.06;
                    if (_rng.NextDouble() < 0.04) r.RequiredKeyId = Guid.NewGuid().ToString();
                    _rooms[x, y] = r;
                }

            int tier = (_depth - 1) / 5;
            double baseChestChance = 0.05 + tier * 0.06;
            for (int x = 0; x < _width; x++) for (int y = 0; y < _height; y++) if (_rng.NextDouble() < baseChestChance) _rooms[x,y].HasChest = true;

            if (_depth <= 2)
            {
                for (int x = 0; x < _width; x++) for (int y = 0; y < _height; y++) _rooms[x,y].HasChest = false;
                if (_rng.NextDouble() < 0.6) _rooms[_rng.Next(_width), _rng.Next(_height)].HasChest = true;
            }

            int ex = _rng.Next(_width), ey = _rng.Next(_height);
            _rooms[ex, ey].HasExit = true;
            if (_depth % 5 == 0)
            {
                _rooms[ex, ey].ExitLocked = true;
                _bossAlive = true;
            }
        }

        public void EnterLoop(Entities.Player player, RunStats stats)
        {
            player.X = _width / 2;
            player.Y = _height / 2;
            MarkVisibleAndRender(player);

            while (player.IsAlive)
            {
                Console.WriteLine();
                ConsoleHelper.Prompt("Actions: move | turn | look | search | inventory | equip | spellbook | stats | rest | save | quit");
                Console.Write("Command: ");
                var cmd = (Console.ReadLine() ?? "").Trim().ToLower();

                var current = _rooms[player.X, player.Y];

                if (cmd == "look") { InspectFacing(player); }
                else if (cmd == "move")
                {
                    ConsoleHelper.Prompt("Direction? (north/south/east/west)"); var dir = (Console.ReadLine() ?? "");
                    if (TryMove(player, dir))
                    {
                        Console.Clear();
                        ConsoleHelper.Color($"You moved {dir}.", ConsoleColor.Cyan);
                        DescribeNearby(player);
                        MarkVisibleAndRender(player);
                        stats.RoomsExplored++;
                        if (_rng.NextDouble() < Math.Min(0.25, 0.06 + _depth * 0.01))
                        {
                            if (_depth % 5 == 0 && _bossAlive)
                            {
                                var boss = Enemy.CreateBossForDepth(_depth, _rng);
                                ConsoleHelper.Danger($"A powerful {boss.Name} appears!");
                                Combat.CombatSystem.Fight(player, boss, _depth, stats, isBoss:true, onBossDefeated: () => { HandleBossDefeat(player, stats); });
                            }
                            else
                            {
                                var enemy = Entities.Enemy.CreateForDepth(_depth, _rng);
                                ConsoleHelper.Danger($"A wild {enemy.Name} appears!");
                                Combat.CombatSystem.Fight(player, enemy, _depth, stats);
                            }
                        }
                    }
                    else ConsoleHelper.Warning("You cannot move that way.");
                }
                else if (cmd == "turn")
                {
                    ConsoleHelper.Prompt("Turn which direction? (north/south/east/west)"); var dir = (Console.ReadLine() ?? "");
                    if (TrySetFacing(player, dir))
                    {
                        Console.Clear();
                        ConsoleHelper.Color($"You turn {dir}.", ConsoleColor.Cyan);
                        InspectFacing(player);
                        MarkVisibleAndRender(player);
                    }
                    else ConsoleHelper.Warning("Invalid direction.");
                }
                else if (cmd == "search")
                {
                    if (HandleSearchAt(player.X, player.Y, player, stats)) continue;
                    var (fx, fy) = player.FacingVector();
                    int tx = player.X + fx, ty = player.Y + fy;
                    if (tx >= 0 && ty >= 0 && tx < _rooms.GetLength(0) && ty < _rooms.GetLength(1))
                    {
                        if (HandleSearchAt(tx, ty, player, stats)) continue;
                    }
                    InspectFacing(player);
                }
                else if (cmd == "inventory") player.ShowInventory(current.IsSafe);
                else if (cmd == "equip") player.ShowEquipmentMenuInteractive();
                else if (cmd == "spellbook") { if (player.IsMage) player.ShowSpellbook(); else ConsoleHelper.Warning("Only mages have a spellbook."); }
                else if (cmd == "stats") player.ShowStats();
                else if (cmd == "rest")
                {
                    if (current.IsSafe)
                    {
                        int cost = 10 * _depth;
                        Console.WriteLine($"Rest for {cost}g? (Y/N)"); var a = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(a) && a.Trim().ToUpper() == "Y" && player.Gold >= cost)
                        {
                            player.Gold -= cost; stats.GoldSpent += cost; player.HP = player.MaxHP; if (player.IsMage) player.Mana = player.MaxMana; ConsoleHelper.Success("You rest and recover.");
                        }
                        else ConsoleHelper.Warning("Cannot rest.");
                    }
                    else ConsoleHelper.Warning("You can only rest in safe zones.");
                }
                else if (cmd == "save") { Core.SaveSystem.Save("autosave", player, stats); ConsoleHelper.Success("Saved."); }
                else if (cmd == "quit") return;
                else ConsoleHelper.Warning("Unknown command.");
            }

            stats.Finish();
        }

        private void HandleBossDefeat(Player player, RunStats stats)
        {
            _bossAlive = false;
            for (int x=0;x<_width;x++) for (int y=0;y<_height;y++) if (_rooms[x,y].HasExit) _rooms[x,y].ExitLocked = false;
            ConsoleHelper.Success($"You have defeated the boss of Depth {_depth}.");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine($"    You have cleared the tier ending at depth {_depth}    ");
            Console.WriteLine(new string('-', 40));
            var bossLoot = LootTable.GenerateGuaranteedBossLoot(player.Class, _depth);
            if (bossLoot != null) { ConsoleHelper.Loot("Boss dropped:"); ConsoleHelper.PrintRarity(bossLoot.Name, bossLoot.Rarity, bossLoot.Value); player.AddToInventory(bossLoot); stats.ItemsFound++; }
            int gold = LootTable.BossGoldForDepth(_depth);
            player.Gold += gold; stats.GoldEarned += gold; ConsoleHelper.Success($"You collect {gold} gold.");
        }

        private bool HandleSearchAt(int rx, int ry, Entities.Player player, RunStats stats)
        {
            var room = _rooms[rx, ry];
            if (room.HasChest)
            {
                var loot = LootTable.ChestRoll(player.Class, _depth);
                if (loot != null) { ConsoleHelper.Loot("You open the chest and find:"); ConsoleHelper.PrintRarity(loot.Name, loot.Rarity, loot.Value); player.AddToInventory(loot); stats.ItemsFound++; }
                else { ConsoleHelper.Prompt("The chest is empty."); }
                room.HasChest = false;
                return true;
            }

            if (room.HasExit)
            {
                if (room.ExitLocked)
                {
                    ConsoleHelper.Warning("The exit is sealed by a powerful force. Defeat the boss to proceed.");
                    return true;
                }
                if (!string.IsNullOrEmpty(room.RequiredKeyId))
                {
                    bool hasKey = player.Inventory.Exists(i => i.IsKey && i.KeyId == room.RequiredKeyId);
                    if (!hasKey)
                    {
                        ConsoleHelper.Warning("This exit is locked. It requires a key to open.");
                        return true;
                    }
                    ConsoleHelper.Prompt("You have the correct key. Use it to open the exit? (y/n)");
                    var ansk = (Console.ReadLine() ?? "").Trim().ToLower();
                    if (ansk == "y")
                    {
                        var keyItem = player.Inventory.Find(i => i.IsKey && i.KeyId == room.RequiredKeyId);
                        if (keyItem != null) { player.Inventory.Remove(keyItem); ConsoleHelper.Success("You used the key and opened the exit."); AdvanceFloor(player, stats); }
                        return true;
                    }
                    else { ConsoleHelper.Prompt("You step back from the locked exit."); return true; }
                }

                ConsoleHelper.Prompt("You found an exit. Go through? (y/n)");
                var ans = (Console.ReadLine() ?? "").Trim().ToLower();
                if (ans == "y") { AdvanceFloor(player, stats); }
                else ConsoleHelper.Prompt("You decide not to go through.");
                return true;
            }

            return false;
        }

        private void AdvanceFloor(Entities.Player player, RunStats stats)
        {
            _depth++; _roomIndex++;
            Generate();
            player.X = _width / 2; player.Y = _height / 2;
            _explored = new bool[_width, _height];
            _minimap = new char[_width, _height];
            Console.Clear();
            MarkVisibleAndRender(player);
            ConsoleHelper.Success($"Descending to depth {_depth}...");
        }

        private bool TrySetFacing(Entities.Player p, string dir)
        {
            switch ((dir ?? "").Trim().ToLower())
            {
                case "north": p.Facing = Entities.PlayerFacing.North; return true;
                case "south": p.Facing = Entities.PlayerFacing.South; return true;
                case "east": p.Facing = Entities.PlayerFacing.East; return true;
                case "west": p.Facing = Entities.PlayerFacing.West; return true;
                default: return false;
            }
        }

        private void InspectFacing(Entities.Player p)
        {
            var (dx, dy) = p.FacingVector();
            int tx = p.X + dx, ty = p.Y + dy;
            if (tx < 0 || ty < 0 || tx >= _rooms.GetLength(0) || ty >= _rooms.GetLength(1))
            {
                ConsoleHelper.Warning("You look out into darkness — nothing there."); return;
            }
            var room = _rooms[tx, ty];
            if (room.HasChest) ConsoleHelper.Loot("You discovered a chest to the " + p.Facing.ToString().ToLower() + ".");
            else if (room.HasExit) ConsoleHelper.Prompt("You see an exit nearby.");
            else ConsoleHelper.Prompt("Nothing of interest.");
        }

        private bool TryMove(Entities.Player p, string dir)
        {
            if (string.IsNullOrWhiteSpace(dir)) return false;
            dir = dir.Trim().ToLower();
            int nx = p.X, ny = p.Y;
            switch (dir)
            {
                case "north": ny--; break;
                case "south": ny++; break;
                case "west": nx--; break;
                case "east": nx++; break;
                default: return false;
            }
            if (nx < 0 || ny < 0 || nx >= _rooms.GetLength(0) || ny >= _rooms.GetLength(1)) return false;
            p.X = nx; p.Y = ny; return true;
        }

        private void DescribeNearby(Entities.Player p)
        {
            var features = new List<string>();
            int r = 3;
            for (int dx = -r; dx <= r; dx++) for (int dy = -r; dy <= r; dy++)
            {
                int x = p.X + dx, y = p.Y + dy;
                if (x < 0 || y < 0 || x >= _rooms.GetLength(0) || y >= _rooms.GetLength(1)) continue;
                var rm = _rooms[x, y];
                if (rm.HasChest) features.Add($"To the {DirectionText(dx,dy)} you see a chest.");
                if (rm.HasExit) features.Add($"To the {DirectionText(dx,dy)} you see an exit.");
            }
            if (features.Count > 0) { foreach (var s in features) Console.WriteLine(s); }
            else { var amb = new[] { "You hear distant dripping.", "A cold draft brushes past.", "The air smells of damp earth.", "Your footsteps echo softly.", "Faint scuttling can be heard far off." }; Console.WriteLine(amb[new Random().Next(amb.Length)]); }
        }

        private string DirectionText(int dx, int dy)
        {
            string dir = "";
            if (dy < 0) dir += "north";
            else if (dy > 0) dir += "south";
            if (dx < 0) dir += (dir != "" ? "-" : "") + "west";
            else if (dx > 0) dir += (dir != "" ? "-" : "") + "east";
            return dir == "" ? "here" : dir;
        }

        private void MarkVisibleAndRender(Entities.Player p)
        {
            int w = _rooms.GetLength(0);
            int h = _rooms.GetLength(1);
            int px = p.X, py = p.Y;
            int r = 3;
            for (int dx = -r; dx <= r; dx++) for (int dy = -r; dy <= r; dy++)
            {
                int x = px + dx, y = py + dy;
                if (x < 0 || y < 0 || x >= w || y >= h) continue;
                _explored[x, y] = true;
                _minimap[x, y] = _rooms[x, y].HasChest ? 'C' : (_rooms[x, y].HasExit ? 'E' : '.');
            }
            var render = new char[w, h];
            for (int x = 0; x < w; x++) for (int y = 0; y < h; y++) render[x, y] = _explored[x, y] ? _minimap[x, y] : '?';
            ConsoleHelper.Header($"Dungeon map (room {_roomIndex} - depth {_depth})");
            ConsoleHelper.RenderMinimapWithHud(render, _explored, px, py, p);
            Console.WriteLine();
            Console.WriteLine(_rooms[px, py].Description);
        }
    }
}
