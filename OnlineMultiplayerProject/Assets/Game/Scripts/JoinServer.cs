using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class JoinServer : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    
    public void JoinClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Connect as Client");
        this.gameObject.SetActive(false);

        _text.text = "CLIENT";
    }
    
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Start as Host");
        this.gameObject.SetActive(false);
        
        _text.text = "HOST";
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        Debug.Log("Connect as Server");
        this.gameObject.SetActive(false);
        
        _text.text = "SERVER";
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
    }

    private void OnClientStarted()
    {
        Debug.Log("Client successfully started");
    }
    
    
}
