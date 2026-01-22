using System;
using Unity.Netcode;
using UnityEngine;

public class InputHandler : NetworkBehaviour
{
    [SerializeField] private InputReader _inputReader;
    
    public NetworkVariable<Vector2> _moveDirectionInput = new NetworkVariable<Vector2>(Vector2.zero, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> _leftAttackInput = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> _rightAttackInput = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> _blockInput = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        ToggleEvents(true);
    }

    public override void OnNetworkDespawn()
    {
        ToggleEvents(false);
    }

    void ToggleEvents(bool shouldSubscribe)
    {
        if (shouldSubscribe)
        {
            _inputReader.MoveEvent += OnMove;
            _inputReader.LeftAttackEvent += OnLeftAttack;
            _inputReader.RightAttackEvent += OnRightAttack;
            _inputReader.BlockEvent += OnBlock;

        }
        else
        {
            _inputReader.MoveEvent -= OnMove;
            _inputReader.LeftAttackEvent -= OnLeftAttack;
            _inputReader.RightAttackEvent -= OnRightAttack;
            _inputReader.BlockEvent -= OnBlock;
        }
    }

    private void OnMove(Vector2 obj)
    {
        SettleMoveInputServerRpc(obj);
        Debug.Log("Player: " + OwnerClientId + "; MoveInput: " + _moveDirectionInput.Value.ToString());
    }

    private void OnLeftAttack(bool obj)
    {
        SettleLeftAttackInputServerRpc(obj);
        Debug.Log("Player: " + OwnerClientId + "; LEFT Attack?: " + _leftAttackInput.Value.ToString());
    }

    private void OnRightAttack(bool obj)
    {
        SettleRightAttackInputServerRpc(obj);
        Debug.Log("Player: " + OwnerClientId + "; RIGHT Attack?: " + _rightAttackInput.Value.ToString());
    }

    private void OnBlock(bool obj)
    {
        SettleBlockInputServerRpc(obj);
        Debug.Log("Player: " + OwnerClientId + "; BLOCK?: " + _blockInput.Value.ToString());
    }

    [ServerRpc]
    void SettleMoveInputServerRpc(Vector2 input)
    {
        _moveDirectionInput.Value = input;
        
        DebugClientRpc((int) OwnerClientId, 1, input.ToString());
    }

    [ServerRpc]
    void SettleLeftAttackInputServerRpc(bool input)
    {
        _leftAttackInput.Value = input;
        
        DebugClientRpc((int) OwnerClientId, 2, input.ToString());
    }
    
    [ServerRpc]
    void SettleRightAttackInputServerRpc(bool input)
    {
        _rightAttackInput.Value = input;
        
        DebugClientRpc((int) OwnerClientId, 3, input.ToString());
    }
    
    [ServerRpc]
    void SettleBlockInputServerRpc(bool input)
    {
        _blockInput.Value = input;

        DebugClientRpc((int) OwnerClientId, 4, input.ToString());
    }

    [ClientRpc]
    void DebugClientRpc(int nr, int state, string msg)
    {
        string a = state switch
        {
            0 => "Move",
            1 => "LeftAttack",
            2 => "RightAttack",
            3 => "Block",
            _ => "Unknown"
        };
        
        
        Debug.Log("Server-Player: " + nr + "; " + a + ": " + msg);
    }
    
}