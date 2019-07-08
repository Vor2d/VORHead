using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using HMTS_enum;
using System.Linq;

public class GameController : GeneralGameController {

    //Direction: 0 is left, 1 is right;

    //Obsolete;
    public Dictionary<string, string> GameModeToIndiText = new Dictionary<string, string>()
    {
        { "Training", "Back Training" },
        { "EyeTest","Test 1"},
        { "GazeTest","Test 2"},
        { "HC_FB_Learning","Learning 1"},
        { "Jump_Learning","Learning 2"},
    };

    //Obsoleted;
    [HideInInspector]
    public bool ShowResultFlag = false;
    [HideInInspector]
    public bool TurnSpeedWindow = false;
    [HideInInspector]
    public GameObject ResultTarget = null;

    public enum AcuityMode { four_dir,eight_dir};

    [Header("Game Object")]
    //Game Object;
    public GameObject Target;
    public GameObject IndiText1;
    public GameObject HeadIndicator;
    //public GameObject CameraParent;
    public GameObject LastPosIndocator;
    public GameObject HeadSimulator;
    public TextMesh TestText;
    public TextMesh TestText2;
    public GameObject CoilData;
    public GameObject HeadSParent;
    public GameObject LogSystem;
    public GameController_Setting GCS_script;
    public AcuityGroup AG_script;
    [SerializeField] private Controller_Input CI_script;
    [SerializeField] private GeneralControllerInput GCI_script;
    [SerializeField] private AcuityLogSystem ALS_script;

    //Hiden;
    public uint simulink_sample { get; set; }
    public int trial_iter { get; set; }
    public float turn_degree_x { get; set; }
    public int turn_direct_x { get; set; }
    public Vector2 last_rot_ang_dir { get; set; }
    public string Current_state { get; set; }
    public bool Hide_time_flag { get; set; }
    public bool Check_stop_flag { get; set; }
    public bool Error_time_flag { get; set; }
    public bool Check_speed_flag { get; set; }
    public bool Target_raycast_flag { get; set; }
    public bool Hide_raycast_flag { get; set; }
    public float Hide_timer { get; set; }
    public int section_number { get; set; }
    public int loop_iter { get; set; }
    public bool TargetTimerFlag { get; set; }


    //Scripts;
    private RayCast ray_cast_scrip;
    private Target tar_script;
    private ResultTarget restar_script;
    private CenterIndicator center_script;
    private DataController DC_script;
    private HeadSimulator HS_script;
    private ChangePosition tar_CP_script;
    private ChangePosition LPI_CP_script;
    private CoilData CD_script;
    private VRLogSystem VRLS_script;
    private JumpLogSystem JLS_script;
    //Objects;
    private Animator GCAnimator;
    //Variables;
    private float result_rotate;
    private float gaze_timer;
    private float head_speed_y;
    private float error_timer;
    private List<Vector2> turn_data;
    private List<Vector2> jump_data;
    private float stop_window_timer;
    private Vector2 current_rot_ang_dir_x;
    private float hide_gaze_timer;
    private float gaze_timer_rand;
    private float target_change_timer;
    private int acuity_change_index;
    private int acuity_wrong_num;
    private int acuity_right_num;
    private int curr_acuity_size;
    private int A_delay_index;
    private float curr_A_delay;
    private int A_delay_right;
    private bool AD_last_inc;
    private int AD_converge_index;
    private AcuityGroup.AcuityDirections acuity_dir;
    private AcuityMode acuity_mode;
    private int AD_repeat_index;
    private float AD_incr_amount;
    private Dictionary<float,int> AD_results;
    private CurveFit curve_fit;
    //Flags;
    private bool head_speed_flag;
    private bool stopped_flag;
    private bool centered_flag;
    private bool collaberating_flag;
    private bool controller_flag;
    private bool show_acuity_flag;
    private bool speed_passed_flag;
    private bool show_text_flag;


    public bool UsingAcuity
    {
        get
        {
            return DC_script.Current_GM.UsingAcuityAfter ||
                    DC_script.Current_GM.UsingAcuityBefore ||
                    DC_script.Current_GM.UsingAcuityWaitTime;
        }
    }

    public List<float> AcuityState
    {
        get
        {
            return new List<float>() { acuity_change_index, acuity_right_num, acuity_wrong_num,
                                curr_acuity_size,A_delay_index,curr_A_delay,A_delay_right,AD_converge_index};
        }
    }

    private void Awake()
    {
    }

