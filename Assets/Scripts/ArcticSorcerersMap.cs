using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcticSorcerersMap : MonoBehaviour
{
    [SerializeField] private TimeSpan spawnReuseCooldown;
    [SerializeField] private Transform spawnContainer;
    [SerializeField] private Transform team1SpawnArea;
    [SerializeField] private Transform team2SpawnArea;
    [HideInInspector] public SpawnPoint[] spawns;
    public Transform[] ctfFlagSpawns = new Transform[2];

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

        // TODO: Anderes Spawn System, wenn Spielmodus nicht teambasiert

        var ret = teamSpawns[forPlayer.config.Team.teamId][teamRespawnCursor[forPlayer.config.Team.teamId]];
        teamRespawnCursor[forPlayer.config.Team.teamId]++;
        teamRespawnCursor[forPlayer.config.Team.teamId] %= (teamSpawns[forPlayer.config.Team.teamId].Length / 3) + 1;
        ret.lastUsed = DateTime.Now;
        return ret.transform.position;
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
