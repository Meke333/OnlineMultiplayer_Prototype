using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public sealed class HostSingleton : MonoBehaviour
{
    public static HostSingleton Instance { get; set; }
    public HostGameManager GameManager { get; private set; }
    
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

    public void CreateHost(List<NetworkObject> playerPrefabs)
    {
        GameManager = new HostGameManager(playerPrefabs);
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}