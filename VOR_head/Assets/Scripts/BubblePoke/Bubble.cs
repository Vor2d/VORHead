using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

    public GameObject BubbleRender;
    public GameObject BubbleAimIndicator;

    public float IncreaseSpeed = 1.0f;
    //public float LastTime = 3.0f;
    public float LastTransparent = 0.3f;
    public float InitTransparent = 0.6f;

    public bool Is_aimed_listener { get; set; }
    public bool Is_aimed_flag { get; private set; }  //Inner is aimed;
    //private float last_timer;
    private bool start_flag;
    private float BR_init_scale;
    private Color init_aimIn_color;
    private bool last_aimed_flag;
    private BP_RC BPRC;


    // Use this for initialization
    void Awake () {
        this.start_flag = false;
        this.last_aimed_flag = false;
        this.Is_aimed_listener = false;
        this.Is_aimed_flag = false;
        this.BR_init_scale = BubbleRender.transform.localScale.x;
        this.init_aimIn_color =
            BubbleAimIndicator.GetComponent<MeshRenderer>().material.color;
    }

    private void Start()
    {
        GeneralMethods.check_ref<BP_RC>(ref BPRC, BP_StrDefiner.RC_name);
    }

    // Update is called once per frame
    void Update () {
        Is_aimed_flag = false;
        Is_aimed_flag = Is_aimed_listener;

        toggle_color();

        Is_aimed_listener = false;
    }

    private void toggle_color()
    {
        if (Is_aimed_flag)
        {
            if (last_aimed_flag != Is_aimed_flag)
            {
                BubbleAimIndicator.GetComponent<MeshRenderer>().material.color = Color.red;
                last_aimed_flag = Is_aimed_flag;
            }
        }
        else
        {
            if (last_aimed_flag != Is_aimed_flag)
            {
                BubbleAimIndicator.GetComponent<MeshRenderer>().material.color =
                                                                        init_aimIn_color;
                last_aimed_flag = Is_aimed_flag;
            }
        }
    }

    public void bubble_shooted()
    {
        BPRC.GC_script.bubble_destroyed();
        BPRC.Bubble_TRANSs.Remove(transform);
        Destroy(gameObject);
    }

    public void start_bubble()
    {
        start_flag = true;
    }

    public void start_bubble(BP_RC _BPRC)
    {
        BPRC = _BPRC;
        start_flag = true;
    }

    public void force_destroy()
    {
        BPRC.Bubble_TRANSs.Remove(transform);
        Destroy(gameObject);
    }

}
