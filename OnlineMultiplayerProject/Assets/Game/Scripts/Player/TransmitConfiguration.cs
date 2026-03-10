using Unity.Netcode;
using UnityEngine;

public class TransmitConfiguration : NetworkBehaviour
{
    public static TransmitConfiguration Instance {get; private set;}
    
    [SerializeField] private AllCharacterVoiceClipConfigurations _allConfigs;
    
    public CharacterVoiceClipConfiguration playerOneVoiceConfig;
    public CharacterVoiceClipConfiguration playerTwoVoiceConfig;

    private int _voiceIndex1 = -1;
    private int _voiceIndex2 = -1;

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
    
    [Rpc(SendTo.Everyone)]
    public void SetAudioConfigurationRpc(PlayerNumberType playerNumber, int index)
    {
        Debug.Log(playerNumber);
        Debug.Log(index);
        Debug.Log(_allConfigs == null);
        
        if (index < 0 || index >= _allConfigs.VoiceClips.Length)
            return;
        
        switch (playerNumber)
        {
            case PlayerNumberType.PlayerOne:
                playerOneVoiceConfig = _allConfigs.VoiceClips[index];
                _voiceIndex1 = index;
                break;
            case PlayerNumberType.PlayerTwo:
            default:
                playerTwoVoiceConfig = _allConfigs.VoiceClips[index];
                _voiceIndex2 = index;
                break;
        }
        
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RetrieveInformationServerRpc()
    {
        if (!IsOwner)
            return;
        
        SendInformation(_voiceIndex1, _voiceIndex2);
    }
    
    private void SendInformation(int index1, int index2)
    {
        SetAudioConfigurationRpc(PlayerNumberType.PlayerOne, index1);
        SetAudioConfigurationRpc(PlayerNumberType.PlayerTwo, index2);
    }

}