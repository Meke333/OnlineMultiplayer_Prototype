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

    private PlayerStateType _lastClientStateType;
        

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
    }

    public override void OnNetworkDespawn()
    {
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

    protected void ToggleActivity(bool active)
    {
        _isActive = active;
    }
    
    //ServerRpc
    [ServerRpc]
    private void MoveInputChangeServerRpc(Vector2 direction)
    {
        _stateData.inputStateData.moveDirection = direction;
        //Debug.Log("S: "+ _playerScript.PlayerNumberValue + "; MoveInput + " + direction.ToString());
        
        _playerAnimator.TransmitDodgeDirection(direction.x == 0 
            ? 0 
            : direction.x < 0 
                ? -1 
                : 1 );
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

    [ServerRpc]
    public void GetDamageServerRpc()
    {
        Debug.Log("R: GetDamageServerRpc; " + IsOwner);
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
    public void AttackEndedServerRpc()
    {
        _stateData.didAttackEnd = true;

        if (!IsOwner)
            return;
        
        _playerAnimator.AttackEndedServerRpc();
    }
    
    private void UpdateChangeToServer()
    {
        //Debug.Log("Server State Invoke Event: " + IsServer + "; " + IsClient + "; " + IsLocalPlayer);

        //Debug.Log("IS THIS THE SERVER??? " + IsServer);

        attackDirection.Value = _stateData.attackDirection;
        attackType.Value = _stateData.attackType;


        //UpdateChangeToClients();
    }

    [ClientRpc]
    private void SendMessageClientRpc(int number)
    {
        Debug.Log("MESSAGE (" + _playerScript.PlayerNumber.Value + "): " + number);
    }
    
    //ClientRpc
    
    private void UpdateChangeToClients()
    {

        if (!IsOwner)
            return;

        if (_lastClientStateType == playerStateType.Value)
            return;
        
        _lastClientStateType = playerStateType.Value;
        //Debug.Log("PLEASE WORK");
        
        _playerAnimator.PlayerStateChangedEventServerRpc(playerStateType.Value);

        switch (playerStateType.Value) 
        {
            case PlayerStateType.Attack:

                _playerAttack.PrepareAttackServerRpc(attackDirection.Value, attackType.Value);
                Debug.Log("ATTACK: " + attackDirection.Value + "; " + attackType.Value);

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
