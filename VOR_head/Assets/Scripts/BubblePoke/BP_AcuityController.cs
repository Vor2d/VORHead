using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BP_EC;

public class BP_AcuityController : MonoBehaviour
{
    [SerializeField] private BP_RC BPRC;

    private Transform last_TRANS;
    private bool ready_flag;

    // Start is called before the first frame update
    void Start()
    {
        GeneralMethods.check_ref<BP_RC>(ref BPRC, BP_StrDefiner.RC_name);

        this.ready_flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        check_ready();
    }

    private void check_ready()
    {
        if(!ready_flag)
        {
            if(BPRC.Bubble_TRANSs.Count > 0)
            {
                ready_flag = true;
                rotate_acuity();
            }
        }
        if(ready_flag && BPRC.Bubble_TRANSs.Count == 0)
        {
            ready_flag = false;
        }
    }

    public void rotate_acuity()
    {
        //Debug.Log("rotate_acuity!!!!!");
        if (BPRC.Bubble_TRANSs.Count > 0)
        {
            deactivate_acuitys();
            int index = 0;
            if (last_TRANS == null)
            {
                index = -1;
            }
            else
            {
                index = BPRC.Bubble_TRANSs.FindIndex(x => (x == last_TRANS));
            }
            index++;
            index %= BPRC.Bubble_TRANSs.Count;
            activate_acuity(BPRC.Bubble_TRANSs[index]);
        }
    }

    private void deactivate_acuitys()
    {
        last_TRANS = null;
        foreach(Transform B_TRANS in BPRC.Bubble_TRANSs)
        {
            if (B_TRANS.GetComponent<Bubble>().activated_flag)
            {
                last_TRANS = B_TRANS;
            }
            B_TRANS.GetComponent<Bubble>().deactivate_acuity();
        }
    }

    private void activate_acuity(Transform B_TRANS)
    {
        B_TRANS.GetComponent<Bubble>().activate_acuity();
    }
}
