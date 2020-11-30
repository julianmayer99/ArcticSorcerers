﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcticSorcerersMap : MonoBehaviour
{
    [SerializeField] private Transform spawnContainer;
    [HideInInspector] public Transform[] spawns;
    public Transform[] ctfFlagSpawns = new Transform[2];

    private void Awake()
    {
        spawns = spawnContainer.GetComponentsInChildren<Transform>();
    }

    public Vector3 GetGoodSpawnPoint(PlayerController forPlayer)
    {
        // TODO: Suche einen Spawn, der möglichst weit weg von den Gegnern ist,
        // damit der Spieler nicht direkt nach dem Respawn stirbt.
        return spawns[0].position;
    }
    
    public Vector3 GetGoodGameStartSpawnPoint(PlayerController forPlayer)
    {
        // TODO: Suche einen Spawn, der möglichst weit weg von den Gegnern ist,
        // damit der Spieler nicht direkt nach dem Respawn stirbt.
        return spawns[0].position;
    }
}