    // Use this for initialization
    void Start() {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();

        //this.ray_cast_scrip = Camera.main.GetComponent<RayCast>();
        this.ray_cast_scrip = HeadSimulator.GetComponent<RayCast>();
        this.gaze_timer = DC_script.SystemSetting.GazeTime;
        this.gaze_timer_rand = DC_script.SystemSetting.GazeTime;
        this.hide_gaze_timer = DC_script.SystemSetting.GazeTime;
        this.tar_script = Target.GetComponent<Target>();
        this.restar_script = ResultTarget.GetComponent<ResultTarget>();
        //this.SNJstatus = SNJSteps.ToReset;
        //this.center_rotatey = 0.0f;
        this.last_rot_ang_dir = new Vector2(0.0f,0.0f);
        this.current_rot_ang_dir_x = new Vector2(0.0f, 0.0f);
        this.GCAnimator = GetComponent<Animator>();
        this.head_speed_flag = false;
        this.Check_speed_flag = false;
        this.Hide_timer = DC_script.SystemSetting.HideTime;
        this.Hide_time_flag = false;
        this.Check_stop_flag = false;
        this.stopped_flag = false;
        this.head_speed_y = 0.0f;
        this.error_timer = DC_script.SystemSetting.ErrorTime;
        this.turn_data = new List<Vector2>(DC_script.Current_TI.Turn_data);
        this.trial_iter = -1;
        this.jump_data = new List<Vector2>(DC_script.Current_TI.Jump_data);
        this.stop_window_timer = DC_script.SystemSetting.StopWinodow;
        this.Target_raycast_flag = true;
        this.HS_script = HeadSimulator.GetComponent<HeadSimulator>();
        this.tar_CP_script = Target.GetComponent<ChangePosition>();
        this.CD_script = CoilData.GetComponent<CoilData>();
        this.VRLS_script = LogSystem.GetComponent<VRLogSystem>();
        this.JLS_script = LogSystem.GetComponent<JumpLogSystem>();
        this.simulink_sample = 0;
        this.turn_degree_x = 0.1f;
        this.turn_direct_x = 0;
        this.collaberating_flag = false;
        this.Current_state = "";
        this.loop_iter = -1;
        this.Hide_raycast_flag = false;
        this.LPI_CP_script = LastPosIndocator.GetComponent<ChangePosition>();
        this.section_number = 0;
        this.TargetTimerFlag = false;
        this.target_change_timer = DC_script.SystemSetting.TargetChangeTime;
        this.controller_flag = false;
        this.acuity_dir = AcuityGroup.AcuityDirections.up;
        this.acuity_mode = (AcuityMode)DC_script.SystemSetting.AcuityMode;
        this.show_acuity_flag = false;
        this.speed_passed_flag = false;
        this.show_text_flag = false;
        this.acuity_change_index = 0;
        this.acuity_wrong_num = 0;
        this.acuity_right_num = 0;
        this.curr_acuity_size = 0;
        this.A_delay_index = 0;
        this.curr_A_delay = DC_script.Current_GM.PostDelayInit;
        this.A_delay_right = 0;
        this.AD_last_inc = false;
        this.AD_converge_index = 0;
        this.AD_repeat_index = 0;
        this.AD_incr_amount = 0.0f;
        this.AD_results = new Dictionary<float, int>();
        this.curve_fit = new CurveFit();

        IndiText1.GetComponent<TextMesh>().text = "";

        //Active another monitor;
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();

        if(DC_script.MSM_script.using_VR)
        {
            CI_script.IndexTrigger += check_controller;
        }
        if(DC_script.MSM_script.using_coil)
        {
            GCI_script.Button5_act += check_controller;
        }
    }

    // Update is called once per frame
    protected override void Update() {

        //CD_script.Left_eye_voltage = 
        //        new Vector2(turn_direct_x == 0 ? -turn_degree_x : turn_degree_x,0.0f);
        //CD_script.Right_eye_voltage =
        //        new Vector2(turn_direct_x == 0 ? -turn_degree_x : turn_degree_x, 0.0f);
        //Debug.Log("CD_script.Left_eye_voltage " + CD_script.Left_eye_voltage);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            recenter_stage();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            VRLS_script.toggle_Thread();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            JLS_script.toggle_Log();
        }

        //if(Input.GetKeyDown(KeyCode.C))
        //{
        //    if(!collaberating_flag)
        //    {
        //        GCAnimator.SetTrigger("Collaborate");
        //    }
        //    else
        //    {
        //        restart_scene();
        //    }
        //}

        //TestText.text = (Screen.width).ToString();
        //TestText2.text = (Screen.dpi).ToString();

        if (Check_stop_flag)
        {
            if(DC_script.using_coil)
            {
                head_speed_y = CD_script.currentHeadVelocity.z;
            }
            else
            {
                head_speed_y = GeneralMethods.getVRspeed().y;
            }
            
            if (Mathf.Abs(head_speed_y) < DC_script.SystemSetting.StopSpeed)
            {
                //record_target();
                //stopped_flag = true;
                stop_window_timer -= Time.deltaTime;
            }
            else
            {
                stop_window_timer = DC_script.SystemSetting.StopWinodow;
            }
        }

        if (Error_time_flag)
        {
            error_timer -= Time.deltaTime;
        }


        if (ray_cast_scrip.hit_flag && Target_raycast_flag)
        {
            //Debug.Log("get_hit_flag");
            tar_script.turn_on_tmesh();
            gaze_timer -= Time.deltaTime;
        }
        else
        {
            tar_script.turn_off_tmesh();
            gaze_timer = gaze_timer_rand;
        }

        if(ray_cast_scrip.hit_hideDetector_flag && Hide_raycast_flag)
        {
            hide_gaze_timer -= Time.deltaTime;
        }
        else
        {
            hide_gaze_timer = gaze_timer_rand;
        }

        if (Hide_time_flag)
        {
            Hide_timer -= Time.deltaTime;
        }

        if(TargetTimerFlag)
        {
            target_change_timer -= Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        if(DC_script.MSM_script.using_VR)
        {
            CI_script.IndexTrigger -= check_controller;
        }
    }

    public void update_data_restart()
    {
        restart_scene();
    }

