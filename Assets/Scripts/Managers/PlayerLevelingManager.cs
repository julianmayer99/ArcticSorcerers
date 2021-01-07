using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Items;
using System.Linq;

public class PlayerLevelingManager : MonoBehaviour
{
    public static PlayerLevelingManager Instance { get; private set; }
    private List<PlayerInfo> players;

    public static List<PlayerInfo> Players
    {
        get
        {
            if (Instance.players == null)
                Instance.LoadPlayerList();

            return Instance.players;
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

    public void AddPlayer(string name)
    {
        var existingPlayer = players.SingleOrDefault(p => p.Name.ToLower().Equals(name.ToLower()));
        int xp = existingPlayer == null ? 0 : existingPlayer.Xp;
        if (existingPlayer != null)
        {
            players.Remove(existingPlayer);
        }

        var newPlayer = new PlayerInfo
        {
            Name = name,
            Xp = xp
        };

        players.Add(newPlayer);
    }

    void LoadPlayerList()
    {
        players = new List<PlayerInfo>();

        var playerNames = Maybers.Prefs.Get("player names", new List<string>());
        if (playerNames.Count < 1)
            return;

        var playerXps = Maybers.Prefs.Get("player xps", new List<int>());

        for (int i = 0; i < playerNames.Count; i++)
        {
            players.Add(new PlayerInfo
            {
                Name = playerNames[i],
                Xp = playerXps[i]
            });
        }
    }

    void SavePlayerList()
    {
        if (players.Count < 1)
            return;

        var names = new List<string>();
        var xps = new List<int>();
        foreach (var player in players)
        {
            names.Add(player.Name);
            xps.Add(player.Xp);
        }

        Maybers.Prefs.Set("player names", names);
        Maybers.Prefs.Set("player xps", xps);
    }

    public class PlayerInfo
    {
        public string Name { get; set; }
        public int Xp { get; set; }
        public int Level => (Xp / 1000) + 1;
    }
}
