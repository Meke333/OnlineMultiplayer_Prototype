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
        
    }

    protected override void BlockEvent(bool isPressed)
    {
        
    }

    protected override void LeftAttack(bool isPressed)
    {
        
    }

    protected override void RightAttack(bool isPressed)
    {
        
    }

    public override void OnContinueFight()
    {
        nextState = new IdleState(stateData);
        stageType = StageType.Exit;
    }
}