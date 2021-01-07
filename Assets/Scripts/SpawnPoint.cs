using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [HideInInspector] public float xDistanceToTeam1Area;
    [HideInInspector] public float xDistanceToTeam2Area;
    [HideInInspector] public DateTime lastUsed;
}
