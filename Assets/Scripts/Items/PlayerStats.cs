using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items
{
    [Serializable]
    public class PlayerStats
    {
        public int ammunitionLeft = 5;
        public int health = 100;
        public int jumps = 0;
        public int kills = 0;
        public int shotsFired = 0;
        public int deaths = 0;
        public int hitsTaken = 0;
        public int hitsGiven = 0;
        public float distanceCovered = 0f;

        public void ResetStatsOnPlayerDeath()
        {
            health = 100;
            ammunitionLeft = 5;
        }
    }
}
