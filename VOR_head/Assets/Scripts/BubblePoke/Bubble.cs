using System.Collections.Generic;
using UnityEngine;
using System;
using BP_EC;


public class Bubble : MonoBehaviour {

    public GameObject BubbleRender;
    public GameObject BubbleAimIndicator;

    public float IncreaseSpeed = 1.0f;
    //public float LastTime = 3.0f;
    public float LastTransparent = 0.3f;
    public float InitTransparent = 0.6f;
    [SerializeField] private Transform AcutityIndicator_TRANS;
    [SerializeField] private Transform AcutityHolder_TRANS;

    public bool activated_flag { get; private set; }
    public List<AcuityDir> acuity_list { get; private set; }
    public bool Is_aimed_listener { get; set; }
    public bool Is_aimed_flag { get; private set; }  //Inner is aimed;
    //private float last_timer;
    private bool start_flag;
    private float BR_init_scale;
    private Color init_aimIn_color;
    private bool last_aimed_flag;
    private BP_RC BPRC;
    
    private int acuity_index;
    private Transform curr_acuity_TRANS;
    private Transform target_chara_TRANS;

    // Use this for initialization
    void Awake () {
        this.start_flag = false;
        this.last_aimed_flag = false;
        this.Is_aimed_listener = false;
        this.Is_aimed_flag = false;
        this.BR_init_scale = BubbleRender.transform.localScale.x;
        this.init_aimIn_color =
            BubbleAimIndicator.GetComponent<MeshRenderer>().material.color;
        this.acuity_list = new List<AcuityDir>();
        this.acuity_index = -1;
        this.curr_acuity_TRANS = null;
    }

    private void Start()
    {
        GeneralMethods.check_ref<BP_RC>(ref BPRC, BP_StrDefiner.RC_name);

        if(BPRC.GC_script.UsingAcuity)
        {
            //Debug.Log("update_acuity()!!!");
            if (BPRC.GC_script.UsingRandomAcuity)
            {
                generate_random_acuity(BPRC.GC_script.AcuityLength);
            }
            update_acuity();
        }
    }

    // Update is called once per frame
    void Update () {
        Is_aimed_flag = false;
        Is_aimed_flag = Is_aimed_listener;

        toggle_color();

        Is_aimed_listener = false;

        //Debug.Log("AcuityIndex " + acuity_index);
    }

    private void generate_random_acuity(int length)
    {
        int dir_size = Enum.GetNames(typeof(AcuityDir)).Length;
        for(int i = 0;i<length;i++)
        {
            int random_dir = UnityEngine.Random.Range(0, dir_size);
            acuity_list.Add((AcuityDir)(random_dir));
        }
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
        bubble_destroied();
    }

    public void bubble_shooted(AcuityDir acuityDir)
    {
        switch(BPRC.GC_script.GameAcuityMode)
        {
            case AcuityMode.OneByOne:
                if(activated_flag && judge_acuity(acuityDir))
                {
                    bubble_hitted();
                }
                break;
            case AcuityMode.None:
                if (judge_acuity(acuityDir))
                {
                    bubble_hitted();
                }
                break;
        }

    }

    private void bubble_hitted()
    {
        BPRC.GC_script.bubble_hitted();
        if (!update_acuity())
        {
            bubble_destroied();
        }
    }

    private void bubble_destroied()
    {
        BPRC.GC_script.bubble_destroyed(target_chara_TRANS);
        force_destroy();
    }

    private bool judge_acuity(AcuityDir acuityDir)
    {
        if(acuityDir == acuity_list[acuity_index])
        {
            return true;
        }
        return false;
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

    public void start_bubble(BP_RC _BPRC,Transform _target_CTRANS)
    {
        BPRC = _BPRC;
        target_chara_TRANS = _target_CTRANS;
        start_flag = true;
    }

    public void bubble_collided()
    {
        switch(BPRC.GC_script.GameAcuityMode)
        {
            case AcuityMode.OneByOne:
                if(activated_flag)
                {
                    BPRC.GC_script.activated_Bcollided();
                }
                break;
        }
        force_destroy();
    }

    public void force_destroy()
    {
        try
        {
            BPRC.Bubble_TRANSs.Remove(transform);
        }
        catch(Exception e) { Debug.Log(e); }
        Destroy(gameObject);
    }

    public bool update_acuity()
    {

        destroy_acuity();
        acuity_index++;
        if(acuity_index < acuity_list.Count)
        {
            //Debug.Log("pre index " + (int)(acuity_list[acuity_index]));
            //Debug.Log("acu enum " + (acuity_list[acuity_index]));
            GameObject temp_Prefab = BPRC.Acuitys_Prefabs[(int)(acuity_list[acuity_index])];
            curr_acuity_TRANS = Instantiate(temp_Prefab, AcutityIndicator_TRANS.position,
                                                temp_Prefab.transform.rotation).transform;
            curr_acuity_TRANS.SetParent(AcutityHolder_TRANS);
            return true;
        }
        return false;
    }

    private void destroy_acuity()
    {
        if(curr_acuity_TRANS != null)
        {
            Destroy(curr_acuity_TRANS.gameObject);
            curr_acuity_TRANS = null;
        }
    }

    public void deactivate_acuity()
    {
        activated_flag = false;
        AcutityHolder_TRANS.gameObject.SetActive(false);
    }

    public void activate_acuity()
    {
        activated_flag = true;
        AcutityHolder_TRANS.gameObject.SetActive(true);
    }

}
