using UnityEngine;

public class InteractionBetweenPlayersMediator : MonoBehaviour
{
    public static InteractionBetweenPlayersMediator Instance {get; private set;}
    
    [SerializeField] private PlayerDamage _playerOne;
    [SerializeField] private PlayerDamage _playerTwo;

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
        PlayerDamage target = attacker == PlayerNumberType.PlayerOne ? _playerTwo : _playerOne;

        bool result = target.DoesAttackLand(direction, type);
        if (result)
            target.ProcessDamage(type);
        
        return result;
    }
}