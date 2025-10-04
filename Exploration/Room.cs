
namespace MazeRPG.Exploration
{
    public class Room
    {
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;
        public bool HasChest { get; set; }
        public bool HasExit { get; set; }
        public bool IsSafe { get; set; }
        public string? RequiredKeyId { get; set; }
        public string Description { get; set; } = "A plain room.";
    }
}
