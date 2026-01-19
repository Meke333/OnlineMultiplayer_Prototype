using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public Action AttackFired;
    public Action AttackEnded;
    

    public Action<int, AttackType> TransmitAttack; //direction, attackType
    public Action<bool> TransmitMissedAttack;
    public Action CheckedForMissedAttack;
    public Action<int> TransmitDodgeDirection; //direction
    public Action<PlayerStateType> PlayerStateChanged;
    public Action<float> TransmitDamage;
    public Action PlayerGetKnockdown;
    public Action PlayerGetUp;

}
