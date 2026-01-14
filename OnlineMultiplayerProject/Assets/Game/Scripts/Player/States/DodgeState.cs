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
     
        Debug.Log("BlockButton: " + stateData.inputReader.BlockButton);
        if (stateData.inputReader.BlockButton)
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

    protected override void IncomingAttack(AttackType type, bool isRightAttack)
    {
        //opposite site to counter!
        if (stateData.inputReader.MoveDirection.x < 0 && !isRightAttack)
            return;
        
        if (stateData.inputReader.MoveDirection.x > 0 && isRightAttack)
            return;
        
        OnDamage(type);
    }
}