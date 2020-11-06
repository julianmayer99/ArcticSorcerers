using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Items
{
    public class PlayerConfiguration
    {
        public string playerName;
        public byte playerId;


        public PlayerInput Input { get; private set; }
        public int PlayerIndex { get; private set; }
        public int Character { get; set; }
        public bool isReady { get; set; }
        public PlayerColor color { get; set; }

        public enum PlayerColor
        {
            Red,
            Green,
            Blue,
            Yellow,
            Orange,
            Cyan
        }

        public PlayerConfiguration() {  }

        public PlayerConfiguration(PlayerInput pi)
        {
            PlayerIndex = pi.playerIndex;
            Input = pi;
        }
    }
}
