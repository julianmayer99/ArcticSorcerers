using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public interface IGameModeUi
    {
        GameObject @GameObject { get; }
        string GamemodeName { get; set; }
        List<Team> CorrespondingTeams { get; set; }
        void InitializeUI(List<Team> teams);
        void UpdateUI();
        void ShowRoundEndScreen();
        void ShowGameEndScreen();

        /// <summary>
        /// This is called from score visualization scrips like a score board
        /// </summary>
        /// <param name="forPlayer">For which player shall we visualize?</param>
        /// <param name="columnIndex">Cloud be the values for: 0 = Kills, 1 = Hits, 2 = Deaths ... for a mode like tdm.</param>
        /// <returns>Information required for the scoreboard</returns>
        string GetScoreBoardRowValueForPlayer(PlayerController forPlayer, int columnIndex);

        /// <summary>
        /// Returns the name of the column header of the scoreboard. This could be something like kills, deaths, hits etc.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        string GetScoreBoardColumnHeader(int columnIndex);
        void UpdateTimeLeftTimer(string timeLeftText);
    }
}
