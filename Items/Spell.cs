
using System.Collections.Generic;

namespace MazeRPG.Items
{
    public class Spell
    {
        public string Name { get; set; }
        public int ManaCost { get; set; }
        public int Damage { get; set; }
        public int Heal { get; set; }

        public Spell(string name, int cost, int dmg = 0, int heal = 0)
        {
            Name = name; ManaCost = cost; Damage = dmg; Heal = heal;
        }
    }

    public static class SpellBook
    {
        public static List<Spell> DefaultSpells()
        {
            return new List<Spell> {
                new Spell("Magic Missile", 4, dmg: 8),
                new Spell("Fireball", 8, dmg: 18),
                new Spell("Frostbolt", 6, dmg: 12),
                new Spell("Minor Heal", 5, heal: 15),
                new Spell("Arcane Missiles", 10, dmg: 20),
                new Spell("Lightning Bolt", 9, dmg: 17),
                new Spell("Ice Lance", 7, dmg: 13),
                new Spell("Greater Heal", 12, heal: 30),
                new Spell("Mana Shield", 6, heal: 0),
                new Spell("Shadow Bolt", 8, dmg: 16),
                new Spell("Burning Hands", 5, dmg: 10),
                new Spell("Chain Lightning", 14, dmg: 26),
                new Spell("Healing Wave", 10, heal: 22),
                new Spell("Arcane Blast", 11, dmg: 24),
                new Spell("Spark", 3, dmg: 5),
                new Spell("Cone of Cold", 9, dmg: 15),
                new Spell("Meteor", 16, dmg: 35),
                new Spell("Leech", 8, dmg: 10, heal: 8),
                new Spell("Gust", 4, dmg: 6),
                new Spell("Time Warp", 15, dmg: 0)
            };
        }
    }
}
