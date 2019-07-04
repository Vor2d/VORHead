﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAM_DataController : ParentDataController
{
    public WAMSetting Setting { get; set; }

    protected override void Awake()
    {
        IS = this;

        base.Awake();

        this.Setting = new WAMSetting();
    }

    private void Start()
    {
        Setting = load_setting<WAMSetting>();
        Debug.Log("Setting load completed: WAMSetting!");
    }

    public override void generate_setting()
    {
        generate_setting<WAMSetting>(Setting);
    }

    public static WAM_DataController IS { get; private set; }

}
