using System.Threading.Tasks;
using UnityEngine;

public class AttackFired : StateMachineBehaviour
{
    //[SerializeField] private PlayerEvents _playerEvents;
    //[SerializeField] private PlayerScript _playerScript;
    [SerializeField] private PlayerAttack _playerAttack;

    public bool isActive;
    
    //To-Do: Put in c# Action here
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (_playerEvents == null || _playerScript == null)
        if (!_playerAttack)    
            SetUp(animator);

        if (_playerAttack)
            _playerAttack.currentAttackFired = this;
        
        Debug.Log("AttackFired has entered the State");

        isActive = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    // 
    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override async void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!_playerAttack)
        {
            Debug.LogError("PlayerAttack is null in " + nameof(AttackFired));
            return;
        }

        float timer = 0.5f;
        
        
        Debug.Log("ATTACK TRIGGER STARTED");

        while (timer > 0 && isActive)
        {

            Debug.Log("TRIGGER YOOOOOOOOO; " + timer + "; " + isActive + ";;; " + _playerAttack.IsOwner);
            if (_playerAttack.IsOwner)
            {
                _playerAttack.PerformAttackServerRpc();
            }
            
            await Task.Yield();
            timer -= Time.deltaTime;
        }

        
        Debug.Log("ENDED TRIGGER");

    }
    
    private void SetUp(Animator animator)
    {
        if (!_playerAttack)
        {
            _playerAttack = animator.transform.root.GetComponentInChildren<PlayerAttack>();
        }
        
    }
}
