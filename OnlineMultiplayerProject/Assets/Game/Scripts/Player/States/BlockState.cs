using UnityEngine;

public class BlockState : PlayerState
{
    public BlockState(StateData input) : base(input)
    {
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.Block;
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
        if (isPressed)
            return;
        
        nextState = new IdleState(stateData);
        stageType = StageType.Exit;
    }

    protected override void LeftAttack(bool isPressed)
    {
        if (!isPressed)
            return;

        stateData.inputStateData.leftAttackInput = false;

        nextState = new LeftAttackState(stateData);
        stageType = StageType.Exit;
    }

    protected override void RightAttack(bool isPressed)
    {
        if (!isPressed)
            return;
        
        stateData.inputStateData.rightAttackInput = false;

        nextState = new RightAttackState(stateData);
        stageType = StageType.Exit;
    }


}