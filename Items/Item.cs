
namespace MazeRPG.Items
{
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }

    public class Item
    {
        public string Name { get; set; } = "";
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
        public bool IsConsumable { get; set; } = false;
        public int HealAmount { get; set; } = 0;
        public int RestoreMana { get; set; } = 0;
        public bool IsKey { get; set; } = false;
        public string? KeyId { get; set; }
        public int Value { get; set; } = 1;
        public int BonusStr { get; set; } = 0;
        public int BonusAgi { get; set; } = 0;
        public int BonusInt { get; set; } = 0;
        public int BonusDef { get; set; } = 0;
    }
}
