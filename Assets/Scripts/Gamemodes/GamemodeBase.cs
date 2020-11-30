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
    public abstract class GamemodeBase : IGameMode
    {
        public abstract IGameModeUi GameModeUi { get; set; }

        public int pointsForScoringObjective = 1;
        public int pointsForLoosingObjective = 0;
        public GameObject uiPreFab;

        public abstract GameObject GameObject { get; }
        public abstract PlayerConfigurationManager.Gamemode ModeName { get; set; }

        public abstract bool IsTeamBased { get; }

        public int NumberOfTeams => throw new NotImplementedException();

        public UnityEvent OnGameEnd { get; set; }
        public UnityEvent OnRoundEnd { get; set; }
        public List<Team> TeamScores { get; set; }
        public int ScoreLimit { get; set; } = 15;
        public int RoundLimit { get; set; } = 1;

        private int roundsLeftToPlay;

        public abstract void InstantiateGamemodeUI();

        public void InitializeInLevel()
        {

            TeamScores = new List<Team>();
            roundsLeftToPlay = RoundLimit;
            InstantiateGamemodeUI();

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

            GameModeUi.InitializeUI(TeamScores);
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
