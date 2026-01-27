using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    
    private List<NetworkObject> _playerPrefabs;

    private int playerNumberIndex = 1;
    
    
    // two events for backfilling:
    public Action<UserData> OnUserJoined;
    public Action<UserData> OnUserLeft;
    
    public Action<string> OnClientLeft;
    
    private Dictionary<ulong, string> _clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authIdToUserData = new Dictionary<string, UserData>();
    private Dictionary<ulong, bool> _clientIdtoActiveCheck = new Dictionary<ulong, bool>();
    
    public NetworkServer(NetworkManager networkManager, List<NetworkObject> playerPrefabs)
    {
        _networkManager = networkManager;
        _playerPrefabs = playerPrefabs;
        
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += DoOnServerStarted;
    }

    public bool OpenConnection(string ip, int port)
    {
        UnityTransport transport = _networkManager.gameObject.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort) port);
        return _networkManager.StartServer();
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        Debug.Log("clientId = " + clientId);
        Debug.Log(_clientIdToAuth.ContainsKey(clientId));
        Debug.Log(_clientIdToAuth[clientId] == null);
        
        if (!_clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            Debug.Log("Nope 1");
            return null;
        }

        if (!_authIdToUserData.TryGetValue(authId, out UserData userData))
        {
            Debug.Log("Nope 2");
            return null;
        }
        
        return userData;

    }

    private void DoOnServerStarted()
    {
        _networkManager.OnClientDisconnectCallback += DoOnDisconnect;
    }

    private void DoOnDisconnect(ulong clientId)
    {
        //For some reason this gets called, whenever a client gets Approved???
        Debug.Log("DoOnDisconnect");
        if (_clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            _clientIdToAuth.Remove(clientId);
            OnUserLeft?.Invoke(_authIdToUserData[authId]);
            _authIdToUserData.Remove(authId);
            OnClientLeft?.Invoke(authId);

            playerNumberIndex--;
        }

        //playerNumberIndex = _clientIdtoActiveCheck.Count;
    }

    //We can make it a Task and make it async and make a time-out!!
    private async void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        response.Pending = true;
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload); //Byte array into string (JSON); better than custom parsing
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        
        Debug.Log("Approval for: " + userData.userName);
        Debug.Log(userData.PrefabIndex);

        //CHECK IF USER DATA IS VALID HERE!!! (name checkbox)

        Debug.Log("ClientNetworkId = " + request.ClientNetworkId);
        
        //To-Do: If ClientID is the same and the it's old in the dictionary: replace it!
        
        if (!_clientIdToAuth.TryAdd(request.ClientNetworkId, userData.userAuthID))
            _clientIdToAuth[request.ClientNetworkId] = userData.userAuthID;
        Debug.Log(_clientIdToAuth[request.ClientNetworkId] == null);
        
        if (!_authIdToUserData.TryAdd(userData.userAuthID, userData))
            _authIdToUserData[userData.userAuthID] = userData;
        Debug.Log(_authIdToUserData[userData.userAuthID] == null);
        
        OnUserJoined?.Invoke(userData);
        
        //This disables instantiating a player prefab
        response.CreatePlayerObject = false;
        response.Approved = true;
        response.Pending = false;
        Debug.Log("Hello 0");
        Debug.Log(_clientIdToAuth[request.ClientNetworkId] == null);
        Debug.Log(_authIdToUserData[userData.userAuthID] == null);
        await SpawnPlayerDelayed(request.ClientNetworkId);
        Debug.Log("Hello 6");
        
        //response.CreatePlayerObject = true;
    }

    private async Task SpawnPlayerDelayed(ulong clientId)
    {
        Debug.Log("Hello 1");
        Debug.Log("Hello 2.1");
        
        
        Debug.Log(_clientIdToAuth[clientId] == null);
        
        UserData userdata = GetUserDataByClientId(clientId);

        //To-Do: Exchange Position and Rotation dependent on Playernumber
        Debug.Log("_playerPrefab == null: " + (_playerPrefabs == null));
        Debug.Log("userData == null: " + (userdata == null));
        
        Debug.Log("Hello 2.2: " + _playerPrefabs.Count + "; " + userdata.PrefabIndex);
        NetworkObject playerObject = GameObject.Instantiate(_playerPrefabs[userdata.PrefabIndex], 
            new Vector3(100, 50, 100),
            Quaternion.identity);
        Debug.Log("Hello 2.3");
        playerObject.SpawnAsPlayerObject(clientId);
        Debug.Log("Spawn Player: " + _playerPrefabs[userdata.PrefabIndex] + "; ID: " + clientId);
        Debug.Log("Hello 3");
        //To-Do: Position
        
        //PlayerNumber
        if (playerNumberIndex > 2)
        {
            Debug.Log("There are already 2 players in the game!");
        }
        else
        {

            await Task.Delay(3000);
            PlayerNumberType number = (PlayerNumberType)playerNumberIndex;
            Debug.Log("PlayerNumber: " + playerNumberIndex + "; " + (PlayerNumberType)playerNumberIndex);
            PlayerScript player = playerObject.GetComponentInChildren<PlayerScript>();
            player.SetPlayerNumberType((PlayerNumberType)playerNumberIndex);
            
            switch (number)
            {
                case PlayerNumberType.PlayerOne:
                    InteractionBetweenPlayersMediator.Instance.playerOne =
                        playerObject.GetComponentInChildren<PlayerDamage>();
                    break;
                case PlayerNumberType.PlayerTwo:
                    InteractionBetweenPlayersMediator.Instance.playerTwo =
                        playerObject.GetComponentInChildren<PlayerDamage>();
                    break;
            }

            if (player.IsHost || player.IsServer)
            {
                player.UpdatePlayerAfterLoadClientRpc();
            }
            Debug.Log("Hello 4");
            
            playerNumberIndex++;
            
            Debug.Log("Hello 5");
        }
    }
    
    
    public void Dispose()
    {
        if (_networkManager != null)
        {
            _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _networkManager.OnClientDisconnectCallback -= DoOnDisconnect;
            _networkManager.OnServerStarted -= DoOnServerStarted;
        }

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}