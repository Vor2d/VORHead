using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForTurn : StateMachineBehaviour {

    private GameController GC_script;
    //private HeadStateController HSC_script;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.GC_script = GameObject.Find("GameController").GetComponent<GameController>();
        //this.HSC_script = 
        //                GameObject.Find("GameController").GetComponent<HeadStateController>();
        GC_script.Current_state = "WaitForTurn";

        GC_script.ToWaitForTurn();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GC_script.WaitForTurn();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //HSC_script.Check_speed_flag = false;
        GC_script.Check_speed_flag = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
