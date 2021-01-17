using Assets.Scripts.Networking.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items.Networking
{
    public class NetworkingInfoManager
    {
        public static int fixedUpdateCounterLimit = 100;
        public static GameLoopMessage currentServerGameLoop;
        public static GameLoopMessage currentLocalClientGameLoop;
        public static GameLoopMessage currentClientRemoteLoop;
        public static GameConfig config = new GameConfig();

        public static void AddCurrentLocalGameLoopState(ref GameLoopMessage toGameloop, bool deletePreviousInstance = false)
        {
            if (deletePreviousInstance)
                toGameloop = new GameLoopMessage();

            toGameloop = toGameloop == null || toGameloop.playerInfos == null || toGameloop.playerInfos.Length != config.playerCount
                ? new GameLoopMessage { playerInfos = new GameLoopMessage.PlayerInfo[config.playerCount] }
                : toGameloop;

            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                if (player.config.NetworkPlayerIndex < 0 || player.config.NetworkPlayerIndex >= config.playerCount)
                    Client_RegisterPlayer(player);

                toGameloop.playerInfos[player.config.NetworkPlayerIndex] = 
                    new GameLoopMessage.PlayerInfo
                    {
                        position = player.transform.position,
                        rotation = player.transform.rotation,
                        ammunitionReserve = player.playerStats.ammunitionLeft,
                        animationState = (int)player.currentStatus
                    } ;
            }
        }

        public static void Server_AddClientMessageToLoop(GameLoopMessage clientMessage)
        {
            currentServerGameLoop = currentServerGameLoop == null ? currentServerGameLoop : new GameLoopMessage();

            for (int i = 0; i < clientMessage.playerInfos.Length; i++)
            {
                if (clientMessage.playerInfos[i] == null)
                    continue;

                currentServerGameLoop.playerInfos[i] = clientMessage.playerInfos[i];
            }
        }

        public static void Client_AddLocalGameLoop()
        {
            AddCurrentLocalGameLoopState(ref currentLocalClientGameLoop, true);
        }

        public static void Client_RegisterPlayer(PlayerController player)
        {
            Debug.Log("Client: Adding player " + player.config.info.name + " to the server ...");
            var msg = new RegisterPlayerMessage
            {
                level = player.config.info.Level,
                localId = player.config.PlayerIndex,
                name = player.config.info.name
            };
            GameClient.instance.tcp.SendMessage(GameServer.MessageType.RegisterPlayer, JsonUtility.ToJson(msg));
        }
    }

    public class GameConfig
    {
        public int playerCount => PlayerConfigurationManager.Instance.Players.Count; // TODO: Add remote players to count
        public List<NetworkPlayerInfo> players = new List<NetworkPlayerInfo>();
        public int selectedMap;
        public int selectedGamemode;
    }
}
