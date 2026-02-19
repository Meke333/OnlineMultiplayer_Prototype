using UnityEngine;

public class DodgeState : PlayerState
{
    public DodgeState(StateData input) : base(input)
    {
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.Dodge;
    }

    protected override void MoveEvent(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) >= 0.01f)
            return;
     
        //Debug.Log("BlockButton: " + stateData.inputStateData.blockInput);
        if (stateData.inputStateData.blockInput)
            nextState = new BlockState(stateData);
        else
            nextState = new IdleState(stateData);
        
        stageType = StageType.Exit;
    }

    protected override void BlockEvent(bool isPressed)
    {
        return;
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