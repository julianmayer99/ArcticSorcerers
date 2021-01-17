using Assets.Scripts.Items;
using Assets.Scripts.Items.Networking;
using Assets.Scripts.Networking.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameClient : MonoBehaviour
{
    public static GameClient instance;

    public string ip = "127.0.0.1";
    public int port = 8082;
    public int myId = 0;
    public TCP tcp;

    private bool isConnected = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        tcp = new TCP();

        isConnected = true;
        tcp.Connect();
        Debug.Log("Connected to server: " + tcp.socket.Client.RemoteEndPoint);
        GameSettings.activeConnection = GameSettings.Connection.Client;
    }

    public void SetIPAddress(string ip)
    {
        this.ip = ip;
    }

    private int fixedUpdateCounter = 0;
    private void FixedUpdate()
    {
        if (!isConnected)
            return;

        fixedUpdateCounter++;

        if (fixedUpdateCounter < NetworkingInfoManager.fixedUpdateCounterLimit)
            return;

        fixedUpdateCounter = 0;
        NetworkingInfoManager.Client_AddLocalGameLoop();
        tcp.SendMessage(GameServer.MessageType.GameUpdateLoop, JsonUtility.ToJson(NetworkingInfoManager.currentLocalClientGameLoop));
    }

    void HandleIncommingMessage(string rawMessage)
    {
        switch (rawMessage[0])
        {
            case (char)GameServer.MessageType.GameUpdateLoop:
                rawMessage = rawMessage.Remove(0, 1);
                var loopMessage = JsonUtility.FromJson<GameLoopMessage>(rawMessage);
                NetworkingInfoManager.currentClientRemoteLoop = loopMessage;
                NetworkPlayerManager.instance.UpdatePlayerPositions();
                break;
            case (char)GameServer.MessageType.ServerConfig:
                rawMessage = rawMessage.Remove(0, 1);
                NetworkingInfoManager.config = JsonUtility.FromJson<GameConfig>(rawMessage);
                break;
            case (char)GameServer.MessageType.AcceptPlayer:
                rawMessage = rawMessage.Remove(0, 1);
                var playerInfo = JsonUtility.FromJson<NetworkPlayerInfo>(rawMessage);
                var localPlayer = PlayerConfigurationManager.Instance.Players.SingleOrDefault(p => p.config.PlayerIndex == playerInfo.localId);
                if (localPlayer != null)
                {
                    localPlayer.config.NetworkPlayerIndex = playerInfo.playerNetworkId;
                    myId = playerInfo.clientId;
                }
                break;

        }
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private byte[] receiveBuffer;

        /// <summary>Attempts to connect to the server via TCP.</summary>
        public void Connect()
        {
            socket = new TcpClient();

            receiveBuffer = new byte[socket.ReceiveBufferSize];
            var ip = IPAddress.Parse(instance.ip);

            socket.BeginConnect(ip, instance.port, ConnectCallback, socket);
        }

        /// <summary>Initializes the newly connected client's TCP-related info.</summary>
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, ReceiveCallback, null);
            GameClient.instance.RegisterPlayers();
        }

        /// <summary>Sends data to the client via TCP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendMessage(GameServer.MessageType type, string message)
        {
            try
            {
                if (socket != null)
                {
                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes((char)type + message);
                    stream.BeginWrite(bytesToSend, 0, bytesToSend.Length, null, null); // Send data to server
                    Debug.Log("Me -> Server : " + message);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        /// <summary>Reads incoming data from the stream.</summary>
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                string recievedMsg = Encoding.ASCII.GetString(receiveBuffer, 0, receiveBuffer.Length);
                Debug.Log("Server -> Me : " + recievedMsg);
                GameClient.instance.HandleIncommingMessage(recievedMsg);

                stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    private void RegisterPlayers()
    {
        foreach (var player in PlayerConfigurationManager.Instance.Players)
        {
            NetworkingInfoManager.Client_RegisterPlayer(player);
        }
    }

    /// <summary>Disconnects from the server and stops all network traffic.</summary>
    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();

            Debug.Log("Disconnected from server.");
            GameSettings.activeConnection = GameSettings.Connection.None;
        }
    }
}
