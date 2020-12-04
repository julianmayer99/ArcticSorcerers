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
    /// Two (or more) teams fight each other. A kill will score a point for the killers team.
    /// The team to reach the score limit first, wins the game/round.
    /// </summary>
    public class GamemodeTeamDeathmatch : MonoBehaviour, IGameMode
    {
        public int pointsForScoringObjective = 1;
        public int pointsForLoosingObjective = 0;

        public GameObject uiPreFab;

        public PlayerConfigurationManager.Gamemode ModeName { get; set; } = PlayerConfigurationManager.Gamemode.TeamDeathmatch;
        public List<Team> TeamScores { get; set; }
        public int ScoreLimit { get; set; } = 15;
        public int RoundLimit { get; set; } = 1;
        private int roundsLeftToPlay;
        public UnityEvent OnGameEnd { get; set; }
        public UnityEvent OnRoundEnd { get; set; }

        public void InitializeInLevel()
        {
            TeamScores = new List<Team>();
            GameModeUi = Instantiate(uiPreFab, FindObjectOfType<Canvas>().transform).GetComponent<IGameModeUi>();
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

            GameModeUi.InitializeUI(TeamScores);
            ResetForNextRound();
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

        public void ResetForNextRound()
        {
            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                player.transform.position = GameManager.Instance.map.GetGoodGameStartSpawnPoint(player);
                player.playerStats.ResetStatsOnPlayerDeath();
            }
        }

        public void OnModeSpawnedInJoinRoom()
        {
            AutoAssignTeams();
        }

        void AutoAssignTeams()
        {
            for (int i = 0; i < PlayerConfigurationManager.Instance.Players.Count; i++)
            {
                PlayerConfigurationManager.Instance.Players[i].config.Team.teamId = i % 2;
                PlayerConfigurationManager.Instance.Players[i].playerUI.UpdateTeamColor();
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

        public IGameModeUi GameModeUi { get; set; }
        public GameObject GameObject => gameObject;

        public bool IsTeamBased => true;

        public int NumberOfTeams => 2;
    }
}
