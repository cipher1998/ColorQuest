using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateDie : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("Started Character Dying Animation");

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("Finished Character Dying Animation");
        animator.enabled = false;

    }
}