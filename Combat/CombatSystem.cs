
using System;
using System.Threading;
using MazeRPG.Core;
using MazeRPG.Entities;
using MazeRPG.Items;

namespace MazeRPG.Combat
{
    public static class CombatSystem
    {
        public static void Fight(Player player, Enemy enemy, int depth, RunStats stats, bool isBoss=false, Action? onBossDefeated=null)
        {
            ConsoleHelper.Danger($"Combat started: {enemy.Name} (HP {enemy.HP})");
            while (player.IsAlive && enemy.IsAlive)
            {
                ConsoleHelper.Prompt("Choose action: attack | spell | use | run");
                var cmd = (Console.ReadLine() ?? "").Trim().ToLower();
                if (cmd == "attack")
                {
                    var dmg = player.Attack();
                    bool crit = player.CheckCritical();
                    if (crit) { dmg *= 2; ConsoleHelper.Color("Critical hit!", ConsoleColor.Magenta); stats.CritsLanded++; }
                    enemy.TakeDamage(dmg);
                    ConsoleHelper.Status($"{player.Name} hit {enemy.Name} for {dmg} dmg. Enemy HP: {enemy.HP}/{enemy.MaxHP}");
                }
                else if (cmd == "spell" && player.IsMage)
                {
                    player.ShowSpellbook();
                    ConsoleHelper.Prompt("Cast which spell (name or number)?");
                    var s = (Console.ReadLine() ?? "").Trim();
                    Items.Spell? spell = null;
                    if (int.TryParse(s, out int si) && si>=1 && si<=player.KnownSpells.Count) spell = player.KnownSpells[si-1];
                    else spell = player.KnownSpells.Find(x=>x.Name.Equals(s, StringComparison.OrdinalIgnoreCase));
                    if (spell!=null)
                    {
                        if (player.Mana < spell.ManaCost) { ConsoleHelper.Warning("Not enough mana."); }
                        else
                        {
                            player.Mana -= spell.ManaCost;
                            ConsoleHelper.Color(spell.EffectText, ConsoleColor.Cyan);
                            int dmg = spell.Damage + player.Intellect/2;
                            bool crit = player.CheckCritical();
                            if (crit) { dmg *= 2; ConsoleHelper.Color("Spell critical!", ConsoleColor.Magenta); stats.CritsLanded++; }
                            enemy.TakeDamage(dmg);
                            ConsoleHelper.Color($"{spell.Name} hits for {dmg} damage.", ConsoleColor.Magenta);
                        }
                    }
                    else ConsoleHelper.Warning("Spell not found.");
                }
                else if (cmd == "use")
                {
                    Core.ConsoleHelper.Prompt("Use: (1) Health Potion  (2) Mana Potion  (Enter to cancel)");
                    var pick = Console.ReadLine()?.Trim();
                    if (pick == "1") player.UseConsumable();
                    else if (pick == "2") player.UseManaPotion();
                }
                else if (cmd == "run")
                {
                    ConsoleHelper.Warning("You flee!"); break;
                }
                else { ConsoleHelper.Warning("Unknown action."); }

                if (enemy.IsAlive)
                {
                    Thread.Sleep(400);
                    var edmg = enemy.Attack();
                    bool ecrit = enemy.CheckCritical();
                    if (ecrit) { edmg *= 2; ConsoleHelper.Color("Enemy critical!", ConsoleColor.Red); stats.CritsTaken++; }
                    player.TakeDamage(edmg);
                    ConsoleHelper.Status($"{enemy.Name} hits you for {edmg}. You HP: {player.HP}/{player.MaxHP} Mana:{player.Mana}/{player.MaxMana}");
                }
            }

            if (!enemy.IsAlive)
            {
                ConsoleHelper.Success($"You defeated {enemy.Name}!");
                if (isBoss)
                {
                    onBossDefeated?.Invoke();
                }
                else
                {
                    var drop = LootTable.EnemyDrop(player.Class, depth);
                    if (drop != null) { ConsoleHelper.Loot("Dropped:"); ConsoleHelper.PrintRarity(drop.Name, drop.Rarity, drop.Value); player.AddToInventory(drop); stats.ItemsFound++; }
                    if (new Random().NextDouble() < 0.08) { var key = new Items.Item { Name = "Brass Key", Rarity = ItemRarity.Common, IsKey=true, KeyId = System.Guid.NewGuid().ToString(), Value=12 }; player.AddToInventory(key); ConsoleHelper.Loot($"Enemy dropped a key: {key.Name}"); }
                    stats.MobKills++;
                }
            }
            if (!player.IsAlive) { ConsoleHelper.Danger("You have died."); var ds = new UI.DeathScreen(player, stats); ds.Show(); }
        }
    }
}
