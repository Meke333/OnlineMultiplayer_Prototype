using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
    

    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager;
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
        if (!clientIdToAuth.TryGetValue(clientId, out string authId)) return null;
        if (!authIdToUserData.TryGetValue(authId, out UserData userData)) return null;
        
        return userData;

    }

    private void DoOnServerStarted()
    {
        _networkManager.OnClientDisconnectCallback += DoOnDisconnect;
    }

    private void DoOnDisconnect(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }

    //We can make it a Task and make it async and make a time-out!!
    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload); //Byte array into string (JSON); better than custom parsing
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        
        Debug.Log("Approval for: " + userData.userName);

        //CHECK IF USER DATA IS VALID HERE!!! (name checkbox)

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthID;
        authIdToUserData[userData.userAuthID] = userData;
        
        response.Approved = true;
        response.CreatePlayerObject = true;
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