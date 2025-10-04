
namespace MazeRPG.Core
{
    public class RunStats
    {
        public int RoomsExplored { get; set; }
        public int MobKills { get; set; }
        public int ItemsFound { get; set; }
        public int GoldEarned { get; set; }
        public int GoldSpent { get; set; }
        public int CritsLanded { get; set; }
        public int CritsTaken { get; set; }
        public void Finish() { }
    }
}
