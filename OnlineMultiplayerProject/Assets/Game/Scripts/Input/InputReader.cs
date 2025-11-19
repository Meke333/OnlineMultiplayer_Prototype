using System;
using Game.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Game.Input.GamePlayerInput;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IGameplayActions
{
    private GamePlayerInput _gamePlayerInputActions;
    
    public Action<Vector2> MoveEvent;
    public Action<bool> LeftAttackEvent;
    public Action<bool> RightAttackEvent;
    public Action<bool> BlockEvent;
    

    private void OnEnable()
    {
        if (_gamePlayerInputActions == null)
        {
            _gamePlayerInputActions = new GamePlayerInput();
            _gamePlayerInputActions.Gameplay.SetCallbacks(this);
            
        }
        _gamePlayerInputActions.Enable();
        
    }

    private void OnDisable()
    {
        _gamePlayerInputActions.Gameplay.RemoveCallbacks(this);
        _gamePlayerInputActions.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLeftAttack(InputAction.CallbackContext context)
    {
        LeftAttackEvent?.Invoke(context.ReadValueAsButton());
    }

    public void OnRightAttack(InputAction.CallbackContext context)
    {
        RightAttackEvent?.Invoke(context.ReadValueAsButton());
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        BlockEvent?.Invoke(context.ReadValueAsButton());
    }
}