using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_HeadSimulator : GeneralHeadSimulatorRNP {

    // Use this for initialization
    protected override void Start()
    {
        this.DC_name = BP_StrDefiner.DataController_name;
        base.Start();
    }


}
