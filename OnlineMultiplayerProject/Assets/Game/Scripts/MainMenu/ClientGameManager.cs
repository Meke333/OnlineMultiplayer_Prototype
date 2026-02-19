using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation _allocation;
    private NetworkClient _networkClient;
    
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        _networkClient = new NetworkClient(NetworkManager.Singleton);
        
        AuthenticationState authState = await AuthenticationHandler.DoAuth();

        if (authState == AuthenticationState.Authenticated)
        {
            return true;
        }
        
        return false;
    }
    
    //Probably put it in a separate SceneLoader script
    private const string sceneName = "MainMenu";
    
    public void GoToMenu()
    {
        SceneManager.LoadScene(sceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            _allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
        
        transport.SetRelayServerData(AllocationUtils.ToRelayServerData(_allocation, "udp")); //encryption via dtls
        
        //connection approval
        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Client Unknown"),
            userAuthID = AuthenticationService.Instance.PlayerId,
            PrefabIndex = PlayerPrefs.GetInt(NameSelector.PlayerPrefabKey, 0)
        };

        string payload = JsonUtility.ToJson(userData); //JSON to Byte Array
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
        
        
        //here for smooth joining for this project
        await Task.Delay(1500);
        
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
        _networkClient?.Dispose();
    }
}