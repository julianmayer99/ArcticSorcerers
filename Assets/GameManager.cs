using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public DynamicMultiTargetCamera dynamicCamera;
    public GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnsContainer;
    [HideInInspector] public Transform[] spawns;
    public static GameManager Instance{ get; set; }

    private void Awake()
    {
        Instance = this;

        spawns = playerSpawnsContainer.GetComponentsInChildren<Transform>();

        if (PlayerConfigurationManager.Instance == null)
        {
            SpawnDevelopementPlayerManager();
            return;
        }
    }

    void SpawnDevelopementPlayerManager()
    {
        SceneManager.LoadScene("PlayerJoin");
    }
}