    public void ToReset()
    {
        //if (DC_script.HeadIndicatorChange)
        //{
        //    HeadIndicator.GetComponent<Renderer>().enabled = true;
        //}
        turn_degree_x = 0;
        tar_CP_script.changePosition(turn_degree_x, 0.0f, 0, 0);
        GCAnimator.SetTrigger("NextStep");
        //center_rotatey = turn_degree;
        last_rot_ang_dir = new Vector2(0.0f, 1);
        //LPI_script.changePosition(last_rot_ang_dir.x, (int)last_rot_ang_dir.y);
        LPI_CP_script.changePosition(last_rot_ang_dir.x, 0.0f, (int)last_rot_ang_dir.y , 0);
    }

    public void Gaze(HMTS_enum.GazeTarget gazeTarget)
    {
        //Debug.Log("gaze_timer" + gaze_timer);
        switch(gazeTarget)
        {
            case HMTS_enum.GazeTarget.DefaultTarget:
                if (gaze_timer <= 0)
                {
                    GCAnimator.SetTrigger("NextStep");
                    gaze_timer = gaze_timer_rand;
                }
                break;

            case HMTS_enum.GazeTarget.HideDetector:
                if(hide_gaze_timer <= 0)
                {
                    GCAnimator.SetTrigger("NextStep");
                    hide_gaze_timer = gaze_timer_rand;
                }
                break;
        }
    }

    public void ToCenterGaze()
    {
        turn_on_raycasts();
        gaze_timer_rand = DC_script.SystemSetting.GazeTime +
                UnityEngine.Random.Range(-DC_script.SystemSetting.RandomGazeTime,
                                            DC_script.SystemSetting.RandomGazeTime);
    }

    public void CenterGaze()
    {
        if ((DC_script.Current_GM.HideFlag && !DC_script.Current_GM.ShowTargetFlag)
            || DC_script.Current_GM.HideHeadIndicator)
        {
            Gaze(HMTS_enum.GazeTarget.HideDetector);
        }
        else
        {
            Gaze(HMTS_enum.GazeTarget.DefaultTarget);
        }
    }

    public void LeaveCenterGaze()
    {
        turn_off_raycasts();
    }

    public void ToTargetToGaze()
    {
        turn_on_raycasts();
        gaze_timer_rand = DC_script.SystemSetting.GazeTime +
                UnityEngine.Random.Range(-DC_script.SystemSetting.RandomGazeTime,
                                            DC_script.SystemSetting.RandomGazeTime);
    }

    private void turn_on_raycasts()
    {
        if ((DC_script.Current_GM.HideFlag && !DC_script.Current_GM.ShowTargetFlag)
            || DC_script.Current_GM.HideHeadIndicator)
        {
            Target_raycast_flag = false;
            Hide_raycast_flag = true;
        }
        else
        {
            Target_raycast_flag = true;
            Hide_raycast_flag = false;
        }
    }

    public void TargetToGaze()
    {
        if ((DC_script.Current_GM.HideFlag && !DC_script.Current_GM.ShowTargetFlag) 
                || DC_script.Current_GM.HideHeadIndicator)
        {
            Gaze(HMTS_enum.GazeTarget.HideDetector);
        }
        else
        {
            Gaze(HMTS_enum.GazeTarget.DefaultTarget);
        }
    }

    public void LeaveTargetGaze()
    {
        turn_off_raycasts();
    }

    private void turn_off_raycasts()
    {
        Target_raycast_flag = false;
        Hide_raycast_flag = false;
    }

