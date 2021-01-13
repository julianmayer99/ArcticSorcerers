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

        public static void InitializeInLevel()
        {
            GameSettings.gameMode.TeamScores = new List<Team>();
            GameSettings.gameMode.RoundsLeftToPlay = GameSettings.gameMode.RoundLimit;

            if (GameSettings.gameMode.OnGameEnd == null)
                GameSettings.gameMode.OnGameEnd = new UnityEvent();

            if (GameSettings.gameMode.OnRoundEnd == null)
                GameSettings.gameMode.OnRoundEnd = new UnityEvent();

            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                var team = GameSettings.gameMode.TeamScores.SingleOrDefault(t => t.teamId == player.config.Team.teamId);
                if (team == null)
                    GameSettings.gameMode.TeamScores.Add(player.config.Team);
            }

            GameSettings.gameMode.GameModeUi.InitializeUI(GameSettings.gameMode.TeamScores);
            GameSettings.gameMode.ResetForNextRound();
        }

        public void OnModeSpawnedInJoinRoom()
        {
            AutoAssignTeams();
        }

        public static void OnPlayerScoredObjective(PlayerController player, int pointsForScoringObjective = 1)
        {
            player.config.Team.score += pointsForScoringObjective;
            CheckIfATeamHasWon();
            GameSettings.gameMode.GameModeUi.UpdateUI();
        }

        public static void ResetForNextRound()
        {
            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                player.transform.position = GameManager.Instance.map.GetGoodGameStartSpawnPoint(player);
                player.playerStats.ResetStatsOnPlayerDeath();
            }
            foreach (var team in GameSettings.gameMode.TeamScores)
            {
                team.totalScore += team.score;
                team.score = 0;
            }
        }

        public static void AutoAssignTeams()
        {
            GameSettings.gameMode.TeamScores = new List<Team>();
            for (int i = 0; i < GameSettings.gameMode.NumberOfTeams; i++)
            {
                GameSettings.gameMode.TeamScores.Add(new Team { teamId = i });
            }

            for (int i = 0; i < PlayerConfigurationManager.Instance.Players.Count; i++)
            {
                PlayerConfigurationManager.Instance.Players[i].config.Team = GameSettings.gameMode.TeamScores[i % 2];
                PlayerConfigurationManager.Instance.Players[i].playerUI.UpdateTeamColor();
            }
        }

        public static void CheckIfATeamHasWon()
        {
            if (ATeamHasPassedTheScoreLimit)
            {
                GameSettings.gameMode.RoundsLeftToPlay--;

                if (GameSettings.gameMode.RoundsLeftToPlay <= 0)
                {
                    GameSettings.gameMode.OnGameEnd.Invoke();
                    GameSettings.gameMode.GameModeUi.ShowGameEndScreen();
                    GameSettings.gameMode.SaveStatsOnGameEnd();
                }
                else
                {
                    GameSettings.gameMode.OnRoundEnd.Invoke();
                    GameSettings.gameMode.ResetForNextRound();
                    GameSettings.gameMode.GameModeUi.ShowRoundEndScreen();
                }
            }
        }

        static bool ATeamHasPassedTheScoreLimit
        {
            get
            {
                foreach (var team in GameSettings.gameMode.TeamScores)
                {
                    if (team.score >= GameSettings.gameMode.ScoreLimit)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static void CopyGenericPlayerStatsOnGameEnd()
        {
            PlayerLevelingManager.Instance.listHasChanged = true;
            foreach (var team in GameSettings.gameMode.TeamScores)
            {
                foreach (var player in team.Players)
                {
                    player.config.info.jumps += player.playerStats.jumps;
                    player.config.info.deaths += player.playerStats.deaths;
                    player.config.info.kills += player.playerStats.kills;

                    // TODO: Copy more player stats
                }
            }
        }

        public string GetTimeString(int seconds)
        {
            int minutes = seconds / 60;
            int relSeconds = seconds % 60;
            return $"{minutes}:{seconds}";
        }
    }
}
