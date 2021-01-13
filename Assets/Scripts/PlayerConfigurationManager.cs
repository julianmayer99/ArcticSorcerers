using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using Assets.Scripts.Gamemodes;
using System.Security.Cryptography;

public class PlayerConfigurationManager : MonoBehaviour
{
    public Transform playerRespawnPostGame;
    public GameObject[] gameModePreFabs;
    public GameObject teamSelectInteractable;
    public PlayerController playerPreFab;

    private List<PlayerController> players;

    private int maxPlayers = 8;

    public static PlayerConfigurationManager Instance { get; private set; }

    private bool keyboardIsBeeingUsed = false;
    private List<Gamepad> unusedGamepads;
    private List<Gamepad> usedGamepads = new List<Gamepad>();

    private void Start()
    {
        InputSystem.onDeviceChange +=
        (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    RecalculateUsedAndUnusedGamepads();
                    break;
                case InputDeviceChange.Disconnected:
                    RecalculateUsedAndUnusedGamepads();
                    break;
                case InputDeviceChange.Reconnected:
                    RecalculateUsedAndUnusedGamepads();
                    // Plugged back in.
                    break;
                case InputDeviceChange.Removed:
                    RecalculateUsedAndUnusedGamepads();
                    // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                    break;
                default:
                    RecalculateUsedAndUnusedGamepads();
                    // See InputDeviceChange reference for other event types.
                    break;
            }
        };

        RecalculateUsedAndUnusedGamepads();
    }

    private void Update()
    {
        if (unusedGamepads == null)
            RecalculateUsedAndUnusedGamepads();

        // Listen for Gamepad input
        foreach (var gamepad in unusedGamepads)
        {
            if (gamepad == null)
                continue;

            if (gamepad.buttonSouth.isPressed)
            {
                HandlePlayerJoin(gamepad);
            }
        }
        if (!keyboardIsBeeingUsed)
            if (Keyboard.current.spaceKey.isPressed)
                HandlePlayerJoin(null);
    }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");

            foreach (var player in Instance.Players)
            {
                player.transform.position = playerRespawnPostGame.position;
                player.gameObject.SetActive(true);
                player.m_Rigidbody.velocity = Vector3.zero;
            }

            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            players = new List<PlayerController>();
        }

        if (GameSettings.gameMode == null)
            ChangeGamemode(Maybers.Prefs.Get("last gamemode", 2));
    }

    void RecalculateUsedAndUnusedGamepads()
    {
        usedGamepads.Clear();
        var all = Gamepad.all.ToList();
        foreach (var player in Players)
        {
            var pad = all.SingleOrDefault(p => p == player.config.Input.gamepad);
            if (pad != null)
            {
                usedGamepads.Add(pad);
                all.Remove(pad);
            }
        }

        var keyboardPad = Players.SingleOrDefault(p => p.config.Input.isKeyboardControlled);
        keyboardIsBeeingUsed = keyboardPad != null;

        unusedGamepads = all;
    }

    public void HandlePlayerJoin(Gamepad pad)
    {
        if (players.Count == 0)
        {
            FindObjectOfType<MenuManager>().SkipMainMenuScreen();
        }

        if (!players.Any(p => p.config.Input.gamepad == pad))
        {
            var player = Instantiate(playerPreFab.gameObject, transform).GetComponent<PlayerController>();

            var config = new PlayerConfiguration(players.Count);
            config.info = PlayerLevelingManager.Instance.GetLastPlayerInfo(config.PlayerIndex);
            config.Input = new PlayerInputMethod(player, pad);
            players.Add(player);

            player.config = config;

            if (GameSettings.gameMode.IsTeamBased)
            {
                player.config.Team = GameSettings.gameMode.TeamScores[player.config.PlayerIndex % GameSettings.gameMode.NumberOfTeams];
            }
            else
            {
                GamemodeBase.AutoAssignTeams();
            }

            StartCoroutine(SetPlayerColorAfterPlayerEnableCall(player));
            RecalculateUsedAndUnusedGamepads();
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

    // TODO:
    /*public void ReadyPlayer(int index)
    {
        players[index].config.isReady = true;
        if (players.Count == MaxPlayers && players.All(p => p.config.isReady))
        {
            FindObjectOfType<JoinScreenController>().dialogueWindow_startGame.SetActive(true);
        }
    }*/

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
            teamSelectInteractable.SetActive(GameSettings.gameMode.IsTeamBased);
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
        KingOfTheHill = 2,
        LastManStanding = 3,
        CoinCollectors = 4,
        FreeForAll = 5,
        SearchAndDestroy = 6,
        Domination = 7
    }
}
