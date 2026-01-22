using System;
using UnityEngine;

public class InteractionBetweenPlayersMediator : MonoBehaviour
{
    public static InteractionBetweenPlayersMediator Instance {get; private set;}
    
    public PlayerDamage playerOne;
    public PlayerDamage playerTwo;

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
        PlayerDamage target = attacker == PlayerNumberType.PlayerOne ? playerTwo : playerOne;

        bool result = target.DoesAttackLand(direction, type);
        if (result)
            target.ProcessDamage(type);
        
        return result;
    }
}