using System;
using TriInspector;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public Action<PlayerState> OnStateChange;

    [SerializeField, ReadOnly] private string statename;
    
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private AttackDamageConfiguration _attackDamageConfiguration;
    
    
    private PlayerState _currentState;

    [SerializeField, ReadOnly] private bool _isActive; 

    public void SetUp()
    {
        StateData data = new StateData();
        data.inputReader = _inputReader;
        data.attackDamageConfiguration = _attackDamageConfiguration;
        data.playerScript = _playerScript;
        
        _currentState = new IdleState(data);
        _isActive = true;
    }

    private void Awake()
    {
        SetUp();
    }

    private void Update()
    {
        if (!_isActive)
            return;
            
        _currentState = _currentState.Process();
        statename = _currentState.ToString();
    }

    protected void ToggleActivity(bool active)
    {
        _isActive = active;
    }

    public bool CheckForAttackHit(int direction, AttackType type)
    {
        return _currentState.DoesIncomingAttackHit(type, direction > 0);
    }
}
