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
        public UnityEvent OnGameEnd { get; set; }
        public UnityEvent OnRoundEnd { get; set; }

        public void InitializeInLevel()
        {
            GameModeUi = Instantiate(uiPreFab, FindObjectOfType<Canvas>().transform).GetComponent<IGameModeUi>();
            
            GamemodeBase.InitializeInLevel();
        }

        public void OnPlayerKilledOtherPlayer(PlayerController attacker, PlayerController victim)
        {
            victim.config.Team.score += pointsForLoosingObjective;
            OnPlayerScoredObjective(attacker);
        }

        public void OnPlayerScoredObjective(PlayerController player)
        {
            GamemodeBase.OnPlayerScoredObjective(player, pointsForScoringObjective);
        }

        public void OnPlayerStartedObjective(PlayerController player)
        {
        }

        public void ResetForNextRound()
        {
            GamemodeBase.ResetForNextRound();
        }

        public void OnModeSpawnedInJoinRoom()
        {
            GamemodeBase.AutoAssignTeams();
        }

        public void SaveStatsOnGameEnd()
        {
            GamemodeBase.CopyGenericPlayerStatsOnGameEnd();

            foreach (var team in GameSettings.gameMode.TeamScores)
            {
                foreach (var player in team.Players)
                {
                    if (team.HasTheMostWonRounds)
                        player.config.info.xp += 25;

                    player.config.info.xp += player.playerStats.kills * 5;
                }
            }
        }

        public IGameModeUi GameModeUi { get; set; }
        public GameObject GameObject => gameObject;

        public bool IsTeamBased => true;

        public int NumberOfTeams => 2;

        public int RoundsLeftToPlay { get; set; }
    }
}
