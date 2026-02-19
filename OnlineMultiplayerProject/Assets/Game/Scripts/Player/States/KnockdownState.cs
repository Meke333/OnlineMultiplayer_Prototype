using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class KnockdownState : PlayerState
{
    //Knockdown Game Implementation here!
    //For now just a timer
    private float _timer;
    private float _maxKnockdownTime;
    private readonly float _backToIdleTime = 3f;

    private DownType _downType = DownType.Knockdown;
    
    enum DownType
    {
        Knockdown,
        BackToIdle,
    }
    
    public KnockdownState(StateData input) : base(input)
    {
        _maxKnockdownTime = Random.Range(6f, 9.9f);
        _timer = _maxKnockdownTime;
    }

    public override void SetStateType()
    {
        stateType = PlayerStateType.Down;
    }

    protected override void Update()
    {
        base.Update();
        
        _timer -= Time.deltaTime;
        
        if (_timer > 0f)
            return;

        switch (_downType)
        {
            case DownType.Knockdown:
                _downType = DownType.BackToIdle;
                _timer = _backToIdleTime;
                stateData.recoveredFromKnockdown = true;
                return;
            case DownType.BackToIdle:
                break;
        }
        
        nextState = new IdleState(stateData);
        stageType = StageType.Exit;
    }

    protected override void Exit()
    {
        InteractionBetweenPlayersMediator.Instance.FightContinueInteraction();
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