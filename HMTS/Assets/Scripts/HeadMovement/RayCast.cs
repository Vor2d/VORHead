using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : GeneralRayCast {

    public bool hit_flag { get; set; }
    public bool hit_center_flag { get; set; }
    public bool hit_centest_flag { get; set; }
    public bool hit_border_flag { get; set; }
    public bool hit_hideDetector_flag { get; set; }

    private Transform object_hit;

    protected override void Update()
    {
        base.Update();
        check_multi_hits();
    }

    private void check_multi_hits()
    {
        hit_flag = false;
        hit_center_flag = false;
        hit_centest_flag = false;
        hit_border_flag = false;
        hit_hideDetector_flag = false;

        //Debug.Log("hits.Length " + hits.Length);

        for (int i = 0; i < Hits.Length; i++)
        {
            RaycastHit hit = Hits[i];
            object_hit = hit.transform;

            if (object_hit != null)
            {
                if (object_hit.tag == "TargetSensor")
                {
                    hit_flag = true;
                }
                //if (objectHit.tag == "CenterSensor")
                //{
                //    hit_center_flag = true;
                //    return;
                //}
                //if (objectHit.tag == "CenterTest")
                //{
                //    hit_centest_flag = true;
                //    return;
                //}
                if(object_hit.tag == "TurnBorder")
                {
                    hit_border_flag = true;
                }
                if(object_hit.tag == "HideDetector")
                {
                    hit_hideDetector_flag = true;
                }
                if(object_hit.tag == "HeadIndicatorDetector")
                {
                    Hit_position = hit.point;
                }

            }
        }

    }


}