    public void ToMoveTarget()
    {
        float turn_deg_temp_x = 0.1f;
        int turn_dir_temp_x = 0;
        float turn_deg_temp_y = 0.1f;
        int turn_dir_temp_y = 0;
        try
        {
            get_degree_direct(turn_data, trial_iter,
                                out turn_deg_temp_x, out turn_dir_temp_x,
                                out turn_deg_temp_y, out turn_dir_temp_y);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        turn_degree_x = turn_deg_temp_x;
        turn_direct_x = turn_dir_temp_x;

        Debug.Log("turning " + turn_degree_x);

        float turned_degree_x = move_target(turn_degree_x, turn_direct_x);
        current_rot_ang_dir_x = new Vector2(turned_degree_x, turn_direct_x);

        GCAnimator.SetTrigger("NextStep");
    }

    private float move_target(float turn_degre,int turn_direc)
    {
        if (DC_script.using_coil)
        {
            float virtual_degree = 0.0f;
            if (!DC_script.SystemSetting.Using_curved_screen)
            {
                virtual_degree = GeneralMethods.
                                RealToVirtualy(DC_script.SystemSetting.Player_screen_cm,
                                                DC_script.SystemSetting.Screen_width_cm,
                                                turn_degre);
            }
            else
            {
                virtual_degree = GeneralMethods.
                        RealToVirtualy_curved(DC_script.SystemSetting.Player_screen_cm,
                                                DC_script.SystemSetting.Screen_width_cm,
                                                turn_degre);
            }

            tar_CP_script.changePosition(virtual_degree, 0.0f, turn_direc, 0);
            return virtual_degree;
        }
        if (DC_script.using_VR)
        {
            tar_CP_script.changePosition(turn_degre, 0.0f, turn_direc, 0);
            return turn_degre;
        }
        return turn_degre;
    }

    public void ToJumpBack()
    {
        float turn_deg_temp_x = 0.1f;
        int turn_dir_temp_x = 0;
        float turn_deg_temp_y = 0.1f;
        int turn_dir_temp_y = 0;
        try
        {
            get_degree_direct(jump_data, trial_iter, 
                                out turn_deg_temp_x, out turn_dir_temp_x,
                                out turn_deg_temp_y, out turn_dir_temp_y);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        turn_degree_x = turn_deg_temp_x;
        turn_direct_x = turn_dir_temp_x;

        Debug.Log("jumping " + turn_degree_x);

        move_target(turn_degree_x, turn_direct_x);

        GCAnimator.SetTrigger("NextStep");
    }

    //public void SNJToGaze()
    //{
    //    if (gaze_timer <= 0)
    //    {
    //        GCAnimator.SetTrigger("NextStep");
    //        gaze_timer = DC_script.Current_GM.GazeTime;
    //    }
    //}

    public void Hide()
    {
        if (Hide_timer < 0.0f)
        {
            tar_script.turn_off_all_tmesh();
            GCAnimator.SetTrigger("NextStep");
            Hide_timer = DC_script.SystemSetting.HideTime;
        }
    }

    public void ToWaitForTurn()
    {
        Check_speed_flag = true;
        Target_raycast_flag = false;
    }

    public void WaitForTurn()
    {
        //Debug.Log("center " + center_rotatey);

        check_speed_no_window();

        //check_border();
    }

    public void LeaveWaitForTurn()
    {
        Check_speed_flag = false;
        speed_passed_flag = false;
    }

    private void check_speed_with_window()
    {
        //if (!HSC_script.check_from_center(center_rotatey))
        //{
        //    if (HSC_script.check_speed())
        //    {
        //        if (HSC_script.speedEvaluationRecord == 0)
        //        {
        //            GCAnimator.SetTrigger("NextStep");
        //        }
        //        else
        //        {
        //            GCAnimator.SetTrigger("Reset");
        //        }
        //    }
        //}
        //else
        //{
        //    GCAnimator.SetTrigger("Reset");
        //}
    }

    private void check_speed_no_window()
    {
        if(DC_script.using_coil)
        {
            head_speed_y = CD_script.currentHeadVelocity.z;
        }
        if(DC_script.using_VR)
        {
            head_speed_y = GeneralMethods.getVRspeed().y;
        }
        if (Mathf.Abs(head_speed_y) > DC_script.SystemSetting.SpeedThreshold)
        {
            speed_passed();
            return;
        }
        if (ray_cast_scrip.hit_border_flag && !speed_passed_flag)
        {
            Debug.Log("hit_border_flag");
            GCAnimator.SetTrigger("Reset");
        }

    }

    private void speed_passed()
    {
        speed_passed_flag = true;
        //Debug.Log("speed_passed !!!!!!");
        if(DC_script.Current_GM.UsingAcuityBefore)
        {
            StartCoroutine(show_acuity(DC_script.SystemSetting.AcuityFlashTime,true));
        }
        else
        {
            GCAnimator.SetTrigger("NextStep");
        }
    }

    private void record_target()
    {
        //result_rotate = Camera.main.transform.localEulerAngles.y;
    }

    public void ToShowTarget()
    {
        tar_script.turn_on_tobjmesh();
        GCAnimator.SetTrigger("NextStep");
    }

    public void ToCheckStop()
    {
        Check_stop_flag = true;
    }

    public void CheckStop()
    {
        if (stop_window_timer < 0.0f)
        {
            Check_stop_flag = false;    //One more step to guarantee the state is closed;
            stop_window_timer = DC_script.SystemSetting.StopWinodow;
            //stopped_flag = false;
            if(DC_script.Current_GM.UsingAcuityAfter)
            {
                if(DC_script.Current_GM.UsingPostDelay)
                {
                    StartCoroutine(show_acuity(curr_A_delay,DC_script.SystemSetting.AcuityFlashTime, true));
                }
                else
                {
                    StartCoroutine(show_acuity(DC_script.SystemSetting.AcuityFlashTime, true));
                }
            }
            else
            {
                GCAnimator.SetTrigger("NextStep");
            }
        }
    }

    private IEnumerator show_acuity(float time_dure,bool jump_next)
    {
        if(!show_acuity_flag)
        {
            show_acuity_flag = true;
            update_SS();
            ALS_script.log_acuity_state(simulink_sample, "Show Acuity");
            acuity_dir = AG_script.turn_on_acuity(true);
            yield return new WaitForSeconds(time_dure);
            AG_script.turn_off_AG();
            if(jump_next)
            {
                GCAnimator.SetTrigger("NextStep");
            }
            show_acuity_flag = false;
        }
    }

    private IEnumerator show_acuity(float delay_time,float time_dure, bool jump_next)
    {
        yield return new WaitForSeconds(delay_time);
        if (!show_acuity_flag)
        {
            show_acuity_flag = true;
            update_SS();
            ALS_script.log_acuity_delay(simulink_sample, delay_time);
            acuity_dir = AG_script.turn_on_acuity(true);
            yield return new WaitForSeconds(time_dure);
            AG_script.turn_off_AG();
            if (jump_next)
            {
                GCAnimator.SetTrigger("NextStep");
            }
            show_acuity_flag = false;
        }
    }

    //Obsoleted;
    //public void LeaveCheckStop()
    //{
    //    Target_raycast_flag = true;
    //}

    [Obsolete("Not showing result anymore")]
    public void ShowResult()
    {
        int direction = 0;
        if (result_rotate < 0.0f)
        {
            direction = 0;
        }
        else
        {
            direction = 1;
        }
        //restar_script.changePosition(result_rotate, direction);
        restar_script.turn_on_mesh();
        GCAnimator.SetTrigger("NextStep");
    }

    //private void active_SNJ_mode()
    //{
    //    HideFlag = false;
    //    JumpFlag = true;
    //    ShowTargetFlag = false;
    //    ShowResultFlag = true;
    //}

    //private void active_SNS_mode()
    //{
    //    HideFlag = true;
    //    JumpFlag = false;
    //    ShowTargetFlag = true;
    //    ShowResultFlag = true;
    //}

    //private void init_game_mode(GameMode game_mode)
    //{
    //    switch (game_mode)
    //    {
    //        case GameMode.ShowNStay:
    //            active_SNS_mode();
    //            break;
    //        case GameMode.ShowNJump:
    //            active_SNJ_mode();
    //            break;
    //        case GameMode.Custom:
    //            break;
    //    }
    //}

    public void ToShowError()
    {
        if(TurnSpeedWindow)
        {
            //IndiText1.GetComponent<TextMesh>().text = HSC_script.speedEvaluationMessage;
            IndiText1.GetComponent<Renderer>().enabled = true;

            if (DC_script.Current_GM.HideFlag)
            {
                tar_script.turn_on_tobjmesh();
            }
        }
        else
        {
            IndiText1.GetComponent<TextMesh>().text = "Too Slow!";
            IndiText1.GetComponent<Renderer>().enabled = true;
            if (DC_script.Current_GM.HideFlag)
            {
                tar_script.turn_on_tobjmesh();
            }
        }

        //Debug.Log("")
    }

    public void ShowError()
    {
        if(error_timer < 0.0f)
        {
            GCAnimator.SetTrigger("NextStep");
            Error_time_flag = false;    //One more setp;
            error_timer = DC_script.SystemSetting.ErrorTime;
        }
    }

    public void LeaveShowError()
    {
        IndiText1.GetComponent<Renderer>().enabled = false;
    }

    //public void to_next_trial()
    //{

    //}

    public void ToStartTrial()
    {
        if(ShowResultFlag)
        {
            restar_script.turn_off_mesh();
        }
        if (!DC_script.Current_GM.HideHeadIndicator)
        {
            HeadIndicator.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            HeadIndicator.GetComponent<MeshRenderer>().enabled = false;
        }

        if (DC_script.Current_GM.UsingAcuityChange)
        {
            if (change_acuity())
            {
                to_next_section();
                return;
            }
        }

        if (DC_script.Current_GM.UsingPostDelay)
        {
            if (change_delay())
            {
                to_next_section();
                return;
            }
        }

        if (trial_iter < 0)
        {
            last_rot_ang_dir = new Vector2(0.0f, 0.0f);
            LPI_CP_script.changePosition(last_rot_ang_dir.x, 0.0f, (int)last_rot_ang_dir.y, 0);
            trial_iter++;
            GCAnimator.SetTrigger("FirstLoopNext");
        }
        else
        {
            last_rot_ang_dir = current_rot_ang_dir_x;
            LPI_CP_script.changePosition(last_rot_ang_dir.x, 0.0f, (int)last_rot_ang_dir.y, 0);
            trial_iter++;
            if (trial_iter >= turn_data.Count)
            {
                loop_iter++;
                if (DC_script.Current_TI.Loop_number == -1
                        || loop_iter < DC_script.Current_TI.Loop_number)
                {
                    trial_iter = 0;
                    GCAnimator.SetTrigger("NextStep");
                }
                else 
                {
                    to_next_section();
                }
            }
            else
            {
                GCAnimator.SetTrigger("NextStep");
            }
            
        }
        Debug.Log("Trial " + trial_iter);
        Debug.Log("Loop " + loop_iter);
        Debug.Log("Section " + section_number);
    }

    private void to_next_section()
    {
        loop_iter = -1;
        trial_iter = -1;
        section_number++;
        if (section_number < DC_script.Sections.Count)
        {
            GCAnimator.SetTrigger("UpdateDC");
        }
        else
        {
            GCAnimator.SetTrigger("Finished");
        }
    }

    private bool change_delay()
    {
        switch(DC_script.Current_GM.PostDelayMode)
        {
            case PostDelayModes.random:
                curr_A_delay = UnityEngine.Random.Range(0.0f, 3.0f);
                return false;
                //break;
            case PostDelayModes.delay_list:
                if(A_delay_index < DC_script.SystemSetting.PostDelayList.Count)
                {
                    curr_A_delay = DC_script.SystemSetting.PostDelayList[A_delay_index];
                    A_delay_index++;
                    return false;
                }
                else
                {
                    A_delay_index = 0;
                    return true;
                }
            //break;
            case PostDelayModes.percent:
                int result = -10;
                result = GeneralMethods.change_by_percent(ref A_delay_index, DC_script.SystemSetting.PostDelayNumber,
                    ref A_delay_right, DC_script.SystemSetting.PostDelayUpPC, DC_script.SystemSetting.PostDelayLoPC);
                switch(result)
                {
                    case 1:
                        return decrease_delay();
                    case -1:
                        return increase_delay();
                    default:
                        break;
                }
                break;
            case PostDelayModes.converge:
                AD_repeat_index++;
                if(AD_repeat_index < DC_script.SystemSetting.PostDelayRepeatNum)
                {

                }
                else
                {
                    if(conv_next_delay())
                    {

                    }
                }
        }
        return false;
    }

    private void record_AD()
    {
        if(AD_results.ContainsKey(curr_A_delay))
        {
            AD_results[curr_A_delay]++;
        }
        else
        {
            AD_results.Add(curr_A_delay, 1);
        }
    }

    private bool conv_next_delay()
    {
        AD_repeat_index = -1;
        A_delay_index++;
        if (A_delay_index < DC_script.SystemSetting.PostDelayNumber)
        {
            curr_A_delay += AD_incr_amount;
            return false;
        }
        else
        {
            return conv_next_converge();
        }
    }

    private bool conv_next_converge()
    {
        A_delay_index = -1;
        AD_converge_index++;
        if(AD_converge_index < DC_script.SystemSetting.PostDelayConvNum)
        {

        }
    }

    private next_conv_cal()
    {
        curve_fit = new CurveFit();
        double[][] x_arr = new double[AD_results.Keys.Count][];
        int iter = 0;
        foreach(float x in AD_results.Keys)
        {
            x_arr[iter] = new double[] { x };
            iter++;
        }
        double[] y_arr = Array.ConvertAll<int,double>(AD_results.Values.ToArray(), 
                                x => ((double)x/DC_script.SystemSetting.PostDelayRepeatNum));
        curve_fit.init_curve_fit(x_arr, y_arr, _fit_mode: CurveFit.FitModes.Logistic);
        curve_fit.learning();
    }

    private bool decrease_delay()
    {
        bool min_de = min_delay();
        if(AD_last_inc || min_de)
        {
            AD_converge_index++;
            if(AD_converge_index >= DC_script.SystemSetting.PostDelayConvNum)
            {
                return true;
            }
        }
        else
        {
            AD_converge_index = 0;
        }
        if(!min_de)
        {
            curr_A_delay -= curr_A_delay * DC_script.SystemSetting.PostDelayIncPC;
        }
        AD_last_inc = false;
        return false;
    }

    private bool increase_delay()
    {
        bool max_de = max_delay();
        if (!AD_last_inc || max_de)
        {
            AD_converge_index++;
            if (AD_converge_index >= DC_script.SystemSetting.PostDelayConvNum)
            {
                return true;
            }
        }
        else
        {
            AD_converge_index = 0;
        }
        if(!max_de)
        {
            curr_A_delay += curr_A_delay * DC_script.SystemSetting.PostDelayIncPC;
        }
        AD_last_inc = true;
        return false;
    }

    private bool max_delay()
    {
        return curr_A_delay >= 1.0f;
    }

    private bool min_delay()
    {
        return curr_A_delay <= 0.001f;
    }

    private bool change_acuity()
    {
        switch(DC_script.Current_GM.CurrAcuityChangeMode)
        {
            case AcuityChangeMode.percent:
                switch(GeneralMethods.change_by_percent(ref acuity_change_index,DC_script.SystemSetting.AcuityChangeNumber,
                                        ref acuity_right_num,DC_script.SystemSetting.AcuityChangeUpPerc,
                                        DC_script.SystemSetting.AcuityChangeDownPerc))
                {
                    case 1:
                        decrease_acuity_size();
                        break;
                    case -1:
                        increase_acuity_size();
                        break;
                    default:
                        break;
                }
                acuity_wrong_num = 0;
                break;
                
            case AcuityChangeMode.acuity_list:
                if(acuity_change_index < DC_script.SystemSetting.AcuityList.Count)
                {
                    curr_acuity_size = DC_script.SystemSetting.AcuityList[acuity_change_index];
                    set_acuity_size(curr_acuity_size);
                }
                else
                {
                    acuity_change_index = 0;
                    return true;
                }
                acuity_change_index++;
                break;
        }
        return false;

    }

    private void increase_acuity_size()
    {
        curr_acuity_size++;
        set_acuity_size(curr_acuity_size);
    }

    private void decrease_acuity_size()
    {
        curr_acuity_size--;
        set_acuity_size(curr_acuity_size);
    }

    private void set_acuity_size(int size)
    {
        if(size >= DC_script.Acuity_sprites.Length)
        {
            Debug.Log("Max acuity size reached");
        }
        else if(size < 0)
        {
            Debug.Log("Min acuity size reached");
        }
        else
        {
            AG_script.change_acuity_size(size);
        }
    }

    public void ToUpdateDC()
    {
        DC_script.Current_GM = DC_script.Sections[section_number].SectionGameMode;
        DC_script.Current_TI = DC_script.Sections[section_number].SectionTrialInfo;
        turn_data = DC_script.Current_TI.Turn_data;
        jump_data = DC_script.Current_TI.Jump_data;
        curr_acuity_size = DC_script.Current_GM.AcuitySize;
        curr_A_delay = DC_script.Current_GM.PostDelayInit;
        if(DC_script.Current_GM.UsingPostDelay && 
            DC_script.Current_GM.PostDelayMode == PostDelayModes.converge)
        {
            AD_incr_amount = (DC_script.Current_GM.PostDelayIMax - DC_script.Current_GM.PostDelayInit) /
                                DC_script.SystemSetting.PostDelayNumber;
            A_delay_index = -1;
            AD_repeat_index = -1;
            AD_converge_index = -1;
            AD_results = new Dictionary<float, int>();
            curve_fit = new CurveFit();
        }
        update_Animator();

        GCAnimator.SetTrigger("NextStep");
    }

    public void ToInit()
    {
        update_Animator();
        trial_iter = -1;
        ToReset();
    }

    public void recenter_stage()
    {
        if(DC_script.using_coil)
        {
            HS_script.reset_originQ();
        }
        if(DC_script.using_VR)
        {
            InputTracking.Recenter();
        }
        
    }

    [Obsolete("Not using setting scene!")]
    public void load_setting_scene()
    {
        SceneManager.LoadScene("HeadMSetting");
    }

    public void ToReturnPosition()
    {
        tar_CP_script.changePosition(last_rot_ang_dir.x, 0.0f, (int)last_rot_ang_dir.y,0);

        GCAnimator.SetTrigger("NextStep");
    }

    public void update_SS()
    {
        simulink_sample = CD_script.simulinkSample;
    }

    public void ToCollaborate()
    {
        collaberating_flag = true;
        Target_raycast_flag = false;
    }

    public void Collaborating()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            GCAnimator.SetTrigger("NextStep");
        }
    }

