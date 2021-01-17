using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Networking.Items
{
    public class NetworkPlayerInfo
    {
        public string name;
        public int level;
        public int playerNetworkId = 0;
        public int localId = 0;
        public int clientId = 0;
    }
}
