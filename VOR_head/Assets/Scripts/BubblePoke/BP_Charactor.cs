using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_Charactor : MonoBehaviour
{
    private BP_RC BPRC;
    private BP_Path BPP_script;

    private int curr_station_index;
    private bool start_flag;

    // Start is called before the first frame update
    void Awake()
    {
        this.curr_station_index = 0;
        this.start_flag = false;
    }

    private void Start()
    {
        if(BPRC == null)
        {
            this.BPRC = GameObject.Find(BP_StrDefiner.RC_name).GetComponent<BP_RC>();
        }
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
        transform.Translate(
            (BPP_script.Stations_TRANS[curr_station_index].position - transform.position).
                normalized * Time.deltaTime * BPRC.GC_script.CharaMovingSpeed, Space.World);
        if (Vector3.Distance(transform.position,
                            BPP_script.Stations_TRANS[curr_station_index].position) < 0.3f)
        {
            curr_station_index++;
            if (curr_station_index >= BPP_script.Stations_TRANS.Length)
            {
                stop_moving_charator();
            }
        }
    }

    private void stop_moving_charator()
    {
        start_flag = false;
        BPP_script.destroy_path();
        Destroy(gameObject);
    }


    public void init_chara(Transform target_TRANS)
    {
        set_target(target_TRANS);
    }

    public void init_chara(BP_RC _BPRC,Transform target_TRANS)
    {
        set_target(target_TRANS);
        BPRC = _BPRC;
    }

    private void set_target(Transform target_TRANS)
    {
        BPP_script = target_TRANS.GetComponent<BP_Path>();
    }

    public void start_chara()
    {
        start_flag = true;
    }
}
