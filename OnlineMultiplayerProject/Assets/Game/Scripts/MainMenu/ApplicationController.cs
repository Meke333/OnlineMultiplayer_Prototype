using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientSingletonPrefab;
    [SerializeField] private HostSingleton _hostSingletonPrefab;
    [SerializeField] private ServerSingleton _serverSingletonPrefab;

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
        hostSingleton.CreateHost();
        
        ClientSingleton clientSingleton = Instantiate(_clientSingletonPrefab);
        bool isAuthenticated = await clientSingleton.CreateClient();

        if (isAuthenticated)
        {
            //go to main menu
            clientSingleton.GameManager.GoToMenu();
            
        }


    }
}