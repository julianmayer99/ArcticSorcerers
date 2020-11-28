using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items
{
    public class Team
    {
        public List<PlayerController> players = new List<PlayerController>();
        public int teamId;
        public int score;
    }
}
