using System;
using System.Data;
using System.Threading.Tasks;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] private InputReader _playerInputReader;
    [SerializeField] private PlayerStateMachine _playerStateMachine; 
    
    public NetworkVariable<PlayerNumberType> PlayerNumber = new NetworkVariable<PlayerNumberType>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [field: SerializeField] public PlayerNumberType PlayerNumberValue { get; private set; }

    [SerializeField] private GameObject _cinemachineCameraTarget;
    
    #region Input

    private Vector2 _inputDirection;
    

    #endregion

    #region Events

    public Action<AttackType, bool> OnIncomingAttack;
    public Action OnKnockdown;

    #endregion

    /*private async void Start()
    {
        await Task.Delay(4000);
        
        if (IsHost || IsServer)
        {
            UpdatePlayerAfterLoadClientRpc();
        }
    }*/



    #region EventCalls

    private void StateChange(PlayerState obj)
    {
        throw new NotImplementedException();
    }


    #endregion
    
    public void SetPlayerNumberType(PlayerNumberType playerNumberType)
    {
       
        PlayerNumberValue = playerNumberType;
        PlayerNumber.Value = PlayerNumberValue;
        
        //UpdatePlayerNumberClientRpc();
    }
    
    /*
    //ClientRpc
    [ClientRpc]
    private void UpdatePlayerNumberClientRpc()
    {
    }
    */
    
    [ClientRpc]
    public void UpdatePlayerAfterLoadClientRpc(int characterIndex, FixedString32Bytes playerName)
    {
        if (!InitialTransformInformation.Instance)
            return;
        //Position

        Transform playerTransform = PlayerNumber.Value switch
        {
            PlayerNumberType.PlayerOne => InitialTransformInformation.Instance.PlayerOneInformation,
            PlayerNumberType.PlayerTwo => InitialTransformInformation.Instance.PlayerTwoInformation,
            _ => InitialTransformInformation.Instance.PlayerTwoInformation
        };
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

        if (HealthBar.Instance)
        {
            HealthBar.Instance.ChangePlayerImageRpc(PlayerNumber.Value, characterIndex);
            HealthBar.Instance.ChangePlayerNameRpc(PlayerNumber.Value, playerName);
            if (IsOwner)
                HealthBar.Instance.RetrieveInformationServerRpc();
        }

        CinemachineVirtualCameraBase cinemachine = CinemachineCore.GetVirtualCamera(0);
        if (cinemachine)
        {
            cinemachine.transform.position = _cinemachineCameraTarget.transform.position;
            cinemachine.transform.rotation = _cinemachineCameraTarget.transform.rotation;
            //cinemachine.LookAt = _cinemachineCameraTarget.transform;
        }


    }
}