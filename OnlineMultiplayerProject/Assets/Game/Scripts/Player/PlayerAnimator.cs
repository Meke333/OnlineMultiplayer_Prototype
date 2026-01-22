using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerEvents _playerEvents;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _playerEvents.TransmitAttack += TransmitAttackEvent;
        _playerEvents.TransmitMissedAttack += TransmitMissedAttackEvent;
        _playerEvents.CheckedForMissedAttack += CheckedForMissedAttackEvent;
        _playerEvents.TransmitDodgeDirection += TransmitDodgeDirectionEvent;
        _playerEvents.PlayerStateChanged += PlayerStateChangedEvent;
        _playerEvents.PlayerGetUp += PlayerGetUpEvent;
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        _playerEvents.TransmitAttack -= TransmitAttackEvent;
        _playerEvents.TransmitMissedAttack -= TransmitMissedAttackEvent;
        _playerEvents.CheckedForMissedAttack -= CheckedForMissedAttackEvent;
        _playerEvents.TransmitDodgeDirection -= TransmitDodgeDirectionEvent;
        _playerEvents.PlayerStateChanged -= PlayerStateChangedEvent;
        _playerEvents.PlayerGetUp -= PlayerGetUpEvent;
    }

    #region Events

    private void TransmitAttackEvent(int direction, AttackType type)
    {
        _animator.SetTrigger(PlayerAnimatorStrings.AttackTrigger);
        _animator.SetFloat(PlayerAnimatorStrings.AttackDirection, direction);
        
        int number = type switch
        {
            AttackType.Jab_Straight => 0,
            AttackType.Hook => 1,
            AttackType.Gut => 2
        };
        
        _animator.SetFloat(PlayerAnimatorStrings.AttackType, number);
        
    }
    
    private void TransmitMissedAttackEvent(bool value)
    {
        _animator.SetBool(PlayerAnimatorStrings.DidAttackMiss, value);
    }

    private void CheckedForMissedAttackEvent()
    {
        _animator.SetTrigger(PlayerAnimatorStrings.MissCheckTrigger);
    }

    private void TransmitDodgeDirectionEvent(int direction)
    {
        _animator.SetFloat(PlayerAnimatorStrings.MoveDirection, direction);
    }

    private void PlayerStateChangedEvent(PlayerStateType state)
    {
        _animator.SetBool(PlayerAnimatorStrings.IsBlocking, state == PlayerStateType.Block);
        
        switch (state)
        {
            case PlayerStateType.Hurt:
                _animator.SetTrigger(PlayerAnimatorStrings.HurtTrigger);
                break;
            case PlayerStateType.Down:
                _animator.SetTrigger(PlayerAnimatorStrings.KnockdownTrigger);
                break;
        }
    }
    private void PlayerGetUpEvent()
    {
        _animator.SetTrigger(PlayerAnimatorStrings.GetUpTrigger);
    }

    #endregion
    




    
    
}

public static class PlayerAnimatorStrings
{
    #region Parameters

    public const string AttackDirection = "AttackDirection";
    public const string AttackType = "AttackType";
    public const string MoveDirection = "MoveDirection";
    public const string IsBlocking = "IsBlocking";
    public const string AttackTrigger = "AttackTrigger";
    public const string GetUpTrigger = "GetUpTrigger";
    public const string DidAttackMiss = "DidAttackMiss";
    public const string MissCheckTrigger = "MissCheckTrigger";
    public const string KnockdownTrigger = "KnockdownTrigger";
    public const string HurtTrigger = "HurtTrigger";

    #endregion
    

}
