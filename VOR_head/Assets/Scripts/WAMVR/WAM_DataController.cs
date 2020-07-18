using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAM_DataController : ParentDataController
{
    public WAMSetting Setting { get; set; }
    public System.Diagnostics.Stopwatch Sesstion_timer { get; set; }

    protected override void Awake()
    {
        IS = this;

        base.Awake();

        this.Setting = new WAMSetting();
        this.Sesstion_timer = new System.Diagnostics.Stopwatch();
    }

    private void Start()
    {
        Setting = load_setting<WAMSetting>();
        Debug.Log("Setting load completed: WAMSetting!");
        generate_frame_list();
    }

    private void generate_frame_list()
    {
        int cnt = 0;
        List<int> templis = new List<int>();
        bool first = true;
        foreach(int elem in Setting.Mole_frame_NUM_IND)
        {
            if (cnt == 0)
            {
                cnt = elem;
                if (!first) { Setting.Mole_frame_Lindex.Add(templis); }
                templis = new List<int>();
                first = false;
                continue;
            }
            templis.Add(elem);
            cnt--;
        }
        Setting.Mole_frame_Lindex.Add(templis);
    }

    public override void generate_setting()
    {
        generate_setting<WAMSetting>(Setting);
    }

    public static WAM_DataController IS { get; private set; }

}
