using System;
using System.Threading.Tasks;
using UnityEngine;

public class AttackEnded : StateMachineBehaviour
{
    [SerializeField] private PlayerStateMachine _playerStateMachine;
    [SerializeField] private PlayerAttack _playerAttack;

    [SerializeField] private AttackFired[] _attackFired;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerStateMachine = animator.transform.root.GetComponentInChildren<PlayerStateMachine>();
        _playerAttack = animator.transform.root.GetComponentInChildren<PlayerAttack>();
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
    public override async void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (!_playerStateMachine)
        {
            //Debug.LogError("PlayerStateMachine is null");
        }

        for (int i = 0; i < _attackFired.Length; i++)
        {
            _attackFired[i].isActive = false;
        }

        float timer = 0.5f;

        while (timer > 0)
        {
            if (_playerStateMachine.IsOwner) 
            {
                //Debug.Log("AttackEnded 2");
                _playerStateMachine.AttackEndedServerRpc();
            }

            if (_playerAttack.IsOwner)
            {
                _playerAttack.AttackEndedServerRpc();
            }

            await Task.Yield();
            timer -= Time.deltaTime;
        }
        //Debug.Log("AttackEnded 1");
        

    }
}
