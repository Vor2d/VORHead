using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Animations;
using UnityEngine;
using WAMEC;

public class WAM_MoleCenter : MonoBehaviour
{
    public List<Transform> mole_TRANSs { get; set; }
    private List<Transform> frame_TRANSs;
    private int list_index;
    private Vector3 last_pos;

    private Sprite AC_sprite;

    private void Awake()
    {
        this.frame_TRANSs = new List<Transform>();
        this.mole_TRANSs = new List<Transform>();
        this.list_index = 0;
        this.last_pos = new Vector3();
        this.AC_sprite = null;
    }

    public void init_mole_center()
    {
        list_index = -1;
        AC_sprite = Resources.Load<Sprite>("Sprites/Acuity/WhiteLandC/" + 
            WAMSetting.IS.Acuity_size.ToString());
    }

    public void generate_mole_frame(int LI = 0)
    {
        switch(WAMSetting.IS.Mole_gener_shape)
        {
            case MoleGenerShape.circle:
                generate_circle();
                break;
            case MoleGenerShape.gird:
                generate_grid(MFT: WAMSetting.IS.Mole_frame_type,
                    ran_num: WAMSetting.IS.Mole_frame_randomNum,list_index = LI);
                break;
        }
    }

    private void generate_grid(MoleCenterType MFT = MoleCenterType.random,int ran_num = 0,int list_index = 0)
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
        if (MFT == MoleCenterType.all)
        {
            for (int row = 0; row < mole_frame_num; row++)
            {
                if (mole_frame_num > 1)
                {
                    y = -up_board + row * vert_dist;
                }
                for (int col = 0; col < mole_frame_num2; col++)
                {
                    if (mole_frame_num2 > 1)
                    {
                        x = -right_board + col * hori_dist;
                    }
                    pos = new Vector3(x, y, transform.position.z);
                    spawn_mole_frame(pos); 
                }
            }
        }
        else if(MFT == MoleCenterType.random)
        {
            float pick_ratio = (float)ran_num / (float)(mole_frame_num * mole_frame_num2);
            int count = ran_num;
            HashSet<(int, int)> HS = new HashSet<(int, int)>();
            bool run = true;
            while (count > 0 && run)
            {
                for (int row = 0; row < mole_frame_num; row++)
                {
                    if (mole_frame_num > 1)
                    {
                        y = -up_board + row * vert_dist;
                    }
                    for (int col = 0; col < mole_frame_num2; col++)
                    {
                        if (mole_frame_num2 > 1)
                        {
                            x = -right_board + col * hori_dist;
                        }
                        pos = new Vector3(x, y, transform.position.z);
                        if (!HS.Contains((row,col)) && UnityEngine.Random.Range(0.0f, 1.0f) < pick_ratio) 
                        {
                            count--;
                            HS.Add((row, col));
                            spawn_mole_frame(pos);
                            if (count <= 0) { run = false; break; }
                        }
                    }
                    if (!run) { break; }
                }
            }
        }
        else if(MFT == MoleCenterType.list)
        {
            int row = 0, col = 0;
            foreach (int index in WAMSetting.IS.Mole_frame_Lindex[list_index])
            {
                row = index / mole_frame_num;
                col = index % mole_frame_num2;
                if (mole_frame_num > 1) { y = -up_board + row * vert_dist; }
                if (mole_frame_num2 > 1) { x = -right_board + col * hori_dist; }
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

    public void generate_mole(MoleGenerType gener_type,bool use_Smesh = false)
    {
        switch(gener_type)
        {
            case MoleGenerType.random:
                random_mole_gener(use_Smesh: use_Smesh);
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

    private void random_mole_gener(bool use_Smesh = false)
    {
        bool good_gener = false;
        int index = 0;
        Vector3 pos = new Vector3();
        int counter = 0;
        int max_i = frame_TRANSs.Count;
        while (!good_gener)
        {
            //index = 0;
            //switch (WAMSetting.IS.Mole_gener_shape)
            //{
            //    case MoleGenerShape.circle:
            //        index = UnityEngine.Random.Range(0, max_i);
            //        break;
            //    case MoleGenerShape.gird:
            //        index = UnityEngine.Random.Range(0, max_i);
            //        break;
            //}
            index = UnityEngine.Random.Range(0, max_i);
            pos = frame_TRANSs[index].position;
            if(Vector3.Distance(pos,last_pos) > WAMSetting.IS.Min_distance)
            {
                good_gener = true;
                break;
            }
            counter += 1;
            if(counter > 5)
            {
                good_gener = true;
                break;
            }
        }

        Transform mole_TRANS = spawn_mole(pos,use_Smesh: use_Smesh);
        last_pos = pos;
        if(WAMSetting.IS.Use_jump)
        {
            StartCoroutine(fish_jump(mole_TRANS,WAMSetting.IS.Fish_jumpH,WAMSetting.IS.Fish_jumpT));
        }
    }

    private IEnumerator fish_jump(Transform MT, float jheight,float jtime)
    {
        float speed = jheight / jtime;
        Vector3 tar_pos = new Vector3(MT.position.x, MT.position.y + jheight, MT.position.z);
        while (jtime >= 0.0f)
        {
            MT.position = Vector3.Lerp(MT.position, tar_pos, speed * Time.deltaTime);
            jtime -= Time.deltaTime;
            yield return null;
        }
    }

    private Transform spawn_mole(Vector3 pos,bool use_Smesh = false)
    {
        Transform mole_TRANS = Instantiate(WAMRC.IS.Mole_Prefab, pos, Quaternion.identity).transform;
        Transform mole_mesh = null;
        if (use_Smesh) 
        {
            int index = UnityEngine.Random.Range(0, WAMRC.IS.Fish_Prefabs.Length);
            mole_mesh = Instantiate(WAMRC.IS.Fish_Prefabs[index], mole_TRANS, false).transform;
        }
        float mole_size = WAMSetting.IS.Mole_size;
        mole_TRANS.localScale = new Vector3(mole_size, mole_size, mole_size);
        mole_TRANS.parent = transform;
        if (use_Smesh) { mole_TRANS.GetComponent<WAM_Mole>().
                init_mole(this, self_mesh: mole_mesh,AC_sprite: AC_sprite); }
        else { mole_TRANS.GetComponent<WAM_Mole>().init_mole(this); }
        mole_TRANS.GetComponent<WAM_Mole>().start_mole();
        mole_to_pool(mole_TRANS);
        if (WAMSetting.IS.Use_Splash) { splash(mole_TRANS); }
        return mole_TRANS;
    }

    private void splash(Transform MT)
    {
        Transform splash_TRANS = WAMRC.IS.Splash_TRANS;
        splash_TRANS.position = MT.position;
        //splash_TRANS.GetComponent<SpriteRenderer>().enabled = true;
        splash_TRANS.GetComponentInChildren<Animator>().SetTrigger(WAMSD.SplAniStart_trigger);
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

    public void correct_whac()
    {

    }

    public void clean_destroy()
    {
        foreach (Transform mole_TRANS in mole_TRANSs.ToArray())
        {
            mole_TRANS.GetComponent<WAM_Mole>().clean_destroy();
        }
        WAMRC.IS.MoleCenter_TRANS = null;
        Destroy(gameObject);
    }

}
