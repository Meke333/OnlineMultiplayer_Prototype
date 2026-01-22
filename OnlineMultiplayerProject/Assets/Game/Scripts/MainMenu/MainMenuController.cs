using System;
using TMPro;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Text _queueStatusText;
    [SerializeField] private TMP_Text _queueTimerText;
    [SerializeField] private TMP_Text _findMatchButtonText;


    private void Start()
    {
        if (ClientSingleton.Instance == null)
            return;
        
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _queueStatusText.text = string.Empty;
        _queueTimerText.text = string.Empty;
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(_inputField.text);
    }

}

