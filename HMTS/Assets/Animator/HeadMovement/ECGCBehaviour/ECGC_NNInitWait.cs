using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECGC_NNInitWait : StateMachineBehaviour
{
    private EC_GameController ECGC_script;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ECGC_script == null)
        {
            this.ECGC_script = animator.GetComponent<EC_GameController>();
        }

        ECGC_script.ToNNInitWait();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ECGC_script.NNInitWait();
    }
}
