using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class Team
    {
        public List<PlayerController> Players
        {
            get
            {
                return PlayerConfigurationManager.Instance.Players
                    .Where(p => p.config.Team == this).ToList();
            }
        }
        public int teamId;
        public int score;
        public int totalScore;
        public int wonRounds;
        public string Name
        {
            get
            {
                switch (teamId)
                {
                    case 0: return "Team Schneesturm";
                    case 1: return "Team Eiskristall";
                    case 2: return "Team Frostbeule";
                    case 3: return "Team Polarstern";
                    default: return "Team Schneesturm";
                }
            }
        }

        public bool HasTheMostWonRounds
            => GameSettings.gameMode.TeamScores.OrderByDescending(t => t.wonRounds).First().wonRounds == wonRounds;

    }
}
