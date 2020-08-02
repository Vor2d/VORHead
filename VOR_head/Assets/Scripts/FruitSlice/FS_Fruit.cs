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
    [SerializeField] private FS_FruitRenderer FR_script;
    [SerializeField] private FS_StopIndicator SI_script;
    [SerializeField] private FS_FruitMesh FM_script;
    [SerializeField] private FS_FruitIndicator FI_script;
    [SerializeField] private FS_CutResult CR_script;
    [SerializeField] private Transform TooSlowText_TRANS;
    [SerializeField] private string CutTooSlowText;
    [SerializeField] private FS_TrialResults TR_script;
    [SerializeField] private float MeshPPU;

    public bool Start_flag { get; set; }
    public float first_weight { get; set; }

    private Animator animator;
    private Texture2D curr_mesh;
    private Vector2[] mesh_ract;
    private List<(Vector3, Vector3)> sta_stop_poss;    
    private Vector3 curr_stop_pos;
    private Vector3 curr_sta_pos;
    private Vector3 curr_stopI_pos; //Stop indicator position;
    private float curr_lenS_bonus;
    private float curr_distS_bonus;
    private Stack<float> cut_score;
    private bool cutted;
    private Transform trial_group_TRANS;

    public static FS_Fruit IS { get; set; }

    // Use this for initialization
    void Awake () {
        IS = this;

        this.Start_flag = false;
        this.animator = GetComponent<Animator>();
        this.curr_mesh = null;
        this.mesh_ract = new Vector2[0];
        this.sta_stop_poss = new List<(Vector3, Vector3)>();
        this.curr_stop_pos = new Vector3();
        this.curr_sta_pos = new Vector3();
        this.curr_lenS_bonus = 0.0f;
        this.curr_distS_bonus = 0.0f;
        this.curr_stopI_pos = new Vector3();
        this.cut_score = new Stack<float>();
        this.cutted = false;
        this.trial_group_TRANS = null;
        this.first_weight = 0.0f;
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
        sta_stop_poss.Add((sta_pos, stop_pos));
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

    }

    public void ToStartCutTrial()
    {
        cutted = false;
        if (FI_script.activated_trial >= FI_script.total_trial - 1) 
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
    }

    public void start_trial()
    {
        clean_trial();
        create_mesh();
        load_indicators();
    }

    private void clean_trial()
    {
        FI_script.clear_indicator();
        //curr_Ctrial_index = -1;
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
        turn_off_indicator();
        show_trial_result();
    }

    private void turn_off_indicator()
    {
        FI_script.turn_off_indicators();
    }

    private void show_trial_result()
    {
        TR_script.show_results(FS_RC.IS.MeshDataPool.Values.ToArray());
    }

    private void cut()
    {
        if (Using_rigidbody) { }
        foreach((Vector3,Vector3) sta_sto_pos in sta_stop_poss)
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
        if (Using_curve_path) { CR_script.generate_path(); }
        if (Using_straight_path) { gener_stra_path(); }
        (float, float) scoreb = cut_score_cal();
        cut_score.Push(scoreb.Item1 + scoreb.Item2);
        if (Using_text_result) { CR_script.show_text_result(scoreb.Item1, scoreb.Item2, curr_stop_pos); }
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
        curr_distS_bonus = cut_dist_cal(curr_stopI_pos, curr_stop_pos, FS_Setting.IS.ScoreDistMax,
            FS_Setting.IS.ScoreMaxDist);
        curr_lenS_bonus = cut_len_cal(
            Vector3.Distance(curr_stopI_pos, curr_sta_pos) - FS_Setting.IS.IndicatorSize / 2.0f, 
            CR_script.cur_len_cal(), FS_Setting.IS.ScoreLenDiffMax, FS_Setting.IS.ScoreMaxLen);
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
        if (cutted) { trace_back_to_curr(); }
        else { trace_back_to_last(); }
        animator.SetTrigger(FS_SD.AniNextStep_str);
    }

    private void trace_back_to_curr()
    {
        FI_script.prepare_TB_to_curr();
        turn_off_res();
        cancel_cut();
    }

    private void trace_back_to_last()
    {
        FI_script.prepare_TB_to_last();
        score_TB_last();
        turn_off_res();
        cancel_cut();
    }

    private void score_TB_last()
    {
        if (cut_score.Count > 0) { cut_score.Pop(); }
    }

    public void trace_back_act()
    {
        animator.SetTrigger(FS_SD.AniTraceBack_str);
    }

    private void cancel_cut()
    {
        FSC2_script.stop_speed_cal();
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
        TooSlowText_TRANS.GetComponent<TextMesh>().text = CutTooSlowText;
        TooSlowText_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    public void LeaveCutTooSlow()
    {
        turn_off_slow_text();
    }

    private void turn_off_slow_text()
    {
        TooSlowText_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }
}
