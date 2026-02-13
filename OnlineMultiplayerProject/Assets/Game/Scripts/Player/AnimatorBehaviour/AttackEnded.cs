using System;
using UnityEngine;

public class AttackEnded : StateMachineBehaviour
{
    [SerializeField] private PlayerStateMachine _playerStateMachine;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerStateMachine = animator.transform.root.GetComponentInChildren<PlayerStateMachine>();
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (!_playerStateMachine)
        {
            Debug.LogError("PlayerStateMachine is null");
        }
        
        Debug.Log("AttackEnded 1");
        if (_playerStateMachine.IsOwner) 
        {
            Debug.Log("AttackEnded 2");
            _playerStateMachine.AttackEndedServerRpc();
        }

    }

    private void SetUp(Animator animator)
    {
        if (!_playerStateMachine)
            _playerStateMachine = animator.transform.root.GetComponentInChildren<PlayerStateMachine>();
    }
}
