using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

//MultiplayAllocationService is deprecated so it can not be used
[Obsolete]
public class ServerGameManager : IDisposable
{
    private string serverIP;
    private int serverPort;
    private int queryPort;
    
    private NetworkServer networkServer;
    //private MultiplayAllocation multiplayAllocationService;

    [SerializeField] private string SceneName = "Gameplay";

    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager networkManager)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;

        networkServer = new NetworkServer(networkManager);

        //multiplayAllocationService = new MultiplayAllocation();
    }

    public async Task StartGameServerAsync()
    {
        //await multiplayAllocationService.BeginServerCheck();

        if (!networkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogError("Error: unable to start server!");
            return;
        }
        
        var status = NetworkManager.Singleton.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Error: Failed to load {SceneName} " +
                $"with a {nameof(SceneEventProgressStatus)}: {status }");
        }
    }
    
    public async void Dispose()
    {
        //multiplayAllocationService?.Dispose();
        networkServer?.Dispose();
    }
}