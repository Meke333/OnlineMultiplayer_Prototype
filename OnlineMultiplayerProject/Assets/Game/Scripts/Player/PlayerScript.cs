using System;
using System.Data;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] private InputReader _playerInputReader;
    [SerializeField] private PlayerStateMachine _playerStateMachine; 
    
    [field: SerializeField] public PlayerNumberType PlayerNumber { get; private set; } 

    #region Input

    private Vector2 _inputDirection;
    

    #endregion

    #region Events

    public Action<AttackType, bool> OnIncomingAttack;
    public Action OnKnockdown;

    #endregion

    private void OnEnable()
    {
        
    }
    

    private void OnDisable()
    {
        
    }
    
    #region EventCalls

    private void StateChange(PlayerState obj)
    {
        throw new NotImplementedException();
    }


    #endregion
    
}