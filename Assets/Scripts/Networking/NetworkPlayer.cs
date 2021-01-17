using Assets.Scripts;
using Assets.Scripts.Items;
using Assets.Scripts.Items.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour, IPlayer
{
    public FloatingPlayerGuiHandler playerUI;
    private int animationState;
    private int ammunitionReserveCount;

    public void UpdateStuff(int id)
    {
        switch (GameSettings.activeConnection)
        {
            case GameSettings.Connection.Client:
                transform.position = NetworkingInfoManager.currentClientRemoteLoop.playerInfos[id].position;
                transform.rotation = NetworkingInfoManager.currentClientRemoteLoop.playerInfos[id].rotation;
                animationState = NetworkingInfoManager.currentClientRemoteLoop.playerInfos[id].animationState;
                ammunitionReserveCount = NetworkingInfoManager.currentClientRemoteLoop.playerInfos[id].ammunitionReserve;
                break;
            case GameSettings.Connection.Host:
                transform.position = NetworkingInfoManager.currentServerGameLoop.playerInfos[id].position;
                transform.rotation = NetworkingInfoManager.currentServerGameLoop.playerInfos[id].rotation;
                animationState = NetworkingInfoManager.currentServerGameLoop.playerInfos[id].animationState;
                ammunitionReserveCount = NetworkingInfoManager.currentServerGameLoop.playerInfos[id].ammunitionReserve;
                break;
        }

        // TODO Update UI
    }

    public Transform Transform => transform;
}
