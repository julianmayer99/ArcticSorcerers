using Assets.Scripts.Items.Networking;
using Assets.Scripts.Networking.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class ServerClient
    {
        public TcpClient tcp;
        public int id;
        public NetworkStream stream;
        private byte[] buffer;

        public void Initialize()
        {
            buffer = new byte[tcp.ReceiveBufferSize];
            if (stream == null)
                stream = tcp.GetStream();

            stream.BeginRead(buffer, 0, tcp.ReceiveBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int _byteLength = stream.EndRead(ar);
                if (_byteLength <= 0)
                {
                    tcp.Close();
                    Debug.Log("Disconnected client " + id);
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(buffer, _data, _byteLength);

                string recievedMsg = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                Debug.Log($"Client {id} -> Server : " + recievedMsg);
                ProcessRawMessage(recievedMsg);
                stream.BeginRead(buffer, 0, tcp.ReceiveBufferSize, ReceiveCallback, null);
            }
            catch
            {
                tcp.Close();
                Debug.Log("Disconnected client " + id);
            }
        }

        void ProcessRawMessage(string rawMessage)
        {
            switch (rawMessage[0])
            {
                case (char)GameServer.MessageType.GameUpdateLoop:
                    rawMessage = rawMessage.Remove(0, 1);
                    var loopMessage = JsonUtility.FromJson<GameLoopMessage>(rawMessage);
                    NetworkingInfoManager.Server_AddClientMessageToLoop(loopMessage);
                    NetworkPlayerManager.instance.UpdatePlayerPositions(id);
                    break;
                case (char)GameServer.MessageType.RegisterPlayer:
                    rawMessage = rawMessage.Remove(0, 1);
                    var rpMsg = JsonUtility.FromJson<RegisterPlayerMessage>(rawMessage);
                    var existingPlayer = NetworkingInfoManager.config.players.SingleOrDefault(p => p.clientId == id && p.localId == rpMsg.localId);
                    if (existingPlayer != null)
                        NetworkingInfoManager.config.players.Remove(existingPlayer);

                    int nid = NetworkingInfoManager.config.playerCount;
                    var pinfo = rpMsg.GetPlayerInfo(id, nid);
                    NetworkingInfoManager.config.players.Add(pinfo);
                    GameServer.instance.SendMessageToAllClients(GameServer.MessageType.ServerConfig, JsonUtility.ToJson(NetworkingInfoManager.config));
                    GameServer.instance.SendMessageToClient(id, GameServer.MessageType.AcceptPlayer, JsonUtility.ToJson(pinfo));

                    break;
                case (char)GameServer.MessageType.Message:
                    break;

            }
        }
    }
}
