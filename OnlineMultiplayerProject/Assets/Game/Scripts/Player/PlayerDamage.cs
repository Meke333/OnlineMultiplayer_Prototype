using Unity.Netcode;
using UnityEngine;

public class PlayerDamage : NetworkBehaviour
{
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private PlayerStateMachine _playerStateMachine;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private AttackDamageConfiguration _configuration;
    
    private PlayerStateType _playerState;

    public bool DoesAttackLand(int direction, AttackType type)
    {
        Debug.Log("ZZZ: 3");
        
        bool result = true;
        
        switch (_playerStateMachine.playerStateType.Value)
        {
            case PlayerStateType.Idle:
            case PlayerStateType.Attack:
            case PlayerStateType.Block:
            case PlayerStateType.Hurt:
                break;
            case PlayerStateType.Dodge:
                if (_inputReader.MoveDirection.x < 0 && direction < 0)
                    break;
                if (_inputReader.MoveDirection.x > 0 && direction > 0)
                    break;
                
                result = false;
                break;
            case PlayerStateType.Down:
                result = false;
                break;
            default:
                Debug.LogError("State not implemented: " + _playerState);
                break;
        }
        
        return result;
    }
    
    public void ProcessDamage(AttackType type)
    {
        Debug.Log("ZZZ: 4");
        
        Debug.Log("ZZZ: 5");
        //Alarm HP Bar
        
        //Determine
        float damage = _configuration.DetermineDamage(type) 
                       * (_playerState == PlayerStateType.Block ? 0.25f : 1f);
        
        HealthBar.Instance?.ChangeHealthRpc(_playerScript.PlayerNumberValue, damage);
        
        
        //Report Back to Statemachine that you got hurt!
    }
}