    public void LeaveCollaborate()
    {
        Target_raycast_flag = true;
    }

    public void ToFinish()
    {
        IndiText1.GetComponent<TextMesh>().text = "Finished!";
        IndiText1.GetComponent<Renderer>().enabled = true;
    }

    public void ToHideHeadIndicator()
    {
        if(DC_script.Current_GM.HeadIndicatorChange)
        {
            HeadIndicator.GetComponent<MeshRenderer>().enabled = false;
        }
        
        GCAnimator.SetTrigger("NextStep");
    }

    public void ToShowHeadIndicator()
    {
        //Debug.Log("ToShowHeadIndicator");
        if (DC_script.Current_GM.HeadIndicatorChange)
        {
            HeadIndicator.GetComponent<MeshRenderer>().enabled = true;
        }

        GCAnimator.SetTrigger("NextStep");
    }

    private void get_degree_direct(List<Vector2> list, int iter, 
                                    out float turn_deg_x, out int turn_dir_x,
                                    out float turn_deg_y, out int turn_dir_y)
    {
        float temp_degree_x = list[iter].x;
        turn_deg_x = Mathf.Abs(temp_degree_x);
        if (temp_degree_x < 0)
        {
            turn_dir_x = 0;
        }
        else
        {
            turn_dir_x = 1;
        }
        float temp_degree_y = list[iter].y;
        turn_deg_y = Mathf.Abs(temp_degree_y);
        if (temp_degree_y < 0)
        {
            turn_dir_y = 0;
        }
        else
        {
            turn_dir_y = 1;
        }
    }

