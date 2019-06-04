using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAMEC;

public class WAM_Mole : MonoBehaviour
{
    [SerializeField] private Transform Collider_TRANS;
    [SerializeField] private Transform Mesh_TRANS;
    [SerializeField] private Transform AcuityMesh_TRANS;

    [SerializeField] private float AcuityOffSet;

    private WAMRC RC;
    private WAM_MoleCenter MC_script;

    public bool aimming_flag { get; private set; }
    private bool start_flag;
    private WAM_RayCast RCT_cache;
    private Color init_color;
    private float timer;
    private int direction;

    private void Awake()
    {
        this.RC = null;
        this.MC_script = null;
        this.RCT_cache = null;
        this.start_flag = false;
        this.aimming_flag = false;
        this.init_color = Mesh_TRANS.GetComponent<MeshRenderer>().material.color;
        this.timer = 0.0f;
        this.direction = 0;
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
        RC.WhacPartical_TRANS.position = transform.position;
        RC.WhacPartical_TRANS.GetComponent<ParticleSystem>().Play();
        clean_destroy();
    }

    public void acuity_whac(int dir)
    {
        if(aimming_flag && dir == direction)
        {
            whaced();
        }
    }

    private void clean_destroy()
    {
        start_flag = false;
        MC_script.mole_TRANSs.Remove(transform);
        Destroy(gameObject);
    }

    public void generate_acuity(AcuityType A_type,float A_size, float A_time)
    {
        switch(A_type)
        {
            case AcuityType.fourdir:
                direction = Random.Range(0, 4);
                break;
            case AcuityType.eightdir:
                direction = Random.Range(0, 8);
                break;
        }
        rotate_acuity(A_type, direction);
        AcuityMesh_TRANS.localScale = new Vector3(A_size, A_size, A_size);
        StartCoroutine(flash_acuity(A_time));
    }

    private IEnumerator flash_acuity(float time)
    {
        
        AcuityMesh_TRANS.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(time);
        AcuityMesh_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    private void rotate_acuity(AcuityType A_type, int dir)
    {
        switch (A_type)
        {
            case AcuityType.fourdir:
                AcuityMesh_TRANS.rotation =
                    Quaternion.Euler(new Vector3(0.0f, 0.0f, dir * -90.0f + AcuityOffSet));
                break;
            case AcuityType.eightdir:
                if (dir < 4)
                {
                    AcuityMesh_TRANS.rotation =
                        Quaternion.Euler(new Vector3(0.0f, 0.0f, dir * -90.0f + AcuityOffSet));
                }
                else
                {
                    AcuityMesh_TRANS.rotation =
                        Quaternion.Euler(new Vector3(0.0f, 0.0f, (dir - 4) * -90.0f + AcuityOffSet - 45.0f));
                }
                break;
        }
    }    
}
