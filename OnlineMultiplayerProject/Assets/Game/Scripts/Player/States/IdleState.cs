
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(StateData input) : base(input)
    {
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.Idle;
    }

    protected override void MoveEvent(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) < 0.01f)
            return;

        nextState = new DodgeState(stateData);
        stageType = StageType.Exit;
    }

    protected override void BlockEvent(bool isPressed)
    {
        if (!isPressed)
            return;
        
        nextState = new BlockState(stateData);
        stageType = StageType.Exit;
    }

    protected override void LeftAttack(bool isPressed)
    {
        if (!isPressed)
            return;

        nextState = new LeftAttackState(stateData);
        stageType = StageType.Exit;
    }

    protected override void RightAttack(bool isPressed)
    {
        if (!isPressed)
            return;

        nextState = new RightAttackState(stateData);
        stageType = StageType.Exit;
    }

    protected override void Update()
    {
        base.Update();
        
        if (Mathf.Abs(stateData.inputReader.MoveDirection.x) >= 0.01f)
        {
            nextState = new DodgeState(stateData);
            stageType = StageType.Exit;
            return;
        }
        if (stateData.inputReader.BlockButton)
        {
            nextState = new BlockState(stateData);
            stageType = StageType.Exit;
            return;
        }
        
    }

    protected override void IncomingAttack(AttackType type, bool isRightAttack)
    {
        OnDamage(type);
    }
}