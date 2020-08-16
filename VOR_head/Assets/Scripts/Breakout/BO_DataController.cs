using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_DataController : ParentDataController 
{

    private BO_Setting Setting;

    public static BO_DataController IS;

    protected override void Awake()
    {
        IS = this;

        base.Awake();
        Setting = new BO_Setting();
    }

    // Use this for initialization
    void Start()
    {
        load_setting();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void generate_setting()
    {
        base.generate_setting<BO_Setting>(Setting);
    }

    public void load_setting()
    {
        Setting = base.load_setting<BO_Setting>();
    }
}
