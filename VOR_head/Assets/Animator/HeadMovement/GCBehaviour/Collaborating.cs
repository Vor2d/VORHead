using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collaborating : StateMachineBehaviour {

    private GameController GC_script;
    private JumpLogSystem JLS_script;


    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.GC_script = GameObject.Find("GameController").GetComponent<GameController>();
        this.JLS_script = GameObject.Find("LogSystem").GetComponent<JumpLogSystem>();

        GC_script.Current_state = "Collaborating";

        GC_script.ToCollaborate();
        GC_script.update_SS();
        JLS_script.log_action(GC_script.simulink_sample, GC_script.trial_iter, "Collaborate",
                                GC_script.turn_degree_x, GC_script.turn_direct_x);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GC_script.Collaborating();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GC_script.LeaveCollaborate();
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
