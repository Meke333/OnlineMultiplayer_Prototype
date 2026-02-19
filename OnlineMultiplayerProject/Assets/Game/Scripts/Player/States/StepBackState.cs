using UnityEngine;

public class StepBackState : PlayerState
{
    public StepBackState(StateData input) : base(input)
    {
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.StepBack;
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

    public override void OnContinueFight()
    {
        nextState = new IdleState(stateData);
        stageType = StageType.Exit;
    }
}