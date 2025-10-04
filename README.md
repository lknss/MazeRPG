
MazeRPG - Full Source (raw .cs files)

Drop these files into a new .NET 6 console project (source-only).

Quick start:
1. dotnet new console -f net6.0 -n MazeRPG
2. Replace Program.cs with the provided Program.cs and add the other folders/files preserving the folder structure.
3. Build in Rider / Visual Studio / dotnet build.

What's new in this build:
- Exit interaction: searching the current tile or tile you face will detect exits and prompt to go through or use a key if locked.
- Items now have randomized Value by rarity (sell/buy value). Merchant supports buy/sell.
- All previous features preserved: minimap + HUD, spellbook for mage, equipment, inventory, chests, loot tables, combat, save system, start screen.

If you hit compile errors, paste the first ~30 errors here and I'll patch them.
