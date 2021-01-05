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
        public string Name => "Team " + teamId;
    }
}
