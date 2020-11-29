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
        List<Team> CorrespondingTeams { get; set; }
        void InitializeUI(List<Team> teams);
        void UpdateUI();
        void ShowRoundEndScreen();
        void ShowGameEndScreen();
    }
}
