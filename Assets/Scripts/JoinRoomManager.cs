using Assets.Scripts.Gamemodes;
using Assets.Scripts.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinRoomManager : MonoBehaviour
{
    public Transform playerRespawnPostGame;
    public GameObject[] gameModePreFabs;
    public GameObject teamSelectInteractable;
    public GameObject controlsInputPanel;
    public DialogueWindow quitGameWindow;
    public TMP_InputField inp_serverPort;
    [Space]
    public TMP_InputField inp_serverIP;

    private void Start()
    {

        AudioManager.instance.Play(AudioManager.audioMainMenu);

        if (PlayerConfigurationManager.Instance.Players.Count > 0)
            StartCoroutine(ReenablePlayersPostGameAfterFirstUpdate());


        if (GameSettings.gameMode == null)
            ChangeGamemode(Maybers.Prefs.Get("last gamemode", 2));

        PlayerControllerInteractionManager.Instance.OnScoreboardPressed.AddListener(OnScoreboardPressed);
        PlayerControllerInteractionManager.Instance.OnCancelButtonPressed.AddListener(OnPauseButtonPressed);
    }

    void OnPauseButtonPressed()
    {
        quitGameWindow.gameObject.SetActive(!controlsInputPanel.activeSelf);
    }

    private void OnScoreboardPressed()
    {
        controlsInputPanel.SetActive(!controlsInputPanel.activeSelf);
    }

    private void OnDestroy()
    {
        PlayerControllerInteractionManager.Instance.OnScoreboardPressed.RemoveListener(OnScoreboardPressed);
    }

    public IEnumerator ReenablePlayersPostGameAfterFirstUpdate()
    {
        yield return new WaitForEndOfFrame();

        foreach (var player in PlayerConfigurationManager.Instance.Players)
        {
            player.transform.position = playerRespawnPostGame.position;
            player.gameObject.SetActive(true);
            player.m_Rigidbody.velocity = Vector3.zero;
        }
    }

    public void ChangeGamemode(int id)
    {
        ChangeGamemode((GamemodeBase.Gamemode)id);
    }

    public void ChangeGamemode(GamemodeBase.Gamemode mode)
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

    public void ChangeMap(int map)
    {
        GameSettings.selectedMap = (GameSettings.Map)map;

        Maybers.Prefs.Set("last map", map);
    }

    public void ChangePlayerColor(PlayerController player)
    {
        player.config.ChangePlayerColorToNextFree();
        player.OnColorChanged();
    }

    public void ChangeTeam(PlayerController player)
    {
        player.config.Team = GameSettings.gameMode.TeamScores[(player.config.Team.teamId + 1) % GameSettings.gameMode.NumberOfTeams];
        player.playerUI.UpdateTeamColor();
        player.OnTeamChanged();
    }

    public void StarServer()
    {
        if (int.TryParse(inp_serverPort.text, out int port))
        {
            GameServer.instance.StartServer(port);
        }
        else
        {
            GameServer.instance.StartServer(8082);
            Debug.LogError("Invalid port "+ inp_serverPort.text + ". Instead using 8082.");
        }
    }

    public void StopServer()
    {
        GameServer.instance.StopServer();
    }

    public void JoinServer()
    {
        GameClient.instance.SetIPAddress(inp_serverIP.text);
        GameClient.instance.ConnectToServer();
    }

    public void LeaveServer()
    {
        GameClient.instance.Disconnect();
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

    public void QuitGame()
    {
        Debug.Log("Quit Game ...");
        Application.Quit();
    }
}
