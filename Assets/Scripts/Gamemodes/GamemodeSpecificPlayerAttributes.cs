using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gamemodes
{
    [System.Serializable]
    public class GamemodeSpecificPlayerAttributes
    {
        public CtfFlag CarryingFlag { get; set; }
        public int killedAFlagCarrier = 0;
        public int killedAsFlagCarrier = 0;
        public int flagsTaken = 0;
        public int flagsCaptured = 0;
        public int flagsReturned = 0;
    }
}
