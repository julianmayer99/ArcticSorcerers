using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Items;
using System.Linq;
using System.IO;
using Assets.Scripts.Items.Json;

public class PlayerLevelingManager : MonoBehaviour
{
    public static PlayerLevelingManager Instance { get; private set; }

    private PlayerInfoListContainer plist;

    public bool listHasChanged = false;

    public static List<PlayerInfo> Players
    {
        get
        {
            if (Instance.plist == null)
                Instance.LoadPlayerList();

            return Instance.plist.players;
        }

    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
            return;

        SavePlayerList();
    }

    private void OnApplicationQuit()
    {
        SavePlayerList();
    }

    public PlayerInfo GetOrCreatePlayer(string name)
    {
        var existingPlayer = Players.SingleOrDefault(p => p.name.ToLower().Equals(name.ToLower()));
        int xp = existingPlayer == null ? 0 : existingPlayer.xp;
        if (existingPlayer != null)
        {
            plist.players.Remove(existingPlayer);
        }
        else
            listHasChanged = true;

        var newPlayer = new PlayerInfo
        {
            name = name,
            xp = xp
        };

        plist.players.Add(newPlayer);

        return newPlayer;
    }

    public PlayerInfo GetLastPlayerInfo(int playerIndex)
    {
        if (Players.Count > playerIndex)
        {
            return Players[Players.Count - 1 - playerIndex];
        }
        else
        {
            return GetOrCreatePlayer("Spieler " + (playerIndex + 1));
        }
    }

    string FilePathJsonPlayers => Path.Combine(Application.persistentDataPath, "players.json");

    void LoadPlayerList()
    {
        if (!File.Exists(FilePathJsonPlayers))
        {
            plist = new PlayerInfoListContainer();
            return;
        }

        if (File.Exists(FilePathJsonPlayers))
            plist = JsonUtility.FromJson<PlayerInfoListContainer>(File.ReadAllText(FilePathJsonPlayers));
        else
            plist = new PlayerInfoListContainer();
    }

    void SavePlayerList()
    {
        if (!listHasChanged)
            return;

        if (plist.players.Count < 1)
            return;

        string jsonText = JsonUtility.ToJson(plist);
        File.WriteAllText(
            FilePathJsonPlayers,
            jsonText);
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string name;
        public int xp;
        public int jumps;
        public int gamesPlayed;
        public int shotsFired;
        public int kills;
        public int deaths;
        public int distanceWalked;
        /// <summary>
        /// Playtime in seconds
        /// </summary>
        public int playtime;
        public int Level => (xp / 1000) + 1;
    }
}
