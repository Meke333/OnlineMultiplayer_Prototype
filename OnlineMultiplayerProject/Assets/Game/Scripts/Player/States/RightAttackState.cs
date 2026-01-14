using UnityEngine;

public class RightAttackState : PlayerState
{
    public RightAttackState(StateData input) : base(input)
    {
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.Attack;
    }

    protected override void MoveEvent(Vector2 direction)
    {
        return;
    }

    protected override void BlockEvent(bool isPressed)
    {
        return;
    }

    protected override void LeftAttack(bool isPressed)
    {
        return;
    }

    protected override void RightAttack(bool isPressed)
    {
        return;
    }

    protected override void IncomingAttack(AttackType type, bool isRightAttack)
    {
        OnDamage(type);
    }
}