    private void restart_scene()
    {
        SceneManager.LoadScene("HeadMovement");
    }

    private void update_Animator()
    {
        GCAnimator.SetBool("HideFlag", DC_script.Current_GM.HideFlag);
        GCAnimator.SetBool("JumpFlag", DC_script.Current_GM.JumpFlag);
        GCAnimator.SetBool("ShowTargetFlag", DC_script.Current_GM.ShowTargetFlag);
        GCAnimator.SetBool("SkipCenter", DC_script.Current_GM.SkipCenterFlag);
        GCAnimator.SetBool("ChangeTargetByTimeFlag", DC_script.Current_GM.ChangeTargetByTime);
    }

    public void ToChangeTargetWaitTime()
    {
        target_change_timer = DC_script.SystemSetting.TargetChangeTime +
                    UnityEngine.Random.Range(-DC_script.SystemSetting.TargetChangeTimeRRange,
                                            DC_script.SystemSetting.TargetChangeTimeRRange);
        TargetTimerFlag = true;
        if(DC_script.Current_GM.UsingAcuityWaitTime)
        {
            StartCoroutine(show_acuity(DC_script.SystemSetting.AcuityFlashTime,false));
        }
    }

    public void ChangeTargetWaitTime()
    {
        if(target_change_timer < 0.0f)
        {
            target_change_timer = DC_script.SystemSetting.TargetChangeTime;
            TargetTimerFlag = false;

            if(DC_script.Current_GM.UsingAcuityWaitTime && 
                DC_script.SystemSetting.UseAcuityIndicator)
            {
                GCAnimator.SetTrigger("CheckController");
            }
            else
            {
                GCAnimator.SetTrigger("NextStep");
            }
        }
    }

