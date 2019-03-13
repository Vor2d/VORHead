using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeRayCast : GeneralRayCast
{
    [SerializeField] private Transform EyeIndicator_TRANS;

    private DataController DC_script;

    protected override void Start()
    {
        base.Start();
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();
    }

    protected override void Update()
    {
        base.Update();
        multi_check();
        EyeIndicator_TRANS.position = Hit_position;
    }

    private void multi_check()
    {
        foreach(RaycastHit hit in Hits)
        {
            if(hit.transform.CompareTag("HeadIndicatorDetector"))
            {
                Hit_position = hit.point;
            }
        }
    }
}
