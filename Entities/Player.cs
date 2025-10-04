
using System;
using System.Collections.Generic;
using System.Linq;
using MazeRPG.Items;

namespace MazeRPG.Entities
{
    public enum PlayerClass { Warrior, Rogue, Hunter, Mage }
    public enum PlayerFacing { North, South, East, West }

    public class Player
    {
        public string Name { get; set; }
        public PlayerClass Class { get; set; }
        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;
        public int Gold { get; set; } = 20;
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
        public int Defense { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intellect { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public PlayerFacing Facing { get; set; } = PlayerFacing.North;
        public List<Item> Inventory { get; set; } = new List<Item>();
        public Dictionary<string, Item?> Equipment { get; set; } = new Dictionary<string, Item?>();

        private Random _rng = new Random();

        public Player(string name, PlayerClass cls)
        {
            Name = name; Class = cls;
            Strength = cls==PlayerClass.Warrior ? 5 : 2;
            Agility = cls==PlayerClass.Rogue ? 5 : cls==PlayerClass.Hunter ? 3 : 2;
            Intellect = cls==PlayerClass.Mage ? 6 : 2;
            MaxHP = 80 + Strength*12; HP = MaxHP;
            MaxMana = 30 + Intellect*10; Mana = MaxMana;
            Defense = cls==PlayerClass.Warrior ? 10 : cls==PlayerClass.Hunter ? 6 : cls==PlayerClass.Rogue ? 4 : 2;
            Inventory.Add(new Item{ Name="Small Health Potion", Rarity=ItemRarity.Common, IsConsumable=true, HealAmount=25, Value=8 });
            KnownSpells = new List<Items.Spell>();
            if (Class==PlayerClass.Mage)
            {
                var spells = Items.SpellBook.DefaultSpells();
                var pick = spells[_rng.Next(spells.Count)];
                KnownSpells.Add(pick);
            }
            Equipment["helm"] = null; Equipment["chest"] = null; Equipment["legs"] = null; Equipment["hands"] = null; Equipment["weapon"] = null; Equipment["shield"] = null; Equipment["ring1"] = null; Equipment["ring2"] = null; Equipment["amulet"] = null;
        }

        public bool IsAlive => HP>0;
        public bool IsMage => Class==PlayerClass.Mage;
        public List<Items.Spell> KnownSpells { get; set; }

        public (int dx, int dy) FacingVector() => Facing switch { PlayerFacing.North => (0,-1), PlayerFacing.South => (0,1), PlayerFacing.East => (1,0), PlayerFacing.West => (-1,0), _ => (0,0) };

        public int Attack()
        {
            int baseD = Class==PlayerClass.Warrior ? 12 : Class==PlayerClass.Hunter ? 9 : Class==PlayerClass.Rogue ? 8 : 6;
            int statBonus = Class==PlayerClass.Warrior ? Strength : Class==PlayerClass.Rogue ? Agility : Class==PlayerClass.Hunter ? Agility : Intellect;
            int dmg = baseD + Level + statBonus + _rng.Next(0,4);
            return Math.Max(1, dmg - (Defense/10));
        }

        public void TakeDamage(int dmg)
        {
            int reduced = Math.Max(0, dmg - (Defense/10));
            HP -= reduced;
        }

        public bool CheckCritical()
        {
            double baseChance = 0.05; baseChance += Agility * 0.01; return _rng.NextDouble() < baseChance;
        }

        public void UseConsumable()
        {
            var pot = Inventory.FirstOrDefault(i=>i.IsConsumable && i.HealAmount>0);
            if (pot==null) { Core.ConsoleHelper.Warning("No usable consumable."); return; }
            HP = Math.Min(MaxHP, HP + pot.HealAmount); Inventory.Remove(pot); Core.ConsoleHelper.Success($"You used {pot.Name} and recovered {pot.HealAmount} HP.");
        }

        public void AddToInventory(Item item) => Inventory.Add(item);

        public void LearnSpell(Items.Spell s) { if (KnownSpells.Any(x=>x.Name==s.Name)) return; KnownSpells.Add(s); Core.ConsoleHelper.Success($"Learned spell: {s.Name}"); }

        public bool CastSpell(string name, Enemy target)
        {
            var sp = KnownSpells.FirstOrDefault(s=>s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); if (sp==null) { Core.ConsoleHelper.Warning("You don't know that spell."); return false; }
            if (Mana < sp.ManaCost) { Core.ConsoleHelper.Warning("Not enough mana."); return false; }
            Mana -= sp.ManaCost;
            if (sp.Damage>0) target.TakeDamage(sp.Damage + Intellect/2);
            if (sp.Heal>0) HP = Math.Min(MaxHP, HP + sp.Heal + Intellect/2);
            return true;
        }

        public void ShowInventory(bool inSafeRoom)
        {
            Core.ConsoleHelper.Header("Inventory");
            for (int i=0;i<Inventory.Count;i++){ var it = Inventory[i]; Core.ConsoleHelper.PrintRarity($"{i+1}. {it.Name}", it.Rarity, it.Value); }
            if (Inventory.Count==0) Core.ConsoleHelper.Prompt("Empty.");
            Core.ConsoleHelper.Prompt("Type the item number to equip/use, or press Enter to go back.");
            var input = Console.ReadLine();
            if (int.TryParse(input, out int idx) && idx>=1 && idx<=Inventory.Count)
            {
                var chosen = Inventory[idx-1];
                if (chosen.IsConsumable && chosen.HealAmount>0) { HP = Math.Min(MaxHP, HP + chosen.HealAmount); Inventory.RemoveAt(idx-1); Core.ConsoleHelper.Success($"You used {chosen.Name} and healed {chosen.HealAmount} HP."); }
                else { Core.ConsoleHelper.Prompt("Cannot directly equip from here. Use 'equip' to manage equipment."); }
            }
        }

        public void ShowEquipmentMenuInteractive()
        {
            Core.ConsoleHelper.Header("Equipment - slots"); var keys = Equipment.Keys.ToArray();
            for (int i=0;i<keys.Length;i++){ var k=keys[i]; var it=Equipment[k]; Core.ConsoleHelper.PrintRarity($"{i+1}. {k}: {(it==null?"(empty)":it.Name)}", it?.Rarity ?? ItemRarity.Common, it?.Value ?? 0); }
            Core.ConsoleHelper.Prompt("Type slot number to equip an item from inventory, or Enter to back."); var input = Console.ReadLine();
            if (int.TryParse(input, out int sidx) && sidx>=1 && sidx<=keys.Length)
            {
                var slot = keys[sidx-1]; Console.WriteLine("Choose inventory item number to equip into this slot:"); for (int i=0;i<Inventory.Count;i++) Console.WriteLine($"{i+1}. {Inventory[i].Name} ({Inventory[i].Rarity}) - {Inventory[i].Value}g"); var ii = Console.ReadLine();
                if (int.TryParse(ii, out int invIdx) && invIdx>=1 && invIdx<=Inventory.Count) { var item = Inventory[invIdx-1]; Equipment[slot] = item; Inventory.RemoveAt(invIdx-1); Core.ConsoleHelper.Success($"Equipped {item.Name} into {slot}."); }
            }
        }

        public void ShowSpellbook() { Core.ConsoleHelper.Header("Spellbook"); if (!IsMage) { Core.ConsoleHelper.Warning("You are not a mage."); return; } for (int i=0;i<KnownSpells.Count;i++){ var s = KnownSpells[i]; Console.WriteLine($"{i+1}. {s.Name} - Cost: {s.ManaCost}  Damage: {s.Damage}  Heal: {s.Heal}"); } if (KnownSpells.Count==0) Console.WriteLine("You know no spells."); }

        public void ShowStats() { Core.ConsoleHelper.Header("Stats"); Console.WriteLine($"Class: {Class} Level: {Level} XP: {XP}"); Console.WriteLine($"HP: {HP}/{MaxHP}   Mana: {Mana}/{MaxMana}"); Console.WriteLine($"STR: {Strength}  AGI: {Agility}  INT: {Intellect}  DEF: {Defense}"); Console.WriteLine($"Gold: {Gold}"); }

        public void ShowEquipmentMenu() { Core.ConsoleHelper.Header("Equipment (quick view)"); foreach(var kv in Equipment) Console.WriteLine($"{kv.Key}: {kv.Value?.Name ?? "(empty)"}"); }
    }
}
