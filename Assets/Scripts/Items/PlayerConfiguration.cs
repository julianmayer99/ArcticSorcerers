using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Items
{
    [Serializable]
    public class PlayerConfiguration
    {
        public PlayerLevelingManager.PlayerInfo info;
        public PlayerInputMethod Input { get; set; }
        public int PlayerIndex { get; private set; }
        public Team Team { get; set; } = new Team();
        public bool isReady { get; set; } // TODO:
        public ColorManager.PlayerColor Color { get; set; }

        private int selectedColorIndex = 0;
        /// <summary>
        /// OnColorChanged() must be called after on the attached PlayerController
        /// </summary>
        public void ChangePlayerColorToNextFree()
        {
            var colors = ColorManager.Instance.UnoccupiedPlayerColors.ToArray();
            selectedColorIndex %= colors.Length;

            if (Color != null)
                Color.isInUse = false;

            Color = colors[selectedColorIndex];
            Color.isInUse = true;
            selectedColorIndex++;
        }


        public PlayerConfiguration() {  }

        public PlayerConfiguration(int playerId)
        {
            this.PlayerIndex = playerId;
        }
    }
}
