
using System;

namespace MazeRPG.Entities
{
    public class Enemy
    {
        public string Name { get; set; } = "Grunt";
        public int HP { get; set; } = 20;
        public int MaxHP { get; set; } = 20;
        public int Level { get; set; } = 1;
        private Random _rng = new Random();

        public bool IsAlive => HP>0;

        public int Attack() { return 4 + _rng.Next(0,4) + Level; }

        public void TakeDamage(int dmg) { HP -= dmg; }

        public bool CheckCritical() => _rng.NextDouble() < 0.06 + (Level * 0.01);

        public static Enemy CreateForDepth(int depth, Random rng)
        {
            var e = new Enemy(); e.Level = depth; e.MaxHP = 10 + depth * 6; e.HP = e.MaxHP; e.Name = depth==1 ? "Rat" : depth<4 ? "Skeleton" : "Ghoul"; return e;
        }
    }
}
