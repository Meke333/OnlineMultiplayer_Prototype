using UnityEngine;

public class AttackFired : StateMachineBehaviour
{
    private PlayerEvents _playerEvents;
    private PlayerScript _playerScript;

    //To-Do: Put in c# Action here
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerEvents == null || _playerScript == null)
            SetUp(animator);

        if (_playerEvents == null)
            return;

        if (_playerScript.IsOwner)
        {
            _playerEvents?.AttackFired.Invoke();
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
