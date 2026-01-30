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
    
    
    private PlayerState _currentState;
    [SerializeField, ReadOnly]private StateData _stateData;
    
    public NetworkVariable<PlayerStateType> playerStateType = new NetworkVariable<PlayerStateType>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    [SerializeField, ReadOnly] private bool _isActive; 

    public async void SetUp()
    {
        await Task.Delay(4000);
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

        if (!IsOwner)
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
        if (!IsOwner)
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
        
        if (playerStateType.Value == _currentState.stateType)
            return;
        
        playerStateType.Value = _currentState.stateType;
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
        Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; MoveInput + " + direction.ToString());
    }

    [ServerRpc]
    private void BlockInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.blockInput = input;
        Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; Block + " + input.ToString());
    }

    [ServerRpc]
    private void LeftAttackInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.leftAttackInput = input;
        Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; LeftAttack + " + input.ToString());
    }

    [ServerRpc]
    private void RightAttackInputChangeServerRpc(bool input)
    {
        _stateData.inputStateData.rightAttackInput = input;
        Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; RightAttack + " + input.ToString());
    }

    [ServerRpc]
    private void GetDamageServerRpc(float damage)
    {
        _currentState.OnDamage();
    }
    
    // In your EventsManagerpublic static event Action SomethingHappened;
    // In a script listening for this event
    //  void Start()
    //  {
    //      EventsManager.SomethingHappened += OnSomethingHappened;
    //  }
    //  void OnSomethingHappened()
    //  {
    //      Run local logic
    //      If multiplayer, send ServerRpc to notify the server
    //
    //      if (NetworkManager.Singleton.IsClient)
    //      {
    //      NotifyServerOfEventServerRpc();
    //      }
    //  }
    //
    // [ServerRpc]
    //  void NotifyServerOfEventServerRpc()
    //  {
            //Run server-side logic, possibly also call a ClientRpc
    //  }

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
    
    [ServerRpc]
    private void UpdateChangeToServerServerRpc()
    {
        if (_currentState == null)
            return;
        
        _playerEvents.StateChanged?.Invoke(playerStateType.Value);

        if (_currentState.stateType == PlayerStateType.Attack)
        {
            _playerEvents.TransmitAttack?.Invoke(_stateData.attackDirection, _stateData.attackType);
            _playerEvents.AttackFired?.Invoke();            
        }
    }
    
    //ClientRpc
    [ClientRpc]
    private void UpdateChangeToClientsClientRpc()
    {
        if (_currentState == null)
            return;
        
        Debug.Log("State Invoke Event: " + IsOwner + "; " + (_playerEvents.StateChanged == null));
        _playerEvents.StateChanged?.Invoke(playerStateType.Value);

        if (_currentState.stateType == PlayerStateType.Attack)
        {
            _playerEvents.TransmitAttack?.Invoke(_stateData.attackDirection, _stateData.attackType);
            _playerEvents.AttackFired?.Invoke();            
        }
    }


    
}
