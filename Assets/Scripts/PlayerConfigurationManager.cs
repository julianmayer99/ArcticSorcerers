using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using Assets.Scripts.Gamemodes;

public class PlayerConfigurationManager : MonoBehaviour
{
    public GameObject[] gameModePreFabs;

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

        if (GameSettings.gameMode == null)
            ChangeGamemode(Maybers.Prefs.Get("last gamemode", 0));
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        if (pi.playerIndex == 0)
        {
            FindObjectOfType<MenuManager>().SkipMainMenuScreen();
        }

        Debug.Log("player joined " + pi.playerIndex);
        pi.transform.SetParent(transform);

        if (!players.Any(p => p.config.PlayerIndex == pi.playerIndex))
        {
            var player = pi.GetComponent<PlayerController>();

            var config = new PlayerConfiguration(pi.playerIndex);
            config.info = PlayerLevelingManager.Instance.GetLastPlayerInfo(config.PlayerIndex);
            config.Input = pi;

            players.Add(player);

            player.config = config;

            if (GameSettings.gameMode.IsTeamBased)
            {
                player.config.Team = GameSettings.gameMode.TeamScores[player.config.PlayerIndex % GameSettings.gameMode.NumberOfTeams];
            }

            StartCoroutine(SetPlayerColorAfterPlayerEnableCall(player));
        }
    }

    IEnumerator SetPlayerColorAfterPlayerEnableCall(PlayerController player)
    {
        yield return new WaitForEndOfFrame();
        ChangePlayerColor(player); // Auto assign on first launch
        player.playerUI.UpdateTeamColor();
    }

    public List<PlayerController> Players => players;

    public void ChangePlayerColor(PlayerController player)
    {
        player.config.ChangePlayerColorToNextFree();
        player.OnColorChanged();
    }

    public void ChangeTeam(PlayerController player)
    {
        player.config.Team = GameSettings.gameMode.TeamScores[(player.config.Team.teamId + 1) % GameSettings.gameMode.NumberOfTeams];
        player.playerUI.UpdateTeamColor();
    }

    public void SetPlayerColor(int index, ColorManager.PlayerColor color)
    {
        players[index].config.Color = color;
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
        DontDestroyOnLoad(GameSettings.gameMode.GameObject);
        ResponseVisualizer.Instance.TintScreenBlackOnLongWaitTime();
        SceneManager.LoadScene("Level");
    }

    public void ChangeGamemode(Gamemode mode)
    {
        if (GameSettings.gameMode != null)
            Destroy(GameSettings.gameMode.GameObject);

        var modeInQuestion = gameModePreFabs.SingleOrDefault(m => m.GetComponent<IGameMode>().ModeName == mode);
        if (modeInQuestion != null)
        {
            GameSettings.gameMode = Instantiate(modeInQuestion).GetComponent<IGameMode>();
            GameSettings.gameMode.OnModeSpawnedInJoinRoom();
        }
        else
            Debug.LogError("Gamemode " + mode + " was not found.");

        Maybers.Prefs.Set("last gamemode", (int)mode);
    }

    public void ChangeGamemode(int id)
    {
        ChangeGamemode((Gamemode)id);
    }

    /// <summary>
    /// @see: classes than implement the interface <see cref="IGameMode"/>
    /// </summary>
    [System.Serializable]
    public enum Gamemode
    {
        TeamDeathmatch = 0,
        CaptureTheFlag = 1,
        LastManStanding = 2,
        KingOfTheHill = 3,
        CoinCollectors = 4,
        FreeForAll = 5,
        SearchAndDestroy = 6,
        Domination = 7
    }
}
