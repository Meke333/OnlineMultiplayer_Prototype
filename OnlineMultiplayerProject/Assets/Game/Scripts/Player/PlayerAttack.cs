using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private PlayerStateMachine _playerStateMachine;

    private int _attackDirection;
    private AttackType _attackType;
    
    private bool _didAttack;

    private AttackFired _attackFired;

    public AttackFired currentAttackFired;
    
    private PlayerStateType _playerStateType;
    
    [ServerRpc]
    public void PrepareAttackServerRpc(int direction, AttackType type)
    {
        _attackDirection =  direction;
        _attackType = type;

        InteractionBetweenPlayersMediator.Instance.TransmitAttackInteraction
            (_playerScript.PlayerNumber.Value,
                _attackDirection,
                _attackType);
        
    }
    
    //ServerRPC
    [ServerRpc]
    public void PerformAttackServerRpc()
    {
        //Debug.Log("TRI: 1; " + _didAttack);
        
        if (_didAttack)
            return;
        _didAttack = true;
        
        //Debug.Log("TRI: 2");
        DismissAttackFiredRpc();
        
        //currentAttackFired.isActive = false;
        
        //Debug.Log("TRI: 4; " + (_playerStateMachine));
        
        if (!_playerStateMachine)
            return;
        
        //Debug.Log("ATTACK TRIGGER: " + _playerScript.PlayerNumber.Value);
        
        bool didHit = NetworkServer.PlayerNumberIndex <= 2 
                      || InteractionBetweenPlayersMediator.Instance.
                          AttackInteraction(
                              _playerScript.PlayerNumber.Value, 
                              _attackDirection, 
                              _attackType);
        
        if (!didHit)
            AttackMissedClientRpc();

        AttackCheckedClientRpc();
        
        InteractionBetweenPlayersMediator.Instance.TransmitAttackInteraction
        (_playerScript.PlayerNumber.Value,
            0,
            AttackType.Jab_Straight, 
            true);
        
    }

    [Rpc(SendTo.Everyone)]
    public void DismissAttackFiredRpc()
    {
        
        //Debug.Log("TRI: 3");
        currentAttackFired.isActive = false;
    }

    [ClientRpc]
    private void AttackCheckedClientRpc()
    {
        if (!IsOwner)
            return;
        
        _playerStateMachine.AttackCheckedServerRpc();
        
    }
    
    [ClientRpc]
    private void AttackMissedClientRpc()
    {
        if (!IsOwner)
            return;
        
        _playerStateMachine.AttackMissedServerRpc();
    }

    [ServerRpc]
    public void AttackEndedServerRpc()
    {
        //Debug.Log("TRIGGER STOP");
        currentAttackFired.isActive = false;
        _didAttack = false;
    }

    [Rpc(SendTo.Everyone)]
    public void PlayerStateChangedRpc(PlayerStateType type)
    {
        if (_playerStateType == type)
            return;
        
        //Debug.Log("STATE!!!" + type);
        _playerStateType = type;

        if (type == PlayerStateType.Down || type == PlayerStateType.Hurt || type == PlayerStateType.StepBack)
        {
            //Debug.Log("STATE 2!!!");
            if (currentAttackFired && currentAttackFired.isActive)
                currentAttackFired.isActive = false;
            _didAttack = false;
        }
    }
}