using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerConfigurationManager : MonoBehaviour
{

    private List<PlayerController> players;

    [SerializeField]
    private int MaxPlayers = 2;

    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            players = new List<PlayerController>();
        }

    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("player joined " + pi.playerIndex);
        pi.transform.SetParent(transform);

        if (!players.Any(p => p.config.PlayerIndex == pi.playerIndex))
        {
            var player = pi.GetComponent<PlayerController>();

            players.Add(player);

            var config = new PlayerConfiguration(pi.playerIndex);
            config.playerName = "Player " + (config.PlayerIndex + 1); // Developoment data
            config.Input = pi;

            players.Add(player);

            player.config = config;
        }
    }

    public List<PlayerController> Players => players;


    public void SetPlayerColor(int index, PlayerConfiguration.PlayerColor color)
    {
        players[index].config.color = color;
    }

    public void SetPlayerCharacter(int index, int characterIndex)
    {
        players[index].config.Character = characterIndex;
    }

    public void ReadyPlayer(int index)
    {
        players[index].config.isReady = true;
        if (players.Count == MaxPlayers && players.All(p => p.config.isReady))
        {
            FindObjectOfType<JoinScreenController>().dialogueWindow_startGame.SetActive(true);
        }
    }

    public void AttemptGameStart(int playerIndex)
    {
        if (FindObjectOfType<JoinScreenController>().dialogueWindow_startGame.activeSelf)
        {
            SceneManager.LoadScene("Level");
        }
        else
        {
            FindObjectOfType<JoinScreenController>().dialogueWindow_startGame.SetActive(true);
        }
    }

    public void LoadLevel()
    {
        foreach (var player in players)
        {
            player.gameObject.SetActive(false);
        }

        SceneManager.LoadScene("Level");
    }
}
