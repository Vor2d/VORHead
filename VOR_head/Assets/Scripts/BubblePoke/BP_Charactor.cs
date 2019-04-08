using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_Charactor : MonoBehaviour
{
    public float DistantToDest = 0.1f;

    public Transform Next_station_TRANS;
    public int curr_station_index;
    public bool start_flag;
    public BP_RC BPRC;
    public BP_Path BPP_script;

    public BP_Charactor(BP_Charactor other_C)
    {
        this.BPRC = other_C.BPRC;
        this.BPP_script = other_C.BPP_script;
        this.DistantToDest = other_C.DistantToDest;
        this.Next_station_TRANS = other_C.Next_station_TRANS;
        this.curr_station_index = other_C.curr_station_index;
        this.start_flag = other_C.start_flag;
    }

    // Start is called before the first frame update
    void Awake()
    {
        this.curr_station_index = 0;
        this.start_flag = false;
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
        }
    }

    private void move_charator()
    {
        transform.Translate((Next_station_TRANS.position - transform.position).normalized * 
                                Time.deltaTime * BPRC.GC_script.CharaMovingSpeed, Space.World);
        if (Vector3.Distance(transform.position, Next_station_TRANS.position) < DistantToDest)
        {
            curr_station_index++;
            if (curr_station_index >= BPP_script.Stations_TRANS.Length)
            {
                stop_moving_charator();
            }
            else
            {
                update_station();
            }
        }
    }

    private void update_station()
    {
        Next_station_TRANS = BPP_script.Stations_TRANS[curr_station_index];
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
    }

    private void set_target_path(Transform target_TRANS)
    {
        BPP_script = target_TRANS.GetComponent<BP_Path>();
    }

    public void start_chara()
    {
        start_flag = true;
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
            if(Vector3.Distance(init_pos, BPP_script.Stations_TRANS[init_Sindex].position) < 
                                                                                DistantToDest)
            {
                init_Sindex++;
                if(init_Sindex >= BPP_script.Stations_TRANS.Length)
                {
                    return Vector3.zero;
                }
            }
            timer += predict_interval;
            move_dist = BPRC.GC_script.CharaMovingSpeed * predict_interval *
                (BPP_script.Stations_TRANS[init_Sindex].position - init_pos).normalized;
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

        start_chara();
    }

    public void force_destroy()
    {
        BPRC.Charators_TRANSs.Remove(transform);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(BP_StrDefiner.Bubble_indi_tag_str))
        {
            BPRC.GC_script.bubble_collidered();
        }
    }
}
