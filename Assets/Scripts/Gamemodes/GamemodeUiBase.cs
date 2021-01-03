using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gamemodes
{
    public static class GamemodeUiBase
    {
        public static void InitializeTeamBasedUI(TeamNamePanel[] teamPanels)
        {
            var ui = GameSettings.gameMode.GameModeUi;

            for (int i = 0; i < teamPanels.Length; i++)
            {
                teamPanels[i].gameObject.SetActive(i < ui.CorrespondingTeams.Count);
                if (i < ui.CorrespondingTeams.Count)
                {
                    teamPanels[i].InitializeUI(ui.CorrespondingTeams[i]);
                }
            }
        }

    }
}
