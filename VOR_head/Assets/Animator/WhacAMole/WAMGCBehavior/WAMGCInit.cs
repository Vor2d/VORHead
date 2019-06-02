using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAMGCInit : StateMachineBehaviour
{
    private WAM_GameController GC_script;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(GC_script == null)
        {
            this.GC_script = animator.GetComponent<WAM_GameController>();
        }

        GC_script.GC_Init();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        
    //}
}
