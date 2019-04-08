using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_TargetRayCast : GeneralRayCast {



    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        check_multi_hits();
    }

    private void check_multi_hits()
    {
        foreach (RaycastHit hit in Hits)
        {
            Transform hit_TRAS = hit.transform;
            if (hit_TRAS.CompareTag(BP_StrDefiner.Bubble_indi_tag_str))
            {
                hit_TRAS.parent.GetComponent<Bubble>().Is_aimed_listener = true;
            }

        }
    }
}
