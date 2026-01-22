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
    
    [field: SerializeField] public PlayerNumberType PlayerNumber { get; private set; }

    [SerializeField] private GameObject _cinemachineCameraTarget;
    
    #region Input

    private Vector2 _inputDirection;
    

    #endregion

    #region Events

    public Action<AttackType, bool> OnIncomingAttack;
    public Action OnKnockdown;

    #endregion

    private async void Start()
    {
        await Task.Delay(2000);
        
        if (IsHost || IsServer)
        {
            UpdatePlayerAfterLoadClientRpc();
        }
    }



    #region EventCalls

    private void StateChange(PlayerState obj)
    {
        throw new NotImplementedException();
    }


    #endregion

    public void SetPlayerNumberType(PlayerNumberType playerNumberType)
    {
        PlayerNumber = playerNumberType;
    }
    
    //ClientRpc
    [ClientRpc]
    private void UpdatePlayerAfterLoadClientRpc()
    {
        //Position
        Transform information = PlayerNumber switch
        {
            PlayerNumberType.PlayerOne => InitialTransformInformation.Instance.PlayerOneInformation,
            PlayerNumberType.PlayerTwo => InitialTransformInformation.Instance.PlayerTwoInformation
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