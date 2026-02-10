using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerEvents _playerEvents;

    /*
    public NetworkVariable<AttackType> AttackTypeVariable = new NetworkVariable<AttackType>(0,
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server); 
    public NetworkVariable<bool> DidAttackMissVariable = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> MissCheckVariable = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MoveDirectionVariable = new NetworkVariable<float>(0, 
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> IsBlockingVariable = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> IsHurtVariable = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> IsKnockedDownVariable = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    */
    
    private async void DelayAnimatorBoolDeactivation(string name)
    {
        await Task.Delay(300);
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
        Debug.Log("HELLO YOU BITCH");
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
        Debug.Log("2" + state.ToString());
//Create a Network Variable
        Debug.Log("PlayerStateChanged: " + state);
        _animator.SetBool(PlayerAnimatorStrings.IsBlocking, state == PlayerStateType.Block);
        
        switch (state)
        {
            case PlayerStateType.Hurt:
                _animator.SetBool(PlayerAnimatorStrings.HurtBool, true);
                DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.HurtBool);
                break;
            case PlayerStateType.Down:
                _animator.SetBool(PlayerAnimatorStrings.KnockdownBool, true);
                DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.KnockdownBool);
                break;
        }
    }
    private void PlayerGetUpEvent()
    {
//Create a Network Variable & Convert Trigger as Bool!!
        _animator.SetBool(PlayerAnimatorStrings.GetUpBool, true);
        
        DelayAnimatorBoolDeactivation(PlayerAnimatorStrings.GetUpBool);
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
    public const string AttackBool = "AttackBool";
    public const string GetUpBool = "GetUpBool";
    public const string DidAttackMiss = "DidAttackMiss";
    public const string MissCheckBool = "MissCheckBool";
    public const string KnockdownBool = "KnockdownBool";
    public const string HurtBool = "HurtBool";

    #endregion
    

}
