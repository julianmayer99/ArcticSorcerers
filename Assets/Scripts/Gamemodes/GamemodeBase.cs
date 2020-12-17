using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Gamemodes
{
    /// <summary>
    /// Provides generic implementations that are valid for all gamemodes
    /// </summary>
    public class GamemodeBase
    {
        public int pointsForScoringObjective = 1;
        public int pointsForLoosingObjective = 0;

        public int ScoreLimit { get; set; } = 15;
        public int RoundLimit { get; set; } = 1;

        public static void InitializeInLevel(IGameMode mode)
        {

            mode.TeamScores = new List<Team>();
            mode.RoundsLeftToPlay = mode.RoundLimit;
            mode.InstantiateGamemodeUI();

            if (mode.OnGameEnd == null)
                mode.OnGameEnd = new UnityEvent();

            if (mode.OnRoundEnd == null)
                mode.OnRoundEnd = new UnityEvent();

            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                var team = mode.TeamScores.SingleOrDefault(t => t.teamId == player.config.Team.teamId);
                if (team == null)
                    mode.TeamScores.Add(player.config.Team);
            }

            mode.GameModeUi.InitializeUI(mode.TeamScores);
        }

        public void OnModeSpawnedInJoinRoom()
        {
            AutoAssignTeams();
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
            GameModeUi.UpdateUI();
        }

        public void OnPlayerStartedObjective(PlayerController player)
        {
        }

        public void ResetForNextRound()
        {
            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                player.transform.position = GameManager.Instance.map.GetGoodGameStartSpawnPoint(player);
                player.playerStats.ResetStatsOnPlayerDeath();
            }
        }

        void AutoAssignTeams()
        {
            for (int i = 0; i < PlayerConfigurationManager.Instance.Players.Count; i++)
            {
                PlayerConfigurationManager.Instance.Players[i].config.Team.teamId = i % 2;
                PlayerConfigurationManager.Instance.Players[i].playerUI.UpdateTeamColor();
            }
        }

        void CheckIfATeamHasWon()
        {
            if (ATeamHasPassedTheScoreLimit)
            {
                roundsLeftToPlay--;

                if (roundsLeftToPlay <= 0)
                {
                    OnGameEnd.Invoke();
                    GameModeUi.ShowGameEndScreen();
                }
                else
                {
                    OnRoundEnd.Invoke();
                    ResetForNextRound();
                    GameModeUi.ShowRoundEndScreen();
                }
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
