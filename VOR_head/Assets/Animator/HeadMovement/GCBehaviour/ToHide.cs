using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToHide : StateMachineBehaviour {

    private GameController GC_script;
    private JumpLogSystem JLS_script;
    private DataController DC_script;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.GC_script = GameObject.Find("GameController").GetComponent<GameController>();
        this.JLS_script = GameObject.Find("LogSystem").GetComponent<JumpLogSystem>();
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();
        GC_script.Current_state = "ToHide";

        //GC_script.Hide_timer = DC_script.HideTime +
        //                    Random.Range(-DC_script.HideTimeRandom, DC_script.HideTimeRandom);
        GC_script.Hide_time_flag = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GC_script.Hide();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GC_script.Hide_time_flag = false;

        GC_script.update_SS();
        JLS_script.log_action(GC_script.simulink_sample, GC_script.trial_iter, "TargetHided",
                                GC_script.turn_degree, GC_script.turn_direct);
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
