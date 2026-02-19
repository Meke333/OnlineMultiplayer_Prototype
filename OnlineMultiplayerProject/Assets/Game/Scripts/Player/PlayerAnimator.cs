using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerEvents _playerEvents;
    
    
    
    private async void DelayAnimatorBoolDeactivation(string name, int delay = 300)
    {
        await Task.Delay(delay);
        DeactivateBoolClientRpc(name);
    }

    private void ImmediateAnimatorBoolDeactivation(string name)
    {
        DeactivateBoolClientRpc(name);
    }

    [ClientRpc]
    private void DeactivateBoolClientRpc(string name)
    {
        _animator.SetBool(name, false);
    }

    #region Events

    //Async Wait Method calling ClientRpc
    
    [ServerRpc]
    public void TransmitAttackServerRpc(int direction, AttackType type)
    {
        TransmitAttackEventClientRpc(direction, type);
    }
    
    [ClientRpc]
    private void TransmitAttackEventClientRpc(int direction, AttackType type)
    {
        _animator.SetFloat(PlayerAnimatorStrings.AttackDirection, direction);
        
//Create a Network Variable
        int number = type switch
        {
            AttackType.Jab_Straight => 0,
            AttackType.Hook => 1,
            AttackType.Gut => 2
        };
        
        _animator.SetFloat(PlayerAnimatorStrings.AttackType, number);
        _animator.SetBool(PlayerAnimatorStrings.AttackBool, true);

        DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.AttackBool);
    }


    public void TransmitMissedAttack(bool value)
    {
        TransmitMissedAttackEventClientRpc(value);
    }
    
    [ClientRpc]
    private void TransmitMissedAttackEventClientRpc(bool value)
    {
//Create a Network Variable
        _animator.SetBool(PlayerAnimatorStrings.DidAttackMiss, value);

        //DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.DidAttackMiss);
    }

    public void CheckedForMissedAttack()
    {
        CheckedForMissedAttackEventClientRpc();
    }
    
    [ClientRpc]
    private void CheckedForMissedAttackEventClientRpc()
    {
//Create a Network Variable & Convert Trigger as Bool!!
        _animator.SetBool(PlayerAnimatorStrings.MissCheckBool, true);

        //DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.MissCheckBool);
    }

    [ServerRpc]
    public void AttackEndedServerRpc()
    {
        AttackEndedClientRpc();
    }

    [ClientRpc]
    private void AttackEndedClientRpc()
    {
        ImmediateAnimatorBoolDeactivation(PlayerAnimatorStrings.DidAttackMiss);
        ImmediateAnimatorBoolDeactivation(PlayerAnimatorStrings.MissCheckBool);
    }
    

    public void TransmitDodgeDirection(int direction)
    {
        TransmitDodgeDirectionEventClientRpc(direction);
    }
    
    [ClientRpc]
    private void TransmitDodgeDirectionEventClientRpc(int direction)
    {
        _animator.SetFloat(PlayerAnimatorStrings.MoveDirection, direction);
    }

    
    [ServerRpc]
    public void PlayerStateChangedEventServerRpc(PlayerStateType state)
    { 
        Debug.Log("1" + state.ToString());
//Create a Network Variable
        PlayerStateChangedEventClientRpc(state);
    }
    
    [ClientRpc]
    private void PlayerStateChangedEventClientRpc(PlayerStateType state)
    { 
        //Debug.Log("2" + state.ToString());
//Create a Network Variable
        //Debug.Log("PlayerStateChanged: " + state);
        _animator.SetBool(PlayerAnimatorStrings.IsBlocking, state == PlayerStateType.Block);
        
        switch (state)
        {
            case PlayerStateType.Idle:
                _animator.SetBool(PlayerAnimatorStrings.ContinueFight, true);

                DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.ContinueFight, 500);
                break;
            case PlayerStateType.StepBack:
                _animator.SetBool(PlayerAnimatorStrings.StepBackBool, true);

                DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.StepBackBool);
                break;
            case PlayerStateType.Hurt:
                _animator.SetBool(PlayerAnimatorStrings.HurtBool, true);
                DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.HurtBool, 100);
                break;
            case PlayerStateType.Down:
                _animator.SetBool(PlayerAnimatorStrings.KnockdownBool, true);
                DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.KnockdownBool);
                break;
        }
    }
    
    public void PlayerGetUpEvent()
    {
        //Debug.Log("GetUp: ServerRpc");
        PlayerGetUpEventClientRpc();
    }
    
    [ClientRpc]
    private void PlayerGetUpEventClientRpc()
    {
//Create a Network Variable & Convert Trigger as Bool!!
        _animator.SetBool(PlayerAnimatorStrings.GetUpBool, true);
        
        DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.GetUpBool);
    }

    public void EnemyTransmitAttack(AttackType type, int direction)
    {
        EnemyTransmitAttackClientRpc(type, direction);
    }

    [ClientRpc]
    private void EnemyTransmitAttackClientRpc(AttackType type, int direction)
    {
        AttackTransition(type, direction);
    }

    private async void AttackTransition(AttackType type, int direction)
    {
        float previousType = _animator.GetFloat(PlayerAnimatorStrings.EnemyAttackType);
        float previousDirection = _animator.GetFloat(PlayerAnimatorStrings.EnemyAttackDirection);
        
        if (type != AttackType.Hook)
        {
            direction = 0;
        }
        
        float goalDirection = (float) -direction;

        float goalType = type switch
        {
            AttackType.Jab_Straight => 0,
            AttackType.Hook => 1,
            AttackType.Gut => 2,
        };

        if (Mathf.Abs(previousType - goalType) < 0.1f
            && Mathf.Abs(previousDirection - goalDirection) < 0.1f)
            return;

        float maxTime = 0.5f;
        float timer = 0;
        
        while (timer < maxTime)
        {
            
            
            _animator.SetFloat(PlayerAnimatorStrings.EnemyAttackType, 
                Mathf.Lerp(previousType, goalType, timer / maxTime));

            _animator.SetFloat(PlayerAnimatorStrings.EnemyAttackDirection, 
                Mathf.Lerp(previousDirection, goalDirection, timer / maxTime));
            
            await Task.Yield();
            timer += Time.deltaTime;
        }
        
        _animator.SetFloat(PlayerAnimatorStrings.EnemyAttackType, goalType);
        _animator.SetFloat(PlayerAnimatorStrings.EnemyAttackDirection, goalDirection);
    }

    #endregion
    
    
}