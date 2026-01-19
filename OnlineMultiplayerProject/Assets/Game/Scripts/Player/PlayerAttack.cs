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
    


    //ClientRPC
    
    
    //ServerRPC
    [ServerRpc]
    private void PerformAttackServerRpc(int direction, AttackType type)
    {
        bool didHit = InteractionBetweenPlayersMediator.Instance.AttackInteraction(_playerScript.PlayerNumber, direction, type);
        
         
    }
}

public class InteractionBetweenPlayersMediator : MonoBehaviour
{
    public static InteractionBetweenPlayersMediator Instance {get; private set;}
    
    [SerializeField] private PlayerScript _playerOne;
    [SerializeField] private PlayerScript _playerTwo;

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

    public bool AttackInteraction(PlayerNumberType attacker, int direction, AttackType type)
    {
        PlayerScript target = attacker == PlayerNumberType.PlayerOne ? _playerTwo : _playerOne;
        
        return target.CheckToBeHit(direction, type);
    }
}

public enum PlayerNumberType
{
    PlayerOne = 1,
    PlayerTwo = 2,
}