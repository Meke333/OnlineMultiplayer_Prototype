using System;
using UnityEngine;

public class AttackEnded : StateMachineBehaviour
{
    private PlayerEvents _playerEvents;
    private PlayerScript _playerScript;


    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
        if (_playerEvents == null || _playerScript == null)
            SetUp(animator);

        if (_playerEvents == null)
            return;

        if (_playerScript.IsOwner)
        {
            _playerEvents?.AttackEnded.Invoke();
        }

    }

    private void SetUp(Animator animator)
    {
        if (_playerEvents == null)
        {
            _playerEvents = animator.GetComponentInParent<PlayerEvents>();
        }

        if (_playerScript == null)
        {
            _playerScript = animator.GetComponentInParent<PlayerScript>();
        }
    }
}
