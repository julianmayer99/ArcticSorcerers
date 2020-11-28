using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gamemodes
{
    public class GamemodeTeamDeathmatch : IGameMode
    {
        public int pointsForScoringObjective = 1;
        public int pointsForLoosingObjective = 0;

        public List<Team> TeamScores { get; set; } = new List<Team>();
        public int ScoreLimit { get; set; } = 15;
        public int RoundLimit { get; set; } = 1;

        public void Initialize()
        {
            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                var team = TeamScores.SingleOrDefault(t => t.teamId == player.config.Team.teamId);
                if (team == null)
                    TeamScores.Add(player.config.Team);
            }
        }

        public void OnPlayerKilledOtherPlayer(PlayerController attacker, PlayerController victim)
        {
            victim.config.Team.score += pointsForLoosingObjective;
            OnPlayerScoredObjective(attacker);
        }

        public void OnPlayerScoredObjective(PlayerController player)
        {
            player.config.Team.score += pointsForScoringObjective;
        }

        public void OnPlayerStartedObjective(PlayerController player)
        {
        }

        void CheckIfATeamHasWon()
        {
            foreach (var team in TeamScores)
            {
                if (team.score >= ScoreLimit)
                {
                    // TODO: Team has won => end game
                }
            }
        }
    }
}
