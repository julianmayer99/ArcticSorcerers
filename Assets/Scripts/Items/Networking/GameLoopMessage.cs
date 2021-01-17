using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items.Networking
{
    [System.Serializable]
    public class GameLoopMessage
    {
        /// <summary>
        /// playerId, playerPosition
        /// </summary>
        public PlayerInfo[] playerInfos = new PlayerInfo[NetworkingInfoManager.config.playerCount];
        public List<Interaction> interactions = new List<Interaction>();

        public GameLoopMessage()
        {
            playerInfos = new PlayerInfo[NetworkingInfoManager.config.playerCount];
        }

        [System.Serializable]
        public class PlayerInfo
        {
            public Vector3 position;
            public Quaternion rotation;
            public int ammunitionReserve;
            public int animationState;
        }

        [System.Serializable]
        public class Interaction
        {
            public int interactionType = (int)InteractionType.Kill;
            public int activePlayerId;
            public int passivePlayerId;
        }

        public enum InteractionType
        {
            Kill = 0
        }
    }
}
