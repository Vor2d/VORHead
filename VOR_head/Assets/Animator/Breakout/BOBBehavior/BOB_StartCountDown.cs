using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOB_StartCountDown : StateMachineBehaviour
{
    private float timer;
    private float target_time;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BO_GameController.IS.ToStartCountDown();
        target_time = BO_Setting.IS.Ball_count_time;
        GeneralMethods.start_timer_down(ref timer,ref BO_GameController.IS.start_count_flag, target_time);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GeneralMethods.up_ch_timer_down(ref timer, ref BO_GameController.IS.start_count_flag))
        { animator.SetTrigger(BO_SD.AniL2NextStepTrigger_str); }
        else { BO_GameController.IS.StartCountDown(timer); }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BO_GameController.IS.LeaveStartCountDown();
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
