using System;
using UnityEngine;

public abstract class PlayerState
{
    protected PlayerState nextState;

    protected StageType stageType = StageType.Enter;

    public PlayerStateType stateType { get; protected set; }
    protected StateData stateData;
    
    public enum StageType
    {
        Enter  = 0,
        Update = 1,
        Exit   = 2
    }

    public PlayerState(StateData input)
    {
        stateData = input;
    }

    public abstract void SetStateType();

    protected virtual void Enter()
    {
        stageType = StageType.Update;
    }

    
    protected virtual void Update()
    {
        MoveEvent(stateData.inputStateData.moveDirection);
        BlockEvent(stateData.inputStateData.blockInput);
        LeftAttack(stateData.inputStateData.leftAttackInput);
        RightAttack(stateData.inputStateData.rightAttackInput);
        
        stageType = StageType.Update;
    }

    protected virtual void Exit()
    {
        stageType = StageType.Exit;
    }
    
    protected abstract void MoveEvent(Vector2 direction);
    protected abstract void BlockEvent(bool isPressed);
    protected abstract void LeftAttack(bool isPressed);
    protected abstract void RightAttack(bool isPressed);

    public PlayerState Process()
    {
        switch (stageType)
        {
            case StageType.Enter:
                Enter();
                break;
            case StageType.Update:
                Update();
                break;
            case StageType.Exit:
                Exit();
                return nextState;
        }

        return this;
    }

    public virtual void OnDamage()
    {
        nextState = new HurtState(stateData);
        stageType = StageType.Exit;
    }

    //Create an Event for this to happen
    public virtual void OnKnockdown()
    {
        //nextState = new KnockdownState(stateData)
        //stageType = StageType.Exit;
    }

}