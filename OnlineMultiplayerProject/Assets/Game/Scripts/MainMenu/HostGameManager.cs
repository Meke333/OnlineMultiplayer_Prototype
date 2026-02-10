using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    //How many players can connect!!
    private const int MaxConnections = 2;
    private Allocation _allocation;
    private List<NetworkObject> _playerPrefabs;
    private string _joinCode;
    private string _lobbyID;
    public NetworkServer NetworkServer {get; private set;}

#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset == null)
            return;
        
        _gameSceneName = SceneAsset.name;
    }
#endif
    
    [SerializeField] private string _gameSceneName = "Gameplay";

    public HostGameManager(List<NetworkObject> playerPrefabs)
    {
        _playerPrefabs = playerPrefabs;
    }
    
    public async Task StartHostAsync()
    {
        try
        {
            //Step 1: allocate a Rely by UGS with a maximum of allowed connections
            _allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return;
        }

        try
        {
            //Step 2: Get current join code for allocated relay for the clients to enter
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return;
        }

        //Configure Transport
        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

        //Step 3: Setup Transport for this allocation
        //Allocationutils: necessary since mid 2025
        //connectionType = "udp", "dtls", "ws", "wss"
        transport.SetRelayServerData(AllocationUtils.ToRelayServerData(_allocation, "udp"));

        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            options.Data = new Dictionary<string, Unity.Services.Lobbies.Models.DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _joinCode
                    )
                }
            };
            
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(NameSelector.Name, MaxConnections, options);
            _lobbyID = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15.0f));

        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
            return;
        }
        
        //connection approval
        NetworkServer = new NetworkServer(NetworkManager.Singleton, _playerPrefabs);

        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Host Unknown"),
            userAuthID = AuthenticationService.Instance.PlayerId,
            PrefabIndex = PlayerPrefs.GetInt(NameSelector.PlayerPrefabKey, 0)
        };
        
        string payload = JsonUtility.ToJson(userData); //JSON to Byte Array
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        
        
        //Step 4: Finally start the host
        NetworkManager.Singleton.StartHost();
        //Step 5: Change to main gameplay scene
        NetworkManager.Singleton.SceneManager.LoadScene(_gameSceneName, LoadSceneMode.Single);
    }

    private IEnumerator HeartbeatLobby(float waitTime)
    {
        WaitForSecondsRealtime delay = new  WaitForSecondsRealtime(waitTime);

        while (true) //runs infinitely - later we have to find a smarter way of shutting  down the lobby - not just in quit
        {
            LobbyService.Instance.SendHeartbeatPingAsync(_lobbyID);
            yield return delay;
        }
    }

    public async void Dispose()
    {
        if (!string.IsNullOrEmpty(_lobbyID))
            return;
        
        HostSingleton.Instance.StopCoroutine(nameof(HeartbeatLobby));

        if (!string.IsNullOrEmpty(_lobbyID))
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_lobbyID);
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogError(ex.Message);
            }

            _lobbyID = string.Empty;
        }
        
        NetworkServer?.Dispose();
    }
}