using UnityEngine;

public class HurtState: PlayerState
{
    
    public HurtState(StateData input) : base(input)
    {
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.Hurt;
        nextState = new IdleState(stateData);
        stageType = StageType.Exit;
    }

    protected override void Enter()
    {
        base.Enter();
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
        return;
    }
}