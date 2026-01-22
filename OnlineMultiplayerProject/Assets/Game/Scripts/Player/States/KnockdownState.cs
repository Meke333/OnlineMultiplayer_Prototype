using UnityEngine;

public class KnockdownState : PlayerState
{
    //Knockdown Game Implementation here!
    
    public KnockdownState(StateData input) : base(input)
    {
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.Down;
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
        throw new System.NotImplementedException();
    }

    protected override void RightAttack(bool isPressed)
    {
        throw new System.NotImplementedException();
    }
}