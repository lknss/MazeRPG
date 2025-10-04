
using System;
using System.IO;
using System.Text.Json;
using MazeRPG.Entities;

namespace MazeRPG.Core
{
    public static class SaveSystem
    {
        private static readonly string SaveFolder = Path.Combine(Environment.CurrentDirectory, "saves");

        public static void EnsureFolder() { if (!Directory.Exists(SaveFolder)) Directory.CreateDirectory(SaveFolder); }

        public static void Save(string slot, Player player)
        {
            EnsureFolder();
            var dto = new
            {
                Name = player.Name,
                Class = player.Class.ToString(),
                Level = player.Level,
                HP = player.HP,
                MaxHP = player.MaxHP,
                Mana = player.Mana,
                MaxMana = player.MaxMana,
                Gold = player.Gold
            };
            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(SaveFolder, slot + ".json"), json);
            ConsoleHelper.Success("Saved to slot: " + slot);
        }

        public static bool Load(string slot, out Player player)
        {
            player = null!;
            var path = Path.Combine(SaveFolder, slot + ".json");
            if (!File.Exists(path)) return false;
            try
            {
                var json = File.ReadAllText(path);
                var doc = JsonDocument.Parse(json).RootElement;
                var name = doc.GetProperty("Name").GetString() ?? "Hero";
                var clsStr = doc.GetProperty("Class").GetString() ?? "Warrior";
                PlayerClass cls = Enum.Parse<PlayerClass>(clsStr);
                player = new Player(name, cls);
                player.Level = doc.GetProperty("Level").GetInt32();
                player.HP = doc.GetProperty("HP").GetInt32();
                player.MaxHP = doc.GetProperty("MaxHP").GetInt32();
                player.Mana = doc.GetProperty("Mana").GetInt32();
                player.MaxMana = doc.GetProperty("MaxMana").GetInt32();
                player.Gold = doc.GetProperty("Gold").GetInt32();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
