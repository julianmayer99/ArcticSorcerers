using Assets.Scripts.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcticSorcerersMap : MonoBehaviour
{
    [Header("Spawning")]
    [Space]
    [SerializeField] private TimeSpan spawnReuseCooldown;
    [SerializeField] private Transform spawnContainer;
    [SerializeField] private Transform team1SpawnArea;
    [SerializeField] private Transform team2SpawnArea;
    [HideInInspector] public SpawnPoint[] spawns;

    [Header("Gamemode Specific Stuff")]
    [Space]
    public Transform[] ctfFlagSpawns = new Transform[2];
    public Transform initial_KOTH_Spawn;

    private List<SpawnPoint[]> teamSpawns;
    private int[] teamRespawnCursor = { 0, 0 };

    private void Awake()
    {
        spawns = spawnContainer.GetComponentsInChildren<SpawnPoint>();
        teamSpawns = new List<SpawnPoint[]>();

        foreach (var spawn in spawns)
        {
            spawn.xDistanceToTeam1Area = Mathf.Abs((spawn.transform.position - team1SpawnArea.position).x);
            spawn.xDistanceToTeam2Area = Mathf.Abs((spawn.transform.position - team2SpawnArea.position).x);
        }

        teamSpawns.Add(spawns.OrderBy(s => s.xDistanceToTeam1Area).ToArray());
        teamSpawns.Add(spawns.OrderBy(s => s.xDistanceToTeam2Area).ToArray());
    }

    public Vector3 GetGoodSpawnPoint(PlayerController forPlayer)
    {
        // TODO: Suche einen Spawn, der möglichst weit weg von den Gegnern ist,
        // damit der Spieler nicht direkt nach dem Respawn stirbt.

        if (GameSettings.gameMode.IsTeamBased)
        {
            var ret = teamSpawns[forPlayer.config.Team.teamId][teamRespawnCursor[forPlayer.config.Team.teamId]];
            teamRespawnCursor[forPlayer.config.Team.teamId]++;
            teamRespawnCursor[forPlayer.config.Team.teamId] %= (teamSpawns[forPlayer.config.Team.teamId].Length / 3) + 1;
            ret.lastUsed = DateTime.Now;
            return ret.transform.position;
        }
        else
        {
            var spawn = spawns[UnityEngine.Random.Range(0, teamSpawns.Count - 1)];
            spawn.lastUsed = DateTime.Now;
            return spawn.transform.position;
        }
    }
    
    public Vector3 GetGoodGameStartSpawnPoint(PlayerController forPlayer)
    {
        // TODO: Suche einen Spawn, der möglichst weit weg von den Gegnern ist,
        // damit der Spieler nicht direkt nach dem Respawn stirbt.

        var ret = teamSpawns[forPlayer.config.Team.teamId][teamRespawnCursor[forPlayer.config.Team.teamId]];
        teamRespawnCursor[forPlayer.config.Team.teamId]++;
        teamRespawnCursor[forPlayer.config.Team.teamId] %= (teamSpawns[forPlayer.config.Team.teamId].Length / 3) + 1;
        ret.lastUsed = DateTime.Now;
        return ret.transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(team1SpawnArea.position, 3);
        Gizmos.DrawWireSphere(team2SpawnArea.position, 3);
    }
}
