using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAMEC;

public class WAM_MoleCenter : MonoBehaviour
{
    public List<Transform> mole_TRANSs { get; set; }
    private List<Transform> frame_TRANSs;
    private int list_index;
    private Vector3 last_pos;

    private void Awake()
    {
        this.frame_TRANSs = new List<Transform>();
        this.mole_TRANSs = new List<Transform>();
        this.list_index = 0;
        this.last_pos = new Vector3();
    }


    public void init_mole_center()
    {
        list_index = -1;
    }

    public void generate_mole_frame()
    {
        switch(WAMSetting.IS.Mole_gener_shape)
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
        float distant2 = WAMSetting.IS.Mole_frame_dist2;
        float distant = WAMSetting.IS.Mole_frame_dist;
        int mole_frame_num = WAMSetting.IS.Mole_frame_num;
        int mole_frame_num2 = WAMSetting.IS.Mole_frame_num2;
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
        float degree_incr = 360.0f / WAMSetting.IS.Mole_frame_num;
        float x = 0.0f;
        float y = 0.0f;
        while(degree < 360.0f)
        {
            x = WAMSetting.IS.Mole_frame_dist * Mathf.Cos(Mathf.PI * degree / 180.0f);
            y = WAMSetting.IS.Mole_frame_dist * Mathf.Sin(Mathf.PI * degree / 180.0f);
            spawn_mole_frame(new Vector3(x, y, transform.position.z));
            degree += degree_incr;
        }
    }

    private void spawn_mole_frame(Vector3 pos)
    {
        Transform frame_TRANS = Instantiate(WAMRC.IS.MoleFrame_Prefab, pos, Quaternion.identity).transform;
        float mole_frame_size = WAMSetting.IS.Mole_frame_size;
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
        list_index %= WAMSetting.IS.Gener_list.Count;
        Vector3 pos = frame_TRANSs[WAMSetting.IS.Gener_list[list_index]].position;
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
            switch (WAMSetting.IS.Mole_gener_shape)
            {
                case MoleGenerShape.circle:
                    index = Random.Range(0, WAMSetting.IS.Mole_frame_num);
                    break;
                case MoleGenerShape.gird:
                    index = Random.Range(0, WAMSetting.IS.Mole_frame_num * WAMSetting.IS.Mole_frame_num2);
                    break;
            }
            pos = frame_TRANSs[index].position;
            if(Vector3.Distance(pos,last_pos) > WAMSetting.IS.Min_distance)
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
        Transform mole_TRANS = Instantiate(WAMRC.IS.Mole_Prefab, pos, Quaternion.identity).transform;
        float mole_size = WAMSetting.IS.Mole_size;
        mole_TRANS.localScale = new Vector3(mole_size, mole_size, mole_size);
        mole_TRANS.parent = transform;
        mole_TRANS.GetComponent<WAM_Mole>().init_mole(this);
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
            mole_TRANS.GetComponent<WAM_Mole>().generate_acuity(WAMSetting.IS.Acuity_type,
                WAMSetting.IS.Acuity_rela_size,WAMSetting.IS.Acuity_flash_time);
        }
    }

    public void whac_acuity(int dir)
    {
        foreach (Transform mole_TRANS in mole_TRANSs.ToArray())
        {
            mole_TRANS.GetComponent<WAM_Mole>().aimmed_acuity_whac(dir);
        }
    }

    public void choose_acuity(int dir)
    {
        foreach (Transform mole_TRANS in mole_TRANSs.ToArray())
        {
            mole_TRANS.GetComponent<WAM_Mole>().choose_acuity(dir);
        }
    }

    public void too_slow()
    {
        foreach (Transform mole_TRANS in mole_TRANSs.ToArray())
        {
            mole_TRANS.GetComponent<WAM_Mole>().wrong_whac();
        }
    }

}
