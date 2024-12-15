using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjuredBehaviour : StateMachineBehaviour
{
    PlayerController playerController;
    CreepController creepController;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController??=animator.gameObject.GetComponent<PlayerController>();
        creepController ??= animator.gameObject.GetComponent<CreepController>();
        //  playerController.SwithCharacterState(2);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController ??= animator.gameObject.GetComponent<PlayerController>();
        creepController ??= animator.gameObject.GetComponent<CreepController>();
        if (playerController&&playerController.state!=3) playerController.SwithCharacterState(0);
        if (creepController && creepController.state != 3) creepController.SwithCharacterState(0);
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
