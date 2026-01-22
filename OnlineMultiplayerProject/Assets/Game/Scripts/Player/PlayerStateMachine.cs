using System;
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
    
    
    private PlayerState _currentState;
    private StateData _stateData;
    
    public NetworkVariable<PlayerStateType> playerStateType = new NetworkVariable<PlayerStateType>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    [SerializeField, ReadOnly] private bool _isActive; 

    public void SetUp()
    {
        _stateData = new StateData();
        _stateData.inputStateData = new InputStateData();
        _stateData.attackDamageConfiguration = _attackDamageConfiguration;
        _stateData.playerScript = _playerScript;
        
        _currentState = new IdleState(_stateData);
        _isActive = true;
    }

    public override void OnNetworkSpawn()
    {
        SetUp();

        if (!IsOwner || !IsClient)
            return;

        _inputReader.MoveEvent += MoveInputChangeServerRpc;
        _inputReader.BlockEvent += BlockInputChangeServerRpc;
        _inputReader.LeftAttackEvent += LeftAttackInputChangeServerRpc;
        _inputReader.RightAttackEvent += RightAttackInputChangeServerRpc;

        _playerEvents.GettingDamage += GetDamageServerRpc;
        _playerEvents.KnockedDown += GetKnockedDownServerRpc;
        _playerEvents.AttackEnded += AttackEndedServerRpc;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner || !IsClient)
            return;

        _inputReader.MoveEvent -= MoveInputChangeServerRpc;
        _inputReader.BlockEvent -= BlockInputChangeServerRpc;
        _inputReader.LeftAttackEvent -= LeftAttackInputChangeServerRpc;
        _inputReader.RightAttackEvent -= RightAttackInputChangeServerRpc;
        
        _playerEvents.GettingDamage -= GetDamageServerRpc;
        _playerEvents.KnockedDown -= GetKnockedDownServerRpc;
    }

    private void Update()
    {
        if (!_isActive)
            return;

        if (!IsServer)
            return;
        
        _currentState = _currentState.Process();
        statename = _currentState.ToString();

        UpdateChangeToClientsClientRpc();
    }

    protected void ToggleActivity(bool active)
    {
        _isActive = active;
    }
    
    //ServerRpc
    [ServerRpc]
    private void MoveInputChangeServerRpc(Vector2 direction)
    {
        _stateData.inputStateData.moveDirection = direction;
    }

    [ServerRpc]
    private void BlockInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.blockInput = input;
    }

    [ServerRpc]
    private void LeftAttackInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.leftAttackInput = input;
    }

    [ServerRpc]
    private void RightAttackInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.rightAttackInput = input;
    }

    [ServerRpc]
    private void GetDamageServerRpc(float damage)
    {
        _currentState.OnDamage();
    }

    [ServerRpc]
    private void GetKnockedDownServerRpc()
    {
        _currentState.OnKnockdown();
    }

    [ServerRpc]
    private void AttackEndedServerRpc()
    {
        _stateData.didAttackEnd = true;
    }
    
    //ClientRpc
    [ClientRpc]
    private void UpdateChangeToClientsClientRpc()
    {
        playerStateType.Value = _currentState.stateType;
        _playerEvents.StateChanged.Invoke(playerStateType.Value);

        if (_currentState.stateType == PlayerStateType.Attack)
        {
            _playerEvents.TransmitAttack?.Invoke(_stateData.attackDirection, _stateData.attackType);
            _playerEvents.AttackFired?.Invoke();            
        }
    }
    
}
