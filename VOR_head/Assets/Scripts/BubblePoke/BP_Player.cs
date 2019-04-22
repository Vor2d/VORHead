using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BP_EC;

public class BP_Player : MonoBehaviour
{
    [SerializeField] private BP_RC BPRC;

    // Start is called before the first frame update
    void Start()
    {
        if(BPRC.GC_script.UsingAcuity)
        {
            BPRC.CI_script.ForwardAction += shoot_up;
            BPRC.CI_script.RightAction += shoot_right;
            BPRC.CI_script.BackAction += shoot_down;
            BPRC.CI_script.LeftAction += shoot_left;
        }
        else
        {
            BPRC.CI_script.IndexTrigger += shoot;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void shoot()
    {
        Bubble bubble_script = null;
        foreach (Transform bubble_TRANS in BPRC.Bubble_TRANSs.ToArray())
        {
            bubble_script = bubble_TRANS.GetComponent<Bubble>();
            if(bubble_script.Is_aimed_flag)
            {
                bubble_script.bubble_shooted();
            }
        }
    }

    private void shoot_up()
    {
        shoot(AcuityDir.Up);
    }

    private void shoot(AcuityDir acuityDir)
    {
        Bubble bubble_script = null;
        foreach (Transform bubble_TRANS in BPRC.Bubble_TRANSs.ToArray())
        {
            bubble_script = bubble_TRANS.GetComponent<Bubble>();
            if (bubble_script.Is_aimed_flag)
            {
                bubble_script.bubble_shooted(acuityDir);
            }
        }
    }

    private void shoot_right()
    {
        shoot(AcuityDir.Right);
    }

    private void shoot_down()
    {
        shoot(AcuityDir.Down);
    }

    private void shoot_left()
    {
        shoot(AcuityDir.Left);
    }
}
