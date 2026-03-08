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
        if (_didAttack)
            return;
        _didAttack = true;
        
        if (!_playerStateMachine)
            return;
        
        Debug.Log("ATTACK TRIGGER");
        
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
        _didAttack = false;
    }
}