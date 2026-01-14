using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

[Obsolete]
public sealed class ServerSingleton : MonoBehaviour
{
    public static ServerSingleton Instance { get; set; }
    public ServerGameManager GameManager {get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();

        //GameManager = new ServerGameManager(
        //ApplicationData.IP(),
        //ApplicationData.Port(),
        //ApplicationData.QPort(),
        //NetworkManager.Singleton);

        //ApplicationData is deprecated
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}