using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private Button _lobbyButton;
    [SerializeField] private TMP_Dropdown PrefabDropdown;
    [SerializeField] private int _minNameLength = 1;
    [SerializeField] private int _maxNameLength = 12;

    private int PrefabSelectedIndex = 0;

    public static string Name {get; private set;}
    public const string PlayerNameKey = "PlayerName";
    public const string PlayerPrefabKey = "PlayerPrefabIndex";

    void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            return;
        }

        _nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        Name = _nameField.text;
        PrefabDropdown.value = PlayerPrefs.GetInt(PlayerPrefabKey, 0);


        HandleNameChanged();
    }

    public void HandleNameChanged()
    {
        bool isFittingName = (_nameField.text.Length >= _minNameLength && _nameField.text.Length <= _maxNameLength);

        _hostButton.interactable = isFittingName;
        _clientButton.interactable = isFittingName;
        _lobbyButton.interactable = isFittingName;
        Name = _nameField.text;
    }

    public void OnPrefabIndexChanged(int newIndex)
    {
        PrefabSelectedIndex = newIndex;
    }

    public void DoConnect()
    {
        OnPrefabIndexChanged(PrefabDropdown.value);
        
        PlayerPrefs.SetString(PlayerNameKey, _nameField.text);
        PlayerPrefs.SetInt(PlayerPrefabKey, PrefabSelectedIndex);
        Name = _nameField.text;
    }
}