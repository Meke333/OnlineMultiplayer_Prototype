using Unity.Netcode;
using UnityEngine;

public class PlayerDamage : NetworkBehaviour
{
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private PlayerEvents _playerEvents;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private AttackDamageConfiguration _configuration;
    
    private PlayerStateType _playerState;
    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _playerEvents.StateChanged += PlayerStateChanged;

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        
        _playerEvents.StateChanged -= PlayerStateChanged;
    }

    private void PlayerStateChanged(PlayerStateType obj)
    {
        _playerState = obj;
    }

    public bool DoesAttackLand(int direction, AttackType type)
    {
        bool result = true;
        
        switch (_playerState)
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
        //Alarm HP Bar
        
        //Determine
        float damage = _configuration.DetermineDamage(type) 
                       * (_playerState == PlayerStateType.Block ? 0.25f : 1f);
        
        HealthBar.Instance.ChangeHealthServerRpc(_playerScript.PlayerNumberValue, damage);
        
        _playerEvents.GettingDamage?.Invoke(damage);
    }
}