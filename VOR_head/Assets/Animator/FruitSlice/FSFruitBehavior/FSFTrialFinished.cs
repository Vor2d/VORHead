using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FSFTrialFinished : StateMachineBehaviour
{
    [SerializeField] private float Time_offset;

    private float timer;
    private bool timer_flag;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FS_Fruit.IS.ToTrialFinished();

        GeneralMethods.start_timer_up(ref timer, ref timer_flag);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(GeneralMethods.up_ch_timer_up(ref timer, ref timer_flag, 
            FS_Setting.IS.ResultTransTime + Time_offset))
        {
            animator.SetTrigger(FS_SD.AniNextStep_str);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FS_Fruit.IS.LeaveTrialFinished();
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
