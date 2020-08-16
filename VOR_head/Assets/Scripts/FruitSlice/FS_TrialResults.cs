using MeshSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_TrialResults : MonoBehaviour
{
    [SerializeField] bool Using_text_ANI;
    [SerializeField] float Text_ANI_time_offset;
    [SerializeField] Transform Star_panel_TRANS;
    [SerializeField] bool Using_show_star;
    [SerializeField] float Star_ani_timeoffset;

    private float frame_width;
    private float frame_height;
    private float text_offsety;
    private float trans_time;
    private Dictionary<Transform, Transform> mesh_to_frame; //Mesh transform to frame transform;
    private float frame_gap;
    private int move_finish_inst;
    private float global_scale;
    private float font_size;
    //private List<Transform> FG_TRANSs;  //Frame group pool;
    private float caled_wei_sca_loss;

    private void Awake()
    {
        this.frame_width = 0.0f;
        this.frame_height = 0.0f;
        this.text_offsety = 0.0f;
        this.trans_time = 0.0f;
        this.mesh_to_frame = new Dictionary<Transform, Transform>();
        this.frame_gap = 0.0f;
        this.move_finish_inst = 0;
        this.global_scale = Int32.MaxValue;
        this.font_size = 0.0f;
        //this.FG_TRANSs = new List<Transform>();
        this.caled_wei_sca_loss = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        init_frame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void init_frame()
    {
        frame_width = FS_Setting.IS.ResultFrameWidth;
        frame_height = FS_Setting.IS.ResultFrameHeight;
        trans_time = FS_Setting.IS.ResultTransTime;
        text_offsety = FS_Setting.IS.ResultTextOffsety;
        frame_gap = FS_Setting.IS.ResultFrameGap;
        font_size = FS_Setting.IS.ResultFontSize;
    }

    public void show_results(Transform[] mesh_TRANSs)
    {
        generate_frame(mesh_TRANSs);
        calculate_global(mesh_TRANSs);
        foreach (Transform mesh_TRANS in mesh_TRANSs)
        {
            Transform frame_TRANS = mesh_to_frame[mesh_TRANS];
            adjust_single(frame_TRANS, mesh_TRANS);
        }
    }

    private void calculate_global(Transform[] mesh_TRANSs)
    {
        foreach (Transform mesh_TRANS in mesh_TRANSs)
        {
            MeshData md = mesh_TRANS.GetComponent<MeshDataComp>().mesh_data;
            float hori_size = Mathf.Abs(md.Left_bound - md.Right_bound);
            float vert_size = Mathf.Abs(md.Up_bound - md.Down_bound);
            Vector2 size = new Vector2(hori_size, vert_size);
            Vector3 target_scale = GeneralMethods.scale_cal(size, new Vector2(frame_width, frame_height));
            float t_scale = Mathf.Min(target_scale.x, target_scale.y);
            global_scale = Mathf.Min(t_scale, global_scale);
        }
    }

    private void adjust_single(Transform frame_TRANS, Transform mesh_TRANS)
    {
        frame_TRANS.GetComponent<FS_FrameGroup>().adjust_results(text_ANI: Using_text_ANI, 
            ANI_time: trans_time + Text_ANI_time_offset);
        MeshData md = mesh_TRANS.GetComponent<MeshDataComp>().mesh_data;
        Vector2 trans_off_set = md.center_pos * global_scale;
        Vector3 target_pos = frame_TRANS.position + (-(Vector3)(trans_off_set));
        move_mesh(mesh_TRANS, target_pos, global_scale, FS_Setting.IS.ResultTransTime);
    }

    /// <summary>
    /// Move and scale mesh, size: V2(horizontal,vertical);
    /// </summary>
    /// <param name="mesh_TRANS"></param>
    /// <param name="size"></param>
    /// <param name="target_pos"></param>
    /// <param name="target_size"></param>
    /// <param name="time"></param>
    private void move_mesh(Transform mesh_TRANS, Vector3 target_pos,
        float target_scale, float time)
    {
        StartCoroutine(move_mesh_coro(mesh_TRANS, target_pos, target_scale, time));
    }



    private IEnumerator move_mesh_coro(Transform mesh_TRANS, Vector3 target_pos,
        float target_scale,float time)
    {
        float sca_speed = (target_scale - 1.0f) / time;
        Vector3 trans_speed = (target_pos - mesh_TRANS.position) / time;
        bool run_flag = true;
        while(run_flag)
        {
            mesh_TRANS.localScale += new Vector3(sca_speed, sca_speed, 0.0f) * Time.deltaTime;
            mesh_TRANS.position += trans_speed * Time.deltaTime;
            time -= Time.deltaTime;
            if (time < 0.0f) { run_flag = false; }
            yield return null;
        }
        move_finish_inst++;
    }

    private void generate_frame(Transform[] mesh_TRANSs)
    {
        Vector2[] poss = pos_cal(mesh_TRANSs.Length);
        Transform temp_TRANS = null;
        Vector3 pos = new Vector3();
        float z_pos = transform.position.z;
        float ideal_wei_sca = ideal_weight_cal(mesh_TRANSs.Length);
        float temp_wei_sca = 0.0f;
        float total_wei_sca_loss = 0.0f;
        for(int i = 0;i<mesh_TRANSs.Length;i++)
        {
            pos = new Vector3(poss[i].x, poss[i].y, z_pos);
            temp_TRANS = instantiate_frame(pos, transform);
            mesh_to_frame.Add(mesh_TRANSs[i], temp_TRANS);
            if (FS_Fruit.IS.using_weight) 
            {
                temp_wei_sca = set_weight_sca(temp_TRANS.GetComponent<FS_FrameGroup>(),
                    mesh_TRANSs[i].GetComponent<MeshDataComp>().mesh_data.Area);
                total_wei_sca_loss += Mathf.Abs(temp_wei_sca - ideal_wei_sca);
            }
        }
        caled_wei_sca_loss = total_wei_sca_loss;
    }

    public float get_weight_sca_loss()
    {
        return caled_wei_sca_loss;
    }

    private float ideal_weight_cal(int num)
    {
        return 1.0f / (float)num;
    }

    private Vector2[] pos_cal(int num)
    {
        int max_cols = FS_Setting.IS.ResultFrameHoriMax;
        int cols = (num / max_cols == 0) ? num : max_cols;
        int rows = Mathf.CeilToInt((float)num / (float)max_cols);
        Vector2[] res = new Vector2[num];
        Array.Copy(GeneralMethods.grid_generation(frame_height, frame_width, rows, cols, frame_gap, frame_gap),
            res, num);
        return res;
    }

    public Transform instantiate_frame(Vector3 pos, Transform parent_TRANS)
    {
        Transform frame_TRANS = Instantiate(FS_RC.IS.TrialResultFrame, pos,
            Quaternion.identity, parent_TRANS).transform;
        init_frame_group(frame_TRANS);
        return frame_TRANS;
    }

    private void init_frame_group(Transform frame_TRANS)
    {
        frame_TRANS.GetComponent<FS_FrameGroup>().init_frame(frame_width, frame_height, text_offsety,
            font_size);
    }

    private float set_weight_sca(FS_FrameGroup FG_script, float weight)
    {
        float WS = calculate_weight_sca(weight, FS_Fruit.IS.first_weight);
        FG_script.set_weight_sca(WS);
        return WS;
    }

    private float calculate_weight_sca(float weight, float FW)
    {
        return weight / FW;
    }

    public void clear_results()
    {
        foreach(Transform FG_TRANS in mesh_to_frame.Values)
        {
            FG_TRANS.GetComponent<FS_FrameGroup>().clean_destroy();
        }
        mesh_to_frame.Clear();
        global_scale = Int32.MaxValue;
        clear_star();
    }

    private void clear_star()
    {
        Star_panel_TRANS.GetComponent<FS_StarPanel>().clear_panel();
    }

    public void show_star(int curr_star)
    {
        if (!Using_show_star) { return; }
        Star_panel_TRANS.GetComponent<FS_StarPanel>().init_SP(FS_Setting.IS.MaxStar, 
            trans_time + Star_ani_timeoffset);
        Star_panel_TRANS.GetComponent<FS_StarPanel>().spawn_star_time(curr_star);
    }


}
