using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAMEC;

public class WAM_MoleCenter : MonoBehaviour
{
    private WAMRC RC;

    public List<Transform> mole_TRANSs { get; set; }
    private MoleGenerShape gener_shape;
    private int mole_frame_num;
    private float distant;
    private List<Transform> frame_TRANSs;
    private float mole_frame_size;
    private float mole_size;
    private float mole_des_time;

    private void Awake()
    {
        this.RC = null;
        this.gener_shape = MoleGenerShape.circle;
        this.mole_frame_num = 1;
        this.distant = 1.0f;
        this.frame_TRANSs = new List<Transform>();
        this.mole_frame_size = 1.0f;
        this.mole_size = 1.0f;
        this.mole_TRANSs = new List<Transform>();
        this.mole_des_time = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init_mole_center(WAMRC _RC,MoleGenerShape _gener_shape, float _distant,int _mole_frame_num,
                                    float _mole_size,float _mole_des_time,float _mole_frame_size)
    {
        RC = _RC;
        gener_shape = _gener_shape;
        mole_frame_num = _mole_frame_num;
        distant = _distant;
        mole_size = _mole_size;
        mole_des_time = _mole_des_time;
        mole_frame_size = _mole_frame_size;
    }

    public void generate_mole_frame()
    {
        switch(gener_shape)
        {
            case MoleGenerShape.circle:
                generate_circle();
                break;
        }
    }

    private void generate_circle()
    {
        float degree = 0.0f;
        float degree_incr = 360.0f / mole_frame_num;
        float x = 0.0f;
        float y = 0.0f;
        while(degree < 360.0f)
        {
            x = distant * Mathf.Cos(Mathf.PI * degree / 180.0f);
            y = distant * Mathf.Sin(Mathf.PI * degree / 180.0f);
            spawn_mole_frame(new Vector3(x, y, transform.position.z));
            degree += degree_incr;
        }
    }

    private void spawn_mole_frame(Vector3 pos)
    {
        Transform frame_TRANS = Instantiate(RC.MoleFrame_Prefab, pos, Quaternion.identity).transform;
        frame_TRANS.localScale = new Vector3(mole_frame_size, mole_frame_size, mole_frame_size);
        frame_TRANS.parent = transform;
        frame_to_pool(frame_TRANS);
    }

    private void frame_to_pool(Transform frame_TRANS)
    {
        frame_TRANSs.Add(frame_TRANS);
    }

    public void generate_mole(MoleGenerType gener_type)
    {
        switch(gener_type)
        {
            case MoleGenerType.random:
                random_mole_gener();
                break;
        }
    }

    private void random_mole_gener()
    {
        int index = Random.Range(0, mole_frame_num);
        Vector3 pos = frame_TRANSs[index].position;
        spawn_mole(pos);
    }

    private void spawn_mole(Vector3 pos)
    {
        Transform mole_TRANS = Instantiate(RC.Mole_Prefab, pos, Quaternion.identity).transform;
        mole_TRANS.localScale = new Vector3(mole_size, mole_size, mole_size);
        mole_TRANS.parent = transform;
        mole_TRANS.GetComponent<WAM_Mole>().init_mole(RC, this, mole_des_time);
        mole_TRANS.GetComponent<WAM_Mole>().start_mole();
        mole_to_pool(mole_TRANS);
    }

    private void mole_to_pool(Transform mole_TRANS)
    {
        mole_TRANSs.Add(mole_TRANS);
    }

    public void whac()
    {
        WAM_Mole mole_cache = null;
        foreach(Transform mole_TRANS in mole_TRANSs.ToArray())
        {
            mole_cache = mole_TRANS.GetComponent<WAM_Mole>();
            if (mole_cache.aimming_flag)
            {
                mole_cache.whaced();
            }
        }
    }

}
