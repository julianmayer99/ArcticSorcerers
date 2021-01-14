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
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            players = new List<PlayerController>();
            GameSettings.selectedMap = (GameSettings.Map)Maybers.Prefs.Get("last map", (int)GameSettings.Map.SnowCastle);
        }
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
                FindObjectOfType<JoinRoomManager>().ChangeTeam(player);
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
        FindObjectOfType<JoinRoomManager>().ChangePlayerColor(player); // Auto assign on first launch
        player.playerUI.UpdateTeamColor();
        player.playerUI.UpdatePlayerProfileUi();
    }

    public List<PlayerController> Players => players;
}
