
using System.Collections.Generic;

namespace MazeRPG.Exploration
{
    public static class RoomDescriptions
    {
        public static readonly Dictionary<string, List<string>> Themes = new Dictionary<string, List<string>>()
        {
            { "Crypt", new List<string> {
                "A cold crypt with mildew and old stone.", "A chamber lined with ancient sarcophagi.", "A low vault where whispers seem to linger."
            }},
            { "Cavern", new List<string> {
                "A dripping cavern with mineral veins.", "A wide grotto with faint echoes.", "A stony hollow with stalagmites."
            }},
            { "Vault", new List<string> {
                "A vault with iron bands and rusted chains.", "A storeroom with iron racks and broken chests.", "A low-lit chamber smelling of old coin."
            }},
            { "Halls", new List<string> {
                "A colonnaded hall with shadowy corners.", "A wide gallery with cracked tiles.", "A long corridor where your footfalls echo."
            }}
        };

        public static string PickForTheme(string theme, int idx)
        {
            if (!Themes.ContainsKey(theme)) return "An empty room.";
            var list = Themes[theme];
            return list[idx % list.Count];
        }

        public static string[] ThemeKeys => new string[] { "Crypt", "Cavern", "Vault", "Halls" };
    }
}
