using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAM_EndSession : StateMachineBehaviour
{
    private float timer;
    private bool timer_flag;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WAM_GameController.IS.ToSessionEnded();
        this.timer = 0.0f;
        this.timer_flag = false;
        GeneralMethods.start_timer_up(ref timer, ref timer_flag);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (update_timer()) { animator.SetTrigger(WAMSD.AniStartLevel_trigger); }
    }

    private bool update_timer()
    {
        if (timer_flag)
        {
            timer += Time.deltaTime;
            return GeneralMethods.check_timer_up(timer, ref timer_flag, WAMSetting.IS.Show_bonus_time);
        }
        return false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WAM_GameController.IS.LeaveSessionEnded();
        GeneralMethods.reset_animator_triggers(animator);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
