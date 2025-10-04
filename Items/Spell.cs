
using System.Collections.Generic;

namespace MazeRPG.Items
{
    public class Spell
    {
        public string Name { get; set; }
        public int ManaCost { get; set; }
        public int Damage { get; set; }
        public string EffectText { get; set; }

        public Spell(string name, int cost, int dmg, string effect)
        {
            Name = name; ManaCost = cost; Damage = dmg; EffectText = effect;
        }
    }

    public static class SpellBook
    {
        public static List<Spell> DefaultSpells()
        {
            return new List<Spell> {
                new Spell("Magic Missile", 4, 8, "You conjure glowing missiles of arcane force."),
                new Spell("Fireball", 10, 18, "You hurl a blazing fireball that erupts on impact."),
                new Spell("Frostbolt", 7, 12, "A shard of ice streaks from your hand, chilling the target."),
                new Spell("Arcane Missiles", 12, 20, "A flurry of crackling arcane bolts slam into the foe."),
                new Spell("Lightning Bolt", 9, 17, "A jagged bolt of lightning cracks toward the enemy."),
                new Spell("Ice Lance", 8, 13, "A spear of condensed ice flies sharp and fast."),
                new Spell("Shadow Bolt", 8, 16, "A bolt of shadow energy strikes the enemy."),
                new Spell("Chain Lightning", 16, 28, "Lightning arcs and chains between nearby targets."),
                new Spell("Arcane Blast", 11, 24, "A concentrated blast of raw arcane energy.")
            };
        }
    }
}
