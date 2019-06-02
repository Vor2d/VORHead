using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAM_Mole : MonoBehaviour
{
    [SerializeField] private Transform Collider_TRANS;
    [SerializeField] private Transform Mesh_TRANS;

    private WAMRC RC;
    private WAM_MoleCenter MC_script;

    public bool aimming_flag { get; private set; }
    private bool start_flag;
    private WAM_RayCast RCT_cache;
    private Color init_color;
    private float timer;

    private void Awake()
    {
        this.RC = null;
        this.MC_script = null;
        this.RCT_cache = null;
        this.start_flag = false;
        this.aimming_flag = false;
        this.init_color = Mesh_TRANS.GetComponent<MeshRenderer>().material.color;
        this.timer = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (start_flag)
        {
            check_timer();
            check_aim();
        }
    }

    public void init_mole(WAMRC _RC,WAM_MoleCenter _MC_script,float _timer)
    {
        RC = _RC;
        MC_script = _MC_script;
        timer = _timer;

        RCT_cache = RC.RCT_script;
    }

    public void start_mole()
    {
        start_flag = true;
    }

    private void check_timer()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = float.MaxValue;
            clean_destroy();
        }
    }


    private void check_aim()
    {
        if(RCT_cache.check_object(WAMSD.Mole_tag, Collider_TRANS))
        {
            to_aim_state();
        }
        else
        {
            to_unaim_state();
        }
    }

    private void to_aim_state()
    {
        if(!aimming_flag)
        {
            aimming_flag = true;
            Mesh_TRANS.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    private void to_unaim_state()
    {
        if(aimming_flag)
        {
            aimming_flag = false;
            Mesh_TRANS.GetComponent<MeshRenderer>().material.color = init_color;
        }
        
    }

    public void whaced()
    {
        clean_destroy();
    }

    private void clean_destroy()
    {
        start_flag = false;
        MC_script.mole_TRANSs.Remove(transform);
        Destroy(gameObject);
    }
    
}
