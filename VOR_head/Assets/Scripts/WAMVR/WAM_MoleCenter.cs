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
    private float distant2;
    private int mole_frame_num2;
    private List<int> gener_list;
    private int list_index;
    private bool using_acuity;
    private float acuity_size;
    private AcuityType acuity_type;
    private float acuity_flash_time;
    private float min_dist;
    private Vector3 last_pos;

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
        this.distant2 = 0.0f;
        this.mole_frame_num2 = 0;
        this.gener_list = new List<int>();
        this.list_index = 0;
        this.using_acuity = false;
        this.acuity_size = 1.0f;
        this.acuity_type = AcuityType.fourdir;
        this.acuity_flash_time = 0.1f;
        this.min_dist = 0.0f;
        this.last_pos = new Vector3();
    }


    public void init_mole_center(WAMRC _RC,MoleGenerShape _gener_shape, float _distant,int _mole_frame_num,
                                    float _mole_size,float _mole_des_time,float _mole_frame_size,
                                    float _distant2,int _mole_frame_num2,List<int> _gener_list,
                                    bool _using_acuity,float _acuity_size,AcuityType _acuity_type,
                                    float _acuity_flash_time,float _min_dist)
    {
        RC = _RC;
        gener_shape = _gener_shape;
        mole_frame_num = _mole_frame_num;
        distant = _distant;
        mole_size = _mole_size;
        mole_des_time = _mole_des_time;
        mole_frame_size = _mole_frame_size;
        distant2 = _distant2;
        mole_frame_num2 = _mole_frame_num2;
        gener_list = _gener_list;
        using_acuity = _using_acuity;
        acuity_size = _acuity_size;
        acuity_type = _acuity_type;
        acuity_flash_time = _acuity_flash_time;
        min_dist = _min_dist;

        list_index = -1;
    }

    public void generate_mole_frame()
    {
        switch(gener_shape)
        {
            case MoleGenerShape.circle:
                generate_circle();
                break;
            case MoleGenerShape.gird:
                generate_grid();
                break;
        }
    }

    private void generate_grid()
    {
        float right_board = distant2 / 2.0f;
        float up_board = distant / 2.0f;
        float x = 0.0f;
        float y = 0.0f;
        float hori_dist = 0.0f;
        float vert_dist = 0.0f;
        if (mole_frame_num <= 1)
        {
            y = 0.0f;
        }
        else
        {
            vert_dist = distant / (float)(mole_frame_num - 1);
        }
        if(mole_frame_num2 <= 1)
        {
            x = 0.0f;
        }
        else
        {
            hori_dist = distant2 / (float)(mole_frame_num2 - 1);
        }

        Vector3 pos = new Vector3();
        for(int row = 0;row<mole_frame_num;row++)
        {
            if(mole_frame_num > 1)
            { 
                y = -up_board + row * vert_dist;
            }
            for(int col = 0;col<mole_frame_num2;col++)
            {
                if(mole_frame_num2 > 1)
                {
                    x = -right_board + col * hori_dist;
                }
                pos = new Vector3(x, y, transform.position.z);
                spawn_mole_frame(pos);
            }
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
            case MoleGenerType.list:
                list_mole_gener();
                break;
        }
    }

    private void list_mole_gener()
    {
        list_index++;
        list_index %= gener_list.Count;
        Vector3 pos = frame_TRANSs[gener_list[list_index]].position;
        spawn_mole(pos);
    }

    private void random_mole_gener()
    {
        bool good_gener = false;
        int index = 0;
        Vector3 pos = new Vector3();
        int counter = 0;
        while (!good_gener)
        {
            index = 0;
            switch (gener_shape)
            {
                case MoleGenerShape.circle:
                    index = Random.Range(0, mole_frame_num);
                    break;
                case MoleGenerShape.gird:
                    index = Random.Range(0, mole_frame_num * mole_frame_num2);
                    break;
            }
            pos = frame_TRANSs[index].position;
            if(Vector3.Distance(pos,last_pos) > min_dist)
            {
                good_gener = true;
                break;
            }
            counter += 1;
            if(counter > 5)
            {
                break;
            }
        }

        spawn_mole(pos);
        last_pos = pos;
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

    public void generate_acuity()
    {
        foreach(Transform mole_TRANS in mole_TRANSs)
        {
            mole_TRANS.GetComponent<WAM_Mole>().generate_acuity(acuity_type,acuity_size,acuity_flash_time);
        }
    }

    public void whac_acuity(int dir)
    {
        foreach (Transform mole_TRANS in mole_TRANSs.ToArray())
        {
            mole_TRANS.GetComponent<WAM_Mole>().acuity_whac(dir);
        }
    }

}
