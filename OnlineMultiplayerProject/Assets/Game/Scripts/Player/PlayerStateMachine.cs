using System;
using System.Threading.Tasks;
using TriInspector;
using Unity.Netcode;
using UnityEngine;

public class PlayerStateMachine : NetworkBehaviour
{
    public Action<PlayerState> OnStateChange;

    [SerializeField, ReadOnly] private string statename;
    
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private PlayerEvents _playerEvents;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private AttackDamageConfiguration _attackDamageConfiguration;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private PlayerAttack _playerAttack;
    
    private PlayerState _currentState;
    [SerializeField, ReadOnly]private StateData _stateData;
    
    public NetworkVariable<PlayerStateType> playerStateType = new NetworkVariable<PlayerStateType>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> attackDirection = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<AttackType> attackType = new NetworkVariable<AttackType>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);
    
    public NetworkVariable<int> dodgeDirection = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private PlayerStateType _lastClientStateType;
        

    [SerializeField, ReadOnly] private bool _isActive; 

    private async void SetUp()
    {
        if (IsServer)
        {
            InteractionBetweenPlayersMediator.SetUpFight();
        }
        
        await Task.Delay(4000);
        _stateData = new StateData
        {
            inputStateData = new InputStateData(),
            attackDamageConfiguration = _attackDamageConfiguration,
            playerScript = _playerScript
        };

        _currentState = new StepBackState(_stateData);
        
        _isActive = true;
        
        Update();
        
        if (IsServer)
        {
            InteractionBetweenPlayersMediator.IncrementPlayerCount();
        }
    }

    public override void OnNetworkSpawn()
    {
        SetUp();

        if (!IsOwner)
            return;

        _inputReader.MoveEvent += MoveInputChangeServerRpc;
        _inputReader.BlockEvent += BlockInputChangeServerRpc;
        _inputReader.LeftAttackEvent += LeftAttackInputChangeServerRpc;
        _inputReader.RightAttackEvent += RightAttackInputChangeServerRpc;
    }
    
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            InteractionBetweenPlayersMediator.DecrementPlayerCount();
            
            EnvironmentHandler.Instance?.SetDirection(_playerScript.PlayerNumberValue, 0);
            EnvironmentHandler.Instance?.ResetEnvironmentTransform();
            HealthBar.Instance?.ResetScore();

            FightHandler.Instance?.CancelCount();
        }
        
        if (!IsOwner)
            return;

        _inputReader.MoveEvent -= MoveInputChangeServerRpc;
        _inputReader.BlockEvent -= BlockInputChangeServerRpc;
        _inputReader.LeftAttackEvent -= LeftAttackInputChangeServerRpc;
        _inputReader.RightAttackEvent -= RightAttackInputChangeServerRpc;
        
    }

    private void Update()
    {
        if (!_isActive)
            return;

        //Debug.Log("" + _playerScript.PlayerNumber.Value + " M: Active");

        if (IsServer)
        {
            
            _currentState = _currentState.Process();
            statename = _currentState.ToString();

            if (_stateData.recoveredFromKnockdown)
            {
                FightHandler.Instance?.CancelCount();
                GetUp();
            }
            
            if (playerStateType.Value == _currentState.stateType)
                return;
        
            playerStateType.Value = _currentState.stateType;
        
            UpdateChangeToServer();
        }

        if (IsClient && IsOwner)
        {
            UpdateChangeToClients();
        }
        
        //Debug.Log("" + _playerScript.PlayerNumber.Value + " M: IsServer");
        
        //UpdateChangeToClientsClientRpc();
    }

    #region Input
    
    //ServerRpc
    [ServerRpc]
    private void MoveInputChangeServerRpc(Vector2 direction)
    {
        _stateData.inputStateData.moveDirection = direction;
        //Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; MoveInput + " + direction.ToString());
        
        int temp = direction.x == 0 ? 0 : direction.x < 0 ? -1 : 1;
        dodgeDirection.Value = temp;
        EnvironmentHandler.Instance?.SetDirection(_playerScript.PlayerNumberValue, temp);
        _playerAnimator.TransmitDodgeDirection(temp);
    }

    [ServerRpc]
    private void BlockInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.blockInput = input;
        //Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; Block + " + input.ToString());
    }

    [ServerRpc]
    private void LeftAttackInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.leftAttackInput = input;
        //Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; LeftAttack + " + input.ToString());
    }

    [ServerRpc]
    private void RightAttackInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.rightAttackInput = input;
        //Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; RightAttack + " + input.ToString());
    }
    
    #endregion

    public void TransmitEnemyAttack(AttackType type, int direction)
    {
        _playerAnimator.EnemyTransmitAttack(type, direction);
    }
    
    public void GetDamage()
    {
        if (!IsServer)
            return;
        
        _currentState.OnDamage();
    }

    public void GetKnockedDown()
    {
        if (!IsServer)
            return;
        
        _currentState.OnKnockdown();
        HealthBar.Instance?.IncreaseScore(
            _playerScript.PlayerNumberValue == PlayerNumberType.PlayerOne
                ? PlayerNumberType.PlayerTwo 
                : PlayerNumberType.PlayerOne
            , 1);
    }

    public void GetStepAwayState()
    {
        if (!IsServer)
            return;
        
        _currentState.OnStepBack();
    }

    public void GetContinueState()
    {
        if (!IsServer)
            return;
        
        _currentState.OnContinueFight();
    }

    public void GetUp()
    {
        _stateData.recoveredFromKnockdown = false;
        _playerAnimator.PlayerGetUpEvent();
        
        HealthBar.Instance?.ChangeHealth(_playerScript.PlayerNumberValue, HealthBar.MaxHealth, false);
        PlayerAudio.Instance.GetUpSound(_playerScript.PlayerNumber.Value);
    }

    [ServerRpc]
    public void AttackEndedServerRpc()
    {
        _stateData.didAttackEnd = true;
        
        //if (!IsOwner)
            //return;
        
        _playerAnimator.AttackEnded();
    }
    
    private void UpdateChangeToServer()
    {
        attackDirection.Value = _stateData.attackDirection;
        attackType.Value = _stateData.attackType;
        
        EnvironmentHandler.Instance?.SetState(_playerScript.PlayerNumberValue, playerStateType.Value);
    }
    
    private void UpdateChangeToClients()
    {

        if (!IsOwner)
            return;

        if (_lastClientStateType == playerStateType.Value)
            return;
        
        _lastClientStateType = playerStateType.Value;
        
        PostProcessingHandler.Instance?.EvaluatePlayerStateClientRpc(playerStateType.Value);
        PlayerAudio.Instance.EvaluatePlayerState(playerStateType.Value, _playerScript.PlayerNumber.Value);
        
        _playerAnimator.PlayerStateChangedEventServerRpc(playerStateType.Value);
        _playerAttack.PlayerStateChangedRpc(playerStateType.Value);
        
        switch (playerStateType.Value) 
        {
            case PlayerStateType.Attack:

                _playerAttack.PrepareAttackServerRpc(attackDirection.Value, attackType.Value);
                //Debug.Log("ATTACK: " + attackDirection.Value + "; " + attackType.Value);

                _playerAnimator.TransmitAttackServerRpc(attackDirection.Value, attackType.Value);
                break;
        }
    }

    [ServerRpc]
    public void AttackCheckedServerRpc()
    {
        _playerAnimator.CheckedForMissedAttack();
    }
    
    [ServerRpc]
    public void AttackMissedServerRpc()
    {
        _playerAnimator.TransmitMissedAttack(true);
    }
    
    

    
}
