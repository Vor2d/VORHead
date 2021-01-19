using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartJumpLog : StateMachineBehaviour {

    private JumpLogSystem JLS_script;
    private VRLogSystem VRLS_script;
    private LogSystem LS_script;
    private AcuityLogSystem ALS_script;
    private HeadLogSystem HLS_script;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject logsystem_OBJ = GameObject.Find("LogSystem");
        if (JLS_script == null)
        {
            JLS_script = logsystem_OBJ.GetComponent<JumpLogSystem>();
            VRLS_script = logsystem_OBJ.GetComponent<VRLogSystem>();
            LS_script = logsystem_OBJ.GetComponent<LogSystem>();
            ALS_script = logsystem_OBJ.GetComponent<AcuityLogSystem>();
            HLS_script = logsystem_OBJ.GetComponent<HeadLogSystem>();
        }

        LS_script.start_stopwatch();
        if(!JLS_script.log_state_flag)
        {
            JLS_script.toggle_Log();
        }

        if(!VRLS_script.thread_state_flag)
        {
            VRLS_script.toggle_Thread();
        }

        if(!ALS_script.thread_state_flag)
        {
            ALS_script.toggle_Thread();
        }

        if (!HLS_script.thread_state_flag)
        {
            HLS_script.toggle_Thread();
        }

        animator.SetTrigger("NextStep");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
