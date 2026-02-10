using UnityEngine;

public class LeftAttackState: PlayerState
{
    public LeftAttackState(StateData input) : base(input)
    {
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.Attack;
        
        stateData.attackType = DetermineAttackType();
        stateData.attackDirection = -1;
        Debug.Log("SETSTATETYPE NOW");
        
    }
    
    protected override void Enter()
    {
        base.Enter();
        stateData.didAttackEnd = false;

    }

    private AttackType DetermineAttackType()
    {
        if (Mathf.Abs(stateData.inputStateData.moveDirection.y) < 0.01f)
            return AttackType.Jab_Straight;
        
        return stateData.inputStateData.moveDirection.y < 0 
            ? AttackType.Gut 
            : AttackType.Hook;
    }

    protected override void Update()
    {
        base.Update();

        if (!stateData.didAttackEnd)
            return;
        
        stateData.didAttackEnd =  false;
        nextState = new IdleState(stateData);
        stageType = StageType.Exit;
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
}