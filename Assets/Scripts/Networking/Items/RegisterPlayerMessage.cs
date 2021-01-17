using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Networking.Items
{
    public class RegisterPlayerMessage
    {
        public string name;
        public int level;
        public int localId;

        public NetworkPlayerInfo GetPlayerInfo(int clientId, int networkId)
        {
            return new NetworkPlayerInfo
            {
                level = level,
                localId = localId,
                name = name,
                clientId = clientId,
                playerNetworkId = networkId
            };
        }
    }
}
