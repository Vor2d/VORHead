﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOB_BallMissed : StateMachineBehaviour
{
    [SerializeField] private float Wait_time;
    private float timer;
    private bool timer_flag; 

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BO_GameController.IS.ToBallMissed();
        GeneralMethods.start_timer_up(ref timer, ref timer_flag);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(GeneralMethods.up_ch_timer_up(ref timer, ref timer_flag, Wait_time))
        { animator.SetTrigger(BO_SD.AniL2NextStepTrigger_str); }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
