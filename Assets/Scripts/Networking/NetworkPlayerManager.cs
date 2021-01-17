using Assets.Scripts.Items.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkPlayerManager : MonoBehaviour
{
    public static NetworkPlayerManager instance;
    public NetworkPlayer networkPlayerPreFab;
    public Dictionary<int, NetworkPlayer> networkPlayers = new Dictionary<int, NetworkPlayer>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    public void UpdatePlayerPositions(int id)
    {
        if (!networkPlayers.ContainsKey(id))
            AddPlayer();

        networkPlayers[id].UpdateStuff(id);
    }

    public void UpdatePlayerPositions()
    {
        if ((NetworkingInfoManager.config.players.Count - PlayerConfigurationManager.Instance.Players.Count) < networkPlayers.Count)
            AddPlayer();

        foreach (var player in networkPlayers)
        {
            player.Value.UpdateStuff(player.Key);
        }
    }

    void AddPlayer()
    {
        foreach (var player in NetworkingInfoManager.config.players)
        {
            if (!IsNetworkIdInLocalPlayers(player.playerNetworkId))
            if (!networkPlayers.ContainsKey(player.playerNetworkId))
                {
                    networkPlayers.Add(player.playerNetworkId,
                        Instantiate(networkPlayerPreFab.gameObject).GetComponent<NetworkPlayer>());
                }
        }
        
    }

    bool IsNetworkIdInLocalPlayers(int id)
    {
        var found = PlayerConfigurationManager.Instance.Players.SingleOrDefault(p => p.config.NetworkPlayerIndex == id);
        return found != null;
    }

}
