using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BO_GameController : GeneralGameController
{   

    [SerializeField] private GameObject TextIndicator1;
    [SerializeField] private GameObject TextIndicator2;
    [SerializeField] private GameObject BOBall_Prefab;
    [SerializeField] public GameObject DebugText1;
    [SerializeField] private Transform PosManuIndicator;
    [SerializeField] private Transform PosPlayIndicator;
    [SerializeField] private Transform Body_TRANS;
    [SerializeField] private GameObject[] Stated1_OBJs;
    [SerializeField] private GameObject[] Stated2_OBJs;

    [SerializeField] private float BallOffset = 1.0f;
    [Obsolete("Use animator instead")]
    [SerializeField] private float LostWaitTime = 1.0f;
    [SerializeField] string ballM_text_str = "Ball Missed!";
    [SerializeField] private bool Using_auto_generate_brick;
    [SerializeField] private int Max_gener_try_num;
    [SerializeField] private float Bottom_z;
    [SerializeField] private float Step_off_set;

    [HideInInspector]
    public bool start_count_flag;
    public bool Game_paused { get; private set; }
    private float start_count_timer;
    private Animator animator;
    private Animator ball_animator;
    private float lost_wait_timer;
    private bool lost_wait_flag;
    private int trial_iter;
    [Obsolete("In update")]
    private bool brick_updated;
    private bool check_start;
    private int Brick_inst_num { get { return BO_RC.IS.Bricks_pool.Count; } }
    private int Brick_max_num { get { return BO_Setting.IS.Auto_gener_number; } }
    private Vector2[] bottom_grid;
    private BO_LevelInfo level_cache;

    public static BO_GameController IS { get; private set; }

    private void Awake()
    {
        IS = this;

        this.animator = GetComponent<Animator>();
        this.start_count_flag = false;
        this.lost_wait_timer = LostWaitTime;
        this.lost_wait_flag = false;
        this.trial_iter = -1;
        this.brick_updated = true;
        this.check_start = false;
        this.Game_paused = false;
        this.bottom_grid = null;
        this.level_cache = null;
    }

    // Use this for initialization
    void Start () 
    {
        register_controller();
        TextIndicator2.SetActive(false);
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if(lost_wait_flag)
        {
            lost_wait_timer -= Time.deltaTime;
        }
	}

    private void OnDestroy()
    {
        deregister_controller();
        try { unpause_physics(); }
        catch { }
    }

    private void register_controller()
    {
        BO_RC.IS.CI.Button_Y += recenter_VR;
        BO_RC.IS.CI.Button_X += pause_game;
    }

    private void deregister_controller()
    {
        BO_RC.IS.CI.Button_Y -= recenter_VR;
        BO_RC.IS.CI.Button_X -= pause_game;
    }

    public void BO_ToInit()
    {
        //BOGCAnimator.SetTrigger("NextStep");
        check_start = true;
        init_var();
    }

    public void ToBallStart()
    {
        //lost_wait_flag = true;
        Transform BOBall_TRANS = Instantiate(BOBall_Prefab, 
                                BO_RC.IS.Pad_TRANS.position + new Vector3(0.0f,0.0f,BallOffset),
                                new Quaternion()).transform;
        BOBall_TRANS.GetComponent<BO_Ball>().init_ball();
        BOBall_TRANS.name = BO_SD.Ball_OBJ_name;
        animator.SetTrigger(BO_SD.AniL2NextStepTrigger_str);
    }

    public void BallStart()
    {
        if(lost_wait_timer < 0.0f)
        {
            lost_wait_timer = LostWaitTime;
            lost_wait_flag = false;
            TextIndicator2.SetActive(false);
            //animator.SetTrigger(BO_SD.AniNextStepTrigger_str);
        }
    }

    public void ToStartCountDown()
    {
        start_count_flag = true;
        TextIndicator2.SetActive(true);
    }

    public void StartCountDown(float timer)
    {
        TextIndicator2.GetComponent<TextMesh>().text = timer.ToString("F1");
        BO_RC.IS.Ball_pool[0].position = BO_RC.IS.Pad_TRANS.position + new Vector3(0.0f, 0.0f, BallOffset);
    }

    public void LeaveStartCountDown()
    {
        start_ball();
        TextIndicator2.SetActive(false);
    }

    private void start_ball()
    {
        BO_RC.IS.Ball_pool[0].GetComponent<BO_Ball>().start_ball();
    }

    public void restart_game()
    {
        SceneManager.LoadScene(BO_SD.Scene_name);
    }

    public void restart_ball()
    {
        animator.SetTrigger(BO_SD.AniL2RestartTrigger_str);
    }

    public void brick_destroied()
    {
        auto_generate_brick();
    }

    private void init_var()
    {
        Body_TRANS.position = PosManuIndicator.position;
        enable_stated1_objs();
        disable_stated2_objs();
        bottom_grid = GeneralMethods.grid_generation(BO_Setting.IS.Brick_gener_range.x * 2,
            BO_Setting.IS.Brick_gener_range.y * 2, BO_Setting.IS.Brick_size.x,
            BO_Setting.IS.Brick_size.y);
    }

    public void pre_start_game()
    {
        if (check_start)
        {
            to_game();
            animator.SetTrigger(BO_SD.AniStartTrigger_str);
        }
    }

    public void quit_game()
    {
        //Debug.Log("back_button");
        //GameObject.Find(sceneM_Oname_str).GetComponent<MySceneManager>().to_start_scene();
        BO_DataController.IS.MSM_script.to_start_scene();
    }

    private void to_game()
    {
        Body_TRANS.position = PosPlayIndicator.position;
        enable_stated2_objs();
        disable_stated1_objs();
    }

    private void disable_stated1_objs()
    {
        foreach(GameObject go in Stated1_OBJs)
        {
            go.SetActive(false);
        }
    }

    private void enable_stated1_objs()
    {
        foreach (GameObject go in Stated1_OBJs)
        {
            go.SetActive(true);
        }
    }

    private void disable_stated2_objs()
    {
        foreach (GameObject go in Stated2_OBJs)
        {
            go.SetActive(false);
        }
    }

    private void enable_stated2_objs()
    {
        foreach (GameObject go in Stated2_OBJs)
        {
            go.SetActive(true);
        }
    }

    protected override void recenter_VR()
    {
        base.recenter_VR();
    }

    public void To_StartGame()
    {
        //auto_generate_brick();
        animator.SetTrigger(BO_SD.AniNextStepTrigger_str);
    }

    private void auto_generate_brick()
    {
        if (!Using_auto_generate_brick) { return; }
        for(int i = Brick_inst_num;i<Brick_max_num;i++)
        {
            random_spawn_brick();
        }
    }

    private void random_spawn_brick()
    {
        Vector3 center = BO_Setting.IS.Brick_gener_center;
        Vector3 range = BO_Setting.IS.Brick_gener_range;
        Vector3 position = new Vector3();
        bool succeed = false;
        for (int i = 0;i<Max_gener_try_num;i++)
        {
            position = GeneralMethods.random_pos_gener(center, range);
            if (brick_position_check(position)) 
            {
                succeed = true;
                break; 
            }
        }
        if (succeed) 
        {
            Transform brick_TRANS = instantiate_brick(position, BO_Setting.IS.Brick_size, 
                BO_RC.IS.Brick_par_TRANS, BO_RC.IS.Brick_prefab, BO_RC.IS.Bricks_pool);
        }
    }

    private Transform instantiate_brick(Vector3 pos, Vector3 size, Transform parent_TRANS, GameObject prefab,
        List<Transform> pool)
    {
        Transform brick_TRANS = Instantiate(prefab, pos, Quaternion.identity, parent_TRANS).transform;
        brick_TRANS.localScale = size;
        pool.Add(brick_TRANS);
        return brick_TRANS;
    }

    private bool brick_position_check(Vector3 pos)
    {
        Vector3 size = BO_Setting.IS.Brick_size;
        foreach(Transform brick_TRANS in BO_RC.IS.Bricks_pool)
        {
            if(!GeneralMethods.overlap_check_3D(brick_TRANS.position, pos, size))
            { return false; }
        }
        return true;
    }

    private void pause_game()
    {
        if (!Game_paused)
        {
            pause_game_action();
            Game_paused = true;
        }
        else
        {
            unpause_game_action();
            Game_paused = false;
        }
    }

    private void pause_game_action()
    {
        pause_physics();
        turn_on_mini_map();
        turn_on_pause_text();
    }

    private void pause_physics()
    {
        foreach(Transform ball_TRANS in BO_RC.IS.Ball_pool)
        {
            ball_TRANS.GetComponent<BO_Ball>().pause_physics();
        }
        start_count_flag = false;
        GeneralGameController.GameTimeScale = 0.0f;
    }

    private void turn_on_mini_map()
    {
        BO_RC.IS.Minimap_TRANS.gameObject.SetActive(true);
    }

    private void turn_on_pause_text()
    {
        BO_RC.IS.Pause_text_TRANS.gameObject.SetActive(true);
    }

    private void unpause_game_action()
    {
        unpause_physics();
        turn_off_mini_map();
        turn_off_pause_text();
    }

    private void unpause_physics()
    {
        foreach (Transform ball_TRANS in BO_RC.IS.Ball_pool)
        {
            ball_TRANS.GetComponent<BO_Ball>().unpause_physics();
        }
        start_count_flag = true;
        GeneralGameController.GameTimeScale = 1.0f;
    }

    private void turn_off_mini_map()
    {
        BO_RC.IS.Minimap_TRANS.gameObject.SetActive(false);
    }

    private void turn_off_pause_text()
    {
        BO_RC.IS.Pause_text_TRANS.gameObject.SetActive(false);
    }

    public void ToInitBall()
    {

    }

    public void ToInGame()
    {
        next_level();
        switch(level_cache.Game_mode)
        {
            case BO_EnumController.LevelMode.TimeDown:
                animator.SetTrigger(BO_SD.AniMode1Trigger_str);
                break;
        }
        animator.SetTrigger(BO_SD.AniL2StartTrigger_str);
    }

    private void next_level()
    {
        trial_iter++;
        level_cache = BO_RC.IS.Level_pool[trial_iter].GetComponent<BO_LevelInfo>();
        set_despawn_panel(level_cache);
    }

    private void set_despawn_panel(BO_LevelInfo level_info)
    {
        switch(level_info.Game_mode)
        {
            case BO_EnumController.LevelMode.TimeDown:
                BO_RC.IS.Despawn_panel_TRANS.gameObject.SetActive(true);
                BO_RC.IS.Despawn_panel_TRANS.position = new Vector3(0.0f, 0.0f, level_info.Despawn_z);
                break;
            default:
                BO_RC.IS.Despawn_panel_TRANS.gameObject.SetActive(false);
                break;
        }
    }

    private void start_ball_action()
    {

    }

    public void ToBallInGame()
    {

    }

    public void ToBallMissed()
    {

    }

    public void ToRanLayOneGenBri()
    {
        gener_brick_bottom(level_cache.Sin_lay_num);
        animator.SetTrigger(BO_SD.AniNextStepTrigger_str);
    }

    /// <summary>
    /// Randomly generate the bricks at the bottom;
    /// </summary>
    private void gener_brick_bottom(int num)
    {
        num = Math.Min(num, bottom_grid.Length);
        int ran_index = 0;
        Vector3 pos = new Vector3();
        HashSet<int> buff = new HashSet<int>();
        int count = 0;
        while(count < num)
        {
            ran_index = UnityEngine.Random.Range(0, bottom_grid.Length);
            if (buff.Contains(ran_index)) { continue; }
            else { buff.Add(ran_index); }
            pos = (Vector3)bottom_grid[ran_index];
            pos.z = Bottom_z;
            instantiate_brick(pos, BO_Setting.IS.Brick_size, BO_RC.IS.Brick_par_TRANS, BO_RC.IS.Brick_prefab,
                BO_RC.IS.Bricks_pool);
            count++;
        }
    }

    public void ToForwardBricks()
    {
        forward_bricks_step(new Vector3(0.0f, 0.0f, BO_Setting.IS.Brick_size.z + Step_off_set));
        check_bricks_despawn(level_cache.Despawn_z);
        animator.SetTrigger(BO_SD.AniNextStepTrigger_str);
    }

    private void forward_bricks_step(Vector3 step)
    {
        foreach (Transform brick_TRANS in BO_RC.IS.Bricks_pool.ToArray())
        {
            brick_TRANS.position -= step;
        }
    }

    private void check_bricks_despawn(float z)
    {
        foreach (Transform brick_TRANS in BO_RC.IS.Bricks_pool.ToArray())
        {
            brick_TRANS.GetComponent<BO_Brick>().despawn_by_pos(z);
        }
    }

    public void brick_despawned()
    {

    }
}
