using UnityEngine;
using System.Collections;
using Assets.Scripts.Items;

public class CtfFlag : MonoBehaviour
{
    public Team team { get; set; }
    public Vector3 carryingVisualOffset;
    [HideInInspector] public bool isAtSpawn = true;
    [HideInInspector] public PlayerController carryingPlayer;
    [HideInInspector] public Vector3 spawnPosition;

    public void Initialize(Team team, Vector3 spawn)
    {
        this.team = team;
        this.spawnPosition = spawn;
    }

    private void Update()
    {
        if (isAtSpawn)
            return;

        if (carryingPlayer == null)
            return;

        transform.position = carryingPlayer.transform.position + carryingVisualOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();

        if (player == null)
            return;

        if (player.config.Team == this.team
            && this.isAtSpawn)
        {
            if (player.gamemodeExtraInfo.CarryingFlag != null)
            {
                GameSettings.gameMode.OnPlayerScoredObjective(player);
            }

            return;
        }
        else if (player.config.Team == this.team)
        {
            ReturnOwnFlagToHomeBase();
            return;
        }

        if (player.config.Team != this.team)
        {
            if (carryingPlayer != null)
                return;

            isAtSpawn = false;

            carryingPlayer = player;
            player.gamemodeExtraInfo.CarryingFlag = this;
            GameSettings.gameMode.OnPlayerStartedObjective(player);
        }

    }

    void ReturnOwnFlagToHomeBase()
    {
        transform.position = spawnPosition;
        isAtSpawn = true;
    }
}