    public void ToSectionPause()
    {
        //string showing_message =
        //            GameModeToIndiText[DC_script.Current_GM.GameModeName.ToString()];
        //IndiText1.GetComponent<TextMesh>().text = showing_message;
        //IndiText1.GetComponent<MeshRenderer>().enabled = true;

        GCS_script.change_indicate_text(DC_script.Current_GM.GameModeName.ToString());

        if(UsingAcuity)
        {
            AG_script.init_acuity(DC_script.Current_GM.AcuitySize,acuity_mode,DC_script);
            curr_acuity_size = DC_script.Current_GM.AcuitySize;
            update_SS();
            ALS_script.log_time("start", simulink_sample,DC_script.Current_GM.GameModeName.ToString());
        }
    }

    public void SectionPause ()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            GCAnimator.SetTrigger("NextStep");
        }
    }

    public void LeaveSectionPause()
    {
        //IndiText1.GetComponent<MeshRenderer>().enabled = false;

        GCS_script.turn_off_text();
    }

    public void back_to_main_menu()
    {
        DC_script.MSM_script.to_start_scene();
    }

    public void ToCheckController()
    {
        if(UsingAcuity)
        {
            controller_flag = true;
            if(DC_script.SystemSetting.UseAcuityIndicator)
            {
                //Debug.Log("DC_script.SystemSetting.UseAcuityIndicator " + DC_script.SystemSetting.UseAcuityIndicator);
                AG_script.turn_on_AG();
                AG_script.start_AI();
            }
        }
        else
        {
            GCAnimator.SetTrigger("NextStep");
        }
        
    }

    private IEnumerator show_text(float time,string text)
    {
        if(!show_text_flag)
        {
            show_text_flag = true;
            IndiText1.GetComponent<MeshRenderer>().enabled = true;
            IndiText1.GetComponent<TextMesh>().text = text;
            yield return new WaitForSeconds(time);
            IndiText1.GetComponent<MeshRenderer>().enabled = false;
            show_text_flag = false;
        }
        
    }

    private void check_controller()
    {
        if(controller_flag)
        {
            bool result = false;
            if (DC_script.MSM_script.using_VR)
            {
                //check_oculusC();
                result = check_oculusC2();
                //update_SS();
                //ALS_script.log_acuity(simulink_sample, curr_acuity_size, result.ToString());
            }
            if(DC_script.MSM_script.using_coil)
            {
                result = check_common_controller();
            }
        }
    }

    private void check_oculusC()
    {
        Debug.Log("acuity " + acuity_dir);
        if(acuity_dir == AcuityGroup.AcuityDirections.up && CI_script.Forward_flag || 
            acuity_dir == AcuityGroup.AcuityDirections.right && CI_script.Right_flag ||
            acuity_dir == AcuityGroup.AcuityDirections.down && CI_script.Back_flag ||
            acuity_dir == AcuityGroup.AcuityDirections.left && CI_script.Left_flag)
        {
            Debug.Log("Right!!!");
            GCAnimator.SetTrigger("NextStep");
        }
        else
        {
            Debug.Log("Wrong!!!!!");
        }
    }

    private bool check_oculusC2()
    {
        //Debug.Log("acuity_dir " + acuity_dir);
        bool empty_flag = CI_script.Eight_dir_input == Controller_Input.EightDirInput.empty;
        bool result = false;
        switch(acuity_mode)
        {
            case AcuityMode.four_dir:
                if ((int)acuity_dir == (int)CI_script.Four_dir_input)
                {
                    acuity_right_num++;
                    A_delay_right++;
                    //StartCoroutine(show_text(1.0f, "Right"));
                    result = true;
                }
                else
                {
                    acuity_wrong_num++;
                    //StartCoroutine(show_text(1.0f, "Wrong"));
                    result = false;
                }
                break;
            case AcuityMode.eight_dir:
                if((int)acuity_dir == (int)CI_script.Eight_dir_input)
                {
                    acuity_right_num++;
                    A_delay_right++;
                    //StartCoroutine(show_text(1.0f, "Right"));
                    result = true;
                }
                else
                {
                    acuity_wrong_num++;
                    //StartCoroutine(show_text(1.0f, "Wrong"));
                    result = false;
                }
                break;
        }
        if(empty_flag)
        {
            update_SS();
            ALS_script.log_acuity(simulink_sample, curr_acuity_size, result.ToString(),
                acuity_dir.ToString(),CI_script.Eight_dir_input.ToString());
        }
        GCAnimator.SetTrigger("NextStep");
        return result;
    }

    private bool check_common_controller()
    {
        bool result = false;
        bool empty_flag = false;
        switch (acuity_mode)
        {
            case AcuityMode.four_dir:
                if(GCI_script.Four_dir_input == GeneralControllerInput.FourDirInput.empty)
                {
                    empty_flag = true;
                    break;
                }
                if ((int)acuity_dir == (int)GCI_script.Four_dir_input)
                {
                    acuity_right_num++;
                    A_delay_right++;
                    //StartCoroutine(show_text(1.0f, "Right"));
                    result = true;
                }
                else
                {
                    acuity_wrong_num++;
                    //StartCoroutine(show_text(1.0f, "Wrong"));
                    result = false;
                }
                break;
            case AcuityMode.eight_dir:
                if(GCI_script.Eight_dir_input == GeneralControllerInput.EightDirInput.empty)
                {
                    empty_flag = true;
                    break;
                }
                if ((int)acuity_dir == (int)GCI_script.Eight_dir_input)
                {
                    acuity_right_num++;
                    A_delay_right++;
                    //StartCoroutine(show_text(1.0f, "Right"));
                    result = true;
                }
                else
                {
                    acuity_wrong_num++;
                    //StartCoroutine(show_text(1.0f, "Wrong"));
                    result = false;
                }
                break;
        }
        if(!empty_flag)
        {
            update_SS();
            ALS_script.log_acuity(simulink_sample, curr_acuity_size, result.ToString(),
                acuity_dir.ToString(),GCI_script.Eight_dir_input.ToString());
            GCAnimator.SetTrigger("NextStep");
        }
        return result;
    }

    public void LeaveCheckController()
    {
        controller_flag = false;
        AG_script.turn_off_AG();
    }

    public List<string> Debug_str { get; set; }
}

