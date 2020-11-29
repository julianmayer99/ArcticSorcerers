using System.Collections.Generic;
using UnityEngine.Events;

namespace Assets.Scripts.Items
{
    public interface IGameMode
    {
        UnityEvent OnGameEnd { get; set; }
        UnityEvent OnRoundEnd { get; set; }
        IGameModeUi GameModeUi { get; set; }
        void Initialize(IGameModeUi ui);
        /// <summary>
        /// Resets stats such as timer, flag positions etc in modes like snd or ctf when the next round should start
        /// </summary>
        void ResetForNextRound();
        List<Team> TeamScores { get; set; }
        int ScoreLimit { get; set; }
        int RoundLimit{ get; set; }
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
    }
}
