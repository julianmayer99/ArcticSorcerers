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

    private Renderer rend;
    public Material matTeam0;
    public Material matTeam1;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void Initialize(Team team, Vector3 spawn)
    {
        Debug.Log("CTF FLAG INITIALIZE");
        //Team
        this.team = team;

        rend.sharedMaterial = matTeam1;

        if (this.team.teamId == 1)
        {
            rend.sharedMaterial = matTeam0;
        }

        //Spawn position
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

        // Own Flag
        if (player.config.Team == this.team
            && this.isAtSpawn)
        {   
            if (player.gamemodeExtraInfo.CarryingFlag != null)
            {
                GameSettings.gameMode.OnPlayerScoredObjective(player);
                player.config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.Pulse);
            }

            return;
        }
        else if (player.config.Team == this.team)
        {
            if (carryingPlayer != null)
                return;

            player.config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.Pulse);
            ReturnOwnFlagToHomeBase();
            player.gamemodeExtraInfo.flagsReturned++;
            return;
        }

        // Enemies Flag
        if (player.config.Team != this.team)
        {
            if (carryingPlayer != null)
                return;

            isAtSpawn = false;
            player.config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.Pulse);

            carryingPlayer = player;
            player.gamemodeExtraInfo.CarryingFlag = this;
            GameSettings.gameMode.OnPlayerStartedObjective(player);
        }

    }

    public void ReturnOwnFlagToHomeBase()
    {
        transform.position = spawnPosition;
        isAtSpawn = true;
    }

    public void ResetFlagOnRoundEnd()
    {
        ReturnOwnFlagToHomeBase();
        if (carryingPlayer != null)
        {
            carryingPlayer.gamemodeExtraInfo.CarryingFlag = null;
            carryingPlayer = null;
        }
    }
}
