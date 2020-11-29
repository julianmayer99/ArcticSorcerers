using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items
{
    public interface IGameModeUi
    {
        List<Team> CorrespondingTeams { get; set; }
        void InitializeUI(List<Team> teams);
        void UpdateUI();
    }
}
