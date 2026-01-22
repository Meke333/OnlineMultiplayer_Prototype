using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private PlayerEvents _playerEvents;
    
    //Here we trigger the check for the server to transmit an incoming attack 
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _playerEvents.TransmitAttack += EvaluateAttack;
    }

    public override void OnNetworkDespawn()
    {
        _playerEvents.TransmitAttack -= EvaluateAttack;
    }

    #region Events

    private void EvaluateAttack(int direction, AttackType type)
    {
        PerformAttackServerRpc(direction, type);
    }

    #endregion
    
    
    //ServerRPC
    [ServerRpc]
    private void PerformAttackServerRpc(int direction, AttackType type)
    {
        bool didHit = InteractionBetweenPlayersMediator.Instance.AttackInteraction(_playerScript.PlayerNumber, direction, type);

        if (!didHit)
            AttackMissedClientRpc();
    }
    
    //ClientRpc
    [ClientRpc]
    private void AttackMissedClientRpc()
    {
        _playerEvents.TransmitMissedAttack?.Invoke(true);
    }
}