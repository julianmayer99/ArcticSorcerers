using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.Scripts.Gamemodes
{
    /// <summary>
    /// Two (or more) teams fight each other. A kill will score a point for the killers team.
    /// The team to reach the score limit first, wins the game/round.
    /// </summary>
    public class GamemodeTeamDeathmatch : IGameMode
    {
        public int pointsForScoringObjective = 1;
        public int pointsForLoosingObjective = 0;

        public List<Team> TeamScores { get; set; } = new List<Team>();
        public int ScoreLimit { get; set; } = 15;
        public int RoundLimit { get; set; } = 1;
        private int roundsLeftToPlay;
        public UnityEvent OnGameEnd { get; set; }
        public UnityEvent OnRoundEnd { get; set; }

        public void Initialize()
        {
            roundsLeftToPlay = RoundLimit;

            if (OnGameEnd == null)
                OnGameEnd = new UnityEvent();

            if (OnRoundEnd == null)
                OnRoundEnd = new UnityEvent();

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
            CheckIfATeamHasWon();
        }

        public void OnPlayerStartedObjective(PlayerController player)
        {
        }

        void CheckIfATeamHasWon()
        {
            if (ATeamHasPassedTheScoreLimit)
            {
                roundsLeftToPlay--;

                if (roundsLeftToPlay <= 0)
                {
                    OnGameEnd.Invoke();
                }
                else
                {
                    OnRoundEnd.Invoke();
                    ResetForNextRound();
                }
            }
        }

        public void ResetForNextRound()
        {
            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                player.transform.position = GameManager.Instance.map.GetGoodGameStartSpawnPoint(player);
                player.playerStats.ResetStatsOnPlayerDeath();
            }
        }

        bool ATeamHasPassedTheScoreLimit
        {
            get
            {
                foreach (var team in TeamScores)
                {
                    if (team.score >= ScoreLimit)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
