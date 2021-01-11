using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public interface IGameMode
    {
        GameObject @GameObject { get; }
        PlayerConfigurationManager.Gamemode ModeName { get; set; }
        bool IsTeamBased { get; }
        /// <summary>
        /// Can be ignored, if <see cref="IsTeamBased"/> is set to false.
        /// </summary>
        int NumberOfTeams { get; }
        UnityEvent OnGameEnd { get; set; }
        UnityEvent OnRoundEnd { get; set; }
        IGameModeUi GameModeUi { get; set; }
        /// <summary>
        /// Will be called after the Object was instantialed in the Join room. Similar to Start().
        /// Used e.g. for team ui initialization.
        /// </summary>
        void OnModeSpawnedInJoinRoom();
        /// <summary>
        /// Called from <see cref="GameManager"/> in Start().
        /// </summary>
        void InitializeInLevel();
        /// <summary>
        /// Resets stats such as timer, flag positions etc in modes like snd or ctf when the next round should start
        /// </summary>
        void ResetForNextRound();
        List<Team> TeamScores { get; set; }
        int ScoreLimit { get; set; }
        int RoundLimit{ get; set; }
        int RoundsLeftToPlay{ get; set; }
        /// <summary>
        /// Inject listener from GameManager.cs. Call whenever a kill is made.
        /// This function has to automatically call <see cref="OnPlayerScoredObjective(PlayerController)"/>
        /// in modes like tdm where the kill is the objective.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="victim"></param>
        void OnPlayerKilledOtherPlayer(PlayerController attacker, PlayerController victim);
        /// <summary>
        /// e.g. pick up a flag in ctf
        /// </summary>
        /// <param name="player"></param>
        void OnPlayerStartedObjective(PlayerController player);
        /// <summary>
        /// e.g. capture and return the enemies flag in ctf to score a point.
        /// This schould be automatically be called from <see cref="OnPlayerKilledOtherPlayer(PlayerController, PlayerController)"/> in modes like tdm.
        /// </summary>
        /// <param name="player"></param>
        void OnPlayerScoredObjective(PlayerController player);
        /// <summary>
        /// Copy stats from <see cref="PlayerController.playerStats"/> to <see cref="PlayerController.config.info"/> and add xp and co.
        /// </summary>
        void SaveStatsOnGameEnd();
    }
}
