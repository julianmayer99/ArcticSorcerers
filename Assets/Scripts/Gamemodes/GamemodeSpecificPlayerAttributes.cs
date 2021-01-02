using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gamemodes
{
    public class GamemodeSpecificPlayerAttributes
    {
        public CtfFlag CarryingFlag { get; set; }
        public int killedAFlagCarrier { get; set; } = 0;
        public int killedAsFlagCarrier { get; set; } = 0;
    }
}
