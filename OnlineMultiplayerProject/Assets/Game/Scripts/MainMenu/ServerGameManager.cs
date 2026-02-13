using System;
using System.Collections.Generic;
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
    
    public NetworkServer networkServer { get; private set; }
    //private MatchplayBackfiller backfiller;/
#if UNITY_SERVER
    private MultiplayAllocation multiplayAllocationService;
#endif
    
    [SerializeField] private string SceneName = "Gameplay";

    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager networkManager, List<NetworkObject> playerPrefabs)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;

#if UNITY_SERVER
        networkServer = new NetworkServer(networkManager);
        multiplayAllocationService = new MultiplayAllocation();
#endif
    }

    public async Task StartGameServerAsync()
    {
#if UNITY_SERVER
        // tells UGS to run a loop to monitor the server

        await multiplayAllocationService.BeginServerCheck();

        try {
           MatchmakingResults matchmakerPayload = await GetMatchmakerPayload();

           if (matchmakerPayload != null)
           {
             await StartBackfill(matchmakerPayload);
             networkServer.OnUserJoined += UserJoined;
             networkServer.OnUserLeft += UserLeft;
           }
           else {
              Debug.LogWarning("WARNING: Matchmaker payload timed out");
           }
        }

        catch (Exception e){
           Debug.LogWarning(e);
        }

        
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
#endif
    }
    
#if UNITY_SERVER

    private async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        Task<MatchmakingResults> matchmakingPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();
        // go on if one of the two tasks are finished:
        if (await Task.WhenAny(matchmakingPayloadTask, Task.Delay(20000)) == matchmakingPayloadTask)
        {
            return matchmakingPayloadTask.Result; // only if payload task was first
        }

        return null; // if timer was first
    }

    private async Task StartBackfill(MatchmakingResults payload)
    {
        backfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}",
            payload.QueueName, payload.MatchProperties, 20); // max player hartdcoded for the moment

        if (backfiller.NeedsPlayers())
        {
            await backfiller.BeginBackfilling();
        }
    }

    private void UserJoined(UserData user)
    {
        backfiller.AddPlayerToMatch(user);
        multiplayAllocationService.AddPlayer();
        if (!backfiller.NeedsPlayers() && backfiller.IsBackfilling)
        {
           // not awaiting, just stop now
            _= backfiller.StopBackfill();
        }
    }

    private void UserLeft(UserData user)
    {
        int playerCount = backfiller.RemovePlayerFromMatch(user.userAuthId);
        multiplayAllocationService.RemovePlayer();

        if (playerCount <= 0)
        {
            ShutdownServer();
            return;
        }

        if (backfiller.NeedsPlayers() && !backfiller.IsBackfilling)
        {
            _ = backfiller.BeginBackfilling();
        }
    }

    private async void ShutdownServer()
    {
        await backfiller.StopBackfill();
        Dispose();
        // now shut the App down
        Application.Quit();
    }
#endif
    
    public async void Dispose()
    {
#if UNITY_SERVER
        networkServer.OnUserJoined -= UserJoined;
        networkServer.OnUserLeft -= UserLeft;
        backFiller?.Dispose();
        multiplayAllocationService?.Dispose();
        networkServer?.Dispose();
#endif
    }
}