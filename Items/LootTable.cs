
using System;
using System.Collections.Generic;
using MazeRPG.Entities;

namespace MazeRPG.Items
{
    public static class LootTable
    {
        private static string[] names = new[] { "Cloth Tunic", "Leather Cap", "Iron Sword", "Mystic Staff", "Gold Ring", "Hardened Plate", "Hunter's Bow", "Silken Robe", "Bronze Shield", "War Axe", "Arcane Circlet", "Gilded Amulet" };
        private static Random rng = new Random();

        private static int ValueForRarity(ItemRarity r)
        {
            return r switch
            {
                ItemRarity.Common => rng.Next(6, 16),
                ItemRarity.Uncommon => rng.Next(18, 40),
                ItemRarity.Rare => rng.Next(45, 90),
                ItemRarity.Epic => rng.Next(100, 250),
                ItemRarity.Legendary => rng.Next(300, 800),
                _ => 10
            };
        }

        private static void ApplyAffixes(Item it)
        {
            int scale = it.Rarity == ItemRarity.Common ? 1 : it.Rarity == ItemRarity.Uncommon ? 2 : it.Rarity == ItemRarity.Rare ? 3 : it.Rarity == ItemRarity.Epic ? 5 : 8;
            if (rng.NextDouble() < 0.5) it.BonusStr += rng.Next(0, scale);
            if (rng.NextDouble() < 0.5) it.BonusAgi += rng.Next(0, scale);
            if (rng.NextDouble() < 0.5) it.BonusInt += rng.Next(0, scale);
            if (rng.NextDouble() < 0.6) it.BonusDef += rng.Next(0, scale);
            it.Value = ValueForRarity(it.Rarity) + (it.BonusStr+it.BonusAgi+it.BonusInt+it.BonusDef)*5;
        }

        public static Item? ChestRoll(PlayerClass cls, int depth)
        {
            double equipChance = depth <= 2 ? 0.12 : 0.25;
            if (rng.NextDouble() < equipChance)
            {
                var item = new Item { Name = names[rng.Next(names.Length)], Rarity = (ItemRarity)rng.Next(0,4) };
                ApplyAffixes(item);
                if (rng.NextDouble() < 0.06) { item.IsKey = true; item.KeyId = Guid.NewGuid().ToString(); item.Name = "Iron Key"; item.Value = 12; }
                return item;
            }
            if (rng.NextDouble() < 0.6) { var itm = new Item { Name = "Health Potion", Rarity = ItemRarity.Common, IsConsumable = true, HealAmount = 20 }; itm.Value = ValueForRarity(itm.Rarity); return itm; }
            if (rng.NextDouble() < 0.3) { var itm = new Item { Name = "Mana Potion", Rarity = ItemRarity.Common, IsConsumable = true, RestoreMana = 30 }; itm.Value = ValueForRarity(itm.Rarity); return itm; }
            var coin = new Item { Name = "Gold Coin", Rarity = ItemRarity.Common }; coin.Value = ValueForRarity(coin.Rarity); return coin;
        }

        public static Item? EnemyDrop(PlayerClass cls, int depth)
        {
            if (rng.NextDouble() < 0.35) { var it = new Item { Name = "Coin Pouch", Rarity = ItemRarity.Common }; it.Value = ValueForRarity(it.Rarity); return it; }
            if (rng.NextDouble() < 0.05) { var it = new Item { Name = "Rare Gem", Rarity = ItemRarity.Rare }; it.Value = ValueForRarity(it.Rarity); return it; }
            return null;
        }

        public static Item GenerateGuaranteedBossLoot(PlayerClass cls, int depth)
        {
            var it = new Item();
            it.Rarity = depth < 10 ? ItemRarity.Rare : ItemRarity.Epic;
            it.Name = depth < 10 ? "Champion's Blade" : "Warlord's Aegis";
            ApplyAffixes(it);
            return it;
        }

        public static int BossGoldForDepth(int depth)
        {
            int tier = (depth-1)/5;
            return tier switch
            {
                0 => rng.Next(50, 101),
                1 => rng.Next(150, 251),
                2 => rng.Next(300, 501),
                _ => rng.Next(500, 1001)
            };
        }
    }
}
