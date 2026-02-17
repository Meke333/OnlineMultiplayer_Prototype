using TMPro;
using Unity.Netcode;
using UnityEngine;

public class FightUIHandler : NetworkBehaviour
{
    public static FightUIHandler Instance;

    [SerializeField] private TMP_Text _countText;
    [SerializeField] private TMP_Text _promptText;
    [SerializeField] private TMP_Text _waitText;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetCountText(int count)
    {
        SetCountTextClientRpc(count);
    }
    
    [ClientRpc]
    private void SetCountTextClientRpc(int count)
    {
        if (count == 0)
        {
            _countText.gameObject.SetActive(false); 
            return;
        }
        
        _countText.text = count.ToString();
        _countText.gameObject.SetActive(true); 
    }

    public void ToggleWaitText(bool isOn)
    {
        ToggleWaitTextClientRpc(isOn);
    }
    
    [ClientRpc]
    private void ToggleWaitTextClientRpc(bool isOn)
    {
        _waitText.gameObject.SetActive(isOn);
    }

    public void SetPromptText(TextType text)
    {
        SetPromptTextClientRpc(text);
    }

    [ClientRpc]
    private void SetPromptTextClientRpc(TextType text)
    {
        if (text == TextType.Nothing)
        {
            _promptText.gameObject.SetActive(false);
            return;
        }
        
        _promptText.text = text switch
        {
            TextType.Fight => TextStrings.Fight,
            TextType.Down => TextStrings.Down,
            TextType.GetReady => TextStrings.GetReady,
            _ => "throw new ArgumentOutOfRangeException(nameof(text), text, null)"
        };
        
        _promptText.gameObject.SetActive(true);
    }
}