using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BP_Charactor : MonoBehaviour
{
    public float DistantToDest = 0.1f;

    public Transform Next_station_TRANS;
    public int curr_station_index;
    public bool start_flag;
    public BP_RC BPRC;
    public BP_Path BPP_script;
    [SerializeField] private BP_FuelIndicator FI_script;
    [SerializeField] private Transform Mesh_TRANS;

    public float Fuel_level { get; private set; }
    private bool using_fuel_flag;
    private float fuel_consu_speed;
    private float chara_move_speed;
    private float init_fuel_level;
    private Color theme_color;

    private BP_TrailController BPTC_script;

    //public BP_Charactor(BP_Charactor other_C)
    //{
    //    this.BPRC = other_C.BPRC;
    //    this.BPP_script = other_C.BPP_script;
    //    this.DistantToDest = other_C.DistantToDest;
    //    this.Next_station_TRANS = other_C.Next_station_TRANS;
    //    this.curr_station_index = other_C.curr_station_index;
    //    this.start_flag = other_C.start_flag;
    //}

    // Start is called before the first frame update
    void Awake()
    {
        this.curr_station_index = 0;
        this.start_flag = false;
        this.Fuel_level = 0.0f;
        this.fuel_consu_speed = 0.0f;
        this.using_fuel_flag = false;
        this.chara_move_speed = 0.0f;
        this.init_fuel_level = 0.0f;
        this.theme_color = Color.white;
        this.BPTC_script = null;
    }

    private void Start()
    {
        GeneralMethods.check_ref<BP_RC>(ref BPRC, BP_StrDefiner.RC_name);

        update_station();
    }

    // Update is called once per frame
    void Update()
    {
        if(start_flag)
        {
            move_charator();
            fuel_system();
        }
    }

    private void move_charator()
    {
        transform.Translate((Next_station_TRANS.position - transform.position).normalized * 
                                Time.deltaTime * chara_move_speed, Space.World);
        if (Vector3.Distance(transform.position, Next_station_TRANS.position) < DistantToDest)
        {
            curr_station_index++;
            if (curr_station_index >= BPP_script.Stations_TRANSs.Count)
            {
                stop_moving_charator();
            }
            else
            {
                update_station();
            }
        }
    }

    private void fuel_system()
    {
        if(BPRC.GC_script.UsingFuelSystem)
        {
            consume_fuel();
            update_fuel_indicator();
        }
    }

    private void consume_fuel()
    {
        Fuel_level -= Time.deltaTime * fuel_consu_speed;
        if(Fuel_level < 0)
        {
            Fuel_level = 0.0f;
        }
    }

    private void update_fuel_indicator()
    {
        FI_script.update_bars(Fuel_level / init_fuel_level);
    }

    public void refill_fuel(float amount)
    {
        Fuel_level += amount;
    }

    private void update_station()
    {
        Next_station_TRANS = BPP_script.Stations_TRANSs[curr_station_index];
    }

    private void stop_moving_charator()
    {
        start_flag = false;
        BPP_script.destroy_path();
        Destroy(gameObject);
    }


    public void init_chara(Transform target_TRANS)
    {
        set_target_path(target_TRANS);
    }

    public void init_chara(BP_RC _BPRC,Transform target_TRANS)
    {
        set_target_path(target_TRANS);
        BPRC = _BPRC;
        init_fuel_level = BPRC.GC_script.InitFuelLevel;
        using_fuel_flag = BPRC.GC_script.UsingFuelSystem;
        fuel_consu_speed = BPRC.GC_script.FuelConsuSpeed;
        chara_move_speed = BPRC.GC_script.CharaMovingSpeed;
        Fuel_level = init_fuel_level;
    }

    public void init_chara(BP_RC _BPRC, Transform target_TRANS,Color _theme_color)
    {
        set_target_path(target_TRANS);
        BPRC = _BPRC;
        init_fuel_level = BPRC.GC_script.InitFuelLevel;
        using_fuel_flag = BPRC.GC_script.UsingFuelSystem;
        fuel_consu_speed = BPRC.GC_script.FuelConsuSpeed;
        chara_move_speed = BPRC.GC_script.CharaMovingSpeed;
        Fuel_level = init_fuel_level;

        set_theme_color(_theme_color);
    }

    public void init_chara(BP_RC _BPRC, Transform target_TRANS, Color _theme_color, 
                            BP_TrailController _BPTC_script)
    {
        set_target_path(target_TRANS);
        BPRC = _BPRC;
        init_fuel_level = BPRC.GC_script.InitFuelLevel;
        using_fuel_flag = BPRC.GC_script.UsingFuelSystem;
        fuel_consu_speed = BPRC.GC_script.FuelConsuSpeed;
        chara_move_speed = BPRC.GC_script.CharaMovingSpeed;
        Fuel_level = init_fuel_level;
        BPTC_script = _BPTC_script;

        set_theme_color(_theme_color);
    }

    private void set_theme_color(Color _theme_color)
    {
        theme_color = _theme_color;
        Mesh_TRANS.GetComponent<MeshRenderer>().material.color = theme_color;
    }

    private void set_target_path(Transform target_TRANS)
    {
        BPP_script = target_TRANS.GetComponent<BP_Path>();
    }

    public void start_chara()
    {
        start_flag = true;
        update_fuel_indicator();
    }

    private void stop_chara()
    {
        start_flag = false;
    }

    public Vector3 simulate_move(float predict_time, float predict_interval)
    {
        Vector3 init_pos = transform.position;
        int init_Sindex = curr_station_index;
        float timer = 0.0f;
        Vector3 move_dist = new Vector3();
        while(timer < predict_time)
        {
            //Debug.Log("destance " +
                //Vector3.Distance(init_pos, BPP_script.Stations_TRANS[init_Sindex].position));
            if(Vector3.Distance(init_pos, BPP_script.Stations_TRANSs[init_Sindex].position) < 
                                                                                DistantToDest)
            {
                init_Sindex++;
                if(init_Sindex >= BPP_script.Stations_TRANSs.Count)
                {
                    return Vector3.zero;
                }
            }
            timer += predict_interval;
            move_dist = BPRC.GC_script.CharaMovingSpeed * predict_interval *
                (BPP_script.Stations_TRANSs[init_Sindex].position - init_pos).normalized;
            init_pos += move_dist;
        }

        return init_pos;
    }

    public void set_state(BP_Charactor other_C)
    {
        this.BPRC = other_C.BPRC;
        this.BPP_script = other_C.BPP_script;
        this.DistantToDest = other_C.DistantToDest;
        this.Next_station_TRANS = other_C.Next_station_TRANS;
        this.curr_station_index = other_C.curr_station_index;
        this.start_flag = other_C.start_flag;
        this.Fuel_level = other_C.Fuel_level;
        this.init_fuel_level = other_C.init_fuel_level;
        this.BPTC_script = other_C.BPTC_script;
    }

    public void set_state(Transform other_TRANS)
    {
        transform.position = other_TRANS.position;
        transform.rotation = other_TRANS.rotation;
        transform.localScale = other_TRANS.localScale;
        BP_Charactor other_C = other_TRANS.GetComponent<BP_Charactor>();
        this.BPRC = other_C.BPRC;
        this.BPP_script = other_C.BPP_script;
        this.DistantToDest = other_C.DistantToDest;
        this.Next_station_TRANS = other_C.Next_station_TRANS;
        this.curr_station_index = other_C.curr_station_index;
        this.start_flag = other_C.start_flag;
        this.Fuel_level = other_C.Fuel_level;
        this.init_fuel_level = other_C.init_fuel_level;
        this.BPTC_script = other_C.BPTC_script;

        start_chara();
    }

    [Obsolete()]
    public void force_destroy()
    {
        BPRC.Charators_TRANSs.Remove(transform);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(BP_StrDefiner.Bubble_indi_tag_str))
        {
            other.GetComponentInParent<Bubble>().bubble_collided();
            //BPRC.GC_script.bubble_collidered();
            BPTC_script.bubble_collided();
        }
    }
}
