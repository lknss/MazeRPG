
using System;

namespace MazeRPG.Items
{
    using MazeRPG.Entities;
    public static class LootTable
    {
        private static string[] names = new[] { "Cloth Tunic", "Leather Cap", "Iron Sword", "Mystic Staff", "Gold Ring", "Hardened Plate", "Hunter's Bow", "Silken Robe", "Bronze Shield" };
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

        public static Item ChestRoll(PlayerClass cls, int depth)
        {
            double equipChance = depth <= 2 ? 0.12 : 0.25;
            if (rng.NextDouble() < equipChance)
            {
                var item = new Item { Name = names[rng.Next(names.Length)], Rarity = ItemRarity.Uncommon };
                item.Value = ValueForRarity(item.Rarity);
                if (rng.NextDouble() < 0.06) { item.IsKey = true; item.KeyId = System.Guid.NewGuid().ToString(); item.Name = "Iron Key"; item.Value = 12; }
                return item;
            }
            if (rng.NextDouble() < 0.6) { var itm = new Item { Name = "Health Potion", Rarity = ItemRarity.Common, IsConsumable = true, HealAmount = 20 }; itm.Value = ValueForRarity(itm.Rarity); return itm; }
            if (rng.NextDouble() < 0.2) { var itm = new Item { Name = "Mana Potion", Rarity = ItemRarity.Common, IsConsumable = true, HealAmount = 0 }; itm.Value = ValueForRarity(itm.Rarity); return itm; }
            var coin = new Item { Name = "Gold Coin", Rarity = ItemRarity.Common }; coin.Value = ValueForRarity(coin.Rarity); return coin;
        }

        public static Item? EnemyDrop(PlayerClass cls, int depth)
        {
            if (rng.NextDouble() < 0.35) { var it = new Item { Name = "Coin Pouch", Rarity = ItemRarity.Common }; it.Value = ValueForRarity(it.Rarity); return it; }
            if (rng.NextDouble() < 0.05) { var it = new Item { Name = "Rare Gem", Rarity = ItemRarity.Rare }; it.Value = ValueForRarity(it.Rarity); return it; }
            return null;
        }
    }
}
