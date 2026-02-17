using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Text _queueStatusText;
    [SerializeField] private TMP_Text _queueTimerText;
    [SerializeField] private TMP_Text _findMatchButtonText;
    [SerializeField] private Toggle _privateServerToggle;


    private void Start()
    {
        if (ClientSingleton.Instance == null)
            return;
        
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _queueStatusText.text = string.Empty;
        _queueTimerText.text = string.Empty;
    }

    public HostData SetHostData()
    {
        HostData result = new HostData();
        result.isPrivate = _privateServerToggle.isOn;
        
        return result;
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync(SetHostData());
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(_inputField.text);
    }

}