using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FS_Fruit : MonoBehaviour {

    public FS_FruitSpeedCal2 FSC2_script;
    public bool Using_rigidbody;
    public bool Using_collider;
    public bool Using_gravity;
    public bool using_weight;
    public bool Using_curve_path;
    [SerializeField] bool Using_straight_path;
    [SerializeField] bool Using_text_result;
    //[SerializeField] private FS_FruitRenderer FR_script;
    [SerializeField] private FS_StopIndicator SI_script;
    [SerializeField] private FS_FruitMesh FM_script;
    [SerializeField] private FS_FruitIndicator FI_script;
    [SerializeField] private FS_CutResult CR_script;
    [SerializeField] private Transform TooSlowText_TRANS;
    [SerializeField] private string CutTooSlowText;
    [SerializeField] private FS_TrialResults TR_script;
    [SerializeField] private float MeshPPU;
    [SerializeField] private bool Using_save_cut;

    public bool Start_flag { get; set; }
    public float first_weight { get; set; }

    private Animator animator;
    private Texture2D curr_mesh;
    private Vector2[] mesh_ract;
    private Dictionary<int, (Vector3, Vector3)> sta_stop_poss;  //Cut index, start-end position;
    private Vector3 curr_stop_pos;
    private Vector3 curr_sta_pos;
    private Vector3 curr_stopI_pos; //Stop indicator position;
    private float curr_lenS_bonus;
    private float curr_distS_bonus;
    private Dictionary<int, float> cut_scores;   //Cut index and score;
    private bool cutted;
    private Transform trial_group_TRANS;
    private Dictionary<int, Transform> CL_TRANSs;   //Cut index, Curved lines transforms;
    private int curr_cut_index { get { return FI_script.activated_cut_trial; } }
    private bool trace_backable;
    private float trial_score;
    private int trial_star;

    public static FS_Fruit IS { get; set; }

    // Use this for initialization
    void Awake () {
        IS = this;

        this.Start_flag = false;
        this.animator = GetComponent<Animator>();
        this.curr_mesh = null;
        this.mesh_ract = new Vector2[0];
        this.sta_stop_poss = new Dictionary<int, (Vector3, Vector3)>();
        this.curr_stop_pos = new Vector3();
        this.curr_sta_pos = new Vector3();
        this.curr_lenS_bonus = 0.0f;
        this.curr_distS_bonus = 0.0f;
        this.curr_stopI_pos = new Vector3();
        this.cut_scores = new Dictionary<int, float>();
        this.cutted = false;
        this.trial_group_TRANS = null;
        this.first_weight = 0.0f;
        this.CL_TRANSs = new Dictionary<int, Transform>();
        this.trace_backable = false;
        this.trial_score = 0.0f;
        this.trial_star = 0;
	}

    private void Start()
    {

    }

    // Update is called once per frame
    void Update () {

	}

    public void start_record_CP()
    {
        if (Using_curve_path) { CR_script.start_record(); }
    }

    public void stop_record_CP()
    {
        CR_script.stop_record();
    }

    public void start_fruit()
    {
        Start_flag = true;
    }

    public void load_trial(GameObject TR_prefab)
    {
        spawn_trial_group(TR_prefab);
        set_mesh(trial_group_TRANS.GetComponent<FS_TrialGroup>().Texture);
        (Vector2[],float) poss_rat = GeneralMethods.mesh_size_cal_ratio(curr_mesh, 
            FS_Setting.IS.FruitFrameSize, MeshPPU);
        set_mesh_ract(poss_rat.Item1);
        adjust_TR(poss_rat.Item2);
    }

    private void spawn_trial_group(GameObject TR_prefab)
    {
        trial_group_TRANS = Instantiate(TR_prefab, transform.position, Quaternion.identity, transform).
            transform;
    }

    private void adjust_TR(float ratio)
    {
        trial_group_TRANS.localScale *= ratio;
    }

    private void set_mesh(Texture2D mesh_tex)
    {
        curr_mesh = mesh_tex;
    }

    private void set_mesh_ract(Vector2[] poss)
    {
        mesh_ract = poss;
    }

    public void pre_cut_once()
    {
        Vector3 stop_pos = get_stop_pos();
        curr_stop_pos = stop_pos;
        Vector3 sta_pos = FI_script.get_curr_sta_pos();
        curr_sta_pos = sta_pos;
        sta_stop_poss[curr_cut_index] = (sta_pos, stop_pos);
        curr_stopI_pos = FI_script.get_curr_stop_pos();
        animator.SetTrigger(FS_SD.AniNextCutTrial_str);
        cutted = true;
    }

    private Vector3 get_stop_pos()
    {
        Vector3 hit_point = new Vector3();
        if (FS_RC.IS.RC_script.check_object_pos(FS_SD.FruitPlane_tag, out hit_point)) { return hit_point; }
        else { Debug.LogError("get_stop_pos RayCast error"); }
        return new Vector3();
    }

    public void ToInitFruit()
    {
        init_fruit();
        animator.SetTrigger(FS_SD.AniNextStep_str);
    }

    private void init_fruit()
    {
        if (Using_curve_path) { CR_script.init_CR(FS_Setting.IS.PathLineWidth, FS_Setting.IS.StraPLineWidth); }
    }

    public void ToStartFruit()
    {
        animator.SetTrigger(FS_SD.AniNextStep_str);
    }

    public void ToStartCutTrial()
    {
        cutted = false;
        if (FI_script.activated_cut_trial >= FI_script.total_trial - 1) 
        {
            animator.SetTrigger(FS_SD.AniNextTrial_str);
            return;
        }
        else
        {
            start_cut_trial();
            animator.SetTrigger(FS_SD.AniNextStep_str);
        }
    }

    private void start_cut_trial()
    {
        activate_cut_trial();
    }

    private void activate_cut_trial()
    {
        FI_script.activate_next_pair();
    }

    public void ToStartTrial()
    {
        start_trial();
        animator.SetTrigger(FS_SD.AniNextStep_str);
        trace_backable = true;
    }

    public void start_trial()
    {
        clean_trial();
        load_trial(FS_RC.IS.Selected_GO);
        create_mesh();
        load_indicators();
    }

    private void clean_trial()
    {
        FI_script.clear_indicator();
        clear_record();
        clear_mesh();
        clear_results();
    }

    private void clear_record()
    {
        CL_clean_destroy();
        sta_stop_poss.Clear();
        cut_scores.Clear();
    }

    private void clear_results()
    {
        TR_script.clear_results();
    }

    private void CL_clean_destroy()
    {
        foreach(Transform CL_TRANS in CL_TRANSs.Values)
        {
            Destroy(CL_TRANS.gameObject);
        }
        CL_TRANSs.Clear();
    }

    private void create_mesh()
    {
        Transform mesh_TRANS = FM_script.first_create_mesh(mesh_ract, curr_mesh, Using_RB: Using_rigidbody);
        if (using_weight) { first_weight = mesh_TRANS.GetComponent<MeshDataComp>().mesh_data.Area; }
    }

    private void load_indicators()
    {
        FI_script.init_indicators(trial_group_TRANS.GetComponent<FS_TrialGroup>().Indicators);
    }

    public void ToTrialFinished()
    {
        cut();
        score_cal();
        star_cal();
        turn_off_indicator();
        show_trial_result();
        show_star_result();
        turn_off_CL();
        trace_backable = false;
        //update_player();
    }

    [Obsolete("Update in the LeaveTrialFinished")]
    private void update_player()
    {
        FS_GameController.IS.update_player_SC_STA(Mathf.RoundToInt(trial_score), trial_star);
    }

    public void LeaveTrialFinished()
    {
        clean_trial();
        FS_GameController.IS.trial_finished(Mathf.RoundToInt(trial_score), trial_star);
    }

    private void turn_off_CL()
    {
        foreach(Transform CL_TRANS in CL_TRANSs.Values)
        {
            CL_TRANS.GetComponent<LineRenderer>().enabled = false;
        }
    }

    private void turn_off_indicator()
    {
        FI_script.turn_off_indicators();
    }

    private void show_trial_result()
    {
        TR_script.show_results(FS_RC.IS.MeshDataPool.Values.ToArray());
    }

    private void show_star_result()
    {
        TR_script.show_star(trial_star);
    }

    private void cut()
    {
        if (Using_rigidbody) { }
        foreach((Vector3,Vector3) sta_sto_pos in sta_stop_poss.Values)
        {
            FM_script.cut_mseh(sta_sto_pos.Item1, sta_sto_pos.Item2, Using_rigidbody: Using_rigidbody, 
                Using_collider: Using_collider, Using_gravity: Using_gravity);
        }
    }

    private void activate_floor()
    {

    }

    public void ToShowCutResult()
    {
        show_cut_res();
    }

    private void show_cut_res()
    {
        if (Using_curve_path) 
        {
            Transform CurLine_TRANS = CR_script.generate_path();
            if (Using_save_cut) { duplicate_CL(FM_script.transform, CurLine_TRANS); }
        }
        if (Using_straight_path) { gener_stra_path(); }
        (float, float) scoreb = cut_score_cal();
        cut_scores[curr_cut_index] = (scoreb.Item1 + scoreb.Item2);
        if (Using_text_result) { CR_script.show_text_result(scoreb.Item1, scoreb.Item2, curr_stop_pos); }
    }

    /// <summary>
    /// Duplicate the curved line; return duplicated transform;
    /// </summary>
    private Transform duplicate_CL(Transform par_TRANS, Transform LR_TRANS)
    {
        Transform new_line_TRANS = Instantiate(LR_TRANS.gameObject, LR_TRANS.position, 
            LR_TRANS.rotation, par_TRANS).transform;
        CL_TRANSs[curr_cut_index] = new_line_TRANS;
        return new_line_TRANS;
    }

    private void gener_stra_path()
    {
        CR_script.generate_stra_path(curr_sta_pos, curr_stop_pos);
    }
    
    public void LeaveShowCutResult()
    {
        turn_off_res();
    }

    private void turn_off_res()
    {
        CR_script.clear_path();
    }

    /// <summary>
    /// Calculate the bonus score per cut;
    /// </summary>
    /// <returns>Dist score, length score;</returns>
    private (float, float) cut_score_cal()
    {
        curr_distS_bonus = cut_dist_cal(curr_stopI_pos, curr_stop_pos, FS_Setting.IS.ScoreCalMaxDist,
            FS_Setting.IS.MaxDistScore);
        curr_lenS_bonus = cut_len_cal(
            Vector3.Distance(curr_stopI_pos, curr_sta_pos) - FS_Setting.IS.IndicatorSize / 2.0f, 
            CR_script.cur_len_cal(), FS_Setting.IS.ScoreCalMaxLenDiff, FS_Setting.IS.MaxLenScore);
        return (curr_distS_bonus, curr_lenS_bonus);
    }

    private float cut_dist_cal(Vector3 target_pos, Vector3 stop_pos, float max_dist, float maxscore)
    {
        float prop = (max_dist - Vector3.Distance(target_pos, stop_pos)) / max_dist;
        return maxscore * (prop < 0.0f ? 0.0f : prop);
    }

    private float cut_len_cal(float target_len, float real_len, float max_len_diff, float maxscore)
    {
        float prop = (max_len_diff - Mathf.Abs(real_len - target_len)) / max_len_diff;
        return maxscore * (prop < 0.0f ? 0.0f : prop);
    }

    public void LeaveWaitCut()
    {
        deact_indi();
    }

    private void deact_indi()
    {
        FI_script.deactive_curr_pair();
    }

    public void ToTraceBack()
    {
        if (!trace_backable) { return; }
        if (cutted) { trace_back_to_curr(); }
        else { trace_back_to_last(); }
        animator.SetTrigger(FS_SD.AniNextStep_str);
    }

    private void trace_back_to_curr()
    {
        turn_off_res();
        cancel_cut();
        remove_last_CL(curr_cut_index);
        FI_script.prepare_TB_to_curr(); //Change index at last;
    }

    private void trace_back_to_last()
    {
        score_TB_last(curr_cut_index-1);
        turn_off_res();
        cancel_cut();
        pop_sta_stop(curr_cut_index-1);
        remove_last_CL(curr_cut_index-1);
        FI_script.prepare_TB_to_last(); //Change index at last;
    }

    private void remove_last_CL(int index)
    {
        if (Using_save_cut) { remove_CL(index); }
    }

    private void remove_CL(int index)
    {
        if(CL_TRANSs.ContainsKey(index))
        {
            Destroy(CL_TRANSs[index].gameObject);
            CL_TRANSs.Remove(index);
        }
    }

    private void score_TB_last(int index)
    {
        if (cut_scores.ContainsKey(index)) { cut_scores.Remove(index); }
    }

    public void trace_back_act()
    {
        animator.SetTrigger(FS_SD.AniTraceBack_str);
    }

    private void cancel_cut()
    {
        FSC2_script.stop_speed_cal();
    }

    private void pop_sta_stop(int index)
    {
        if (sta_stop_poss.ContainsKey(index)) { sta_stop_poss.Remove(index); }
    }

    public void cut_too_slow()
    {
        cutted = true;
        animator.SetTrigger(FS_SD.AniTooSlow_str);
    }

    public void ToCutTooSlow()
    {
        show_too_slow_text();
    }

    private void show_too_slow_text()
    {
        TooSlowText_TRANS.GetComponent<GeneralTextController>().turn_on(CutTooSlowText);
    }

    public void LeaveCutTooSlow()
    {
        turn_off_slow_text();
    }

    private void turn_off_slow_text()
    {
        TooSlowText_TRANS.GetComponent<GeneralTextController>().turn_off();
    }

    public void ToWaitForSelect()
    {
        clean_trial();
    }

    public void selected()
    {
        animator.SetTrigger(FS_SD.AniSelect_str);
    }

    private void clear_mesh()
    {
        FM_script.clear_mesh();
    }

    private float score_cal()
    {
        float Tscore = 0.0f;
        foreach(float score in cut_scores.Values)
        {
            Tscore += score;
        }
        trial_score = Tscore;
        return trial_score;
    }

    private int star_cal()
    {
        float max_scr = FI_script.total_trial * (FS_Setting.IS.MaxDistScore + FS_Setting.IS.MaxLenScore);
        int star = FS_GameController.Start_cal(trial_score, max_scr);
        trial_star = star;
        return star;
    }
}
