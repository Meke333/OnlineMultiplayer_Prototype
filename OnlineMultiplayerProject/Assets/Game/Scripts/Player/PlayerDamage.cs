using Unity.Netcode;
using UnityEngine;

public class PlayerDamage : NetworkBehaviour
{
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private PlayerStateMachine _playerStateMachine;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private AttackDamageConfiguration _configuration;

    public PlayerNumberType GetNumberType()
    {
        return _playerScript.PlayerNumber.Value;
    }
    
    public bool DoesAttackLand(int direction, AttackType type)
    {
        bool result = true;
        
        switch (_playerStateMachine.playerStateType.Value)
        {
            case PlayerStateType.Idle:
            case PlayerStateType.Attack:
            case PlayerStateType.Block:
            case PlayerStateType.Hurt:
                break;
            case PlayerStateType.Dodge:
                Debug.Log("R: DODGE: " + _playerStateMachine.dodgeDirection.Value  + "; " + direction);
                Debug.Log("R: DODGE: " + "left dodge: " + (_playerStateMachine.dodgeDirection.Value < 0 && direction > 0) );
                Debug.Log("R: DODGE: " + "right dodge: " + (_playerStateMachine.dodgeDirection.Value > 0 && direction < 0) );
                
                if (_playerStateMachine.dodgeDirection.Value < 0 && direction > 0)
                    break;
                if (_playerStateMachine.dodgeDirection.Value > 0 && direction < 0)
                    break;
                
                result = false;
                break;
            case PlayerStateType.Down:
                result = false;
                break;
            default:
                Debug.LogError("State not implemented: " + _playerStateMachine.playerStateType.Value);
                break;
        }
        
        return result;
    }

    public bool IsAttackGettingBlocked()
    {
        Debug.Log("R: AttackBlocked? " + _playerStateMachine.playerStateType.Value);
        return _playerStateMachine.playerStateType.Value ==  PlayerStateType.Block;
    }
    
    public void ProcessDamage(AttackType type)
    {
        Debug.Log("R: Processing Damage: " + _playerStateMachine.playerStateType.Value);
        
        //Determine
        float damage = _configuration.DetermineDamage(type) 
                       * (_playerStateMachine.playerStateType.Value == PlayerStateType.Block ? 0.125f : 1f);
        
        HealthBar.Instance?.ChangeHealthRpc(_playerScript.PlayerNumberValue, damage);
        
        
        
        Debug.Log("R: Processing Damage: IsOwner? " + IsOwner);
        //if (!IsOwner)
            //return;
        
        //Check here if Player goes down
        
        if (IsAttackGettingBlocked())
            return;
        
        Debug.Log("R: Processing Damage: Get DamageServerRpc");
        _playerStateMachine.GetDamage();
    }
}