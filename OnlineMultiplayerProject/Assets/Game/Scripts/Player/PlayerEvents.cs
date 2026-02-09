using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerEvents : NetworkBehaviour
{
    [SerializeField] private Delegate[] _invoctionList;
    [SerializeField] private int count;
    
    public Action AttackFired;
    public Action AttackEnded;
    

    public Action<PlayerStateType> StateChanged;
    public Action<float> GettingDamage;
    public Action KnockedDown;
    
    public Action<int, AttackType> TransmitAttack; //direction, attackType
    public Action<bool> TransmitMissedAttack;
    public Action CheckedForMissedAttack;
    public Action<int> TransmitDodgeDirection; //direction
    public Action<PlayerStateType> PlayerStateChanged;
    public Action<float> TransmitDamage;
    public Action PlayerGetKnockdown;
    public Action PlayerGetUp;

    private void FixedUpdate()
    {
        if (!IsServer)
            return;
        _invoctionList = PlayerStateChanged.GetInvocationList();
        count = _invoctionList.Length;
    }
}
