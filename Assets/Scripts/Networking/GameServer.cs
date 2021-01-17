using Assets.Scripts.Items;
using Assets.Scripts.Items.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using UnityEngine;

public class GameServer : MonoBehaviour
{
    public static GameServer instance;
    private TcpListener listener;
    private Dictionary<int, ServerClient> clients = new Dictionary<int, ServerClient>();
    public int Port { get; private set; } = 8082;
    private IPAddress localAddr;
    const int maxPlayers = 8;
    private bool serverIsRunning = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        localAddr = IPAddress.Parse("127.0.0.1");
    }

    public void StartServer(int port)
    {
        this.Port = port;

        if (serverIsRunning)
        {
            Debug.LogError("Trying to start the server, but is is already running");
            return;
        }

        listener = new TcpListener(localAddr, Port);
        Debug.Log("Listening on " + Port + " ...");
        Debug.Log("Local ip: " + GetLocalIPAddress());

        foreach (var player in PlayerConfigurationManager.Instance.Players)
        {
            player.config.NetworkPlayerIndex = player.config.PlayerIndex;
        }

        listener.Start();
        GameSettings.activeConnection = GameSettings.Connection.Host;
        listener.BeginAcceptTcpClient(OnTcpClientConnected, null);
        serverIsRunning = true;
    }

    private void OnTcpClientConnected(IAsyncResult ar)
    {
        var client = listener.EndAcceptTcpClient(ar);
        listener.BeginAcceptTcpClient(OnTcpClientConnected, null);


        if (clients.Count >= maxPlayers)
        {
            Debug.LogWarning("Cant accept client - Server is full");
            return;
        }
        var nclient = new ServerClient { tcp = client, id = clients.Count };
        clients.Add(nclient.id, nclient);
        Debug.Log($"Incoming connection from {nclient.tcp.Client.RemoteEndPoint}...");
        nclient.Initialize();
    }

    private int fixedUpdateCounter = 0;

    private void FixedUpdate()
    {
        if (!serverIsRunning)
            return;

        if (clients.Count < 1)
            return;

        fixedUpdateCounter++;

        if (fixedUpdateCounter < NetworkingInfoManager.fixedUpdateCounterLimit)
            return;

        fixedUpdateCounter = 0;
        NetworkingInfoManager.AddCurrentLocalGameLoopState(ref NetworkingInfoManager.currentServerGameLoop);
        SendMessageToAllClients(NetworkingInfoManager.currentServerGameLoop);
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public void SendMessageToAllClients(GameLoopMessage message)
        => SendMessageToAllClients(MessageType.GameUpdateLoop,
            JsonUtility.ToJson(NetworkingInfoManager.currentServerGameLoop));

    public void SendMessageToAllClients(MessageType type, string message)
    {
        foreach (var client in clients)
        {
            SendMessageToClient(client.Value.id, type, message);
        }
    }

    public void SendMessageToClient(int clientId, MessageType type, string message)
    {
        if (!clients.ContainsKey(clientId))
        {
            Debug.LogError("Client not found " + clientId);
            return;
        }

        var client = clients[clientId];

        if (client.stream == null) client.stream = client.tcp.GetStream();

        Debug.Log($"<color=#7eff70>" + $"Server -> {clientId}" + "</color>" + " : " + (char)type + message);
        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes((char)type + message);
        client.stream.Write(bytesToSend, 0, bytesToSend.Length);
    }

    public void StopServer()
    {
        if (!serverIsRunning)
            return;

        listener.Stop();
        foreach (var client in clients)
        {
            client.Value.tcp.Close();
        }

        serverIsRunning = false;
        Debug.Log("Server stopped. Disconnected " + clients.Count + " clients.");
        clients.Clear();
        GameSettings.activeConnection = GameSettings.Connection.None;
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }

    public enum MessageType
    {
        ServerConfig = 'c',
        GameUpdateLoop = 'u',
        Message = 'm',
        RegisterPlayer = 'r',
        AcceptPlayer = 'a'
    }
}
