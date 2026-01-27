using System;
using System.Data;
using System.Threading.Tasks;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] private InputReader _playerInputReader;
    [SerializeField] private PlayerStateMachine _playerStateMachine; 
    
    public NetworkVariable<PlayerNumberType> PlayerNumber = new NetworkVariable<PlayerNumberType>();
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
        
        UpdatePlayerNumberClientRpc();
    }
    
    //ClientRpc
    [ClientRpc]
    private void UpdatePlayerNumberClientRpc()
    {
        PlayerNumber.Value = PlayerNumberValue;
    }
    
    [ClientRpc]
    public void UpdatePlayerAfterLoadClientRpc()
    {
        if (InitialTransformInformation.Instance == null)
            return;
        //Position
        Transform information = PlayerNumber.Value switch
        {
            PlayerNumberType.PlayerOne => InitialTransformInformation.Instance.PlayerOneInformation,
            PlayerNumberType.PlayerTwo => InitialTransformInformation.Instance.PlayerTwoInformation,
            _ => InitialTransformInformation.Instance.PlayerTwoInformation
        };

        transform.SetPositionAndRotation(information.position, information.rotation);
        
        //Cinemachine
        if (!IsOwner) return;

        CinemachineVirtualCameraBase cinemachine = CinemachineCore.GetVirtualCamera(0);
        if (cinemachine)
        {
            cinemachine.Follow = _cinemachineCameraTarget.transform;
            //cinemachine.LookAt = _cinemachineCameraTarget.transform;
        }


    }
}