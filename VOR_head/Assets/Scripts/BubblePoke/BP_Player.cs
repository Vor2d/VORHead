using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_Player : MonoBehaviour
{
    [SerializeField] private BP_RC BPRC;

    // Start is called before the first frame update
    void Start()
    {
        BPRC.CI_script.IndexTrigger += shoot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void shoot()
    {
        Bubble bubble_COMP = null;
        foreach (Transform bubble_TRANS in BPRC.Bubble_TRANSs.ToArray())
        {
            bubble_COMP = bubble_TRANS.GetComponent<Bubble>();
            if(bubble_COMP.Is_aimed_flag)
            {
                bubble_COMP.bubble_shooted();
            }
        }
    }
}
