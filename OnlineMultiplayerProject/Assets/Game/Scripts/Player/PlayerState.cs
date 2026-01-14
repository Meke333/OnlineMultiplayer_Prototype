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
        stateData.inputReader.MoveEvent += MoveEvent;
        stateData.inputReader.BlockEvent += BlockEvent;
        stateData.inputReader.LeftAttackEvent += LeftAttack;
        stateData.inputReader.RightAttackEvent += RightAttack;
        stageType = StageType.Update;
    }

    
    protected virtual void Update()
    {
        stageType = StageType.Update;
    }

    protected virtual void Exit()
    {
        stateData.inputReader.MoveEvent -= MoveEvent;
        stateData.inputReader.BlockEvent -= BlockEvent;
        stateData.inputReader.LeftAttackEvent -= LeftAttack;
        stateData.inputReader.RightAttackEvent -= RightAttack;
        stageType = StageType.Exit;
    }
    
    protected abstract void MoveEvent(Vector2 direction);
    protected abstract void BlockEvent(bool isPressed);
    protected abstract void LeftAttack(bool isPressed);
    protected abstract void RightAttack(bool isPressed);
    protected abstract void IncomingAttack(AttackType type, bool isRightAttack);


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

    protected virtual void OnDamage(AttackType type)
    {
        float damage = stateData.attackDamageConfiguration.DetermineDamage(type);

        stateData.playerScript.TakingDamage(damage);
        
        nextState = new HurtState(stateData);
        stageType = StageType.Exit;
        
        
    }

    //Create an Event for this to happen
    protected virtual void OnKnockdown()
    {
        throw new NotImplementedException();
    }

}