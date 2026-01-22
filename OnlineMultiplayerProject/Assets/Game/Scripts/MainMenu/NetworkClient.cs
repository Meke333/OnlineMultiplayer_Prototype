using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;
    private string MenuSceneName = "MainMenu";

    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        _networkManager.OnClientDisconnectCallback += DoOnDisconnect;
    }

    private void DoOnDisconnect(ulong clientId)
    {
        if (clientId != 0 && clientId != _networkManager.LocalClientId) //Check if Host
            return;

        if (SceneManager.GetActiveScene().name != MenuSceneName && !string.IsNullOrEmpty(MenuSceneName))
        {
            SceneManager.LoadScene(MenuSceneName, LoadSceneMode.Single);
        }

        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if (_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= DoOnDisconnect;
        }
    }
}