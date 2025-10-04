
using System;
using System.Collections.Generic;
using MazeRPG.Items;

namespace MazeRPG.Entities
{
    public class Merchant
    {
        public List<Item> Stock { get; } = new List<Item>();
        private Random _rng = new Random();

        public Merchant(PlayerClass cls, int depth)
        {
            if (cls == PlayerClass.Mage) Stock.Add(new Item { Name="Mana Potion", Rarity=ItemRarity.Common, IsConsumable=true, RestoreMana=30, Value=18 });
            Stock.Add(new Item { Name="Health Potion", Rarity=ItemRarity.Common, IsConsumable=true, HealAmount=30, Value=12 });
            if (cls!=PlayerClass.Mage) Stock.Add(new Item { Name="Sturdy Boots", Rarity=ItemRarity.Uncommon, Value=30 });
            for (int i=0;i<2;i++)
            {
                var it = LootTable.ChestRoll(cls, depth);
                if (it!=null) Stock.Add(it);
            }
        }

        public void Show(Entities.Player player)
        {
            Core.ConsoleHelper.Header("Merchant");
            for (int i=0;i<Stock.Count;i++) { Core.ConsoleHelper.PrintRarity($"{i+1}. {Stock[i].Name}", Stock[i].Rarity, Stock[i].Value); }
            Console.WriteLine("[S] Sell an item  [B] Buy an item  [Q] Quit");
            var input = (Console.ReadLine() ?? "").Trim().ToLower();
            if (input == "b" || input == "buy")
            {
                Core.ConsoleHelper.Prompt("Type item number to buy:");
                var sel = Console.ReadLine();
                if (int.TryParse(sel, out int idx) && idx>=1 && idx<=Stock.Count)
                {
                    var it = Stock[idx-1];
                    if (player.Gold >= it.Value) { player.Gold -= it.Value; player.AddToInventory(it); Core.ConsoleHelper.Success($"Purchased {it.Name} for {it.Value}g."); }
                    else Core.ConsoleHelper.Warning("Not enough gold.");
                }
            }
            else if (input == "s" || input == "sell")
            {
                Core.ConsoleHelper.Prompt("Type inventory item number to sell:");
                for (int i=0;i<player.Inventory.Count;i++) Core.ConsoleHelper.PrintRarity($"{i+1}. {player.Inventory[i].Name}", player.Inventory[i].Rarity, player.Inventory[i].Value);
                var sel = Console.ReadLine();
                if (int.TryParse(sel, out int idx) && idx>=1 && idx<=player.Inventory.Count)
                {
                    var it = player.Inventory[idx-1];
                    int price = Math.Max(1, it.Value/2);
                    player.Inventory.RemoveAt(idx-1);
                    player.Gold += price;
                    Core.ConsoleHelper.Success($"Sold {it.Name} for {price}g.");
                }
            }
        }
    }
}
