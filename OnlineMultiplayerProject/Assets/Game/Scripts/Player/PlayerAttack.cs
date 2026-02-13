using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private PlayerStateMachine _playerStateMachine;

    private int _attackDirection;
    private AttackType _attackType;
    
    [ServerRpc]
    public void PrepareAttackServerRpc(int direction, AttackType type)
    {
        _attackDirection =  direction;
        _attackType = type;
    }
    
    //ServerRPC
    [ServerRpc]
    public void PerformAttackServerRpc()
    {
        if (!_playerStateMachine)
            return;
        
        Debug.Log("ZZZ: 1");
        
        bool didHit = NetworkServer.PlayerNumberIndex <= 2 
                      || InteractionBetweenPlayersMediator.Instance.
                          AttackInteraction(
                              _playerScript.PlayerNumber.Value, 
                              _attackDirection, 
                              _attackType);
        
        if (!didHit)
            AttackMissedClientRpc();

        AttackCheckedClientRpc();
    }

    [ClientRpc]
    private void AttackCheckedClientRpc()
    {
        if (!IsOwner)
            return;
        
        Debug.Log("ATTACK 1");
        _playerStateMachine.AttackCheckedServerRpc();
    }
    
    [ClientRpc]
    private void AttackMissedClientRpc()
    {
        if (!IsOwner)
            return;
        
        Debug.Log("ATTACK 2");
        
        _playerStateMachine.AttackMissedServerRpc();
    }
}