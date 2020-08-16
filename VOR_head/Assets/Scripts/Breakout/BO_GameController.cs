using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BO_GameController : GeneralGameController
{   

    [SerializeField] private GameObject TextIndicator1;
    [SerializeField] private GameObject TextIndicator2;
    [SerializeField] private Transform BOPad_TRANS;
    [SerializeField] private GameObject BOBall_Prefab;
    [SerializeField] public GameObject DebugText1;
    [SerializeField] private Transform PosManuIndicator;
    [SerializeField] private Transform PosPlayIndicator;
    [SerializeField] private Transform Body_TRANS;
    //[SerializeField] private RightController RC_script;
    //[SerializeField] private Transform FakePad;
    [SerializeField] private GameObject[] Stated1_OBJs;
    [SerializeField] private GameObject[] Stated2_OBJs;

    [SerializeField] private float StartCountTime = 3.0f;
    [SerializeField] private float BallOffset = 1.0f;
    [SerializeField] private float LostWaitTime = 1.0f;
    [SerializeField] string ballM_text_str = "Ball Missed!";
    [SerializeField] private bool Using_auto_generate_brick;
    [SerializeField] private int Max_gener_try_num;

    private float start_count_timer;
    private bool start_count_flag;
    private Animator animator;
    private Transform BOBall_TRANS;
    private float lost_wait_timer;
    private bool lost_wait_flag;
    private int trial_iter;
    [Obsolete("In update")]
    private bool brick_updated;
    private bool check_start;
    private int Brick_inst_num { get { return BO_RC.IS.Bricks_pool.Count; } }
    private int Brick_max_num { get { return BO_Setting.IS.Auto_gener_number; } }

    public static BO_GameController IS;

    private void Awake()
    {
        IS = this;

        this.start_count_timer = StartCountTime;
        this.animator = GetComponent<Animator>();
        this.start_count_flag = false;
        this.lost_wait_timer = LostWaitTime;
        this.lost_wait_flag = false;
        this.trial_iter = -1;
        this.brick_updated = true;
        this.check_start = false;
    }

    // Use this for initialization
    void Start () 
    {
        register_controller();
        TextIndicator2.SetActive(false);
	}
	
	// Update is called once per frame
	protected override void Update () {
        if (start_count_flag)
        {
            start_count_timer -= Time.deltaTime;
        }

        if(lost_wait_flag)
        {
            lost_wait_timer -= Time.deltaTime;
        }
	}

    private void OnDestroy()
    {
        deregister_controller();
    }

    private void register_controller()
    {
        BO_RC.IS.CI.Button_Y += recenter_VR;
    }

    private void deregister_controller()
    {
        BO_RC.IS.CI.Button_Y -= recenter_VR;
    }

    public void BO_ToInit()
    {
        //BOGCAnimator.SetTrigger("NextStep");
        check_start = true;
        init_var();
    }

    public void ToBO_Start()
    {
        lost_wait_flag = true;
        BOBall_TRANS = Instantiate(BOBall_Prefab, 
                                BOPad_TRANS.position + new Vector3(0.0f,0.0f,BallOffset),
                                new Quaternion()).transform;
        BOBall_TRANS.name = BO_SD.Ball_OBJ_name;
        trial_iter++;
        if(trial_iter > 0)
        {
            TextIndicator2.SetActive(true);
            TextIndicator2.GetComponent<TextMesh>().text = ballM_text_str;
        }
    }

    public void BO_Start()
    {
        if(lost_wait_timer < 0.0f)
        {
            lost_wait_timer = LostWaitTime;
            lost_wait_flag = false;
            TextIndicator2.SetActive(false);
            animator.SetTrigger(BO_SD.AniNextStepTrigger_str);
        }
    }

    public void ToStartCountDown()
    {
        start_count_flag = true;
        TextIndicator2.SetActive(true);
    }

    public void StartCountDown()
    {
        TextIndicator2.GetComponent<TextMesh>().text = ((int)start_count_timer).ToString();
        BOBall_TRANS.position = BOPad_TRANS.position + new Vector3(0.0f, 0.0f, BallOffset);
        if(start_count_timer < 0.0f)
        {
            start_game();
            start_count_timer = StartCountTime;
            start_count_flag = false;
            TextIndicator2.SetActive(false);
            animator.SetTrigger(BO_SD.AniNextStepTrigger_str);
        }
    }

    private void start_game()
    {
        BOBall_TRANS.GetComponent<BO_Ball>().start_ball();
    }

    public void restart_game()
    {
        SceneManager.LoadScene(BO_SD.Scene_name);
    }

    public void restart_ball()
    {
        animator.SetTrigger(BO_SD.AniRestartTrigger_str);
    }

    public void brick_destroied()
    {
        auto_generate_brick();
    }

    [Obsolete("Not using shadows")]
    public void update_brick()
    {
        foreach (GameObject brick_OBJ in GameObject.FindGameObjectsWithTag(BO_SD.Brick_tag))
        {
            brick_OBJ.GetComponent<BO_Brick>().update_shadow();
        }
    }

    private void init_var()
    {
        //CameraParent_TRANS.transform.position = PosManuIndicator.position;
        Body_TRANS.position = PosManuIndicator.position;
        //RC_script.turn_on_controller();
        //BOPad_TRANS.GetComponent<BO_Pad>().move_with_raycast = false;
        //BOPad_TRANS.gameObject.SetActive(false);
        //FakePad.gameObject.SetActive(true);
        enable_stated1_objs();
        disable_stated2_objs();
        Debug.Log("init_var");
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
        //CameraParent_TRANS.transform.position = PosPlayIndicator.position;
        Body_TRANS.position = PosPlayIndicator.position;
        //RC_script.turn_off_controller();
        //BOPad_TRANS.GetComponent<BO_Pad>().move_with_raycast = true;
        //BOPad_TRANS.gameObject.SetActive(true);
        //FakePad.gameObject.SetActive(false);
        enable_stated2_objs();
        disable_stated1_objs();
        Debug.Log("to_game");
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
        auto_generate_brick();
        animator.SetTrigger(BO_SD.AniNextStepTrigger_str);
    }

    private void auto_generate_brick()
    {
        if (!Using_auto_generate_brick) { return; }
        for(int i = Brick_inst_num;i<Brick_max_num;i++)
        {
            spawn_brick();
        }
    }

    private void spawn_brick()
    {
        Vector3 center = BO_Setting.IS.Brick_gener_center;
        Vector3 range = BO_Setting.IS.Brick_gener_range;
        Vector3 position = new Vector3();
        bool succeed = false;
        for (int i = 0;i<Max_gener_try_num;i++)
        {
            position = GeneralMethods.random_pos_gener(center, range);
            if (position_check(position)) 
            {
                succeed = true;
                break; 
            }
        }
        if (succeed) 
        {
            Transform brick_TRANS = instantiate_brick(position, BO_Setting.IS.Brick_size, 
                BO_RC.IS.Brick_par_TRANS, BO_RC.IS.Brick_prefab);
            BO_RC.IS.Bricks_pool.Add(brick_TRANS);
        }
    }

    private Transform instantiate_brick(Vector3 pos, Vector3 size, Transform parent_TRANS, GameObject prefab)
    {
        Transform brick_TRANS = Instantiate(prefab, pos, Quaternion.identity, parent_TRANS).transform;
        brick_TRANS.localScale = size;
        return brick_TRANS;
    }

    private bool position_check(Vector3 pos)
    {
        Vector3 size = BO_Setting.IS.Brick_size;
        foreach(Transform brick_TRANS in BO_RC.IS.Bricks_pool)
        {
            if(!GeneralMethods.overlap_check_3D(brick_TRANS.position, pos, size))
            { return false; }
        }
        return true;
    }

}
