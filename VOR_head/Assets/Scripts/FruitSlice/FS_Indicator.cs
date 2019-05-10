using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_Indicator : MonoBehaviour
{
    public bool Activated { get; private set; }

    public bool Is_aimed_flag { get; set; }
    private bool Last_is_aimed_flag;

    private FS_FruitIndicator FSFI_script;
    private FS_RayCast RC_cache;

    private void Awake()
    {
        this.Activated = false;
        this.Is_aimed_flag = false;
        this.Last_is_aimed_flag = false;
        this.FSFI_script = null;
        this.RC_cache = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        RC_cache = FSFI_script.FSF_script.FSRC.RC_script;
    }

    // Update is called once per frame
    void Update()
    {
        if(Activated && FSFI_script.FSF_script.Start_flag)
        {
            check_aim();
        }
    }

    public void init_indicator(FS_FruitIndicator _FSFI_script, bool on_off)
    {
        FSFI_script = _FSFI_script;
        set_act_state(on_off);
    }

    public void set_act_state(bool on_off)
    {
        Activated = on_off;
        change_act_color();
    }

    public void change_act_color()
    {
        if(Activated)
        {
            set_color(FSFI_script.ActivateColor);
        }
        else
        {
            set_color(FSFI_script.DeActivatedColor);
        }
    }

    private void check_aim()
    {
        Is_aimed_flag = RC_cache.check_object(FS_SD.FruitStartI_Tag,transform);
        if (Is_aimed_flag != Last_is_aimed_flag)
        {
            aim_changed(Is_aimed_flag);
        }
        Last_is_aimed_flag = Is_aimed_flag;
    }

    private void aim_changed(bool aimmed)
    {
        if (aimmed)
        {
            set_color(FSFI_script.FocusColor);
        }
        else
        {
            set_color(FSFI_script.ActivateColor);
        }
    }

    private void set_color(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}
