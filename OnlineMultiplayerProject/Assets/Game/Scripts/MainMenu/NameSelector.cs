using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private Button _lobbyButton;
    [SerializeField] private int _minNameLength = 1;
    [SerializeField] private int _maxNameLength = 12;

    public static string Name {get; private set;}
    public const string PlayerNameKey = "PlayerName";

    void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            return;
        }

        _nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        Name = _nameField.text;

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

    public void DoConnect()
    {
        PlayerPrefs.SetString(PlayerNameKey, _nameField.text);
        Name = _nameField.text;
    }
}