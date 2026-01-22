using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientSingletonPrefab;
    [SerializeField] private HostSingleton _hostSingletonPrefab;
    [SerializeField] private ServerSingleton _serverSingletonPrefab;
    [SerializeField] private List<NetworkObject> _playerPrefabs;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchAs(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchAs(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            ServerSingleton serverSingleton = Instantiate(_serverSingletonPrefab);
            await serverSingleton.CreateServer();
            return;
        }

        HostSingleton hostSingleton = Instantiate(_hostSingletonPrefab);
        hostSingleton.CreateHost(_playerPrefabs);
        
        ClientSingleton clientSingleton = Instantiate(_clientSingletonPrefab);
        bool isAuthenticated = await clientSingleton.CreateClient();

        if (isAuthenticated)
        {
            //go to main menu
            clientSingleton.GameManager.GoToMenu();
            
        }


    }
}