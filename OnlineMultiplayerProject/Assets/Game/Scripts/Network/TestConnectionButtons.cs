using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestConnectionButtons : MonoBehaviour
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    
    private void OnEnable()
    {
        _hostButton.onClick.AddListener(() => { StartHost();});
        _clientButton.onClick.AddListener(() => { StartClient(); });
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
    }

    //Why does this not work?
    /*
    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
    }*/


    private void OnDisable()
    {
        _hostButton.onClick.RemoveListener(() => { StartHost();});
        _clientButton.onClick.RemoveListener(() => { StartClient(); });
    }
    
    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Started Host initiated");
    }
    
    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Started Client initiated");
    }
    
    private void OnClientStarted()
    {
        Debug.Log("Client sucessfully started!");
    }
    
}
