using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] private InputReader _playerInputReader;
    [SerializeField] private PlayerStateMachine _playerStateMachine; 
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private int _characterVoiceConfigIndex;
    
    public NetworkVariable<PlayerNumberType> PlayerNumber = new NetworkVariable<PlayerNumberType>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [field: SerializeField] public PlayerNumberType PlayerNumberValue { get; private set; }

    [SerializeField] private GameObject _cinemachineCameraTarget;

    [SerializeField] private GameObject _characterGraphics;
    [SerializeField] private GameObject _glovesGraphics;
    
    #region Input

    private Vector2 _inputDirection;
    

    #endregion

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
        
        if (!HealthBar.Instance)
            return;
        
        HealthBar.Instance.ChangeHealthRpc(PlayerNumber.Value, HealthBar.MaxHealth, false);
        HealthBar.Instance.ChangePlayerImageRpc(PlayerNumber.Value, -1);
        HealthBar.Instance.ChangePlayerNameRpc(PlayerNumber.Value, new FixedString32Bytes());
    }

    public void SetPlayerNumberType(PlayerNumberType playerNumberType)
    {
       
        PlayerNumberValue = playerNumberType;
        PlayerNumber.Value = PlayerNumberValue;
        
        //UpdatePlayerNumberClientRpc();
    }
    
    [ClientRpc]
    public void UpdatePlayerAfterLoadClientRpc(int characterIndex, FixedString32Bytes playerName)
    {
        if (!InitialTransformInformation.Instance)
            return;

        Transform playerTransform = PlayerNumber.Value switch
        {
            PlayerNumberType.PlayerOne => InitialTransformInformation.Instance.PlayerOneInformation,
            PlayerNumberType.PlayerTwo => InitialTransformInformation.Instance.PlayerTwoInformation,
            _ => InitialTransformInformation.Instance.PlayerTwoInformation
        };
        
        /*switch (PlayerNumber.Value)
        {
            case PlayerNumberType.PlayerOne:
                InteractionBetweenPlayersMediator.Instance.playerOneDamage =
                    gameObject.GetComponentInChildren<PlayerDamage>();
                InteractionBetweenPlayersMediator.Instance.playerOneStateMachine =
                    gameObject.GetComponentInChildren<PlayerStateMachine>();
                InteractionBetweenPlayersMediator.Instance.playerOneVoiceConfig =
                    CharacterVoiceConfiguration;
                break;
            case PlayerNumberType.PlayerTwo:
            default:
                InteractionBetweenPlayersMediator.Instance.playerTwoDamage =
                    gameObject.GetComponentInChildren<PlayerDamage>();
                InteractionBetweenPlayersMediator.Instance.playerTwoStateMachine =
                    gameObject.GetComponentInChildren<PlayerStateMachine>();
                InteractionBetweenPlayersMediator.Instance.playerTwoVoiceConfig =
                    CharacterVoiceConfiguration;
                break;
        }
        */
/*
        Transform opponentTransform;
        
        switch (PlayerNumber.Value)
        {
            case PlayerNumberType.PlayerOne:
                playerTransform = InitialTransformInformation.Instance.PlayerOneInformation;
                opponentTransform = InitialTransformInformation.Instance.PlayerTwoInformation;
                break;
            case PlayerNumberType.PlayerTwo:
            default:
                playerTransform = InitialTransformInformation.Instance.PlayerTwoInformation;
                opponentTransform = InitialTransformInformation.Instance.PlayerOneInformation;
                break;
        
        }
  */      
        transform.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
        
        
        //Cinemachine
        if (!IsOwner) 
            return;

        
        if (TransmitConfiguration.Instance)
        {
            Debug.Log(TransmitConfiguration.Instance == null);
            Debug.Log(PlayerNumber.Value);
            Debug.Log(_characterVoiceConfigIndex);
            
            TransmitConfiguration.Instance.SetAudioConfigurationRpc(PlayerNumber.Value, _characterVoiceConfigIndex);
            TransmitConfiguration.Instance.RetrieveInformationServerRpc();
        }

        if (HealthBar.Instance)
        {
            HealthBar.Instance.ChangePlayerImageRpc(PlayerNumber.Value, characterIndex);
            HealthBar.Instance.ChangePlayerNameRpc(PlayerNumber.Value, playerName);
            HealthBar.Instance.RetrieveInformationServerRpc();
        }

        _playerAnimator.ToggleAnimators(true);
        
        _characterGraphics.SetActive(false);
        _glovesGraphics.SetActive(true);
        
        CinemachineVirtualCameraBase cinemachine = CinemachineCore.GetVirtualCamera(0);
        if (cinemachine)
        {
            cinemachine.transform.position = _cinemachineCameraTarget.transform.position;
            cinemachine.transform.rotation = _cinemachineCameraTarget.transform.rotation;
            //cinemachine.LookAt = _cinemachineCameraTarget.transform;
        }


    }
}